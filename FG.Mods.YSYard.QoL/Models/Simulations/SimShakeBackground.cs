using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimShakeBackground(
    Action<blo> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<blo>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation _) { }
}
