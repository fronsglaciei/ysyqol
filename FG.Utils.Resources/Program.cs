//
// genassets.exe
//

// CommandLineParser
// MIT License
// https://github.com/commandlineparser/commandline

// PsdParser
// MIT License
// https://github.com/manju-summoner/PsdParser

// SkiaSharp
// MIT License
// https://github.com/mono/SkiaSharp

using CommandLine;
using FG.Defs.YSYard.QoL;
using System.Text.Json;

namespace FG.Utils.Resources;

internal class Program
{
    private static void Main(string[] args)
    {
        var parseRes = Parser.Default.ParseArguments<Options>(args);
        if (parseRes.Tag != ParserResultType.Parsed
            || parseRes is not Parsed<Options> opts)
        {
            Console.Error.WriteLine("Invalid command line args");
            return;
        }

        var dir = opts.Value.InputDirectory;
        if (string.IsNullOrEmpty(dir)
            || !Directory.Exists(dir))
        {
            Console.Error.WriteLine($"Invalid input directory : {dir}");
            return;
        }

        var stagingPath = Path.Combine(dir, $"{ModConstants.FILENAME_STAGING_MOD_TEXTS}");
        var stagingModTexts = JsonSerializer.Deserialize<StagingModTexts>(
            File.ReadAllText(stagingPath));
        if (stagingModTexts == null)
        {
            Console.Error.WriteLine($"Failed to deserialize staging mod texts : {stagingPath}");
            return;
        }

        var psdPath = Path.Combine(dir, $"{ModConstants.BASENAME_CONTROLS}.psd");
        if (!File.Exists(psdPath))
        {
            Console.Error.WriteLine($"Failed to get PSD file : {psdPath}");
            return;
        }

        var pngPath = Path.Combine(dir, $"{ModConstants.BASENAME_CONTROLS}.png");
        if (!PsdLayerPacker.TrySavePackedTexture(
            psdPath, pngPath, out var textureRegions))
        {
            Console.Error.WriteLine("Input PSD file is invalid");
            return;
        }

        var modConstsPath = Path.Combine(dir, $"{ModConstants.FILENAME_MOD_CONSTANTS}");
        var modConstsJson = JsonSerializer.Serialize(new ModConstants
        {
            Texts = stagingModTexts.Texts,
            ControlsTextureRegions = textureRegions
        });
        File.WriteAllText(modConstsPath, modConstsJson);
    }

    private class Options
    {
        [Option('i', Required = true)]
        public string InputDirectory { get; set; } = string.Empty;
    }
}
