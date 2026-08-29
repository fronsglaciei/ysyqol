using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(er), nameof(er.dkh))]
public static class HotelIllustrationManager_PushIllustrations_Patch
{
    public static void Prefix(
        int a, Il2CppSystem.Action b, ref bool __runOriginal)
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

        b?.Invoke();
    }
}
