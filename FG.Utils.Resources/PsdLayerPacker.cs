using FG.Defs.YSYard.QoL;
using PsdParser;
using SkiaSharp;

namespace FG.Utils.Resources;

internal static class PsdLayerPacker
{
    internal static bool TrySavePackedTexture(
        string srcPath, string pngPath,
        out Dictionary<string, TextureRegion> textureRegions)
    {
        textureRegions = [];

        using var psd = new PsdFile(srcPath);

        var images = psd.LayerAndMaskInformationSection
            .LayerInfo
            .Items
            .Where(x => 
                0 < x.Image.Width
                && 0 < x.Image.Height
                && !x.Record.LayerName.StartsWith("--"))
            .Select(x => new PsdLayerImage(x))
            .ToList();
        using var _ = new CompositeDisposable(images);

        images.Sort(
            (a, b) =>
                -1 * Math.Max(a.Width, a.Height)
                    .CompareTo(Math.Max(b.Width, b.Height)));
        if (!TrySetRect(images, out var surfaceWidth))
        {
            return false;
        }

        using var surf = new SurfaceWrapper(surfaceWidth);
        foreach (var img in images)
        {
            surf.Draw(img);
        }
        surf.SaveSnapshotAsPng(pngPath);

        foreach (var img in images)
        {
            if (textureRegions.ContainsKey(img.Name))
            {
                throw new InvalidDataException(
                    "Input psd file has same name layers");
            }
            else
            {
                textureRegions[img.Name] = new()
                {
                    Name = img.Name,
                    X = img.Rect.X,
                    Y = img.Rect.Y,
                    W = img.Rect.W,
                    H = img.Rect.H,
                };
            }
        }

        return true;
    }

    private static bool TrySetRect(
        IEnumerable<IPackableImage> images, out int surfaceWidth)
    {
        surfaceWidth = 0;

        var pixels = images.Sum(x => x.Width * x.Height);
        if (!TryGet2PowCeiledValue((int)Math.Sqrt(pixels), out var w))
        {
            Console.Error.WriteLine("Cannot pack layers : total size of layers is too large");
            return false;
        }
        if (2048 <= w)
        {
            Console.Error.WriteLine("Cannot pack layers : total size of layers is too large");
            return false;
        }

        var rectMap = new Dictionary<IPackableImage, PackedRect>();
        while (true)
        {
            var rootNode = new RectNode(new(0, 0, w, w));
            rectMap.Clear();
            var success = true;
            foreach (var img in images)
            {
                var rect = rootNode.Insert(img.Width, img.Height, 1);
                if (rect == null)
                {
                    success = false;
                    break;
                }
                rectMap[img] = rect.Value;
            }
            if (success)
            {
                break;
            }

            w *= 2;
            if (4096 <= w)
            {
                Console.Error.WriteLine("Cannot pack layers : total size of layers is too large");
                return false;
            }
        }

        foreach (var kvp in rectMap)
        {
            kvp.Key.Rect = kvp.Value;
        }
        surfaceWidth = w;
        return true;
    }

    private static bool TryGet2PowCeiledValue(int x, out int ret)
    {
        ret = -1;
        var tmp = 2;
        while (true)
        {
            if (x < tmp)
            {
                ret = tmp;
                return true;
            }
            else if (0x40000000 <= tmp)
            {
                return false;
            }
            tmp <<= 1;
        }
    }

    private class CompositeDisposable(
        IEnumerable<IDisposable> disposables)
        : IDisposable
    {
        public void Dispose()
        {
            foreach (var d in disposables)
            {
                d.Dispose();
            }
        }
    }

    private readonly struct PackedRect(int x, int y, int w, int h)
    {
        internal int X { get; } = x;

        internal int Y { get; } = y;

        internal int W { get; } = w;

        internal int H { get; } = h;
    }

    private interface IPackableImage : IDisposable
    {
        string Name { get; }

        int Width { get; }

        int Height { get; }

        PackedRect Rect { get; set; }

        void DrawOnCanvas(
            SKCanvas canvas, int canvasWidth, int canvasHeight);
    }

    private class PsdLayerImage(LayerRecordAndImage ri) : IPackableImage
    {
        public string Name => ri.Record.LayerName;

        public int Width => ri.Image.Width;

        public int Height => ri.Image.Height;

        public PackedRect Rect { get; set; }

        private SKImage? _image;

        private static SKImage ConvertImage(LayerImage li) =>
            SKImage.FromPixelCopy(
                new SKImageInfo(
                    li.Width, li.Height,
                    SKColorType.Bgra8888, SKAlphaType.Unpremul),
                li.Read(), li.Width * 4);

        public void DrawOnCanvas(
            SKCanvas canvas, int canvasWidth, int canvasHeight)
        {
            this._image ??= ConvertImage(ri.Image);
            var opts = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None);
            using var paint = new SKPaint()
            {
                IsAntialias = true,
            };
            canvas.DrawImage(
                this._image,
                this.Rect.X,
                canvasHeight - this.Rect.Y - this.Rect.H,
                opts, paint);
        }

        public void Dispose() => this._image?.Dispose();
    }

    private class RectNode(PackedRect rect)
    {
        private bool _isOccupied;

        private RectNode? _rest0;

        private RectNode? _rest1;

        internal PackedRect? Insert(int srcW, int srcH, int margin)
        {
            // https://tyfkda.github.io/blog/2013/10/05/texture-pakcer.html

            if (this._isOccupied)
            {
                return this._rest0?.Insert(srcW, srcH, margin)
                    ?? this._rest1?.Insert(srcW, srcH, margin);
            }
            else
            {
                if (rect.W < srcW + margin
                    || rect.H < srcH + margin)
                {
                    return null;
                }

                var w = srcW + margin;
                var h = srcH + margin;
                var dw = rect.W - w;
                var dh = rect.H - h;
                if (dw < dh)
                {
                    this._rest0 = new(new(
                        rect.X + w, rect.Y, dw, h));
                    this._rest1 = new(new(
                        rect.X, rect.Y + h, rect.W, dh));
                }
                else
                {
                    this._rest0 = new(new(
                        rect.X, rect.Y + h, w, dh));
                    this._rest1 = new(new(
                        rect.X + w, rect.Y, dw, rect.H));
                }
                this._isOccupied = true;

                return new(rect.X, rect.Y, srcW, srcH);
            }
        }
    }

    private class SurfaceWrapper(int width) : IDisposable
    {
        private readonly SKSurface _surface = Create(width);

        private static SKSurface Create(int width)
        {
            var surf = SKSurface.Create(
                new SKImageInfo(
                    width, width,
                    SKColorType.Bgra8888, SKAlphaType.Unpremul));
            surf.Canvas.Clear(SKColors.Transparent);
            return surf;
        }

        internal void Draw(IPackableImage image)
            => image.DrawOnCanvas(
                this._surface.Canvas, width, width);

        internal void SaveSnapshotAsPng(string path)
        {
            this._surface.Canvas.Save();

            using var img = this._surface.Snapshot();
            using var data = img.Encode(SKEncodedImageFormat.Png, 100);
            using var fs = File.Create(path);
            data.SaveTo(fs);
        }

        public void Dispose() => this._surface.Dispose();
    }
}
