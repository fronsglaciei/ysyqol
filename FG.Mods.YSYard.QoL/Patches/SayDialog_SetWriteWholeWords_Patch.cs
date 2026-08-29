using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using PlotDesigher;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(SayDialog), nameof(SayDialog.ltq))]
public static class SayDialog_SetWriteWholeWords_Patch
{
    public static void Postfix(bool a, SayDialog __instance)
    {
        if (!ConfigProvider.ForceInstantText.Value)
        {
            return;
        }
        if (!a && __instance.gameObject is not null)
        {
            //__instance.GetWriter().writeWholeWords = true;
            __instance.lsy().writeWholeWords = true;
        }
    }
}
