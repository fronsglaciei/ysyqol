using BagSystem;
using Example;
using FG.Mods.YSYard.QoL.Helpers;
using FG.Mods.YSYard.QoL.Models;
using FG.Mods.YSYard.QoL.Models.Saves;
using Foundation.UI;
using HotelModule;
using HotelModule.BlockPhaseEvent;
using HotelModule.Event;
using Plot;
using PlotDesigher;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FG.Mods.YSYard.QoL.Services;

internal static class ExAuctionManager
{
    private const string KEY_CONFIRM_QSAVE = "ConfirmQuickSave";

    private static readonly List<int> _retrievableRoundStates =
        [
        (int)qx.qw.BeginPlot, //AuctionRoundManager.EAuctionRoundState.BeginPlot
        (int)qx.qw.BidingPlot,
        (int)qx.qw.EndPlot
        ];

    internal static AuctionState CurrentState
    {
        get
        {
            //var am = AuctionManager.Instance;
            var am = qv.bhes;
            //return am.IsInAuctionRound()
            //    ? new()
            //    {
            //        AuctionId = am._currentAuctionData.AuctionID,
            //        RoundState = am._auctionRoundManager
            //            ._auctionRoundStateMachine.CurrentStateName,
            //        InitialRelicIds = am.AuctionRelicList
            //            .ToManagedList()
            //            .Select(x => x.ArtifactID).ToList(),
            //        SelectedRelicIds = am.ExecuteRelicList
            //            .ToManagedList()
            //            .Select(x => x.ArtifactID).ToList(),
            //        ConsumedRelicIds = am.AlreadyChosenRelicList
            //            .ToManagedList()
            //            .Select(x => x.ArtifactID).ToList(),
            //        SubmitItemId = am._submitAuctionItemId,
            //        SubmitItemIndex = am.AuctionPlayerIdx,
            //        BiddingItemId = am._currentBiddingAuctionItemId,
            //        BiddingIndex = am._currentBidingItemIdx,
            //        PoolItemIds = am.AuctionItemIds.ToManagedList(),
            //        PoolItemBidPrices = am
            //            .AuctionItemFinalBidValues
            //            .ToManagedList(),
            //        PoolItemCount = am._currentAuctionItemNum,
            //        SettleItemIds = am.PlayerSettleItems.ToManagedList()
            //    }
            //    : null;
            return am.hos()
                ? new()
                {
                    AuctionId = am.yeb.xah,
                    RoundState = am.ydy.yfm.bhhd,
                    InitialRelicIds = am.yed
                        .ToManagedList()
                        .Select(x => x.vli).ToList(),
                    SelectedRelicIds = am.yee
                        .ToManagedList()
                        .Select(x => x.vli).ToList(),
                    ConsumedRelicIds = am.yef
                        .ToManagedList()
                        .Select(am => am.vli).ToList(),
                    SubmitItemId = am.yeh,
                    SubmitItemIndex = am.yfi,
                    BiddingItemId = am.yej,
                    BiddingIndex = am.yeu,
                    PoolItemIds = am.yek.ToManagedList(),
                    PoolItemBidPrices = am.yel.ToManagedList(),
                    PoolItemCount = am.yei,
                    SettleItemIds = am.yey.ToManagedList()
                }
                : null;
        }
    }

    internal static void OnAfterQuickLoad()
        => ForceStopAuction();

    internal static void RetrieveState()
    {
        var state = ExSaveLoadManager.QuickSaveDataCache?.AuctionState;
        // regards state == null as not in auction event
        if (state is null)
        {
            return;
        }

        if (!IsStateRetrievable(state.RoundState))
        {
            throw new InvalidOperationException(
                $"Cannot retrieve auction RoundState {state.RoundState}");
        }

        ForceStopAuction();

        //var be = new AuctionStartEvent(state.AuctionId);
        var be = new tx(state.AuctionId);
        
        ExBlockPhaseEventManager.ExecuteEventWithoutStart(be);

        //StartAuction(state, (Il2CppSystem.Action)be.EndEvent);
        StartAuction(state, (Il2CppSystem.Action)be.igw);
    }

