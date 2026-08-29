using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace FG.Mods.YSYard.QoL.Installer
{
    internal class InstallInfo
    {
        internal string AppPath { get; set; } = string.Empty;

        internal string ModPath { get; set; } = string.Empty;

        internal Version ModVersion { get; set; }

        internal bool IsYsytransInstalled { get; set; }

        internal void SetModVersion(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            var fvi = FileVersionInfo.GetVersionInfo(filePath);
            if (fvi is null)
            {
                return;
            }

            var version = fvi.FileVersion;
            if (string.IsNullOrEmpty(version))
            {
                return;
            }

            var tokens = version.Split('.');
            if (tokens.Length < 2)
            {
                return;
            }

            var i = 0;
            var verNums = new List<int>();
            while (i < tokens.Length && int.TryParse(tokens[i], out var res))
            {
                verNums.Add(res);
                i++;
            }

            if (0 < verNums.Count && verNums.Count < 5)
            {
                this.ModVersion = new Version(string.Join(".", verNums));
            }
        }
    }
}
