using Plot;
using System.Text.Json;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimPlotBranch(
    PlotBranch original, ISimulatedTask parent, int indexInParent)
    : SimulatedParentTask<PlotBranch>(original, parent, indexInParent)
{
    protected override bool Validate()
        => this.Original is not null
        && this.Original.children is not null
        && 0 < this.Original.children.Count
        && this.Original.branch is not null
        && this.Original.children.Count == this.Original.branch.Length
        //&& -1 < this.Original.branchIndex
        && -1 < this.Original.bdip
        //&& this.Original.branchIndex < this.Original.children.Count;
        && this.Original.bdip < this.Original.children.Count;

    protected override void SimulateCore(LevelSimulation _) { }

    public override string Serialize()
        //=> JsonSerializer.Serialize(new SimPlotBranchState
        //{
        //    Status = this.Original.executionStatus,
        //    SelectedIndex = this.Original.branchIndex,
        //});
        => JsonSerializer.Serialize(new SimPlotBranchState
        {
            Status = this.Original.bhlz,
            SelectedIndex = this.Original.bdip
        });

    public override void Deserialize(string data)
    {
        var obj = JsonSerializer.Deserialize<SimPlotBranchState>(data);
        if (obj == null)
        {
            return;
        }

        //this.Original.executionStatus = obj.Status;
        this.Original.bhlz = obj.Status;
        //this.Original.branchIndex = obj.SelectedIndex;
        this.Original.bdip = obj.SelectedIndex;
    }

    private class SimPlotBranchState
    {
        public Plot.bmq Status { get; set; } //public Plot.TaskStatus Status { get; set; }

        public int SelectedIndex { get; set; }
    }
}
