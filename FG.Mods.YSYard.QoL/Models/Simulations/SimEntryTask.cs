using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimEntryTask(
    EntryTask original, ISimulatedTask parent, int indexInParent)
    : SimulatedParentTask<EntryTask>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation _) { }
}
