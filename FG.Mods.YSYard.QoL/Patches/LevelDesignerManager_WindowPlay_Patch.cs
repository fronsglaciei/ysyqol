using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;
using System;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(
    typeof(bmo),
    nameof(bmo.ngi))]
public static class LevelDesignerManager_WindowPlay_Patch
{
    private static bool _usePatch = true;

    public static void Prefix(int b, Level c)
    {
        if (_usePatch)
        {
            ExLevelManager.OnLevelStart(b, c);
        }
    }

    public static void Postfix(Level c)
    {
        if (ConfigProvider.ForceInstantText.Value)
        {
            ExLevelManager.ClearAllDelays(c);
        }
    }

    internal static IDisposable RunOriginal()
        => new RunOriginalContext();

    private class RunOriginalContext : IDisposable
    {
        internal RunOriginalContext() => _usePatch = false;

        public void Dispose() => _usePatch = true;
    }
}
