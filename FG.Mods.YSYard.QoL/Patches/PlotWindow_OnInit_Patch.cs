using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(PlotWindow), nameof(PlotWindow.njh))]
public static class PlotWindow_OnInit_Patch
{
    public static void Postfix(PlotWindow __instance)
    {
        ExUIManager.OnPlotWindowInit(__instance);
    }
}
