using FG.Mods.YSYard.QoL.Services;
using Foundation.UI;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal class HotelMainWindowQuickSaveLoadButton : IDisposable
{
    private const string GAMEOBJECT_LOAD = "Load";

    private const string KEY_QSAVED = "QuickSaved";

    private const string KEY_CONFIRM_QLOAD = "ConfirmQuickLoad";

    private GameObject _goQuickSaveButton;

    private GameObject _goQuickLoadButton;

    internal static HotelMainWindowQuickSaveLoadButton CreateOn(
        HotelMainWindow hmw)
    {
        //var sl = hmw.SettingList
        var sl = hmw.bbhp;
        if (sl is null)
        {
            return null;
        }

        // get Load button as template
        var load = sl.GetChild(sl.childCount - 1);
        if (load is null || load.name != GAMEOBJECT_LOAD)
        {
            return null;
        }

        // Quick Save
        var goQS = CloneButton(
            load.gameObject, sl, ModAssetProvider.QuickSaveMainSprite,
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
        var goQL = CloneButton(
            load.gameObject, sl, ModAssetProvider.QuickLoadMainSprite,
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

        var ret = new HotelMainWindowQuickSaveLoadButton
        {
            _goQuickSaveButton = goQS,
            _goQuickLoadButton = goQL,
        };
        ret.OnUseQuickSaveLoadChanged(null, null);
        ConfigProvider.UseQuickSaveLoad.SettingChanged +=
            ret.OnUseQuickSaveLoadChanged;
        return ret;
    }

    private static GameObject CloneButton(
        GameObject refObj, Transform parent,
        Sprite btnSprite, System.Action onClick)
    {
        if (refObj is null)
        {
            throw new ArgumentNullException($"{nameof(refObj)}");
        }

        var goBtn = GameObject.Instantiate(refObj);
        goBtn.transform.SetParent(parent, false);

        var img = goBtn.GetComponent<Image>();
        if (img is not null)
        {
            img.preserveAspect = true;
            img.sprite = btnSprite;
        }

        var btn = goBtn.GetComponent<Button>();
        if (btn is not null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener((UnityAction)onClick);
        }

        return goBtn;
    }

    private void OnUseQuickSaveLoadChanged(object _, EventArgs __)
    {
        var enabled = ConfigProvider.UseQuickSaveLoad.Value;
        this._goQuickSaveButton?.SetActive(enabled);
        this._goQuickLoadButton?.SetActive(enabled);
    }

    public void Dispose() =>
        ConfigProvider.UseQuickSaveLoad.SettingChanged
            -= this.OnUseQuickSaveLoadChanged;
}
