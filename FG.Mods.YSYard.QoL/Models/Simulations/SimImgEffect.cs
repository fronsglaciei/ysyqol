using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimImgEffect(
    Action<bjp> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bjp>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation _) { }
}
