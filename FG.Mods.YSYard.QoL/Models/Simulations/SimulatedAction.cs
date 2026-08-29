using System.Text.Json;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal abstract class SimulatedAction<T>(
    Plot.Action<T> original, ISimulatedTask parent, int indexInParent)
    : ISimulatedTask
    where T : Il2CppSystem.Object
{
    internal Plot.Action<T> Original { get; } = original;

    internal ISimulatedTask Parent { get; } = parent;

    public void OnEntry()
    {
        this.OnEntryCore();
        this.Parent?.OnChildEntry(indexInParent);
    }

    protected virtual void OnEntryCore() { }

    public void OnChildEntry(int index) { }

    public void OnComplete()
    {
        this.OnCompleteCore();
        this.Parent?.OnChildComplete(indexInParent);
    }

    protected virtual void OnCompleteCore() { }

    public void OnChildComplete(int index) { }

    public void Simulate(LevelSimulation simulation)
    {
        if (!this.Validate())
        {
            return;
        }
        this.SimulateCore(simulation);
    }

    protected virtual bool Validate()
        => this.Original != null
        //&& this.Original.Data != null;
        && this.Original.bhlf != null;

    protected abstract void SimulateCore(LevelSimulation simulation);

    public virtual string Serialize()
        => JsonSerializer.Serialize(
            //new SimulatedActionState { Status = this.Original.executionStatus });
            new SimulatedActionState { Status = this.Original.bhlz });

    public virtual void Deserialize(string data)
    {
        var obj = JsonSerializer.Deserialize<SimulatedActionState>(data);
        if (obj == null)
        {
            return;
        }
        //this.Original.executionStatus = obj.Status;
        this.Original.bhlz = obj.Status;
    }

    protected class SimulatedActionState
    {
        public Plot.bmq Status { get; set; } //public Plot.TaskStatus Status { get; set; }
    }
}
