using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimParallel(
    Parallel original, ISimulatedTask parent, int indexInParent)
    : SimulatedParentTask<Plot.Parallel>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation _) { }
}
