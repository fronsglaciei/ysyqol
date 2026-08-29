using Il2CppInterop.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace FG.Mods.YSYard.QoL.Services;

internal static class FontManager
{
    private static readonly Dictionary<FontInfoPair, Font> _createdFont = [];

    internal static List<string> OSFonts { get; } = [];

    internal static void Init()
    {
        OSFonts.Clear();
        OSFonts.AddRange(Font.GetOSInstalledFontNames());
    }

    internal static Font CreateOSFont(string fontName, int fontSize)
    {
        if (string.IsNullOrEmpty(fontName))
        {
            Plugin.Log.LogError("Cannot create OS font; argument is null");
            return null;
        }

        var key = new FontInfoPair
        {
            FontName = fontName,
            FontSize = fontSize
        };
        if (_createdFont.TryGetValue(key, out var cachedFont))
        {
            return cachedFont;
        }

        if (!OSFonts.Contains(fontName))
        {
            Plugin.Log.LogError($"Cannot create OS font; \"{fontName}\" is not installed");
            return null;
        }

        // Font.ctor is stripped while Font.Internal_CreateDynamicFont is unstripped
        // -> you need some tricks to create new Font instance
        var ret = new Font(
            IL2CPP.il2cpp_object_new(
                Il2CppClassPointerStore<Font>.NativeClassPtr));
        Font.Internal_CreateDynamicFont(ret, new([fontName]), fontSize);
        _createdFont[key] = ret;

        return ret;
    }

    private record FontInfoPair
    {
        internal string FontName { get; set; }

        internal int FontSize { get; set; }
    }
}
