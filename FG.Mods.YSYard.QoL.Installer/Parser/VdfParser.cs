using System;
using System.Collections.Generic;
using System.IO;

namespace FG.Mods.YSYard.QoL.Installer.Parser
{
    internal static class VdfParser
    {
        internal static VdfObject Parse(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path}が存在しません");
            }
            var content = File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException($"{content}が空です");
            }

            var ret = new VdfObject();
            using (var sr = new StringReader(content))
            using (var vtr = new VdfTextReader(sr))
            {
                if (!vtr.ReadToken())
                {
                    throw new InvalidDataException("VDFファイルが不正です");
                }
                var kvp = ReadProperty(vtr);
                ret.Properties[kvp.Key] = kvp.Value;
            }
            return ret;
        }

        private static KeyValuePair<string, dynamic> ReadProperty(VdfTextReader vtr)
        {
            var key = vtr.Value;
            if (!vtr.ReadToken())
            {
                throw new InvalidDataException("VDFファイルが不正です");
            }
            return vtr.State == ReaderState.Property
                ? new KeyValuePair<string, dynamic>(key, vtr.Value.Replace(@"\\", @"\"))
                : new KeyValuePair<string, dynamic>(key, ReadObject(vtr));
        }

        private static VdfObject ReadObject(VdfTextReader vtr)
        {
            if (!vtr.ReadToken())
            {
                throw new InvalidDataException("VDFファイルが不正です");
            }

            var ret = new VdfObject();
            while (vtr.State != ReaderState.Object || vtr.Value != "}")
            {
                var kvp = ReadProperty(vtr);
                ret.Properties[kvp.Key] = kvp.Value;
                if (!vtr.ReadToken())
                {
                    throw new InvalidDataException("VDFファイルが不正です");
                }
            }
            return ret;
        }
    }
}
