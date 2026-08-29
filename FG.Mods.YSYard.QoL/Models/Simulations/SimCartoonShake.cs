using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimCartoonShake(
    Plot.Action<blh> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<blh>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation _) { }
}
