using FG.Mods.YSYard.QoL.Services;
using Foundation.UI;
using HotelModule.UI;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal class ArtifactSelectWindowQuickLoadButton : IDisposable
{
    private const string KEY_CONFIRM_QLOAD = "ConfirmQuickLoad";

    private const string WINDOW_NAME_RELIC_RECOVERY = "UI_RelicRecovery";

    private GameObject _goQuickLoadButton;

    internal static ArtifactSelectWindowQuickLoadButton CreateOn(
        //ArtifactSelectWindow asw)
        va asw)
    {
        if (asw is null)
        {
            return null;
        }
        var mBtnOK = asw.yvk;
        if (mBtnOK is null || mBtnOK.transform.parent is null)
        {
            return null;
        }

        var goQL = new GameObject("QuickLoad");
        goQL.transform.SetParent(mBtnOK.transform.parent, false);
        var rtQL = goQL.AddComponent<RectTransform>();
        var goQLImg = new GameObject("QuickLoadImage");
        goQLImg.transform.SetParent(goQL.transform, false);
        var rtQLImg = goQLImg.AddComponent<RectTransform>();
        goQLImg.AddComponent<CanvasGroup>();
        var imgQL = goQLImg.AddComponent<Image>();
        imgQL.sprite = ModAssetProvider.QuickLoadSprite;
        var rect = imgQL.sprite.textureRect;

        rtQLImg.sizeDelta = new Vector2(rect.width, rect.height);

        rtQL.localPosition += new Vector3(-550f, -280f, 0f);

        var btnQL = goQLImg.AddComponent<Button>();
        btnQL.onClick.AddListener((UnityAction)(() =>
        {
            if (!ModAssetProvider.TryGetModText(
                    KEY_CONFIRM_QLOAD, out var wndText)
                    || string.IsNullOrEmpty(wndText))
            {
                throw new InvalidOperationException("string key not found");
            }
            if (ConfigProvider.SkipConfirmQuickLoad.Value)
            {
                ExecuteQuickLoad(asw);
            }
            else
            {
                //GameAPI.ShowSecondaryConfirmWindow((Il2CppSystem.Action)(() =>
                cy.cux((Il2CppSystem.Action)(() =>
                {
                    ExecuteQuickLoad(asw);
                }),
                (Il2CppSystem.Action)(() => { }), wndText);
            }
        }));

        var ret = new ArtifactSelectWindowQuickLoadButton
        {
            _goQuickLoadButton = goQL
        };
        ret.OnUseQuickSaveLoadChanged(null, null);
        ConfigProvider.UseQuickSaveLoad.SettingChanged +=
            ret.OnUseQuickSaveLoadChanged;
        return ret;
    }

    private static void ExecuteQuickLoad(
        //ArtifactSelectWindow asw
        va asw)
    {
        asw.Close(false, false);
        //var uim = UIManager.Instance;
        var uim = UIManager.bhje;
        //if (uim.HasWindowOpen(WINDOW_NAME_RELIC_RECOVERY))
        if (uim.jvi(WINDOW_NAME_RELIC_RECOVERY))
        {
            //uim.CloseUI(WINDOW_NAME_RELIC_RECOVERY);
            uim.juw(WINDOW_NAME_RELIC_RECOVERY);
            //ArtifactManager.Instance.ClearArtifact();
            eg.bgtc.dga();
        }
        ExHotelManager.QuickContinueGame();
    }

    private void OnUseQuickSaveLoadChanged(object _, EventArgs __)
    {
        this._goQuickLoadButton.SetActive(
            ConfigProvider.UseQuickSaveLoad.Value);
    }

    public void Dispose() =>
        ConfigProvider.UseQuickSaveLoad.SettingChanged
            -= this.OnUseQuickSaveLoadChanged;
}
