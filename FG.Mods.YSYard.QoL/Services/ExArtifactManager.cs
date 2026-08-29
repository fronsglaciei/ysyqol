using Common;
using FG.Mods.YSYard.QoL.Models.UI;
using HotelModule.Event;
using System;

namespace FG.Mods.YSYard.QoL.Services;

internal static class ExArtifactManager
{
    internal static void ShowMonthArtifactGroup(
        //HotelEventRelicRecovery ev)
        sx ev)
    {
        if (ev is null)
        {
            throw new ArgumentNullException(nameof(ev));
        }

        //var relicGroupId = ev.EventData.EventValue1;
        var relicGroupId = ev.ymr.xnj;
        //var relicCntReq = ev.EventData.EventValue2;
        var relicCntReq = ev.ymr.xnk;

        //var am = ArtifactManager.Instance;
        var am = eg.bgtc;
        //var curRelicCnt = am.GetCurrentNormalArtifactNum();
        var curRelicCnt = am.deu();
        var isFail = curRelicCnt < relicCntReq;

        var rrwFast = new RelicRecoveryWindowFastAnim();
        rrwFast.Open();

        rrwFast.RefreshNoAnim(
            curRelicCnt, isFail,
            (Il2CppSystem.Action)(() =>
            {
                if (isFail)
                {
                    rrwFast.Close();
                    OnRelicRecoveryEnd(ev);
                    //EventManager.FireEvent(18);
                    wr.jds(18);
                }
                else
                {
                    //ArtifactManager.Instance.ShowArtifactGroup(
                    //    relicGroupId,
                    //    selectEndCallback: (Il2CppSystem.Action)(() =>
                    //    {
                    //        rrwFast.Close();
                    //        OnRelicRecoveryEnd(ev);
                    //    }),
                    //    isMonth: true);
                    eg.bgtc.dew(
                        relicGroupId,
                        d: (Il2CppSystem.Action)(() =>
                        {
                            rrwFast.Close();
                            OnRelicRecoveryEnd(ev);
                        }),
                        e: true);
                }
            }));
    }

    private static void OnRelicRecoveryEnd(
        //HotelEventRelicRecovery ev)
        sx ev)
    {
        //ArtifactManager.Instance.ClearArtifact();
        eg.bgtc.dga();
        //ev.OnFinish();
        ev.ifw();
    }
}
