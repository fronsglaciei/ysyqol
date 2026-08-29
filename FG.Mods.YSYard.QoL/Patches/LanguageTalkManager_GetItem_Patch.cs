using Example;
using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(hn), nameof(hn.GetItem))]
public static class LanguageTalkManager_GetItem_Patch
{
    public static void Postfix(ref LanguageTalk __result, int key)
    {
        if (ConfigProvider.UseModTranslations.Value
            && __result is not null
            && ModAssetProvider.TryGetLanguageTalkTranslation(key, out var translation))
        {
            //__result.LanguageJP = translation;
            __result.xmr = translation;
        }
    }
}
