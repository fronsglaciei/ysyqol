using FG.Mods.YSYard.QoL.Services;
using Foundation.UI;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(LoginPage), nameof(LoginPage.cnm))]
public static class LoginPage_OnInit_Patch
{
    public static void Prefix()
    {
        FontManager.Init();
        ExSaveLoadManager.ExGlobalLoad(null);
    }
}
