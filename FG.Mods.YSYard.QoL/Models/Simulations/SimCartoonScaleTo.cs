using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimCartoonScaleTo(
    Plot.Action<bli> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bli>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //var cstData = this.Original.Data;
        var cstData = this.Original.bhlf;
        //simulation.SetImageScale(cstData.image, cstData.toScale.GetVector3());
        simulation.SetImageScale(cstData.bdxt, cstData.bdxu.mkq());
    }
}
