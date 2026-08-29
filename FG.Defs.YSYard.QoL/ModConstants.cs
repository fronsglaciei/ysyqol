using System.Collections.Generic;

namespace FG.Defs.YSYard.QoL
{
    public class ModConstants
    {
        public const string FILENAME_MOD_CONSTANTS = "constants.json";

        public const string FILENAME_STAGING_MOD_TEXTS = "staging.json";

        public const string FILENAME_TL_DATA = "tldata.json";

        public const string BASENAME_CONTROLS = "controls";

        public Dictionary<string, ModText> Texts { get; set; }
            = new Dictionary<string, ModText>();

        public Dictionary<string, TextureRegion> ControlsTextureRegions { get; set; }
            = new Dictionary<string, TextureRegion>();
    }
}
