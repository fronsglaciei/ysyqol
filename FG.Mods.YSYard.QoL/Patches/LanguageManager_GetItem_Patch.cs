using Example;
using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(hm), nameof(hm.GetItem))]
public static class LanguageManager_GetItem_Patch
{
    public static void Postfix(ref Language __result, int key)
    {
        if (ConfigProvider.UseModTranslations.Value
            && __result is not null
            && ModAssetProvider.TryGetLanguageTranslation(key, out var translation))
        {
            //__result.LanguageJpn = translation;
            __result.xmi = translation;
        }
    }
}
