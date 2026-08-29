using FG.Mods.YSYard.QoL.Services;
using Foundation.UI;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(yr), nameof(yr.cnw))]
public static class PlotControlWindow_OnDestroy_Patch
{
    public static void Postfix()
    {
        ExUIManager.OnPlotControlWindowDestroyed();
    }
}
