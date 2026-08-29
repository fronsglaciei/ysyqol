using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimCartoonSet(
    Plot.Action<blf> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<blf>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //var csData = this.Original.Data;
        var csData = this.Original.bhlf;
        //if (csData.closeImage)
        if (csData.bdxi)
        {
            simulation.HideAllCartoons();
        }
        else
        {
            //simulation.ShowCartoon(
            //    csData.targetImage,
            //    csData.positionOffset.GetVector3(),
            //    csData.image);
            simulation.ShowCartoon(
                csData.bdxj,
                csData.bdxe.mkq(),
                csData.bdxd);
        }
    }
}
