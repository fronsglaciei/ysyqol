using DG.Tweening;
using FG.Mods.YSYard.QoL.Helpers;
using FG.Mods.YSYard.QoL.Services;
using Foundation.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UGUIExtend;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal partial class ModSettingPanel : IDisposable
{
    private const string KEY_MOD_SETTING_PANEL_TITLE
        = "ModSettingPanelTitle";

    private const string KEY_MOUSE_WHEEL_DOWN_TO_PROGRESS
        = nameof(ConfigProvider.MouseWheelDownToProgress);

    private const string KEY_MOUSE_WHEEL_UP_TO_BACKLOG
        = nameof(ConfigProvider.MouseWheelUpToBacklog);

    private const string KEY_MOUSE_RIGHT_UP_TO_CLOSE_BACKLOG
        = nameof(ConfigProvider.MouseRightUpToCloseBacklog);

    private const string KEY_FORCE_INSTANT_TEXT
        = nameof(ConfigProvider.ForceInstantText);

    private const string KEY_TEXT_FONT_NAME
        = nameof(ConfigProvider.TextFontName);

    private const string KEY_AP_MAX_POINT
        = nameof(ConfigProvider.APMaxPoint);

    private const string KEY_SKIP_ILLUSTRATIONS
        = nameof(ConfigProvider.SkipAutoOpenVisitedPlayGuides);

    private const string KEY_SKIP_BLOOD_BOON_ANIM
        = nameof(ConfigProvider.SkipBloodMoonAnimations);

    private const string KEY_USE_RESTAURANT_FEATURES
        = nameof(ConfigProvider.UseRestaurantFeatures);

    private const string KEY_USE_QSAVELOAD
        = nameof(ConfigProvider.UseQuickSaveLoad);

    private const string KEY_SKIP_CONFIRM_QLOAD
        = nameof(ConfigProvider.SkipConfirmQuickLoad);

    private const string KEY_USE_MOD_TRANSLATIONS
        = nameof(ConfigProvider.UseModTranslations);

    private const float POS_X_SHOW = 450f;

    private const float POS_X_HIDE = 900f;

    private RectTransform _rtPanelRoot;

    private TitleText _titleText;

    private CheckBoxSetting _mouseWheelDownToProgress;

    private CheckBoxSetting _mouseWheelUpToBacklog;

    private CheckBoxSetting _mouseRightUpToCloseBacklog;

    private CheckBoxSetting _forceInstantText;

    private DropDownSetting<string> _textFontName;

    private NumericFieldSetting<long> _apMaxPoint;

    private CheckBoxSetting _skipAutoOpenVisitedPlayGuides;

    private CheckBoxSetting _skipBloodMoonAnimations;

    private CheckBoxSetting _useRestaurantFeatures;

    private CheckBoxSetting _useQuickSaveLoad;

    private CheckBoxSetting _skipConfirmQuickLoad;

    private CheckBoxSetting _useModTranslations;

    private CompositeEventHandler _onLanguageChanged;

    private bool _isVisible;

    private Image _showButtonImage;

    internal static ModSettingPanel CreateOn(
        //GameSettingWindow gsw)
        bfi gsw)
    {
        if (gsw is null)
        {
            return null;
        }
        //var tgRefObj = gsw.FullScreen?.transform.parent.parent.gameObject;
        var tgRefObj = gsw.bclj?.transform.parent.parent.gameObject;
        if (tgRefObj is null)
        {
            return null;
        }
        //var ddRefObj = gsw.Dropdown?.gameObject;
        var ddRefObj = gsw.bclh?.gameObject;
        if (ddRefObj is null)
        {
            return null;
        }
        //var cbRefObj = gsw.MusicButton?.transform.parent.parent.gameObject;
        var cbRefObj = gsw.bclq?.transform.parent.parent.gameObject;
        if (cbRefObj is null)
        {
            return null;
        }
        var nifRefFont = ddRefObj.transform.GetChild(0)?.GetComponent<Text>().font;
        if (nifRefFont is null)
        {
            return null;
        }
        var nifRefImage = ddRefObj.GetComponent<Image>();
        if (nifRefImage is null)
        {
            return null;
        }
        //var imgLineRefObj = gsw.mGameObject.transform
        //    .DigFind("HuaMian")?
        //    .DigFind("ImageLine")?.gameObject;
        var imgLineRefObj = gsw.zqg.transform
            .DigFind("HuaMian")?
            .DigFind("ImageLine")?.gameObject;
        if (imgLineRefObj is null)
        {
            return null;
        }

        // create panel base
        var panelRoot = new GameObject();
        var rtRoot = panelRoot.AddComponent<RectTransform>();
        rtRoot.localPosition += new Vector3(POS_X_HIDE, -20f, 0f);
        panelRoot.AddComponent<CanvasRenderer>();
        var imgRoot = panelRoot.AddComponent<Image>();
        var csfRoot = panelRoot.AddComponent<ContentSizeFitter>();
        csfRoot.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        csfRoot.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var vlgRoot = panelRoot.AddComponent<VerticalLayoutGroup>();
        vlgRoot.spacing = 10f;
        vlgRoot.padding = new()
        {
            left = 20,
            top = 20,
            right = 20,
            bottom = 20,
        };

        //var uiNode = gsw.mGameObject.transform.DigFind("UINode");
        var uiNode = gsw.zqg.transform.DigFind("UINode");
        var bgImgRef = uiNode.DigFind("Bg_Main")?.GetComponent<AdvancedImage>();
        if (bgImgRef is null)
        {
            return null;
        }
        imgRoot.sprite = bgImgRef.sprite;
        imgRoot.material = bgImgRef.material;
        imgRoot.type = bgImgRef.type;
        panelRoot.transform.SetParent(uiNode, false);

        var ttRef = uiNode.DigFind("TitleText")?.gameObject;
        if (ttRef is null)
        {
            return null;
        }

        var ret = new ModSettingPanel
        {
            _rtPanelRoot = rtRoot,
            _titleText = TitleText
                .CreateFromTemplate(
                    ttRef, KEY_MOD_SETTING_PANEL_TITLE,
                    panelRoot.transform),
            _mouseWheelDownToProgress = CheckBoxSetting
                .CreateFromTemplate(
                    cbRefObj, KEY_MOUSE_WHEEL_DOWN_TO_PROGRESS,
                    panelRoot.transform,
                    nameof(ConfigProvider.MouseWheelDownToProgress))
        };
        ret._mouseWheelDownToProgress.SetOption(
            x => ConfigProvider.MouseWheelDownToProgress.Value = x,
            ConfigProvider.MouseWheelDownToProgress.Value);

        ret._mouseWheelUpToBacklog = CheckBoxSetting
            .CreateFromTemplate(
                cbRefObj, KEY_MOUSE_WHEEL_UP_TO_BACKLOG,
                panelRoot.transform,
                nameof(ConfigProvider.MouseWheelUpToBacklog));
        ret._mouseWheelUpToBacklog.SetOption(
            x => ConfigProvider.MouseWheelUpToBacklog.Value = x,
            ConfigProvider.MouseWheelUpToBacklog.Value);

        ret._mouseRightUpToCloseBacklog = CheckBoxSetting
            .CreateFromTemplate(
                cbRefObj, KEY_MOUSE_RIGHT_UP_TO_CLOSE_BACKLOG,
                panelRoot.transform,
                nameof(ConfigProvider.MouseRightUpToCloseBacklog));
        ret._mouseRightUpToCloseBacklog.SetOption(
            x => ConfigProvider.MouseRightUpToCloseBacklog.Value = x,
            ConfigProvider.MouseRightUpToCloseBacklog.Value);

        ret._forceInstantText = CheckBoxSetting
            .CreateFromTemplate(
                cbRefObj, KEY_FORCE_INSTANT_TEXT,
                panelRoot.transform,
                nameof(ConfigProvider.ForceInstantText));
        ret._forceInstantText.SetOption(
            x => ConfigProvider.ForceInstantText.Value = x,
            ConfigProvider.ForceInstantText.Value);

        ret._textFontName = DropDownSetting<string>
            .CreateFromTemplate(
                ddRefObj, KEY_TEXT_FONT_NAME,
                panelRoot.transform,
                nameof(ConfigProvider.TextFontName));
        ret._textFontName.SetOptions(
            [.. FontManager.OSFonts.Select(
                x => new ValueCaptionPair<string>{ Value = x, CaptionKey = x })],
            x => ConfigProvider.TextFontName.Value = x,
            ConfigProvider.TextFontName.Value);

        SplitLine.CreateFromTemplate(
            imgLineRefObj, panelRoot.transform);

        ret._apMaxPoint = NumericFieldSetting<long>
            .CreateFromTemplate(
                nifRefFont, nifRefImage, KEY_AP_MAX_POINT,
                panelRoot.transform,
                nameof(ConfigProvider.APMaxPoint));
        ret._apMaxPoint.SetOption(
            5L, long.MaxValue - 1,
            x => ConfigProvider.APMaxPoint.Value = x,
            ConfigProvider.APMaxPoint.Value);

        ret._skipAutoOpenVisitedPlayGuides = CheckBoxSetting
            .CreateFromTemplate(
                cbRefObj, KEY_SKIP_ILLUSTRATIONS,
                panelRoot.transform,
                nameof(ConfigProvider.SkipAutoOpenVisitedPlayGuides));
        ret._skipAutoOpenVisitedPlayGuides.SetOption(
            x => ConfigProvider.SkipAutoOpenVisitedPlayGuides.Value = x,
            ConfigProvider.SkipAutoOpenVisitedPlayGuides.Value);

        ret._skipBloodMoonAnimations = CheckBoxSetting
            .CreateFromTemplate(
                cbRefObj, KEY_SKIP_BLOOD_BOON_ANIM,
                panelRoot.transform,
                nameof(ConfigProvider.SkipBloodMoonAnimations));
        ret._skipBloodMoonAnimations.SetOption(
            x => ConfigProvider.SkipBloodMoonAnimations.Value = x,
            ConfigProvider.SkipBloodMoonAnimations.Value);

        ret._useRestaurantFeatures = CheckBoxSetting
            .CreateFromTemplate(
                cbRefObj, KEY_USE_RESTAURANT_FEATURES,
                panelRoot.transform,
                nameof(ConfigProvider.UseRestaurantFeatures));
        ret._useRestaurantFeatures.SetOption(
            x => ConfigProvider.UseRestaurantFeatures.Value = x,
            ConfigProvider.UseRestaurantFeatures.Value);

        SplitLine.CreateFromTemplate(
            imgLineRefObj, panelRoot.transform);

        ret._useQuickSaveLoad = CheckBoxSetting
            .CreateFromTemplate(
                cbRefObj, KEY_USE_QSAVELOAD,
                panelRoot.transform,
                nameof(ConfigProvider.UseQuickSaveLoad));
        ret._useQuickSaveLoad.SetOption(
            x => ConfigProvider.UseQuickSaveLoad.Value = x,
            ConfigProvider.UseQuickSaveLoad.Value);

        ret._skipConfirmQuickLoad = CheckBoxSetting
            .CreateFromTemplate(
                cbRefObj, KEY_SKIP_CONFIRM_QLOAD,
                panelRoot.transform,
                nameof(ConfigProvider.SkipConfirmQuickLoad));
        ret._skipConfirmQuickLoad.SetOption(
            x => ConfigProvider.SkipConfirmQuickLoad.Value = x,
            ConfigProvider.SkipConfirmQuickLoad.Value);

        SplitLine.CreateFromTemplate(
            imgLineRefObj, panelRoot.transform);

        ret._useModTranslations = CheckBoxSetting
            .CreateFromTemplate(
                cbRefObj, KEY_USE_MOD_TRANSLATIONS,
                panelRoot.transform,
                nameof(ConfigProvider.UseModTranslations));
        ret._useModTranslations.SetOption(
            x => ConfigProvider.UseModTranslations.Value = x,
            ConfigProvider.UseModTranslations.Value);

        ret._onLanguageChanged =
            CompositeEventHandler.OnLanguageChanged(
                [
                ret._titleText.UpdateText,
                ret._mouseWheelDownToProgress.UpdateText,
                ret._mouseWheelUpToBacklog.UpdateText,
                ret._mouseRightUpToCloseBacklog.UpdateText,
                ret._forceInstantText.UpdateText,
                ret._textFontName.UpdateText,
                ret._apMaxPoint.UpdateText,
                ret._skipAutoOpenVisitedPlayGuides.UpdateText,
                ret._skipBloodMoonAnimations.UpdateText,
                ret._useRestaurantFeatures.UpdateText,
                ret._useQuickSaveLoad.UpdateText,
                ret._skipConfirmQuickLoad.UpdateText,
                ]);

        // create show <-> hide toggle button
        var goShowHide = new GameObject();
        var rtShowHide = goShowHide.AddComponent<RectTransform>();
        rtShowHide.localPosition += new Vector3(600f, -300f, 0f);
        var btnShowHide = goShowHide.AddComponent<Button>();
        btnShowHide.onClick.AddListener((UnityAction)ret.ToggleVisible);

        var goShowHideImg = new GameObject();
        goShowHideImg.AddComponent<RectTransform>();
        goShowHideImg.AddComponent<CanvasRenderer>();
        var imgShowHideMain = goShowHideImg.AddComponent<Image>();
        imgShowHideMain.sprite = ModAssetProvider.ShowModSettingsSprite;
        imgShowHideMain.preserveAspect = true;
        ret._showButtonImage = imgShowHideMain;

        goShowHideImg.transform.SetParent(goShowHide.transform, false);
        goShowHide.transform.SetParent(uiNode, false);

        return ret;
    }

    internal void Refresh()
    {
        this._mouseWheelDownToProgress?.SetValueWithoutNotify(
            ConfigProvider.MouseWheelDownToProgress.Value);
        this._mouseWheelUpToBacklog?.SetValueWithoutNotify(
            ConfigProvider.MouseWheelUpToBacklog.Value);
        this._mouseRightUpToCloseBacklog?.SetValueWithoutNotify(
            ConfigProvider.MouseRightUpToCloseBacklog.Value);
        this._forceInstantText?.SetValueWithoutNotify(
            ConfigProvider.ForceInstantText.Value);
        this._textFontName?.SetValueWithoutNotify(
            ConfigProvider.TextFontName.Value);
        this._apMaxPoint?.SetValueWithoutNotify(
            ConfigProvider.APMaxPoint.Value);
        this._skipAutoOpenVisitedPlayGuides?.SetValueWithoutNotify(
            ConfigProvider.SkipAutoOpenVisitedPlayGuides.Value);
        this._skipBloodMoonAnimations?.SetValueWithoutNotify(
            ConfigProvider.SkipBloodMoonAnimations.Value);
        this._useQuickSaveLoad?.SetValueWithoutNotify(
            ConfigProvider.UseQuickSaveLoad.Value);
        this._skipConfirmQuickLoad?.SetValueWithoutNotify(
            ConfigProvider.SkipConfirmQuickLoad.Value);
    }

    private void ToggleVisible()
    {
        this._isVisible = !this._isVisible;
        this._rtPanelRoot?.DOLocalMoveX(
            this._isVisible ? POS_X_SHOW : POS_X_HIDE,
            0.5f);
        this._showButtonImage.sprite
            = this._isVisible
            ? ModAssetProvider.HideModSettingsSprite
            : ModAssetProvider.ShowModSettingsSprite;
    }

    public void Dispose() => this._onLanguageChanged?.Dispose();
}
