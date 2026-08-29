using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(iz), nameof(iz.ehs))]
public static class VideoPlayerWnd_BuffingVideo_Patch
{
    public static void Postfix(string a, iz __instance)
    {
        if (ConfigProvider.UseModTranslations.Value)
        {
            SubtitleManager.SetSrt(__instance, a);
        }
        else
        {
            SubtitleManager.ClearSrt(__instance);
        }
    }
}
