using FiniteStateMachine;

namespace FG.Mods.YSYard.QoL.Helpers;

internal static class FSMStateMachineExtensions
{
    internal static void SwitchStateWithoutExitEnter(
        this wm fsm, int dstState)
    {
        //if (!fsm.stateDic.TryGetValue(dstState, out var state))
        if (!fsm.zgn.TryGetValue(dstState, out var state))
        {
            return;
        }

        //var old = fsm.CurState;
        var old = fsm.zgm;
        //state.canUpdate = true;
        state.zgu = true;
        //fsm.CurState = state;
        fsm.zgm = state;
        //old.canUpdate = false;
        old?.zgu = false;
    }
}
