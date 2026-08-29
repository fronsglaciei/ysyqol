using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimFadeScreen(
    Action<bjn> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bjn>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //var fsData = this.Original.Data;
        var fsData = this.Original.bhlf;
        //var show = !((fsData.ToAlpha == 0f) | fsData.FadeWhenDone);
        var show = !((fsData.bdmk == 0f) | fsData.bdmn);
        //var color = fsData.color.GetColor();
        var color = fsData.bdmm.mgn();
        //color.a = fsData.ToAlpha;
        color.a = fsData.bdmk;
        simulation.SetScreenFade(show, color);
    }
}
