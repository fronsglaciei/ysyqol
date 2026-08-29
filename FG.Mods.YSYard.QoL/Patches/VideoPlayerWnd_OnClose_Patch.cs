using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(iz), nameof(iz.cnp))]
public static class VideoPlayerWnd_OnClose_Patch
{
    public static void Postfix(iz __instance)
    {
        SubtitleManager.ClearSrt(__instance);
    }
}
