using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimMoveTo(
    Action<bjv> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bjv>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //var mtData = this.Original.Data;
        var mtData = this.Original.bhlf;
        //if (!string.IsNullOrEmpty(mtData.resource))
        if (!string.IsNullOrEmpty(mtData.bdoi))
        {
            //simulation.SetImageResource(mtData.image, mtData.resource);
            simulation.SetImageResource(mtData.bdog, mtData.bdoi);
            //simulation.SetImageVisible(mtData.image, true);
            simulation.SetImageVisible(mtData.bdog, true);
        }
        //simulation.SetImagePosition(mtData.image, mtData.toPostion.GetVector3());
        simulation.SetImagePosition(mtData.bdog, mtData.bdoh.mkq());
    }
}
