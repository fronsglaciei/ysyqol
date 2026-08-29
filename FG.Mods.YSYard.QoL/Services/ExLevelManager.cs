using FG.Mods.YSYard.QoL.Models.Saves;
using FG.Mods.YSYard.QoL.Models.Simulations;
using FG.Mods.YSYard.QoL.Patches;
using Foundation.UI;
using Il2CppInterop.Runtime;
using Plot;
using UnityEngine;

namespace FG.Mods.YSYard.QoL.Services;

internal static class ExLevelManager
{
    private const string UINAME_PLOTCONTROLWINDOW = "PlotWindow";

    private static LevelPlayer _currentPlayer;
    internal static LevelPlayerState CurrentPlayerState
    {
        get
        {
            //if (LevelDesignerManager.instance.activeLevel is null)
            if (bmo.bhlr.bead is null)
            {
                // Don't return state when Level is inactive
                return null;
            }
            return _currentPlayer?.SerializableState;
        }
    }

    internal static void ClearAllDelays(Level lv)
    {
        //if (lv?.entryTask is null)
        if (lv?.bdzg is null)
        {
            return;
        }
        ClearAllDelays(lv.bdzg);
    }

    private static void ClearAllDelays(ParentTask pt)
    {
        pt.delay = 0f;
        foreach (var child in pt.children ?? new())
        {
            if (child is null)
            {
                continue;
            }

            var ptChild = child.TryCast<ParentTask>();
            if (ptChild is null)
            {
                child.delay = 0f;
            }
            else
            {
                ClearAllDelays(ptChild);
            }
        }
    }

    internal static void OnLevelStart(int id, Level lv)
    {
        _currentPlayer = LevelPlayer.Parse(id, lv);
    }

    internal static void OnTaskStart(int taskId)
    {
        _currentPlayer?.OnTaskStart(taskId);
    }

    internal static void OnAfterQuickLoad()
        => ForceStopCurrentLevel();

    internal static void RetrieveState()
    {
        var lpState = ExSaveLoadManager.QuickSaveDataCache?.LevelPlayerState;
        if (lpState is null)
        {
            return;
        }

        ForcePlayLevel(
            lpState.StoryId,
            //(Il2CppSystem.Action<PlotControlWindow>)(_ => RetrieveStateCore(lpState)));
            (Il2CppSystem.Action<yr>)(_ => RetrieveStateCore(lpState)));
    }

    private static void ForceStopCurrentLevel()
    {
        //var lvMng = LevelDesignerManager.instance;
        var lvMng = bmo.bhlr;
        //if (lvMng.activeLevel is null)
        if (lvMng.bead is null)
        {
            return;
        }

        //lvMng.activeLevel.executionStatus = Plot.TaskStatus.Inactive;
        lvMng.bead.bdzo = bmq.Inactive;
        //AssetPool.Hide();
        biv.maz();
        //AbortLevel(LevelDesignerManager.instance.activeLevel);
        AbortLevel(lvMng.bead);
    }

    private static void ForcePlayLevel(
        int id,
        //Il2CppSystem.Action<PlotControlWindow> uiCallback)
        Il2CppSystem.Action<yr> uiCallback)
    {
        if (id < 1 || uiCallback is null)
        {
            return;
        }

        ForceStopCurrentLevel();

        //PlotHelper.Instance.mCurID = id;
        bi.bgrz.uuk = id;
        //PlotHelper.Instance.inPlot = true;
        bi.bgrz.uuh = true;
        //UIManager.Instance.OpenUI(UINAME_PLOTCONTROLWINDOW, uiCallback, true);
        UIManager.bhje.jvm(UINAME_PLOTCONTROLWINDOW, uiCallback);
    }

    private static void RetrieveStateCore(LevelPlayerState lpState)
    {
        //var ldm = LevelDesignerManager.instance;
        var ldm = bmo.bhlr;
        //ldm.PlotId = lpState.StoryId;
        ldm.beap = lpState.StoryId;
        //ldm.ReviewData.Clear();
        ldm.bean.Clear();
        //ldm._plotWindow.OnPlay();
        ldm.beas.nji();
        //ldm.BlockControlWindowUpdate = false;
        ldm.beaq = false;

        Level lv;
        if (_currentPlayer?.StoryId == lpState.StoryId
            && _currentPlayer?.Level is not null)
        {
            lv = _currentPlayer.Level;
            //ldm.activeLevel = lv;
            ldm.bead = lv;
            //ldm.levelFadeExit = lv.IsFadeExit;
            ldm.beag = lv.bhll;
        }
        else
        {
            lv = LoadLevel(lpState.StoryId);
            //if (lv is null || !lv.canPlay)
            if (lv is null || !lv.bdzm)
            {
                Plugin.Log.LogError($"Cannot play loaded level");
                return;
            }

            //ldm.levelFadeExit = lv.IsFadeExit;
            ldm.beag = lv.bhll;
            //if (0f < lv.Delay)
            if (0f < lv.bhlj)
            {
                Plugin.Log.LogWarning($"Level {lpState.StoryId} was loaded but not playable.");
                return;
            }

            _currentPlayer = LevelPlayer.Parse(lpState.StoryId, lv);
        }

        _currentPlayer.SerializableState = lpState;
        _currentPlayer.ApplyStateTo(ldm);
        using var _ = LevelDesignerManager_WindowPlay_Patch.RunOriginal();
        //ldm.WindowPlay(false, _currentPlayer.StoryId, lv);
        ldm.ngi(false, _currentPlayer.StoryId, lv);
    }

