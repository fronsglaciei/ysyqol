using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimMovePath(
    Action<bju> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bju>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //var mpData = this.Original.Data;
        var mpData = this.Original.bhlf;
        //if (!string.IsNullOrEmpty(mpData.resource))
        if (!string.IsNullOrEmpty(mpData.bdnz))
        {
            //simulation.SetImageResource(mpData.image, mpData.resource);
            simulation.SetImageResource(mpData.bdny, mpData.bdnz);
            //simulation.SetImageVisible(mpData.image, true);
            simulation.SetImageVisible(mpData.bdny, true);
        }
        //if (mpData.movePoint != null && 0 < mpData.movePoint.Count)
        if (mpData.bdoc != null && 0 < mpData.bdoc.Count)
        {
            //var lastPoint = mpData.movePoint[mpData.movePoint.Count - 1];
            var lastPoint = mpData.bdoc[mpData.bdoc.Count - 1];
            //simulation.SetImagePosition(mpData.image, lastPoint.GetVector3());
            simulation.SetImagePosition(mpData.bdny, lastPoint.mkq());
        }
    }
}
