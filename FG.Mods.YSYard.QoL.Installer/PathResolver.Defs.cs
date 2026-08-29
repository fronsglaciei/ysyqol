using System;
using System.IO;

namespace FG.Mods.YSYard.QoL.Installer
{
    internal static partial class PathResolver
    {
        private const string APP_ID = "2194530";
        private const string APP_DIR_NAME = "Yog-Sothoth's Yard";

        private const string MOD_URL = "https://github.com/fronsglaciei/ysyqol/releases/latest/download/FG.Mods.YSYard.QoL.zip";
        private const string MOD_DOWNLOAD_DIR_NAME = "ysyqol";
        private const string MOD_INSTALL_DIR_NAME = "FG.Mods.YSYard.QoL";
        private const string MOD_ASSEMBLY_FILE_NAME = "FG.Mods.YSYard.QoL.dll";

        private const string BEPINEX_URL = "https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip";

        private const string STEAM_DIR_NAME = "Steam";
        private const string STEAM_REG_KEY_PATH_64 = @"SOFTWARE\WOW6432Node\Valve\Steam";
        private const string STEAM_REG_KEY_PATH_32 = @"SOFTWARE\Valve\Steam";
        private const string STEAM_REG_INSTALL_PATH = "InstallPath";
        private const string STEAM_CONFIG_LIBFOLDER_VDF_PATH = @"config\libraryfolders.vdf";
        private const string LIB_FOLDER_PROP_NAME = "libraryfolders";
        private const string PATH_PROP_NAME = "path";
        private const string APPS_PROP_NAME = "apps";
        private const string STEAM_APPS_PATH = "steamapps";
        private const string COMMON_PATH = "common";
        private const string EXT_ACF = ".acf";
        private const string APP_ID_PROP_NAME = "appid";
        private const string APP_INSTALL_PATH_PROP_NAME = "installdir";

        private const string PLUGINS_DIR_PATH = @"BepInEx\plugins";
        private const string PATCHERS_DIR_PATH = @"BepInEx\patchers";

        private const string YSYTRANS_DIR_PATH = "FG.Mods.YSYard.Translations";

        internal static Uri BepInExUri { get; } = new Uri(BEPINEX_URL);

        internal static Uri ModUri { get; } = new Uri(MOD_URL);

        internal static string TempDownloadDirPath { get; }
            = Path.Combine(Path.GetTempPath(), MOD_DOWNLOAD_DIR_NAME);

        internal static string TempBepInZipPath { get; }
            = Path.Combine(TempDownloadDirPath, "bepinex.zip");

        internal static string TempModZipPath { get; }
            = Path.Combine(TempDownloadDirPath, "mod.zip");

        internal static string[] BepInExFiles { get; }
            = new string[]
            {
                "winhttp.dll", "doorstop_config.ini", "BepInEx", "dotnet"
            };
    }
}