    private static bool IsStateRetrievable(int roundState)
        => _retrievableRoundStates.Contains(roundState);

    private static void ForceStopAuction()
    {
        //var am = AuctionManager.Instance;
        var am = qv.bhes;
        //if (!am.IsInAuctionRound())
        if (!am.hos())
        {
            return;
        }
        //am.AuctionWindow.ActiveMask(false);
        am.yea.lfp(false);
        //am.AuctionWindow.Close(true, false);
        am.yea.Close(true, false);
        //am._endAuctionCallback?.Invoke();
        am.yeg?.Invoke();
        //am._isInBid = false;
        am.yfl = false;

        //am._auctionRoundManager
        //    ._auctionRoundStateMachine
        //    .SwitchStateWithoutExitEnter(
        //        (int)AuctionRoundManager.EAuctionRoundState.Stop);
        am.ydy.yfm.SwitchStateWithoutExitEnter((int)qx.qw.Stop);
    }

    private static void StartAuction(AuctionState state, Il2CppSystem.Action onEndAuction)
    {
        //var am = AuctionManager.Instance;
        var am = qv.bhes;

        #region (almost) original StartAuction
        //am._isFirstBidding = state.AuctionId == 700;
        am.yez = state.AuctionId == 700;
        //am._submitGolden = state.AuctionId == 702;
        am.yfb = state.AuctionId == 702;
        //am._isInBid = true;
        am.yfl = true;
        //am._currentAuctionData = AuctionBaseManager.Instance.GetItem(state.AuctionId);
        am.yeb = gf.bgul.GetItem(state.AuctionId);
        //am._jumpSubmitItem = am._currentAuctionData.ItemSkip == 1;
        am.yfa = am.yeb.xam == 1;
        //am.PlayerSettleItems.Clear();
        am.yey.Clear();

        //UIManager.Instance.OpenUI("UI_Auction", (Il2CppSystem.Action<AuctionWindow>)(x =>
        //{
        //    if (am.AuctionWindow != null)
        //    {
        //        return;
        //    }
        //    am.AuctionWindow = x;
        //    am.AuctionSayHelper = new(
        //        am.AuctionWindow.SayDialog.GetComponent<SayDialog>());
        //}), true);
        UIManager.bhje.jvm("UI_Auction", (Il2CppSystem.Action<bes>)(x =>
        {
            if (am.yea is not null)
            {
                return;
            }
            am.yea = x;
            am.ydz = new(am.yea.bcej.GetComponent<SayDialog>());
        }), true);
        //am.AuctionWindow = UIManager.Instance.GetUIByName<AuctionWindow>("UI_Auction");
        am.yea = UIManager.bhje.jvq<bes>("UI_Auction");
        //am.AuctionSayHelper.SetSayDialog(am.AuctionWindow.SayDialog.GetComponent<SayDialog>());
        am.ydz.hsz(am.yea.bcej.GetComponent<SayDialog>());
        //am.AuctionWindow.InitBiding(
        //    (Il2CppSystem.Action)am.OnSelectHighValue,
        //    (Il2CppSystem.Action)am.OnSelectLowValue,
        //    (Il2CppSystem.Action)am.OnSelectObserve);
        am.yea.lfj(
            (Il2CppSystem.Action)am.hpw,
            (Il2CppSystem.Action)am.hpx,
            (Il2CppSystem.Action)am.hqa);
        //am.AuctionWindow.InteractiveAllPlayer(false);
        am.yea.lgd(false);
        //am.AuctionWindow.InteractiveSkip(false);
        am.yea.lft(false);

        //if (am._submitGolden)
        if (am.yfb)
        {
            //var bagNode = BagManager.Instance.GetBagNodeByItemID(30260);
            var bagNode = qo.bheq.hnk(30260);
            bagNode ??= new()
            {
                //ItemData = ItemManager.Instance.GetItem(30260),
                ycb = hl.bgvr.GetItem(30260),
                //Count = 0,
                ycc = 0,
                //GetItemTime = HotelManager.Instance.GetCurrentGameTime(),
                ycd = rp.bhfn.hzr(),
            };
            //am.ExItems = new();
            am.yec = new();
            //am.ExItems.Add(bagNode);
            am.yec.Add(bagNode);
        }
        else
        {
            //am.ExItems = BagManager.Instance.GetBagShowList(ItemEItemType.E_Transcendent);
            am.yec = qo.bheq.hnr(no.E_Transcendent);
            //for (var i = am.ExItems.Count - 1; -1 < i; i--)
            for (var i = am.yec.Count - 1; -1 < i; i--)
            {
                //if (am._avoidList.Contains(am.ExItems[i].ItemData.ItemID))
                if (am.yfk.Contains(am.yec[i].ycb.xld))
                {
                    //am.ExItems.RemoveAt(i);
                    am.yec.RemoveAt(i);
                }
            }
        }

        //am.AuctionRelicList = ArtifactManager.Instance.GetAuctionArtifactList();

        //am.AlreadyChosenRelicList.Clear();
        am.yef.Clear();
        //am.ExecuteRelicList.Clear();
        am.yee.Clear();
        //am.AuctionItemIds.Clear();
        am.yek.Clear();
        //am.AuctionItemFinalBidValues.Clear();
        am.yel.Clear();

        //am.AuctionSayHelper.SetHostTalkerData();
        am.ydz.htf();
        //am.AuctionSayHelper.plotPlayAutoStateCache = LevelDesignerManager.instance.AutoPlay;
        am.ydz.ygl = bmo.bhlr.beah;

        //am._endAuctionCallback = onEndAuction;
        am.yeg = onEndAuction;
        #endregion

        #region AuctionState main application
        //am.AuctionRelicList = state.InitialRelicIds
        //    .Select(ArtifactManager.Instance.GetArtifact)
        //    .ToIl2CppList();
        am.yed = state.InitialRelicIds
            .Select(eg.bgtc.dfe).ToIl2CppList();
        //am.ExecuteRelicList = state.SelectedRelicIds
        //    .Select(ArtifactManager.Instance.GetArtifact)
        //    .ToIl2CppList();
        am.yee = state.SelectedRelicIds
            .Select(eg.bgtc.dfe).ToIl2CppList();
        //am.AlreadyChosenRelicList = state.ConsumedRelicIds
        //    .Select(ArtifactManager.Instance.GetArtifact)
        //    .ToIl2CppList();
        am.yef = state.ConsumedRelicIds
            .Select(eg.bgtc.dfe).ToIl2CppList();
        //am._submitAuctionItemId = state.SubmitItemId;
        am.yeh = state.SubmitItemId;
        //am.AuctionPlayerIdx = state.SubmitItemIndex;
        am.yfi = state.SubmitItemIndex;
        //am._currentBiddingAuctionItemId = state.BiddingItemId;
        am.yej = state.BiddingItemId;
        //am._currentBidingItemIdx = state.BiddingIndex;
        am.yeu = state.BiddingIndex;
        foreach (var itemId in state.PoolItemIds)
        {
            //am.AuctionItemIds.Add(itemId);
            am.yek.Add(itemId);
        }
        foreach (var bidPrice in state.PoolItemBidPrices)
        {
            //am.AuctionItemFinalBidValues.Add(bidPrice);
            am.yel.Add(bidPrice);
        }
        //am._currentAuctionItemNum = state.PoolItemCount;
        am.yei = state.PoolItemCount;
        foreach (var settleItemId in state.SettleItemIds)
        {
            //am.PlayerSettleItems.Add(settleItemId);
            am.yey.Add(settleItemId);
        }
        #endregion

        //am.PlayAuctionBGM();
        am.hot();

        //am._auctionRoundManager.StartAuction();

        //am._auctionRoundManager
        //    ._auctionRoundStateMachine
        //    .SwitchStateWithoutExitEnter(state.RoundState);
        am.ydy.yfm.SwitchStateWithoutExitEnter(state.RoundState);
    }
}
