using FG.Mods.YSYard.QoL.Services;
using Foundation.UI;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(bbu), nameof(bbu.kmg))]
public static class HotelIllustration_SetData_Patch
{
    public static void Postfix(int a)
        => ExSaveLoadManager.ExGlobalSave(x => x.VisitedPlayGuides.Add(a));
}
