using Plot;
using System.Text.Json;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimSequence(
    Sequence original, ISimulatedTask parent, int indexInParent)
    : SimulatedParentTask<Sequence>(original, parent, indexInParent)
{
    protected override void SimulateCore(LevelSimulation _) { }

    public override string Serialize()
        //=> JsonSerializer.Serialize(new SimSequenceState
        //{
        //    Status = this.Original.executionStatus,
        //    CurrentIndex = this.Original.currentChildIndex,
        //});
        => JsonSerializer.Serialize(new SimSequenceState
        {
            Status = this.Original.bhlz,
            CurrentIndex = this.Original.bdit
        });

    public override void Deserialize(string data)
    {
        var obj = JsonSerializer.Deserialize<SimSequenceState>(data);
        if (obj == null)
        {
            return;
        }

        //this.Original.executionStatus = obj.Status;
        this.Original.bhlz = obj.Status;
        //this.Original.currentChildIndex = obj.CurrentIndex;
        this.Original.bdit = obj.CurrentIndex;
    }

    private class SimSequenceState
    {
        public Plot.bmq Status { get; set; } //public Plot.TaskStatus Status { get; set; }

        public int CurrentIndex { get; set; }
    }
}
