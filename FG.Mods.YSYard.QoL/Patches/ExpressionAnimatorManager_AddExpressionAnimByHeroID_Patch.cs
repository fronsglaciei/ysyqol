using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using UnityEngine;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(dx), nameof(dx.dci))]
public static class ExpressionAnimatorManager_AddExpressionAnimByHeroID_Patch
{
    public static void Prefix(
        int a, Transform b,
        ref bool __runOriginal, dx __instance, ref ExpressionAnimator __result)
    {
        __runOriginal = true;

        if (!MissingAssetProvider.TryGetExpressionAnimator(a, out var ea)
            || ea is null)
        {
            return;
        }
        __runOriginal = false;

        ea.gameObject.SetActive(true);
        ea.transform.SetParent(b);

        //ea.ResetExpressionAnim();
        ea.eck();
        ea.transform.localPosition = Vector3.zero;
        //__instance.PlayExpressionAnimByID(690000, ea, false, false);
        __instance.dcj(690000, ea, false, false);

        __result = ea;
    }
}
