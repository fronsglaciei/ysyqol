using Common;
using System.Collections.Generic;

namespace FG.Mods.YSYard.QoL.Services;

internal static class ExCommonEventManager
{
    internal static void UnregisterAllInstanceMethods(
        int eventType, List<string> instTypeNames)
    {
        if (instTypeNames is null
            //|| !EventManager.m_EventTable.TryGetValue(
            || !wr.zhg.TryGetValue(
                eventType, out var delegates))
        {
            return;
        }

        var removals = new List<Il2CppSystem.Delegate>();
        foreach (var d in delegates)
        {
            if (d.Target is null)
            {
                continue;
            }
            if (instTypeNames.Contains(
                d.Target.GetIl2CppType().Name))
            {
                removals.Add(d);
            }
        }
        foreach (var d in removals)
        {
            delegates.Remove(d);
        }
    }
}
