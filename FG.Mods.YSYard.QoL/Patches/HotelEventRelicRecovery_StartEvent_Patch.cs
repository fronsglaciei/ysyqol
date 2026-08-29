using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using HotelModule.Event;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(
    typeof(sx),
    nameof(sx.ifc))]
public static class HotelEventRelicRecovery_StartEvent_Patch
{
    public static void Prefix(
        sx __instance,
        ref bool __runOriginal)
    {
        if (ConfigProvider.SkipBloodMoonAnimations.Value)
        {
            __runOriginal = false;
            ExArtifactManager.ShowMonthArtifactGroup(__instance);
        }
        else
        {
            __runOriginal = true;
        }
    }
}
