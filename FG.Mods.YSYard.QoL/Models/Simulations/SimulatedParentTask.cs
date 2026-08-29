using System.Text.Json;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal abstract class SimulatedParentTask<T>(
    T original, ISimulatedTask parent, int indexInParent)
    : ISimulatedTask
    where T : Plot.ParentTask
{
    internal T Original { get; } = original;

    internal ISimulatedTask Parent { get; } = parent;

    public void OnEntry()
    {
        this.OnEntryCore();
        this.Parent?.OnChildEntry(indexInParent);
    }

    protected virtual void OnEntryCore() { }

    public virtual void OnChildEntry(int index) { }

    public void OnComplete()
    {
        this.OnCompleteCore();
        this.Parent?.OnChildComplete(indexInParent);
    }

    protected virtual void OnCompleteCore() { }

    public virtual void OnChildComplete(int index) { }

    public void Simulate(LevelSimulation simulation)
    {
        if (!this.Validate())
        {
            return;
        }
        this.SimulateCore(simulation);
    }

    protected virtual bool Validate()
        => this.Original is not null
        && this.Original.children is not null
        && 0 < this.Original.children.Count;

    protected abstract void SimulateCore(LevelSimulation simulation);

    public virtual string Serialize() =>
        JsonSerializer.Serialize(
            //new SimulatedParentTaskState { Status = this.Original.executionStatus });
            new SimulatedParentTaskState { Status = this.Original.bhlz });

    public virtual void Deserialize(string data)
    {
        var obj = JsonSerializer.Deserialize<SimulatedParentTaskState>(data);
        if (obj == null)
        {
            return;
        }
        this.Original.bhlz = obj.Status;
    }

    protected class SimulatedParentTaskState
    {
        public Plot.bmq Status { get; set; } //public Plot.TaskStatus Status { get; set; }
    }
}
