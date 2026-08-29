using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(iz), nameof(iz.cnm))]
public static class VideoPlayerWnd_OnInit_Patch
{
    public static void Postfix(iz __instance)
    {
        SubtitleManager.OnVideoPlayerInit(__instance);
    }
}
