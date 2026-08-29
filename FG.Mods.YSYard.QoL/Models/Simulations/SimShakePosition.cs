using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimShakePosition(
    Action<bkp> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bkp>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //var spData = this.Original.Data;
        var spData = this.Original.bhlf;
        //if (!string.IsNullOrEmpty(spData.resource))
        if (!string.IsNullOrEmpty(spData.bdvc))
        {
            //simulation.SetImageVisible(spData.image, true);
            simulation.SetImageVisible(spData.bdvb, true);
            //simulation.SetImageResource(spData.image, spData.resource);
            simulation.SetImageResource(spData.bdvb, spData.bdvc);
        }
    }
}
