using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimOpenWindow(
    Action<bjw> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bjw>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation _) { }
}
