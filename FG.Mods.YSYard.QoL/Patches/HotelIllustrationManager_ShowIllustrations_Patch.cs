using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(
    typeof(er),
    nameof(er.dkl))]
public static class HotelIllustrationManager_ShowIllustrations_Patch
{
    public static void Prefix(int a, ref bool __runOriginal)
    {
        if (!ConfigProvider.SkipAutoOpenVisitedPlayGuides.Value)
        {
            __runOriginal = true;
            return;
        }

        if (ExSaveLoadManager.ExGlobalSaveDataCache.VisitedPlayGuides.Contains(a))
        {
            __runOriginal = false;
        }
        else
        {
            __runOriginal = true;
            return;
        }

        ExHotelIllustrationManager.SetIllustrationFinished(a);
    }
}
