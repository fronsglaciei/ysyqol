using FG.Mods.YSYard.QoL.Models.UI;
using Foundation.UI;
using HotelModule.UI;
using Plot;

namespace FG.Mods.YSYard.QoL.Services;

internal static class ExUIManager
{
    private static ModSettingPanel _msp;

    private static HotelMainWindowQuickSaveLoadButton _hmwQSLButton;

    private static PlotWindowWriterFontSetter _pwFontSetter;

    private static HotelRestaurantWindowReportButton _hrwReportButton;

    private static PlotWindowQuickSaveLoadButton _pwQSLButton;

    private static RelicRecoveryWindowQuickSaveLoadButton _rrwQSLButton;

    private static ArtifactSelectWindowQuickLoadButton _aswQLButton;

    internal static T GetOrLoad<T>(string uiName, bool addsToDictionary = true)
        //where T : UIBase, new()
        where T : yv, new()
    {
        //var uim = UIManager.Instance;
        var uim = UIManager.bhje;

        //UIManager.GetUIName is deprecated?
        //uiName = uim.GetUIName(uiName);

        //if (uim.mUIMap.TryGetValue(uiName, out var ui))
        if (uim.zqv.TryGetValue(uiName, out var ui))
        {
            return ui.TryCast<T>();
        }
        else
        {
            ui = uim.Load<T>(uiName);
            if (ui is not null)
            {
                if (addsToDictionary)
                {
                    //uim.curUIList.Add(uiName);
                    uim.zrg.Add(uiName);
                    //uim.mUIMap.Add(uiName, ui);
                    uim.zqv.Add(uiName, ui);
                }
                return ui.TryCast<T>();
            }
        }
        return null;
    }

    internal static void CreateModSettingPanel(
        //GameSettingWindow gsw)
        bfi gsw)
    {
        _msp?.Dispose();
        _msp = ModSettingPanel.CreateOn(gsw);
    }

    internal static void OnHotelRestaurantWindowInit(
        //HotelRestaurantWindow hrw)
        bcx hrw)
    {
        _hrwReportButton?.Dispose();
        _hrwReportButton = HotelRestaurantWindowReportButton.CreateOn(hrw);
    }

    internal static void OnHotelMainWindowInit(HotelMainWindow hmw)
    {
        _hmwQSLButton?.Dispose();
        _hmwQSLButton = HotelMainWindowQuickSaveLoadButton.CreateOn(hmw);
    }

    internal static void OnHotelMainWindowDestroyed()
    {
        _hmwQSLButton?.Dispose();
        _hmwQSLButton = null;
    }

    internal static void OnPlotWindowInit(PlotWindow pw)
    {
        PlotWindowMouseWatcher.CreateOn(pw);

        _pwFontSetter?.Dispose();
        _pwFontSetter = PlotWindowWriterFontSetter.Create(pw);

        _pwQSLButton?.Dispose();
        _pwQSLButton = PlotWindowQuickSaveLoadButton.CreateOn(pw);
    }

    internal static void OnPlotControlWindowDestroyed()
    {
        _pwFontSetter?.Dispose();
        _pwFontSetter = null;

        _pwQSLButton?.Dispose();
        _pwQSLButton = null;
    }

    internal static void OnRelicRecoveryWindowInit(
        //RelicRecoveryWindow rrw)
        vc rrw)
    {
        _rrwQSLButton?.Dispose();
        _rrwQSLButton = RelicRecoveryWindowQuickSaveLoadButton
            .CreateOn(rrw);
    }

    internal static void OnArtifactSelectWindowInit(
        //ArtifactSelectWindow asw)
        va asw)
    {
        _aswQLButton?.Dispose();
        _aswQLButton = ArtifactSelectWindowQuickLoadButton
            .CreateOn(asw);
    }
}
