using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimModifyAttribute(
    Action<bll> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bll>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation _) { }
}
