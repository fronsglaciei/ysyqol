using FG.Mods.YSYard.QoL.Helpers;
using Foundation.UI;
using HotelModule;
using System;

namespace FG.Mods.YSYard.QoL.Services;

internal static class ExHotelManager
{
    private const string KEY_QLOAD_FAILURE = "QuickLoadFailure";

    private const string UINAME_SCENEINFO = "UI_SceneInfo";

    private const int FLOOR_ID_ROOT = 110001;

    internal static void QuickContinueGame()
    {
        if (!ExSaveLoadManager.QuickLoadCache())
        {
            if (!ModAssetProvider.TryGetModText(
                KEY_QLOAD_FAILURE, out var text)
                || string.IsNullOrEmpty(text))
            {
                throw new InvalidOperationException("string key not found");
            }
            //GameAPI.ShowTips(text);
            cy.cuw(text);
            return;
        }

        ExAuctionManager.OnAfterQuickLoad();
        ExLevelManager.OnAfterQuickLoad();

        //UIManager.Instance.DestroyAll(false);
        UIManager.bhje.jvf(false);
        //GameAPI.ShowCutSceneWindow((Il2CppSystem.Action)QuickContinueGameCore);
        cy.cuy((Il2CppSystem.Action)QuickContinueGameCore);
    }

    private static void QuickContinueGameCore()
    {
        //var hm = HotelManager.Instance;
        var hm = rp.bhfn;

        //hm.ClearData();
        hm.hzs();

        // BlockPhaseEventManager needs to be cleared
        // as Example.Task.EndStory is played using BlockPhaseEvent system
        ExBlockPhaseEventManager.ClearAll();

        //hm.ResetData();
        hm.hym();

        //if (!UIManager.Instance.HasWindowOpen(UINAME_SCENEINFO))
        if (!UIManager.bhje.jvi(UINAME_SCENEINFO))
        {
            //UIManager.Instance.OpenUI(UINAME_SCENEINFO, (Il2CppSystem.Action<SceneInfoWindow>)(window =>
            UIManager.bhje.jvm(UINAME_SCENEINFO, (Il2CppSystem.Action<bgl>)(window =>
            {
                //window.RefreshScene(
                //    false, FLOOR_ID_ROOT,
                //    HotelBuildingManager.Instance.mFloors[FLOOR_ID_ROOT].mHotelFloorData.RoomNamePos,
                //    true);
                window.lnm(false, FLOOR_ID_ROOT, fm.bgts.vsk[FLOOR_ID_ROOT].vsw.RoomNamePos);
            }), true);
        }

        ExSaveLoadManager.ApplyCacheToGame();
        //hm.HandleLoadParam();
        hm.hys();

        ExHotelEventManager.RetrieveState();

        //StartFSM(hm.RoundState);
        StartFSM(hm.yjh);

        ExAuctionManager.RetrieveState();
        ExLevelManager.RetrieveState();
    }

    private static void StartFSM(
        //HotelRoundManager.RoundState roundState)
        rv.rq roundState)
    {
        //var hrm = HotelManager.Instance.mHotelRoundManager;
        var hrm = rp.bhfn.yji;
        switch (roundState)
        {
            case rv.rq.FirstBeginPhase:
            case rv.rq.SecondaryBeginPhase:
            case rv.rq.EndPhase:
            case rv.rq.SettleMentPhase:
                // roundFSM gets ready in HotelManager.HandleLoadParam
                //hrm.roundFSM.SwitchStateWithoutExitEnter((int)roundState);
                hrm.ykc.SwitchStateWithoutExitEnter((int)roundState);
                break;
            case rv.rq.PrimaryPhase:
            case rv.rq.CutTo:
                //hrm.StartFSM(roundState);
                hrm.iag(roundState);
                break;
        }
    }
}
