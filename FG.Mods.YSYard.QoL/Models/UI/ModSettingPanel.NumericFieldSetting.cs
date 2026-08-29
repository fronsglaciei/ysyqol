using FG.Mods.YSYard.QoL.Services;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal partial class ModSettingPanel
{
    private class NumericFieldSetting<T>
        where T : IComparable<T>, IConvertible, IFormattable
    {
        private string _settingNameKey;

        private Text _settingNameText;

        private InputField _inputField;

        private Action<T> _onValueChanged;

        private NumericFieldSetting() { }

        internal static NumericFieldSetting<T> CreateFromTemplate(
            Font font, Image imgRef, string settingNameKey,
            Transform parent = null, string objName = "")
        {
            if (font is null)
            {
                return null;
            }

            var goRoot = new GameObject();
            if (!string.IsNullOrEmpty(objName))
            {
                goRoot.name = objName;
            }
            goRoot.AddComponent<RectTransform>();
            var vlg = goRoot.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.MiddleLeft;
            vlg.spacing = 1f;

            var goSettingName = new GameObject();
            goSettingName.AddComponent<RectTransform>();
            goSettingName.AddComponent<CanvasRenderer>();
            var leSettingName = goSettingName.AddComponent<LayoutElement>();
            leSettingName.minWidth = 280f;
            leSettingName.minHeight = 24f;
            var txtSettingName = goSettingName.AddComponent<Text>();
            txtSettingName.font = font;
            goSettingName.transform.SetParent(goRoot.transform, false);

            var goInputField = new GameObject();
            goInputField.AddComponent<RectTransform>();
            goInputField.AddComponent<CanvasRenderer>();
            var leInputField = goInputField.AddComponent<LayoutElement>();
            leInputField.minWidth = 280f;
            leInputField.minHeight = 28f;
            var imgInputField = goInputField.AddComponent<Image>();
            imgInputField.type = imgRef.type;
            imgInputField.sprite = imgRef.sprite;
            imgInputField.material = imgRef.material;

            var goInputText = new GameObject();
            goInputText.AddComponent<RectTransform>();
            goInputText.AddComponent<CanvasRenderer>();
            var leInputText = goInputText.AddComponent<LayoutElement>();
            leInputText.minWidth = 280f;
            leInputText.minHeight = 24f;
            var txtInputField = goInputText.AddComponent<Text>();
            txtInputField.font = font;
            txtInputField.alignment = TextAnchor.MiddleLeft;
            goInputText.transform.SetParent(goInputField.transform, false);

            var inputField = goInputField.AddComponent<InputField>();
            inputField.textComponent = txtInputField;
            inputField.image = imgInputField;
            if (typeof(T) == typeof(float)
                || typeof(T) == typeof(double)
                || typeof(T) == typeof(decimal))
            {
                inputField.contentType
                    = InputField.ContentType.DecimalNumber;
                inputField.characterValidation
                    = InputField.CharacterValidation.Decimal;
            }
            else
            {
                inputField.contentType
                    = InputField.ContentType.IntegerNumber;
                inputField.characterValidation
                    = InputField.CharacterValidation.Integer;
            }
            goInputField.transform.SetParent(goRoot.transform, false);

            if (parent is not null)
            {
                goRoot.transform.SetParent(parent, false);
            }

            var ret = new NumericFieldSetting<T>
            {
                _settingNameKey = settingNameKey,
                _settingNameText = txtSettingName,
                _inputField = inputField,
            };
            ret.UpdateText();
            return ret;
        }

        internal void SetOption(
            T minValue, T maxValue, Action<T> onValueChanged,
            T initialValue = default)
        {
            if (maxValue.CompareTo(minValue) < 0)
            {
                throw new ArgumentException(
                    $"{nameof(minValue)} {minValue} is bigger than {nameof(maxValue)} {maxValue}");
            }
            if (initialValue.CompareTo(minValue) < 0)
            {
                throw new ArgumentException(
                    $"{nameof(initialValue)} {initialValue} is smaller than {nameof(minValue)} {minValue}");
            }
            if (maxValue.CompareTo(initialValue) < 0)
            {
                throw new ArgumentException(
                    $"{nameof(initialValue)} {initialValue} is bigger than {nameof(maxValue)} {maxValue}");
            }
            if (this._inputField is null)
            {
                throw new NullReferenceException(nameof(this._inputField));
            }

            this._inputField.onValueChanged.RemoveAllListeners();
            this._inputField.text = initialValue.ToString();

            var capturedMin = minValue;
            var capturedMax = maxValue;
            var capturedInitial = initialValue;
            this._onValueChanged = onValueChanged;
            this._inputField.onValueChanged.AddListener((UnityAction<string>)(x =>
            {
                if (!decimal.TryParse(x, out var decVal))
                {
                    this._inputField.SetTextWithoutNotify(
                        capturedInitial.ToString());
                    this._onValueChanged?.Invoke(capturedInitial);
                    return;
                }
                if (decVal < capturedMin.ToDecimal(null))
                {
                    this._inputField.SetTextWithoutNotify(
                        capturedMin.ToString());
                    this._onValueChanged?.Invoke(capturedMin);
                    return;
                }
                if (capturedMax.ToDecimal(null) < decVal)
                {
                    this._inputField.SetTextWithoutNotify(
                        capturedMax.ToString());
                    this._onValueChanged?.Invoke(capturedMax);
                    return;
                }

                try
                {
                    var tmp = Convert.ChangeType(decVal, typeof(T));
                    if (tmp is not null)
                    {
                        this._onValueChanged?.Invoke((T)tmp);
                        return;
                    }
                }
                catch { }
                this._inputField.SetTextWithoutNotify(
                        capturedInitial.ToString());
                this._onValueChanged?.Invoke(capturedInitial);
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

        internal void SetValueWithoutNotify(T value) =>
            this._inputField?.SetTextWithoutNotify(value.ToString());
    }
}
