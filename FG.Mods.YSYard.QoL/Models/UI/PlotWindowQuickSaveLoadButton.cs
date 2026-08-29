using FG.Mods.YSYard.QoL.Helpers;
using FG.Mods.YSYard.QoL.Services;
using Plot;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal class PlotWindowQuickSaveLoadButton : IDisposable
{
    private const string GAMEOBJECT_OPERATION = "Operation";

    private const string GAMEOBJECT_HIDE = "Hide";

    private const string KEY_QSAVED = "QuickSaved";

    private const string KEY_CONFIRM_QLOAD = "ConfirmQuickLoad";

    private RectTransform _rtLayoutGroup;

    private Vector3 _initialLocalPosition;

    private GameObject _goQuickSaveButton;

    private GameObject _goQuickLoadButton;

    private PlotWindowQuickSaveLoadButton() { }

    internal static PlotWindowQuickSaveLoadButton CreateOn(PlotWindow pw)
    {
        //var operation = pw.mTrans.DigFind(GAMEOBJECT_OPERATION);
        var operation = pw.beey.DigFind(GAMEOBJECT_OPERATION);
        if (operation is null)
        {
            return null;
        }
        var rtOperation = operation.GetComponent<RectTransform>();
        if (rtOperation is null)
        {
            return null;
        }
        var glgOperation = operation.GetComponent<GridLayoutGroup>();
        if (glgOperation is null)
        {
            return null;
        }
        glgOperation.spacing = new Vector2(glgOperation.spacing.x, 5f);

        // get Hide button as template
        var hide = operation.GetChild(operation.childCount - 1);
        if (hide is null || hide.name != GAMEOBJECT_HIDE)
        {
            return null;
        }

        // Quick Save
        var goQS = CloneButton(
            hide.gameObject, operation, ModAssetProvider.QuickSaveSprite,
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
            hide.gameObject, operation, ModAssetProvider.QuickLoadSprite,
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

        var ret = new PlotWindowQuickSaveLoadButton
        {
            _rtLayoutGroup = rtOperation,
            _initialLocalPosition = rtOperation.localPosition,
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

        goBtn.GetComponent<Image>()?.sprite = btnSprite;

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
        this._rtLayoutGroup.localPosition = this._initialLocalPosition;
        if (enabled)
        {
            this._rtLayoutGroup.localPosition
                += new Vector3(0f, 60f, 0f);
        }
        this._goQuickSaveButton.SetActive(enabled);
        this._goQuickLoadButton.SetActive(enabled);
    }

    public void Dispose() =>
        ConfigProvider.UseQuickSaveLoad.SettingChanged
            -= this.OnUseQuickSaveLoadChanged;
}
