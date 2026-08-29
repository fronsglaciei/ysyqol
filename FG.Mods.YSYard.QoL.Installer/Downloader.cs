using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FG.Mods.YSYard.QoL.Installer
{
    internal static class Downloader
    {
        private static readonly HttpClient _client = new HttpClient(new HttpClientHandler
        {
            MaxConnectionsPerServer = 2
        });

        private static readonly ConcurrentDictionary<Uri, bool> _dlLocks
            = new ConcurrentDictionary<Uri, bool>();

        internal static async Task DownloadFileAsync(
            Uri uri, string dstPath,
            Action<long, long> onProgress, Action<Exception> onError,
            CancellationToken token)
        {
            if (_dlLocks.TryGetValue(uri, out _))
            {
                onError?.Invoke(
                    new InvalidOperationException($"{uri}のダウンロードは進行中です"));
                return;
            }
            _dlLocks.TryAdd(uri, true);

            var res = await _client
                .GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, token)
                .ConfigureAwait(false);
            var len = res?.Content?.Headers.ContentLength ?? -1;
            if (len < 1)
            {
                onError?.Invoke(new HttpRequestException($"{uri}のダウンロードに失敗しました"));
                _dlLocks.TryRemove(uri, out _);
                return;
            }

            try
            {
                using (var fs = File.Create(dstPath))
                using (var iStream = await
                    res.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var buf = new byte[0x10000];
                    var total = 0L;
                    while (true)
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        var tmpRead = await iStream
                            .ReadAsync(buf, 0, 0x10000, token).ConfigureAwait(false);
                        if (tmpRead < 1)
                        {
                            break;
                        }
                        await fs.WriteAsync(buf, 0, tmpRead, token).ConfigureAwait(false);

                        total += tmpRead;
                        onProgress?.Invoke(total, len);
                    }
                }
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
            finally
            {
                _dlLocks.TryRemove(uri, out _);
            }
        }
    }
}
