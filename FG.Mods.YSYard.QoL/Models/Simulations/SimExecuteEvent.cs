using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimExecuteEvent(
    Action<blk> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<blk>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation _) { }
}
