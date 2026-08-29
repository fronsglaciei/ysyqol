using FG.Mods.YSYard.QoL.Services;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal partial class ModSettingPanel
{
    private class CheckBoxSetting
    {
        private string _settingNameKey;

        private Text _settingNameText;

        private Button _btnCheck;

        private GameObject _objCheckImage;

        private Action<bool> _onValueChanged;

        private CheckBoxSetting() { }

        internal static CheckBoxSetting CreateFromTemplate(
            GameObject cbRefObj, string settingNameKey,
            Transform parent = null, string objName = "")
        {
            if (!ValidateTemplateStructure(cbRefObj))
            {
                return null;
            }

            var goRoot = new GameObject();
            if (!string.IsNullOrEmpty(objName))
            {
                goRoot.name = objName;
            }
            goRoot.AddComponent<RectTransform>();
            goRoot.AddComponent<CanvasRenderer>();
            var hlgRoot = goRoot.AddComponent<HorizontalLayoutGroup>();
            hlgRoot.spacing = 5f;

            var goCloned = GameObject.Instantiate(cbRefObj);
            var settingNameText = goCloned.GetComponent<Text>();
            settingNameText.alignment = TextAnchor.MiddleLeft;
            var leSettingName = goCloned.AddComponent<LayoutElement>();
            leSettingName.minWidth = 280f;
            leSettingName.minHeight = 24f;

            GameObject.Destroy(goCloned.transform.GetChild(1).gameObject);

            var checkRoot = goCloned.transform.GetChild(0);
            var leCheckRoot = checkRoot.gameObject.AddComponent<LayoutElement>();
            leCheckRoot.minWidth = 12f;
            leCheckRoot.minHeight = 12f;
            checkRoot.SetParent(goRoot.transform, false);

            var btnCheck = checkRoot.GetChild(0).GetComponent<Button>();
            btnCheck.onClick.RemoveAllListeners();
            var rtButton = btnCheck.GetComponent<RectTransform>();
            rtButton.anchoredPosition -= new Vector2(0f, 1.5f);

            var rtCheck = checkRoot.GetChild(1).GetComponent<RectTransform>();
            rtCheck.anchoredPosition -= new Vector2(0f, 1.5f);

            goCloned.transform.SetParent(goRoot.transform, false);

            if (parent is not null)
            {
                goRoot.transform.SetParent(parent, false);
            }

            var ret = new CheckBoxSetting
            {
                _settingNameKey = settingNameKey,
                _settingNameText = goCloned.GetComponent<Text>(),
                _btnCheck = btnCheck,
                _objCheckImage = checkRoot.GetChild(1).gameObject
            };
            ret.UpdateText();
            return ret;
        }

        private static bool ValidateTemplateStructure(GameObject objTemplate)
        {
            if (objTemplate is null)
            {
                return false;
            }

            var root = objTemplate.transform;
            if (root.GetComponent<Text>() is null)
            {
                return false;
            }
            if (root.GetChildCount() < 2)
            {
                return false;
            }

            var checkRoot = root.GetChild(0);
            if (checkRoot.GetChildCount() < 2)
            {
                return false;
            }
            if (checkRoot.GetChild(0).GetComponent<Button>() is null)
            {
                return false;
            }
            return true;
        }

        internal void SetOption(
            Action<bool> onValueChanged, bool initialValue = false)
        {
            if (this._btnCheck is null)
            {
                throw new NullReferenceException($"{nameof(this._btnCheck)}");
            }
            if (this._objCheckImage is null)
            {
                throw new NullReferenceException($"{nameof(this._objCheckImage)}");
            }

            this._onValueChanged = onValueChanged;
            this._objCheckImage.SetActive(initialValue);

            this._btnCheck.onClick.RemoveAllListeners();
            this._btnCheck.onClick.AddListener((UnityAction)(() =>
            {
                var current = this._objCheckImage.active;
                this._objCheckImage.SetActive(!current);
                this._onValueChanged.Invoke(!current);
            }));
        }

        internal void UpdateText()
        {
            if (this._settingNameText is not null
                && ModAssetProvider.TryGetModText(
                    this._settingNameKey, out var settingName))
            {
                this._settingNameText.text = settingName;
            }
        }

        internal void SetValueWithoutNotify(bool value)
            => this._objCheckImage?.SetActive(value);
    }
}
