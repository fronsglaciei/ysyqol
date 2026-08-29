using HotelModule.BlockPhaseEvent;

namespace FG.Mods.YSYard.QoL.Services;

internal static class ExBlockPhaseEventManager
{
    internal static void ClearAll()
    {
        //BlockPhaseEventManager.Instance.ClearAll();
        uc.bhfz.ihf();

        ExCommonEventManager.UnregisterAllInstanceMethods(
            //5, [ nameof(PlayPlotBlockEvent) ]);
            5, [ nameof(uh) ]);
    }

    internal static void ExecuteEventWithoutStart(
        //BlockPhaseEvent ev, bool endOld = false)
        ub ev, bool endOld = false)
    {
        //var old = BlockPhaseEventManager.Instance._curEvent;
        var old = uc.bhfz.ypc;
        //BlockPhaseEventManager.Instance._curEvent = ev;
        uc.bhfz.ypc = ev;
        if (endOld)
        {
            //old?.EndEvent();
            old?.igw();
        }
    }
}