    private static Level LoadLevel(int id)
    {
        //var filePath = PlotHelper.Instance.GetPathOfStory($"{id}");
        var filePath = bi.bgrz.cnd($"{id}");
        //var ta = ResourcesManager.Instance.Load<TextAsset>(filePath);
        var ta = dg.bgsl.Load<TextAsset>(filePath);
        var lv = new Level();
        if (ta is null || ta.bytes is null)
        {
            Plugin.Log.LogError($"Failed to load {filePath}.");

            //lv.canPlay = false;
            lv.bdzm = false;
            AbortLevel(null);

            return null;
        }

        //var lvData = UtilitySpace.Utility.Deserialize2Proto<LevelData>(ta.bytes);
        var lvData = UtilitySpace.bhi.lrx<bjs>(ta.bytes);
        if (lvData is null)
        {
            Plugin.Log.LogError($"Cannot deserialize {filePath}.");

            //lv.canPlay = false;
            lv.bdzm = false;
            AbortLevel(lv);

            return lv;
        }

        //lv.entryTask = LevelDesignerUtility
        //    .LoadTaskSource(lvData.EntrySource)
        //    .Cast<EntryTask>();
        lv.bdzg = bmp.ngv(lvData.bdnk).Cast<EntryTask>();
        //lv.type = lvData.Type;
        lv.bdzh = lvData.bdnj;
        //lv.ignoreBGM = lvData.IgnoreBGM;
        lv.bdzi = lvData.bdnn;
        //lv.uiMode = lvData.UIMode;
        lv.bdzk = lvData.bdnp;
        //lv.endClose = lvData.EndClose;
        lv.bdzj = lvData.bdno;
        //lv.hideherosAtEnd = lvData.HideHerosAtEnd;
        lv.bdzl = lvData.bdnq;
        //lv.canPlay = true;
        lv.bdzm = true;
        return lv;
    }

    private static void AbortLevel(Level lv)
    {
        //var lvMng = LevelDesignerManager.instance;
        var lvMng = bmo.bhlr;
        //lvMng.detectedTask.Clear();
        lvMng.beae.Clear();
        //if (lvMng.activeLevel != lv)
        if (lvMng.bead != lv)
        {
            return;
        }

        //lvMng.activeLevel = null;
        lvMng.bead = null;
        //var pw = lvMng._plotWindow;
        var pw = lvMng.beas;
        if (pw is not null)
        {
            //pw.StraightTransitions(false);
            pw.nko(false);
            //pw.Release();
            pw.nkh();
        }

        //var ph = PlotHelper.Instance;
        var ph = bi.bgrz;
        //ph.inPlot = false;
        ph.uuh = false;
        //ph.mCurID = 0;
        ph.uuk = 0;
        //UIManager.Instance.CloseUI(UINAME_PLOTCONTROLWINDOW, true, false);
        UIManager.bhje.juw(UINAME_PLOTCONTROLWINDOW);
    }

    public static void PlayGalleryMode(int id)
        //=> ForcePlayLevel(id, (Il2CppSystem.Action<PlotControlWindow>)(_ =>
        => ForcePlayLevel(id, (Il2CppSystem.Action<yr>)(_ =>
        {
            //var ldm = LevelDesignerManager.instance;
            var ldm = bmo.bhlr;
            //ldm.PlotId = id;
            ldm.beap = id;
            //ldm.ReviewData.Clear();
            ldm.bean.Clear();
            //ldm._plotWindow.OnPlay();
            ldm.beas.nji();
            //ldm.BlockControlWindowUpdate = false;
            ldm.beaq = false;

            var lv = LoadLevel(id);
            //if (lv is null || !lv.canPlay)
            if (lv is null || !lv.bdzm)
            {
                Plugin.Log.LogError($"Cannot play loaded level");
                return;
            }

            //ldm.levelFadeExit = lv.IsFadeExit;
            ldm.beag = lv.bhll;
            //if (0f < lv.Delay)
            if (0f < lv.bhlj)
            {
                Plugin.Log.LogWarning($"Level {id} was loaded but not playable.");
                return;
            }

            PreProcessGalleryMode(lv);

            using var __ = LevelDesignerManager_WindowPlay_Patch.RunOriginal();
            //ldm.WindowPlay(false, id, lv);
            ldm.ngi(false, id, lv);
        }));

    private static void PreProcessGalleryMode(Level lv)
    {
        //if (lv?.entryTask is null)
        if (lv?.bdzg is null)
        {
            return;
        }
        //PreProcessGalleryMode(lv.entryTask);
        PreProcessGalleryMode(lv.bdzg);
    }

    private static void PreProcessGalleryMode(ParentTask pt)
    {
        if (pt?.children is null)
        {
            return;
        }
        foreach (var child in pt.children)
        {
            var ptChild = child.TryCast<ParentTask>();
            if (ptChild is null)
            {
                var typeName = child.GetIl2CppType().Name;
                switch (typeName)
                {
                    // skip Task that affects game state
                    case nameof(ExecuteEvent):
                    case nameof(ModifyAttribute):
                        //child.executionStatus = Plot.TaskStatus.Success;
                        child.bhlz = bmq.Success;
                        break;
                }
            }
            else
            {
                PreProcessGalleryMode(ptChild);
            }
        }
    }
}
