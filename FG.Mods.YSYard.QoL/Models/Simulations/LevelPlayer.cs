using FG.Mods.YSYard.QoL.Models.Saves;
using Plot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class LevelPlayer
{
    private readonly Dictionary<int, ISimulatedTask> _stDict = [];

    private readonly List<KeyValuePair<int, ISimulatedTask>> _visitedOrder = [];

    internal int StoryId { get; private set; }

    internal Level Level { get; private set; }

    internal LevelPlayerState SerializableState
    {
        get => new()
        {
            StoryId = this.StoryId,
            TaskStates = this._stDict
                .ToDictionary(x => x.Key, y => y.Value.Serialize()),
            PlayedTasks = [.. this._visitedOrder.Select(x => x.Key)]
        };
        set
        {
            this.StoryId = value.StoryId;
            foreach (var kvp in value.TaskStates)
            {
                if (this._stDict.TryGetValue(kvp.Key, out var st))
                {
                    st.Deserialize(kvp.Value);
                }
            }
            this._visitedOrder.Clear();
            foreach (var taskId in value.PlayedTasks)
            {
                if (this._stDict.TryGetValue(taskId, out var st))
                {
                    this._visitedOrder.Add(new(taskId, st));
                }
            }
        }
    }

    internal static LevelPlayer Parse(int id, Level lv)
    {
        //if (lv is null || lv.entryTask is null)
        if (lv is null || lv.bdzg is null)
        {
            return null;
        }

        var ret = new LevelPlayer
        {
            StoryId = id,
            Level = lv,
        };
        //ret.TraverseTasks(lv.entryTask, null, -1);
        ret.TraverseTasks(lv.bdzg, null, -1);
        return ret;
    }

    internal void OnTaskStart(int taskId)
    {
        if (0 < this._visitedOrder.Count)
        {
            this._visitedOrder[^1].Value.OnComplete();
        }
        if (this._stDict.TryGetValue(taskId, out var nextSt))
        {
            this._visitedOrder.Add(new(taskId, nextSt));
            nextSt.OnEntry();
        }
    }

    internal void ApplyStateTo(
        // LevelDesignerManager ldm
        bmo ldm)
    {
        var ls = new LevelSimulation();
        foreach (var kvp in this._visitedOrder)
        {
            kvp.Value.Simulate(ls);
        }
        ls.ApplySimulationTo(ldm);
    }

    private void TraverseTasks(Plot.Task task, ISimulatedTask parent, int indexInParent)
    {
        //if (this._stDict.ContainsKey(task.ID))
        if (this._stDict.ContainsKey(task.bhlx))
        {
            throw new InvalidOperationException($"Task.ID {task.bhlx} is not unique in Level");
        }

        var st = CreateSimulatedTask(task, parent, indexInParent);
        //this._stDict[task.ID] = st;
        this._stDict[task.bhlx] = st;

        var pt = task.TryCast<ParentTask>();
        if (pt is null || pt.children is null)
        {
            return;
        }

        for (var i = 0; i < pt.children.Count; i++)
        {
            var child = pt.children[i];
            this.TraverseTasks(child, st, i);
        }
    }

    private static ISimulatedTask CreateSimulatedTask(
        Plot.Task task, ISimulatedTask parent, int indexInParent)
        => task.GetIl2CppType().Name switch
        {
            nameof(BackGround) => new SimBackground(task.Cast<BackGround>(), parent, indexInParent),
            nameof(CartoonMoveTo) => new SimCartoonMoveTo(task.Cast<CartoonMoveTo>(), parent, indexInParent),
            nameof(CartoonScaleTo) => new SimCartoonScaleTo(task.Cast<CartoonScaleTo>(), parent, indexInParent),
            nameof(CartoonSet) => new SimCartoonSet(task.Cast<CartoonSet>(), parent, indexInParent),
            nameof(CartoonShake) => new SimCartoonShake(task.Cast<CartoonShake>(), parent, indexInParent),
            nameof(ConditionBranch) => new SimConditionBranch(task.Cast<ConditionBranch>(), parent, indexInParent),
            nameof(EntryTask) => new SimEntryTask(task.Cast<EntryTask>(), parent, indexInParent),
            nameof(ExecuteEvent) => new SimExecuteEvent(task.Cast<ExecuteEvent>(), parent, indexInParent),
            nameof(FadeScreen) => new SimFadeScreen(task.Cast<FadeScreen>(), parent, indexInParent),
            nameof(ImgEffect) => new SimImgEffect(task.Cast<ImgEffect>(), parent, indexInParent),
            nameof(ModifyAttribute) => new SimModifyAttribute(task.Cast<ModifyAttribute>(), parent, indexInParent),
            nameof(MovePath) => new SimMovePath(task.Cast<MovePath>(), parent, indexInParent),
            nameof(MoveTo) => new SimMoveTo(task.Cast<MoveTo>(), parent, indexInParent),
            nameof(OpenWindow) => new SimOpenWindow(task.Cast<OpenWindow>(), parent, indexInParent),
            nameof(OptionalTask) => new SimOptionalTask(task.Cast<OptionalTask>(), parent, indexInParent),
            nameof(Plot.Parallel) => new SimParallel(task.Cast<Plot.Parallel>(), parent, indexInParent),
            nameof(ParallelComplete) => new SimParallelComplete(task.Cast<ParallelComplete>(), parent, indexInParent),
            nameof(PlayBGM) => new SimPlayBGM(task.Cast<PlayBGM>(), parent, indexInParent),
            nameof(PlayPlot) => new SimPlayPlot(task.Cast<PlayPlot>(), parent, indexInParent),
            nameof(PlaySound) => new SimPlaySound(task.Cast<PlaySound>(), parent, indexInParent),
            nameof(PlotBranch) => new SimPlotBranch(task.Cast<PlotBranch>(), parent, indexInParent),
            nameof(Say) => new SimSay(task.Cast<Say>(), parent, indexInParent),
            nameof(ScaleTo) => new SimScaleTo(task.Cast<ScaleTo>(), parent, indexInParent),
            nameof(ScreenMask) => new SimScreenMask(task.Cast<ScreenMask>(), parent, indexInParent),
            nameof(Sequence) => new SimSequence(task.Cast<Sequence>(), parent, indexInParent),
            nameof(SetImage) => new SimSetImage(task.Cast<SetImage>(), parent, indexInParent),
            nameof(ShakeBackGround) => new SimShakeBackground(task.Cast<ShakeBackGround>(), parent, indexInParent),
            nameof(ShakePosition) => new SimShakePosition(task.Cast<ShakePosition>(), parent, indexInParent),
            nameof(SwitchTransition) => new SimSwitchTransition(task.Cast<SwitchTransition>(), parent, indexInParent),
            _ => throw new NotImplementedException()
        };
}
