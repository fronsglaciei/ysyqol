using Plot;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class SimOptionalTask(
    OptionalTask original, ISimulatedTask parent, int indexInParent)
    : SimulatedParentTask<OptionalTask>(original, parent, indexInParent)
{
    private readonly List<int> _selectedIndices = [];

    public override void OnChildEntry(int index)
        => this._selectedIndices.Add(index);

    protected override bool Validate()
    {
        if (this.Original is null)
        {
            return false;
        }
        //if (this.Original.noOption)
        if (this.Original.bdio)
        {
            return true;
        }

        var opts = this.Original.Options;
        if (opts is null || opts.Count < 1)
        {
            return false;
        }
        foreach (var index in this._selectedIndices)
        {
            if (index < 0 || opts.Count <= index)
            {
                return false;
            }
        }
        return true;
    }

    protected override void SimulateCore(LevelSimulation simulation)
    {
        //if (this.Original.executionStatus == TaskStatus.Running)
        if (this.Original.bhlz == bmq.Running)
        {
            simulation.SetReplayOptionalTask(this.Original.Cast<OptionalTask>());
            return;
        }

        SimOptionalTaskLog sotLog;
        //if (simulation.TryGetLog(this.Original.ID, out var log))
        if (simulation.TryGetLog(this.Original.bhlx, out var log))
        {
            if (log is not SimOptionalTaskLog tmpSotLog)
            {
                throw new InvalidOperationException(
                    $"Invalid log type for simulation : {log.GetType().FullName}");
            }
            sotLog = tmpSotLog;
        }
        else
        {
            //sotLog = new(this.Original.ID);
            sotLog = new(this.Original.bhlx);
            simulation.SetLog(sotLog);
        }

        if (this._selectedIndices.Count <= sotLog.Counter)
        {
            return;
        }

        //simulation.SetTextData(new PlotReviewData
        //{
        //    optionTalkID = this.Original.Options[this._selectedIndices[sotLog.Counter]].OptionID,
        //});
        simulation.SetTextData(new()
        {
            bdzq = this.Original.Options[this._selectedIndices[sotLog.Counter]].bdpl
        });
        sotLog.Counter++;
    }

    public override string Serialize()
        //=> JsonSerializer.Serialize(new SimOptionalTaskState
        //{
        //    Status = this.Original.executionStatus,
        //    CanSelect = [.. this.Original.CanSelect],
        //    NoOption = this.Original.noOption,
        //    CurrentSelectedIndex = this.Original.mSelectIndex,
        //    SelectedIndices = this._selectedIndices,
        //});
        => JsonSerializer.Serialize(new SimOptionalTaskState
        {
            Status = this.Original.bhlz,
            CanSelect = [.. this.Original.bdim],
            NoOption = this.Original.bdio,
            CurrentSelectedIndex = this.Original.bdik,
            SelectedIndices = this._selectedIndices
        });

    public override void Deserialize(string data)
    {
        var obj = JsonSerializer.Deserialize<SimOptionalTaskState>(data);
        if (obj == null)
        {
            return;
        }

        //this.Original.executionStatus = obj.Status;
        this.Original.bhlz = obj.Status;
        //var cnt = Math.Min(this.Original.CanSelect.Count, obj.CanSelect.Length);
        var cnt = Math.Min(this.Original.bdim.Count, obj.CanSelect.Length);
        for (var i = 0; i < cnt; i++)
        {
            //this.Original.CanSelect[i] = obj.CanSelect[i];
            this.Original.bdim[i] = obj.CanSelect[i];
        }
        //this.Original.noOption = obj.NoOption;
        this.Original.bdio = obj.NoOption;
        //this.Original.mSelectIndex = obj.CurrentSelectedIndex;
        this.Original.bdik = obj.CurrentSelectedIndex;
        this._selectedIndices.Clear();
        this._selectedIndices.AddRange(obj.SelectedIndices);
    }

    internal class SimOptionalTaskLog(int taskId)
        : LevelSimulationLogBase(taskId)
    {
        internal int Counter { get; set; }
    }

    private class SimOptionalTaskState
    {
        public Plot.bmq Status { get; set; } //public Plot.TaskStatus Status { get; set; }

        public bool[] CanSelect { get; set; } = [];

        public bool NoOption { get; set; }

        public int CurrentSelectedIndex { get; set; }

        public List<int> SelectedIndices { get; set; } = [];
    }
}
