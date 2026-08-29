using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimPlayBGM(
    Action<bja> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bja>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //var pbData = this.Original.Data;
        var pbData = this.Original.bhlf;
        //if (pbData.start)
        if (pbData.bdjo)
        {
            //if (pbData.ID != 0)
            if (pbData.bdjn != 0)
            {
                simulation.PlayBGM(pbData.bdjn);
            }
        }
        else
        {
            simulation.PauseBGM();
        }
    }
}
