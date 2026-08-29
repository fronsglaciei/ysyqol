using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimPlaySound(
    Action<bjb> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bjb>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation _) { }
}
