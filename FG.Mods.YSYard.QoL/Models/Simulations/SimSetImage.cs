using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimSetImage(
    Action<bkm> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bkm>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //var siData = this.Original.Data;
        var siData = this.Original.bhlf;
        //var imageType = siData.image;
        var imageType = siData.bdtx;
        //var show = !siData.hide;
        var show = !siData.bdty;

        simulation.SetImageVisible(imageType, show);
        //if (siData.changeColor)
        if (siData.bduh)
        {
            //simulation.SetImageColor(imageType, siData.toColor.GetColor());
            simulation.SetImageColor(imageType, siData.bduj.mgn());
        }
        if (show)
        {
            //simulation.SetImagePosition(imageType, siData.localPostion.GetVector3());
            simulation.SetImagePosition(imageType, siData.bdud.mkq());
            //if (!string.IsNullOrEmpty(siData.resource))
            if (!string.IsNullOrEmpty(siData.bdue))
            {
                //simulation.SetImageResource(imageType, siData.resource);
                simulation.SetImageResource(imageType, siData.bdue);
                //simulation.SetImageFlipped(imageType, siData.flip);
                simulation.SetImageFlipped(imageType, siData.bduf);
            }
            //simulation.SetCharacterExpressionColor(
            //    imageType,
            //    siData.setImageColor ? siData.imageColor.GetColor() : UnityEngine.Color.white);
            simulation.SetCharacterExpressionColor(
                imageType,
                siData.bdun ? siData.bduo.mgn() : UnityEngine.Color.white);
            //simulation.SetCharacterBlockColor(
            //    imageType,
            //    siData.setBlockColor ? siData.blockColor.GetColor() : UnityEngine.Color.gray);
            simulation.SetCharacterBlockColor(
                imageType,
                siData.bdup ? siData.bduq.mgn() : UnityEngine.Color.gray);
        }
        //simulation.SetImageAutoResize(imageType, siData.autoSize);
        simulation.SetImageAutoResize(imageType, siData.bdum);
    }
}
