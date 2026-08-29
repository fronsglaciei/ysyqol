using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimParallelComplete(
    ParallelComplete original, ISimulatedTask parent, int indexInParent)
    : SimulatedParentTask<ParallelComplete>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation _) { }
}
