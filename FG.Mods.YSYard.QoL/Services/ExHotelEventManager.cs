using Common;
using FG.Mods.YSYard.QoL.Models;
using FG.Mods.YSYard.QoL.Models.Saves;
using HotelModule;
using HotelModule.Event;
using System;
using System.Collections.Generic;

namespace FG.Mods.YSYard.QoL.Services;

internal static class ExHotelEventManager
{
    private static readonly List<string> _retrievableTypeNames =
        [
        nameof(so), // HotelEventEnding
        nameof(ss), // HotelEventPlayMultipleStory
        nameof(st) // HotelEventPlayStory
        ];

    internal static HotelEventManagerState CurrenState
    {
        get
        {
            //var hem = HotelEventManager.Instance;
            var hem = sr.bhfx;
            var ret = new List<HotelEventState>();
            //foreach (var item in hem.hotelEvents)
            foreach (var item in hem.ynb)
            {
                var state = GetState(item);
                if (state is not null)
                {
                    ret.Add(state);
                }
            }
            return new()
            {
                //CurrentEvent = GetState(hem.currentEvent),
                CurrentEvent = GetState(hem.ync),
                EventQueue = ret
            };
        }
    }

    internal static void RetrieveState()
    {
        var state = ExSaveLoadManager.QuickSaveDataCache?.HotelEventManagerState;
        if (state is null)
        {
            return;
        }

        using var context = new RetrievingContext();

        ExCommonEventManager.UnregisterAllInstanceMethods(
            5, _retrievableTypeNames);

        if (state.CurrentEvent is not null)
        {
            context.RegisterCurrentState(state.CurrentEvent);
        }
        foreach (var queuedState in state.EventQueue)
        {
            var queuedEv = GetEvent(queuedState);
            if (queuedEv is not null)
            {
                //HotelEventManager.Instance.hotelEvents.Enqueue(queuedEv);
                sr.bhfx.ynb.Enqueue(queuedEv);
            }
        }
    }

    private static HotelEventState GetState(
        //HotelEventBase hotelEvent)
        sk hotelEvent)
    {
        if (hotelEvent is null)
        {
            return null;
        }

        //var npcEventId = hotelEvent.EventData.NPCEventID;
        var npcEventId = hotelEvent.ymr.xng;
        var state = new HotelEventState
        {
            NPCEventId = npcEventId
        };
        //if (hotelEvent.GetIl2CppType().Name == nameof(HotelEventEnding))
        if (hotelEvent.GetIl2CppType().Name == nameof(so))
        {
            //var evEnding = hotelEvent.Cast<HotelEventEnding>();
            var evEnding = hotelEvent.Cast<so>();
            //state.PostCreditStory = evEnding._postCreditScenePlot;
            state.PostCreditStory = evEnding.ymy;
        }
        return state;
    }

    private static
        //HotelEventBase GetEvent(HotelEventState state)
        sk GetEvent(HotelEventState state)
    {
        if (state is null)
        {
            return null;
        }

        //var evData = NpcEventManager.Instance.GetItem(state.NPCEventId);
        var evData = hq.bgvw.GetItem(state.NPCEventId);
        if (evData is null)
        {
            return null;
        }

        //return HotelEventManager.Instance.GenerateEvent(evData);
        return sr.bhfx.ifq(evData);
    }

    private class RetrievingContext : IDisposable
    {
        private readonly HotelEventBlockUpdate _ev;

        private HotelEventState _currentEventState;

        internal RetrievingContext()
        {
            //this._ev = new(new() { NPCEventID = -100 });
            this._ev = new(new() { xng = -100 });
            //HotelEventManager.Instance.currentEvent = this._ev;
            sr.bhfx.ync = this._ev;
        }

        internal void RegisterCurrentState(HotelEventState state)
            => this._currentEventState = state;

