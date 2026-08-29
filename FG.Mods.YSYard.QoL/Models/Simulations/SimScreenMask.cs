using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimScreenMask(
    Action<bkl> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bkl>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //var smData = this.Original.Data;
        var smData = this.Original.bhlf;
        //var show = smData.option == BackGroundOption.open;
        var show = smData.bdtp == blt.open;
        //var resourceName = string.Format(GameConst.PLOT_BACKGROUND_PATH_FORMAT, smData.resource);
        var resourceName = string.Format(cz.vgb, smData.bdtq);
        //simulation.SetBackgroundMask(show, resourceName, smData.color.GetColor());
        simulation.SetBackgroundMask(show, resourceName, smData.bdtr.mgn());
    }
}
