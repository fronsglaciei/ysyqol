using Plot;
using System.Text.Json;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimConditionBranch(
    ConditionBranch original, ISimulatedTask parent, int indexInParent)
    : SimulatedParentTask<ConditionBranch>(original, parent, indexInParent)
{
    private int _selectedIndex = -1;

    public override void OnChildEntry(int index)
        => this._selectedIndex = index;

    protected override bool Validate()
        => this.Original is not null
        && this.Original.children is not null
        && 1 < this.Original.children.Count
        && (this._selectedIndex == 0 || this._selectedIndex == 1);

    protected override void SimulateCore(LevelSimulation _) { }

    public override string Serialize()
        => JsonSerializer.Serialize(new SimConditionBranchState
        {
            SelectedIndex = this._selectedIndex,
        });

    public override void Deserialize(string data)
    {
        var obj = JsonSerializer.Deserialize<SimConditionBranchState>(data);
        if (obj is null)
        {
            return;
        }

        this._selectedIndex = obj.SelectedIndex;
        if (this.Validate())
        {
            //this.Original.targetTask = this.Original.children[this._selectedIndex];
            this.Original.bdhy = this.Original.children[this._selectedIndex];
        }
    }

    private class SimConditionBranchState
    {
        public int SelectedIndex { get; set; }
    }
}