        public void Dispose()
        {
            if (this._currentEventState is not null)
            {
                //var evData = NpcEventManager.Instance.GetItem(this._currentEventState.NPCEventId);
                var evData = hq.bgvw.GetItem(this._currentEventState.NPCEventId);
                //var ev = HotelEventManager.Instance.GenerateEvent(evData);
                var ev = sr.bhfx.ifq(evData);
                if (ev is not null)
                {
                    //HotelEventManager.Instance.currentEvent = ev;
                    sr.bhfx.ync = ev;
                    switch (ev.GetIl2CppType().Name)
                    {
                        case nameof(so): // HotelEventEnding
                            //StartEnding(ev.Cast<HotelEventEnding>(), this._currentEventState.PostCreditStory);
                            StartEnding(ev.Cast<so>(), this._currentEventState.PostCreditStory);
                            break;

                        case nameof(ss): // HotelEventPlayMultipleStory
                            //StartPlayMultipleStory(ev.Cast<HotelEventPlayMultipleStory>());
                            StartPlayMultipleStory(ev.Cast<ss>());
                            break;

                        case nameof(st): // HotelEventPlayStory
                            //StartPlayStory(ev.Cast<HotelEventPlayStory>());
                            StartPlayStory(ev.Cast<st>());
                            break;

                        default:
                            //ev.StartEvent();
                            ev.ifc();
                            break;
                    }
                }
            }
            this._ev.ExplicitFinish();
        }

        private static void StartEnding(
            //HotelEventEnding ev, int postCreditStory)
            so ev, int postCreditStory)
        {
            //var evData = ev.EventData;
            var evData = ev.ymr;
            //if (evData.EventValue6 is not null && 2 <= evData.EventValue6.Count)
            if (evData.xno is not null && 2 <= evData.xno.Count)
            {
                //ev._bgmId = evData.EventValue6[1];
                ev.ymu = evData.xno[1];
                //if (3 <= evData.EventValue6.Count)
                if (3 <= evData.xno.Count)
                {
                    //ev._isGameEnd =
                    //    evData.EventValue6[2] == 1 || evData.EventValue6[2] == 2;
                    ev.ymv = evData.xno[2] == 1 || evData.xno[2] == 2;
                    //ev._gameEndState = evData.EventValue6[2] switch
                    //{
                    //    1 => HotelEventEnding.EGameEndState.NormalEnd,
                    //    2 => HotelEventEnding.EGameEndState.HappyEnd,
                    //    _ => HotelEventEnding.EGameEndState.None
                    //};
                    ev.ymw = evData.xno[2] switch
                    {
                        1 => so.sm.NormalEnd,
                        2 => so.sm.HappyEnd,
                        _ => so.sm.None
                    };
                }
            }
            //EndingHelper.IsEnding = true;
            rc.yhj = true;

            if (postCreditStory == 0)
            {
                //EventManager.RegisterEvent(
                //    5, (Il2CppSystem.Action<int, bool>)ev.OnStoryEnd);
                wr.jdl(5, (Il2CppSystem.Action<int, bool>)ev.ifd);
            }
            else
            {
                //ev._postCreditScenePlot = postCreditStory;
                ev.ymy = postCreditStory;
                //ev._postCreditVideoPath =
                //    GlobalParamStringManager.Instance
                //        .GetItem("EndingVideo").Value;
                ev.ymx = hb.bgvh.GetItem("EndingVideo").xhr;
                //ev._foundersVideoPath =
                //    GlobalParamStringManager.Instance
                //        .GetItem("CrowdfundingVideo").Value;
                ev.ymz = hb.bgvh.GetItem("CrowdfundingVideo").xhr;
                //EventManager.RegisterEvent(
                //    5, (Il2CppSystem.Action<int, bool>)ev.AfterCreditsScenePlayEnd);
                wr.jdl(5, (Il2CppSystem.Action<int, bool>)ev.iff);
            }
        }

        private static void StartPlayMultipleStory(
            //HotelEventPlayMultipleStory ev)
            ss ev)
        {
            //EventManager.RegisterEvent(5, (Il2CppSystem.Action<int, bool>)ev.FinishPlot);
            wr.jdl(5, (Il2CppSystem.Action<int, bool>)ev.ift);
        }

        private static void StartPlayStory(
            //HotelEventPlayStory ev)
            st ev)
        {
            //EventManager.RegisterEvent(5, (Il2CppSystem.Action<int, bool>)ev.FinishPlot);
            wr.jdl(5, (Il2CppSystem.Action<int, bool>)ev.ifu);
        }
    }
}
