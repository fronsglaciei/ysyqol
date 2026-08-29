using FG.Mods.YSYard.QoL.Helpers;
using FG.Mods.YSYard.QoL.Services;
using HotelModule.UI;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal class RelicRecoveryWindowQuickSaveLoadButton : IDisposable
{
    private const string KEY_QSAVED = "QuickSaved";

    private const string KEY_CONFIRM_QLOAD = "ConfirmQuickLoad";

    private GameObject _goLayoutGroup;

    internal static RelicRecoveryWindowQuickSaveLoadButton CreateOn(
        //RelicRecoveryWindow rrw)
        vc rrw)
    {
        if (rrw is null)
        {
            return null;
        }

        //var goCloseButton = rrw.mGameObject.transform
        //    .DigFind("CloseButton").gameObject;
        var goCloseButton = rrw.zqg.transform
            .DigFind("CloseButton").gameObject;

        var goHlg = new GameObject();
        goHlg.AddComponent<RectTransform>();
        goHlg.AddComponent<HorizontalLayoutGroup>();
        goHlg.transform.SetParent(goCloseButton.transform.parent, false);

        // Quick Save
        CreateButton(
            goCloseButton, goHlg.transform,
            ModAssetProvider.QuickSaveSprite,
            () =>
            {
                if (!ModAssetProvider.TryGetModText(KEY_QSAVED, out var tipText)
                    || string.IsNullOrEmpty(tipText))
                {
                    throw new InvalidOperationException("string key not found");
                }
                ExSaveLoadManager.QuickSave();
                //GameAPI.ShowTips(tipText);
                cy.cuw(tipText);
            });

        // Quick Load
        CreateButton(
            goCloseButton, goHlg.transform,
            ModAssetProvider.QuickLoadSprite,
            () =>
            {
                if (!ModAssetProvider.TryGetModText(
                    KEY_CONFIRM_QLOAD, out var wndText)
                    || string.IsNullOrEmpty(wndText))
                {
                    throw new InvalidOperationException("string key not found");
                }
                if (ConfigProvider.SkipConfirmQuickLoad.Value)
                {
                    ExHotelManager.QuickContinueGame();
                }
                else
                {
                    //GameAPI.ShowSecondaryConfirmWindow((Il2CppSystem.Action)(() =>
                    cy.cux((Il2CppSystem.Action)(() =>
                    {
                        ExHotelManager.QuickContinueGame();
                    }),
                    (Il2CppSystem.Action)(() => { }), wndText);
                }
            });

        var ret = new RelicRecoveryWindowQuickSaveLoadButton
        {
            _goLayoutGroup = goHlg,
        };
        ret.OnUseQuickSaveLoadChanged(null, null);
        ConfigProvider.UseQuickSaveLoad.SettingChanged +=
            ret.OnUseQuickSaveLoadChanged;
        return ret;
    }

    private static GameObject CreateButton(
        GameObject refObj, Transform parent,
        Sprite btnSprite, System.Action onClick)
    {
        if (refObj is null)
        {
            throw new ArgumentNullException($"{nameof(refObj)}");
        }

        var goBtn = new GameObject();

        goBtn.AddComponent<RectTransform>();
        goBtn.AddComponent<CanvasRenderer>();

        var img = goBtn.AddComponent<Image>();
        img.material = refObj.GetComponent<Image>().material;
        img.sprite = btnSprite;
        img.preserveAspect = true;

        var btn = goBtn.AddComponent<Button>();
        btn.onClick.AddListener((UnityAction)onClick);

        goBtn.transform.SetParent(parent, false);

        return goBtn;
    }

    private void OnUseQuickSaveLoadChanged(object _, EventArgs __)
    {
        this._goLayoutGroup.SetActive(
            ConfigProvider.UseQuickSaveLoad.Value);
    }

    public void Dispose() =>
        ConfigProvider.UseQuickSaveLoad.SettingChanged
            -= this.OnUseQuickSaveLoadChanged;
}
