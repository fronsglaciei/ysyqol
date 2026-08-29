using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimSwitchTransition(
    Action<blq> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<blq>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //var stData = this.Original.Data;
        var stData = this.Original.bhlf;
        //simulation.SetBackground(true, stData.bgResource);
        simulation.SetBackground(true, stData.bdyv);
        //if (stData.BGMID != 0)
        if (stData.bdyw != 0)
        {
            //simulation.PlayBGM(stData.BGMID);
            simulation.PlayBGM(stData.bdyw);
        }
        simulation.CleanCharacters();
        simulation.CleanCartoons();
        // simulation.CleanEffects();
    }
}
