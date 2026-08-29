using FG.Mods.YSYard.QoL.Services;
using Foundation.UI;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal class HotelRestaurantWindowReportButton : IDisposable
{
    private const string PATH_UI_MAIN = "arts/ui/interface/ui_main";

    private const string SPRITE_NAME_INFO_ICON = "InfoIcon";

    private const string SPRITE_NAME_INFO_BG = "InfoBg";

    private const string SPRITE_NAME_CLOSE_BUTTON_NORMAL = "CloseButtonNormal";

    private const string SPRITE_NAME_LINE = "Line";

    private const string KEY_MENU_REPORT_TITLE = "MenuReportTitle";

    private const string KEY_MENU_REPORT_TIP = "MenuReportTip";

    private static Sprite _spriteInfoIcon;

    private static Sprite _spriteInfoBg;

    private static Sprite _spriteCloseButtonNormal;

    private static Sprite _spriteLine;

    private Button _btnShowReport;

    private GameObject _wndReportRoot;

    private Text _titleText;

    private Text _tipText;

    private Text _reportText;

    internal static HotelRestaurantWindowReportButton CreateOn(
        bcx hrw)
    {
        if (!Init())
        {
            return null;
        }

        //if (hrw?.QuickAdjustment is null
        if (hrw?.bbkb is null
            //|| hrw._quickAdjustmentWidget is null
            || hrw.bbkq is null
            //|| hrw.Income is null
            || hrw.bbjt is null)
        {
            return null;
        }

        #region report button
        var goReport = new GameObject("OpenMenuReport");
        goReport.transform.SetParent(hrw.bbkb, false);
        var rtReport = goReport.AddComponent<RectTransform>();
        goReport.AddComponent<CanvasRenderer>();
        var imgReport = goReport.AddComponent<Image>();
        imgReport.sprite = _spriteInfoIcon;

        rtReport.sizeDelta = new Vector2(30f, 30f);
        rtReport.localPosition += new Vector3(410f, 285f, 0f);
        #endregion

        #region sub window
        var goWnd = new GameObject("MenuReportWindow");
        goWnd.SetActive(false);
        goWnd.transform.SetParent(hrw.bbkb, false);
        var rtWnd = goWnd.AddComponent<RectTransform>();
        goWnd.AddComponent<CanvasRenderer>();
        var imgWnd = goWnd.AddComponent<Image>();
        imgWnd.sprite = _spriteInfoBg;
        imgWnd.type = Image.Type.Sliced;
        var vlgWnd = goWnd.AddComponent<VerticalLayoutGroup>();
        vlgWnd.padding = new(30, 30, 30, 30);
        vlgWnd.spacing = 10f;
        vlgWnd.childAlignment = TextAnchor.MiddleLeft;
        vlgWnd.childForceExpandHeight = false;
        vlgWnd.childForceExpandWidth = true;
        var csfWnd = goWnd.AddComponent<ContentSizeFitter>();
        csfWnd.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        csfWnd.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        var goHeader = new GameObject("Header");
        goHeader.transform.SetParent(goWnd.transform, false);
        goHeader.AddComponent<RectTransform>();
        var hlgHeader = goHeader.AddComponent<HorizontalLayoutGroup>();
        hlgHeader.spacing = 30f;
        hlgHeader.childAlignment = TextAnchor.MiddleCenter;
        hlgHeader.childForceExpandHeight = false;
        hlgHeader.childForceExpandWidth = false;

        var goSpacer0 = new GameObject("Spacer0");
        goSpacer0.transform.SetParent(goHeader.transform, false);
        goSpacer0.AddComponent<RectTransform>();
        var leSpacer0 = goSpacer0.AddComponent<LayoutElement>();
        leSpacer0.preferredWidth = 30f;
        leSpacer0.preferredHeight = 30f;

        var goTitle = new GameObject("MenuReportTitle");
        goTitle.transform.SetParent(goHeader.transform, false);
        goTitle.AddComponent<RectTransform>();
        var title = goTitle.AddComponent<Text>();
        title.font = hrw.bbjt.font;
        title.fontSize = 18;

        var goClose = new GameObject("CloseMenuReport");
        goClose.transform.SetParent(goHeader.transform, false);
        goClose.AddComponent<RectTransform>();
        var leClose = goClose.AddComponent<LayoutElement>();
        leClose.preferredWidth = 30f;
        leClose.preferredHeight = 30f;
        var imgClose = goClose.AddComponent<Image>();
        imgClose.sprite = _spriteCloseButtonNormal;
        var btnClose = goClose.AddComponent<Button>();

        var goLine = new GameObject("HeaderLine");
        goLine.transform.SetParent(goWnd.transform, false);
        goLine.AddComponent<RectTransform>();
        var leLine = goLine.AddComponent<LayoutElement>();
        leLine.preferredWidth = 205f;
        leLine.preferredHeight = 2f;
        var imgLine = goLine.AddComponent<Image>();
        imgLine.sprite = _spriteLine;

        var goTip = new GameObject("TipSpace");
        goTip.transform.SetParent(goWnd.transform, false);
        goTip.AddComponent<RectTransform>();
        var leTip = goTip.AddComponent<LayoutElement>();
        leTip.preferredWidth = 200f;
        var tip = goTip.AddComponent<Text>();
        tip.font = hrw.bbjt.font;
        tip.fontSize = 10;

        var goText = new GameObject("MenuReportText");
        goText.transform.SetParent(goWnd.transform, false);
        goText.AddComponent<RectTransform>();
        var txt = goText.AddComponent<Text>();
        txt.font = hrw.bbjt.font;
        txt.fontSize = 12;
        #endregion

        var ret = new HotelRestaurantWindowReportButton()
        {
            _btnShowReport = goReport.AddComponent<Button>(),
            _wndReportRoot = goWnd,
            _titleText = title,
            _tipText = tip,
            _reportText = txt,
        };
        ret._btnShowReport.onClick.AddListener(
            (UnityAction)ret.ToggleReportWindow);
        btnClose.onClick.AddListener(
            (UnityAction)ret.ToggleReportWindow);
        ret.OnUseRestaurantFeaturesChanged(null, null);
        ConfigProvider.UseRestaurantFeatures.SettingChanged +=
            ret.OnUseRestaurantFeaturesChanged;

        return ret;
    }

    private static bool Init()
    {
        if (_spriteInfoIcon is not null)
        {
            return true;
        }

        var uiSprites = Resources.LoadAll<Sprite>(PATH_UI_MAIN);
        if (uiSprites is null)
        {
            return false;
        }

        var allFound = false;
        foreach (var sprite in uiSprites)
        {
            switch (sprite.name)
            {
                case SPRITE_NAME_INFO_ICON:
                    _spriteInfoIcon = sprite;
                    break;
                case SPRITE_NAME_INFO_BG:
                    _spriteInfoBg = sprite;
                    break;
                case SPRITE_NAME_CLOSE_BUTTON_NORMAL:
                    _spriteCloseButtonNormal = sprite;
                    break;
                case SPRITE_NAME_LINE:
                    _spriteLine = sprite;
                    break;
            }

            if (_spriteInfoIcon is not null
                && _spriteInfoBg is not null
                && _spriteCloseButtonNormal is not null
                && _spriteLine is not null)
            {
                allFound = true;
                break;
            }
        }
        return allFound;
    }

    private void ToggleReportWindow()
    {
        var openWindow = this._btnShowReport.interactable;
        if (openWindow)
        {
            if (!ModAssetProvider.TryGetModText(
                KEY_MENU_REPORT_TITLE, out var title)
                || string.IsNullOrEmpty(title))
            {
                throw new InvalidOperationException("string key not found");
            }
            this._titleText.text = title;

            if (!ModAssetProvider.TryGetModText(
                KEY_MENU_REPORT_TIP, out var tip)
                || string.IsNullOrEmpty(tip))
            {
                throw new InvalidOperationException("string key not found");
            }
            this._tipText.text = tip;

            this._reportText.text = ExRestaurantSystemManager.ReportCurrentMenuIngredients();
        }

        this._btnShowReport.interactable = !openWindow;
        this._wndReportRoot.SetActive(openWindow);
    }

    private void OnUseRestaurantFeaturesChanged(object _, EventArgs __)
    {
        this._btnShowReport.interactable = true;
        this._btnShowReport.gameObject.SetActive(
            ConfigProvider.UseRestaurantFeatures.Value);
        this._wndReportRoot.SetActive(false);
    }

    public void Dispose() =>
        ConfigProvider.UseRestaurantFeatures.SettingChanged
            -= this.OnUseRestaurantFeaturesChanged;
}
