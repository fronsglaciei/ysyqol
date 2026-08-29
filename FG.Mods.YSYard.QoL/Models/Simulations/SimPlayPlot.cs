using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimPlayPlot(
    Action<blj> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<blj>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation _) { }
}
