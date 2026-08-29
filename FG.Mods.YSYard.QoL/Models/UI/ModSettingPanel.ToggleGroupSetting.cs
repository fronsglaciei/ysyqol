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
    private const string KEY_SETTING_ON = "SettingOn";

    private const string KEY_SETTING_OFF = "SettingOff";

    private class ToggleGroupSetting<T> where T : IEquatable<T>
    {
        private GameObject _rootObject;

        private GameObject _childTemplate;

        private string _settingNameKey;

        private Text _settingNameText;

        private List<ValueCaptionPair<T>> _options;

        private Action<T> _onValueChanged;

        private ToggleGroupSetting() { }

        internal static ToggleGroupSetting<T> CreateFromTemplate(
            GameObject tgRefObj, string settingNameKey,
            Transform parent = null, string objName = "")
        {
            if (!ValidateTemplateStructure(tgRefObj))
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
            goRoot.AddComponent<VerticalLayoutGroup>();

            var goSettingName = new GameObject();
            goSettingName.AddComponent<RectTransform>();
            goSettingName.AddComponent<CanvasRenderer>();
            var leSettingName = goSettingName.AddComponent<LayoutElement>();
            leSettingName.minWidth = 240f;
            leSettingName.minHeight = 24f;
            var textSettingName = goSettingName.AddComponent<Text>();
            goSettingName.transform.SetParent(goRoot.transform, false);

            var cloned = GameObject.Instantiate(tgRefObj);
            cloned.transform.SetParent(goRoot.transform, false);

            if (parent is not null)
            {
                goRoot.transform.SetParent(parent, false);
            }

            var objFirstChildRoot = cloned.transform.GetChild(0).gameObject;

            var cnt = cloned.transform.GetChildCount();
            for (var i = 0; i < cnt; i++)
            {
                var childRoot = cloned.transform.GetChild(i);
                var t = childRoot.GetChild(0).GetComponent<Toggle>();
                t.onValueChanged.RemoveAllListeners();
                if (i == 0)
                {
                    var text = childRoot.GetChild(1).GetComponent<Text>();
                    textSettingName.font = text.font;
                }
            }

            var ret = new ToggleGroupSetting<T>()
            {
                _rootObject = goRoot,
                _childTemplate = objFirstChildRoot,
                _settingNameKey = settingNameKey,
                _settingNameText = textSettingName
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

            var tg = objTemplate.GetComponent<ToggleGroup>();
            var rtRoot = objTemplate.GetComponent<RectTransform>();
            var hlg = objTemplate.GetComponent<HorizontalLayoutGroup>();
            if (tg is null || rtRoot is null || hlg is null)
            {
                return false;
            }

            if (objTemplate.transform.GetChildCount() < 1)
            {
                return false;
            }
            var transFirstChildRoot = objTemplate.transform.GetChild(0);
            if (transFirstChildRoot.transform.GetChildCount() < 3)
            {
                return false;
            }

            var transT = transFirstChildRoot.transform.GetChild(0);
            var t = transT.GetComponent<Toggle>();
            if (t is null)
            {
                return false;
            }

            var transText = transFirstChildRoot.transform.GetChild(1);
            var text = transText.GetComponent<Text>();
            if (text is null)
            {
                return false;
            }

            return true;
        }

        internal void SetOptions(
            List<ValueCaptionPair<T>> options, Action<T> onValueChanged,
            T initialValue = default)
        {
            if (options is null || options.Count < 2)
            {
                return;
            }
            this._options = options;
            this._onValueChanged = onValueChanged;

            var transRoot = this._childTemplate.transform.parent;
            var cnt = transRoot.GetChildCount();
            while (cnt != options.Count)
            {
                if (cnt < options.Count)
                {
                    var cloned = GameObject.Instantiate(this._childTemplate);
                    cloned.transform.SetParent(transRoot);
                }
                else
                {
                    GameObject.Destroy(transRoot.GetChild(cnt - 1).gameObject);
                }
                cnt = transRoot.GetChildCount();
            }

            for (var i = 0; i < cnt; i++)
            {
                var child = transRoot.GetChild(i);
                var t = child.GetChild(0).GetComponent<Toggle>();
                t.onValueChanged.RemoveAllListeners();
                if (options[i].Value.Equals(initialValue))
                {
                    t.isOn = true;
                }
                var capturedIndex = i;
                t.onValueChanged.AddListener(
                    (UnityAction<bool>)(flag =>
                    {
                        if (flag)
                        {
                            this.OnValueChanged(capturedIndex);
                        }
                    }));

                options[i].CaptionText = child.GetChild(1).GetComponent<Text>();
            }
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
            if (this._options is null || this._options.Count < 2)
            {
                return;
            }

            var idx = this._options.FindIndex(x => x.Value.Equals(value));
            var transRoot = this._rootObject.transform;
            var cnt = transRoot.GetChildCount();
            if (idx < 0 || cnt <= idx)
            {
                return;
            }

            var t = transRoot.GetChild(idx).GetChild(0).GetComponent<Toggle>();
            t.SetIsOnWithoutNotify(true);
        }

        private void OnValueChanged(int index)
            => this._onValueChanged?.Invoke(
                this._options is null
                    || index < 0 || this._options.Count <= index
                ? default : this._options[index].Value);
    }
}
