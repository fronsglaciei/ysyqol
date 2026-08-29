using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FG.Mods.YSYard.QoL.Installer
{
    internal static class Installer
    {
        private const double PROG_APP_DIR_CHECK = 0.04;
        private const double PROG_WEIGHT_DL = 0.9;
        private const double PROG_EXTRACT_BEPINEX = 0.97;
        private const double PROG_COMPLETE = 1.0;

        private const double PROG_DELETE_MOD = 0.3;

        private static readonly object _lock = new object();

        private static readonly Regex _reZip = new Regex(@"\.[zZ][iI][pP]$");

        internal static InstallInfo GetInstallInfo(
            out string errorMessage, string userSelectAppPath = "")
        {
            errorMessage = string.Empty;
            var ret = new InstallInfo();

            var appPath = userSelectAppPath;
            if (string.IsNullOrEmpty(appPath))
            {
                if (!PathResolver.TryGetSteamPath(out var steamPath))
                {
                    errorMessage = "Steamがインストールされていないか、インストール先ディレクトリが正常に取得できませんでした。";
                    return ret;
                }
                if (!PathResolver.TryGetAppPath(steamPath, out appPath))
                {
                    errorMessage = "インストール情報の読み取りに失敗しました。ゲームがインストールされていないか、steamの構成が想定されていない形式です。";
                    return ret;
                }
            }

            if (!PathResolver.IsDirectoryWritable(appPath))
            {
                errorMessage = "このユーザーアカウントの権限ではゲームのディレクトリに書き込みができません。\nヒント：ゲームのインストール先をこのユーザーアカウントの書き込み権限があるディレクトリにしてください。\nProgram FIles以下など、UACによって書き込みが禁止される場合の動作については未確認のため、サポート対象外です。";
                return ret;
            }
            ret.AppPath = appPath;

            var tmpModPath = PathResolver.GetModPath(appPath);
            if (Directory.Exists(tmpModPath))
            {
                ret.ModPath = tmpModPath;
                ret.SetModVersion(PathResolver.GetModAssemblyPath(tmpModPath));
            }

            var ysytransPath = PathResolver.GetYsytransPath(appPath);
            if (Directory.Exists(ysytransPath))
            {
                ret.IsYsytransInstalled = true;
            }

            return ret;
        }

        internal static async Task<InstallInfo> InstallAsync(
            string appPath,
            Action<double> onProgress, Action<Exception> onError,
            CancellationToken token)
        {
            if (string.IsNullOrEmpty(appPath))
            {
                throw new ArgumentException($"{nameof(appPath)}");
            }
            var ret = new InstallInfo
            {
                AppPath = appPath,
            };

            #region download bepin and mod
            if (token.IsCancellationRequested) { return null; }
            try
            {
                Directory.CreateDirectory(PathResolver.TempDownloadDirPath);

                var xBepin = 0L;
                var lenBepin = 0L;
                var xMod = 0L;
                var lenMod = 0L;

                var tBepInEx = Downloader.DownloadFileAsync(
                    PathResolver.BepInExUri,
                    PathResolver.TempBepInZipPath,
                    (x, len) =>
                    {
                        lock (_lock)
                        {
                            xBepin = x;
                            lenBepin = len;
                        }
                        var total = lenBepin + lenMod;
                        if (total == 0)
                        {
                            return;
                        }
                        onProgress?.Invoke(
                            (xBepin + xMod) / total * PROG_WEIGHT_DL + PROG_APP_DIR_CHECK);
                    },
                    ex => onError?.Invoke(ex), token);
                var tMod = Downloader.DownloadFileAsync(
                    PathResolver.ModUri,
                    PathResolver.TempModZipPath,
                    (x, len) =>
                    {
                        lock (_lock)
                        {
                            xMod = x;
                            lenMod = len;
                        }
                        var total = lenBepin + lenMod;
                        if (total == 0)
                        {
                            return;
                        }
                        onProgress?.Invoke(
                            (xBepin + xMod) / total * PROG_WEIGHT_DL + PROG_APP_DIR_CHECK);
                    },
                    ex => onError?.Invoke(ex), token);
                await tBepInEx.ConfigureAwait(false);
                await tMod.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                return null;
            }
            #endregion

            #region extract bepin and mod
            if (token.IsCancellationRequested) { return null; }
            try
            {
                ExtractBepInEx(appPath);
                onProgress?.Invoke(PROG_EXTRACT_BEPINEX);

                var modPath = PathResolver.GetModPath(appPath);
                Directory.CreateDirectory(modPath);
                ret.ModPath = modPath;

                ZipFile.ExtractToDirectory(
                    PathResolver.TempModZipPath,
                    modPath);
                ret.SetModVersion(PathResolver.GetModAssemblyPath(modPath));
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                return null;
            }
            #endregion

            try
            {
                Directory.Delete(
                    PathResolver.TempDownloadDirPath, true);
            }
            catch { }
            onProgress?.Invoke(PROG_COMPLETE);
            return ret;
        }

        internal static bool Uninstall(
            string appPath,
            Action<double> onProgress, Action<Exception> onError,
            CancellationToken token)
        {
            if (string.IsNullOrEmpty(appPath))
            {
                throw new ArgumentException($"{nameof(appPath)}");
            }

            #region delete mod
            if (token.IsCancellationRequested) { return false; }
            try
            {
                Directory.Delete(PathResolver.GetModPath(appPath), true);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                return false;
            }
            onProgress?.Invoke(PROG_DELETE_MOD);
            #endregion

            #region delete bepin if empty
            if (token.IsCancellationRequested) { return false; }
            try
            {
                if (0 < Directory.GetFileSystemEntries(
                    PathResolver.GetPluginsPath(appPath)).Length)
                {
                    onProgress?.Invoke(PROG_COMPLETE);
                    return true;
                }
                var patchersDir = PathResolver.GetPatchersPath(appPath);
                if (Directory.Exists(patchersDir)
                    && 0 < Directory.GetFileSystemEntries(patchersDir).Length)
                {
                    onProgress?.Invoke(PROG_COMPLETE);
                    return true;
                }

                foreach (var entry in PathResolver.BepInExFiles)
                {
                    var src = Path.Combine(appPath, entry);
                    if (File.Exists(src))
                    {
                        File.Delete(src);
                        continue;
                    }
                    if (Directory.Exists(src))
                    {
                        Directory.Delete(src, true);
                    }
                }
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                return false;
            }
            #endregion

            onProgress?.Invoke(PROG_COMPLETE);
            return true;
        }

        private static void ExtractBepInEx(string appPath)
        {
            var tmpExtract = _reZip.Replace(
                PathResolver.TempBepInZipPath, string.Empty);
            if (Directory.Exists(tmpExtract))
            {
                Directory.Delete(tmpExtract, true);
            }
            Directory.CreateDirectory(tmpExtract);
            ZipFile.ExtractToDirectory(
                PathResolver.TempBepInZipPath,
                tmpExtract);

            foreach (var entry in PathResolver.BepInExFiles)
            {
                var src = Path.Combine(tmpExtract, entry);
                if (File.Exists(src))
                {
                    File.Copy(src, Path.Combine(appPath, entry), true);
                    continue;
                }
                if (Directory.Exists(src))
                {
                    CopyDirectory(src, Path.Combine(appPath, entry));
                }
            }
        }

        private static void CopyDirectory(string srcDir, string dstDir)
        {
            if (!Directory.Exists(srcDir))
            {
                throw new DirectoryNotFoundException();
            }
            Directory.CreateDirectory(dstDir);

            foreach (var file in Directory.EnumerateFiles(srcDir))
            {
                File.Copy(
                    file, Path.Combine(dstDir, Path.GetFileName(file)),
                    true);
            }
            foreach (var dir in Directory.EnumerateDirectories(srcDir))
            {
                CopyDirectory(
                    dir, Path.Combine(dstDir, Path.GetFileName(dir)));
            }
        }

        internal static bool UninstallYsytrans(
            string appPath, Action<Exception> onError)
        {
            if (string.IsNullOrEmpty(appPath))
            {
                throw new ArgumentException($"{nameof(appPath)}");
            }

            try
            {
                Directory.Delete(PathResolver.GetYsytransPath(appPath), true);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                return false;
            }
            return true;
        }
    }
}
