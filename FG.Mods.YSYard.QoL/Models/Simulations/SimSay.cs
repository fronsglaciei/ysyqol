using Plot;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimSay(
    Action<bki> original, ISimulatedTask parent, int indexInParent)
    : SimulatedAction<bki>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation simulation)
    {
        //if (this.Original.executionStatus == TaskStatus.Running)
        if (this.Original.bhlz == bmq.Running)
        {
            simulation.SetReplaySay(this.Original.Cast<Say>());
        }
        else
        {
            //var sd = this.Original.Data;
            var sd = this.Original.bhlf;
            //simulation.SetSayCharacters(sd.ShowCharacterA, sd.ShowCharacterB, sd.ShowCharacterC);
            simulation.SetSayCharacters(sd.bdsa, sd.bdsb, sd.bdsc);
            //simulation.SetTextData(new PlotReviewData
            //{
            //    conversationID = sd.conversationID,
            //});
            simulation.SetTextData(new bmj
            {
                bdzp = sd.bdry
            });
            //if (sd.SetDialogPositon)
            if (sd.bdsr)
            {
                //simulation.SetTextOffset(
                //    sd.ConversationPos.GetVector2(),
                //    sd.SpeakerNamePos.GetVector2());
                simulation.SetTextOffset(
                    sd.bdss.mkd(),
                    sd.bdst.mkd());
            }
            //simulation.SetDialogVisible(!sd.FadeWhenDone);
            simulation.SetDialogVisible(!sd.bdsf);

            simulation.SetLastPlayedSay(this.Original.Cast<Say>());
        }
    }
}
