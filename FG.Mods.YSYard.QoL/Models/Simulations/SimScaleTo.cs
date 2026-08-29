using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimScaleTo(
    Action<bkj> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bkj>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //var stData = this.Original.Data;
        var stData = this.Original.bhlf;
        //simulation.SetImageScale(stData.character, stData.toScale.GetVector3());
        simulation.SetImageScale(stData.bdsx, stData.bdtb.mkq());
    }
}
