using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimBackground(
    Plot.Action<bje> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bje>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //var bgData = this.Original.Data;
        var bgData = this.Original.bhlf;
        //var show = bgData.option == BackGroundOption.open;
        var show = bgData.bdkg == blt.open;
        //var resourceName = string.Format(GameConst.PLOT_BACKGROUND_PATH_FORMAT, bgData.resource);
        var resourceName = string.Format(cz.vgb, bgData.bdkh);
        //if (bgData.isCG)
        if (bgData.bdkk)
        {
            simulation.SetCGBackground(show, resourceName);
            if (show)
            {
                simulation.SetBackground(false, string.Empty);
            }
        }
        else
        {
            simulation.SetBackground(show, resourceName);
            if (show)
            {
                simulation.SetCGBackground(false, string.Empty);
            }
        }
    }
}
