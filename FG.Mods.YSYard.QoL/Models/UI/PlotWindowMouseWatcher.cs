using Common;
using FG.Mods.YSYard.QoL.Services;
using Foundation.UI;
using Il2CppInterop.Runtime.Injection;
using Plot;
using UnityEngine;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal class PlotWindowMouseWatcher : MonoBehaviour
{
    private const string WINDOW_NAME_PLOT_REVIEW = "PlotReview";

    private PlotWindow _pw;

    static PlotWindowMouseWatcher()
    {
        ClassInjector.RegisterTypeInIl2Cpp<PlotWindowMouseWatcher>();
    }

    internal static void CreateOn(PlotWindow pw)
    {
        //if (pw.mBGButton is null)
        if (pw.becz is null)
        {
            //Plugin.Log.LogError($"{nameof(PlotWindow.mBGButton)} is null");
            Plugin.Log.LogError($"{nameof(PlotWindow)}.mBGButton is null");
            return;
        }
        //var mw = pw.mBGButton.gameObject.GetOrAddCompent<PlotWindowMouseWatcher>();
        var mw = pw.becz.gameObject.cvf<PlotWindowMouseWatcher>();
        mw._pw = pw;
    }

    private void Update()
    {
        var amount = Input.mouseScrollDelta.y;
        if (!Mathf.Approximately(amount, 0f))
        {
            if (amount < 0f)
            {
                this.OnWheelDown();
            }
            else if (0f < amount)
            {
                this.OnWheelUp();
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            OnRightButtonUp();
        }
    }

    private void OnWheelDown()
    {
        if (!ConfigProvider.MouseWheelDownToProgress.Value)
        {
            return;
        }
        //if (UIManager.Instance.HasWindowOpen(WINDOW_NAME_PLOT_REVIEW))
        if (UIManager.bhje.jvi(WINDOW_NAME_PLOT_REVIEW))
        {
            return;
        }

        // reference: Plot.PlotWindow.OnClickNext
        //if (this._pw?.mHideDialog ?? false)
        if (this._pw?.befw ?? false)
        {
            //this._pw.mHideDialog = false;
            this._pw.befw = false;
            //this._pw?.SetDialogHide(false);
            this._pw.njx(false);
            return;
        }
        //this._pw?.mCurDialogInput?.SetDialogClickedFlag();
        this._pw?.begb?.SetDialogClickedFlag();
        //if (LevelDesignerManager.instance.mFast)
        if (bmo.bhlr.beai)
        {
            //this._pw?.onSpeedUpUp();
            this._pw?.njq();
        }
        //EventManager.FireEvent(11);
        wr.jds(11);
    }

    private void OnWheelUp()
    {
        if (!ConfigProvider.MouseWheelUpToBacklog.Value)
        {
            return;
        }
        //if (UIManager.Instance.HasWindowOpen(WINDOW_NAME_PLOT_REVIEW))
        if (UIManager.bhje.jvi(WINDOW_NAME_PLOT_REVIEW))
        {
            return;
        }

        //this._pw?.OpenReviewWindow();
        this._pw?.nka();
    }

    private static void OnRightButtonUp()
    {
        if (!ConfigProvider.MouseRightUpToCloseBacklog.Value)
        {
            return;
        }

        //var uim = UIManager.Instance;
        var uim = UIManager.bhje;
        if (!uim.jvi(WINDOW_NAME_PLOT_REVIEW))
        {
            return;
        }
        //uim.CloseUI(WINDOW_NAME_PLOT_REVIEW);
        uim.juw(WINDOW_NAME_PLOT_REVIEW);
        //uim.DestroyUI(WINDOW_NAME_PLOT_REVIEW, false);
        uim.jvg(WINDOW_NAME_PLOT_REVIEW, false);
    }
}
