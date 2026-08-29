using FG.Mods.YSYard.QoL.Helpers;
using FG.Mods.YSYard.QoL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal partial class ModSettingPanel
{
    private class DropDownSetting<T> where T : IEquatable<T>
    {
        private string _settingNameKey;

        private Text _settingNameText;

        private Dropdown _dropdown;

        private List<ValueCaptionPair<T>> _options;

        private Action<T> _onValueChanged;

        private DropDownSetting() { }

        internal static DropDownSetting<T> CreateFromTemplate(
            GameObject ddRefObj, string settingNameKey,
            Transform parent = null, string objName = "")
        {
            if (!ValidateTemplateStructure(ddRefObj))
            {
                return null;
            }

            var goRoot = new GameObject();
            if (!string.IsNullOrEmpty(objName))
            {
                goRoot.name = objName;
            }
            var rt = goRoot.AddComponent<RectTransform>();
            goRoot.AddComponent<CanvasRenderer>();
            goRoot.AddComponent<VerticalLayoutGroup>();

            var goSettingName = new GameObject();
            goSettingName.AddComponent<RectTransform>();
            goSettingName.AddComponent<CanvasRenderer>();
            var leSettingName = goSettingName.AddComponent<LayoutElement>();
            leSettingName.minWidth = 240f;
            leSettingName.minHeight = 24f;
            var textSettingName = goSettingName.AddComponent<Text>();
            goSettingName.transform.SetParent(goRoot.transform, false);

            var cloned = GameObject.Instantiate(ddRefObj);
            var le = cloned.AddComponent<LayoutElement>();
            le.minWidth = 240f;
            le.minHeight = 32f;
            cloned.transform.SetParent(goRoot.transform, false);

            if (parent is not null)
            {
                goRoot.transform.SetParent(parent, false);
            }

            var dd = cloned.GetComponent<Dropdown>();
            dd.ClearOptions();
            dd.onValueChanged.RemoveAllListeners();

            textSettingName.font = cloned.transform.GetChild(0).GetComponent<Text>().font;

            var ret = new DropDownSetting<T>()
            {
                _settingNameKey = settingNameKey,
                _settingNameText = textSettingName,
                _dropdown = dd,
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
            var dd = objTemplate.GetComponent<Dropdown>();
            var rt = objTemplate.GetComponent<RectTransform>();

            return dd is not null && rt is not null;
        }

        internal void SetOptions(
            List<ValueCaptionPair<T>> options, Action<T> onValueChanged,
            T initialValue = default)
        {
            if (options is null)
            {
                return;
            }
            this._options = options;
            this._onValueChanged = onValueChanged;

            this._dropdown.ClearOptions();
            this._dropdown.onValueChanged.RemoveAllListeners();

            List<string> optionStrings = [.. options.Select(x => x.CaptionKey)];
            optionStrings.Add(string.Empty);
            this._dropdown.AddOptions(optionStrings.ToIl2CppList());

            var initialIndex = options.FindIndex(x => x.Value.Equals(initialValue));
            this._dropdown.value =
                -1 < initialIndex && initialIndex < options.Count
                ? initialIndex : optionStrings.Count - 1;
            this._dropdown.onValueChanged.AddListener(
                (UnityAction<int>)(i => this.OnValueChanged(i)));
        }

        internal void UpdateText()
        {
            if (this._settingNameText is not null
                && ModAssetProvider.TryGetModText(
                    this._settingNameKey, out var settingName))
            {
                this._settingNameText.text = settingName;
            }

            foreach (var opt in this._options
                ?? Enumerable.Empty<ValueCaptionPair<T>>())
            {
                if (opt.CaptionText is not null
                    && ModAssetProvider.TryGetModText(
                        opt.CaptionKey, out var caption))
                {
                    opt.CaptionText.text = caption;
                }
            }
        }

        internal void SetValueWithoutNotify(T value)
        {
            if (this._options is null)
            {
                return;
            }

            var idx = this._options.FindIndex(x => x.Value.Equals(value));
            this._dropdown.SetValueWithoutNotify(
                -1 < idx && idx < this._options.Count
                ? idx : this._dropdown.options.Count - 1);
        }

        private void OnValueChanged(int index)
            => this._onValueChanged?.Invoke(
                this._options is null
                    || index < 0 || this._options.Count <= index
                ? default : this._options[index].Value);
    }
}
