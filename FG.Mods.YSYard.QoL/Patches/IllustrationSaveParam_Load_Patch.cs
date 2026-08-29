using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using SaveLoadSystem;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(IllustrationSaveParam), nameof(IllustrationSaveParam.Load))]
public static class IllustrationSaveParam_Load_Patch
{
    public static void Postfix(IllustrationSaveParam __instance)
    {
        if (__instance.illustrationFinished is null)
        {
            return;
        }
        ExSaveLoadManager.ExGlobalSave(x =>
        {
            foreach (var id in __instance.illustrationFinished)
            {
                x.VisitedPlayGuides.Add(id);
            }
        });
    }
}
