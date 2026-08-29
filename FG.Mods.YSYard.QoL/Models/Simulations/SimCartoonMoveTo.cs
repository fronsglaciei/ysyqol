using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimCartoonMoveTo(
    Plot.Action<blg> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<blg>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //var cmtData = this.Original.Data;
        var cmtData = this.Original.bhlf;
        //simulation.SetImagePosition(cmtData.image, cmtData.toPostion.GetVector3());
        simulation.SetImagePosition(cmtData.bdxk, cmtData.bdxl.mkq());
    }
}
