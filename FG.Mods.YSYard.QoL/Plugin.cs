using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using System.Reflection;

namespace FG.Mods.YSYard.QoL;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;

    public override void Load()
    {
        Log = base.Log;

        ConfigProvider.Init(this.Config);

        ModAssetProvider.LoadTranslations();

        Harmony.CreateAndPatchAll(
            Assembly.GetExecutingAssembly());
    }
}
