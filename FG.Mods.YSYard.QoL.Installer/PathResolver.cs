using FG.Mods.YSYard.QoL.Installer.Parser;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace FG.Mods.YSYard.QoL.Installer
{
    internal static partial class PathResolver
    {
        internal static bool TryGetSteamPath(out string steamPath)
        {
            steamPath = string.Empty;

            var pfx86 = Environment.GetFolderPath(
                Environment.SpecialFolder.ProgramFilesX86);
            if (string.IsNullOrEmpty(pfx86))
            {
                return false;
            }

            steamPath = Path.Combine(pfx86, STEAM_DIR_NAME);
            if (Directory.Exists(steamPath))
            {
                return true;
            }

            using (var regBase = RegistryKey.OpenBaseKey(
                RegistryHive.LocalMachine,
                Environment.Is64BitOperatingSystem
                    ? RegistryView.Registry64 : RegistryView.Registry32))
            {
                var regSub = regBase.OpenSubKey(
                    Environment.Is64BitOperatingSystem
                        ? STEAM_REG_KEY_PATH_64 : STEAM_REG_KEY_PATH_32);
                if (regSub is null)
                {
                    return false;
                }
                using (regSub)
                {
                    var tmp = regSub.GetValue(STEAM_REG_INSTALL_PATH);
                    if (tmp is null)
                    {
                        return false;
                    }
                    steamPath = tmp as string;
                    return Directory.Exists(steamPath);
                }
            }
        }

        internal static bool TryGetAppPath(string steamPath, out string appPath)
        {
            appPath = string.Empty;

            var libfldVdfPath = Path.Combine(steamPath, STEAM_CONFIG_LIBFOLDER_VDF_PATH);
            var folders = new List<string>();
            if (File.Exists(libfldVdfPath))
            {
                try
                {
                    var rootObj = VdfParser.Parse(libfldVdfPath);
                    if (!(rootObj is null)
                        && rootObj.Properties.TryGetValue<string, VdfObject>(LIB_FOLDER_PROP_NAME, out var libFld))
                    {
                        var i = 0;
                        while (libFld.Properties.TryGetValue($"{i}", out var dynVal))
                        {
                            if (dynVal is string strVal)
                            {
                                folders.Add(strVal);
                            }
                            else if (dynVal is VdfObject objVal)
                            {
                                if (objVal.Properties.TryGetValue<string, string>(
                                    PATH_PROP_NAME, out var strPath))
                                {
                                    if (objVal.Properties.TryGetValue<string, VdfObject>(
                                        APPS_PROP_NAME, out var objApps)
                                        && objApps.Properties.ContainsKey(APP_ID))
                                    {
                                        var appInstDir = Path.Combine(
                                            strPath, STEAM_APPS_PATH, COMMON_PATH, APP_DIR_NAME);
                                        if (Directory.Exists(appInstDir))
                                        {
                                            appPath = appInstDir;
                                            return true;
                                        }
                                    }
                                    folders.Add(strPath);
                                }
                            }
                            i++;
                        }
                    }
                }
                catch { }
            }
            foreach (var folder in folders)
            {
                var dirPath = Path.Combine(folder, STEAM_APPS_PATH);
                if (!Directory.Exists(dirPath))
                {
                    continue;
                }

                foreach (var filePath in Directory.EnumerateFiles(dirPath))
                {
                    if (!filePath.EndsWith(EXT_ACF, StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                    try
                    {
                        var acf = VdfParser.Parse(filePath);
                        if (!(acf is null)
                            && acf.Properties.TryGetValue<string, string>(
                                APP_ID_PROP_NAME, out var appId)
                            && appId == APP_ID
                            && acf.Properties.TryGetValue<string, string>(
                                APP_INSTALL_PATH_PROP_NAME, out var appInstPath)
                            && Directory.Exists(appInstPath))
                        {
                            appPath = appInstPath;
                            return true;
                        }
                    }
                    catch { }
                }
            }
            return false;
        }

        internal static bool IsDirectoryWritable(string path)
        {
            try
            {
                var tmpPath = Path.Combine(path, "mytemp");
                _ = Directory.CreateDirectory(tmpPath);
                Directory.Delete(tmpPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch
            {
                throw;
            }
        }

        internal static string GetPluginsPath(string appPath)
            => Path.Combine(appPath, PLUGINS_DIR_PATH);

        internal static string GetModPath(string appPath)
            => Path.Combine(
                appPath, PLUGINS_DIR_PATH, MOD_INSTALL_DIR_NAME);

        internal static string GetModAssemblyPath(string modPath)
            => Path.Combine(modPath, MOD_ASSEMBLY_FILE_NAME);

        internal static string GetPatchersPath(string appPath)
            => Path.Combine(appPath, PATCHERS_DIR_PATH);

        internal static string GetYsytransPath(string appPath)
            => Path.Combine(GetPluginsPath(appPath), YSYTRANS_DIR_PATH);
    }
}
