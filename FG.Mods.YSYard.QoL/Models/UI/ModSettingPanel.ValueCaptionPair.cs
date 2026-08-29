using System;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal partial class ModSettingPanel
{
    internal class ValueCaptionPair<T> where T : IEquatable<T>
    {
        internal T Value { get; set; }

        internal string CaptionKey { get; set; }

        internal Text CaptionText { get; set; }
    }
}
