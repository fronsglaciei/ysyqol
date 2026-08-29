using FG.Mods.YSYard.QoL.Services;
using UnityEngine;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal partial class ModSettingPanel
{
    private class TitleText
    {
        private string _titleKey;

        private Text _titleText;

        private TitleText() { }

        internal static TitleText CreateFromTemplate(
            GameObject ttRefObj, string titleKey,
            Transform parent = null)
        {
            if (!ValidateTemplateStructure(ttRefObj))
            {
                return null;
            }

            var goRoot = GameObject.Instantiate(ttRefObj);
            var leRoot = goRoot.AddComponent<LayoutElement>();
            leRoot.minWidth = 200f;
            leRoot.minHeight = 36f;

            if (parent is not null)
            {
                goRoot.transform.SetParent(parent, false);
            }

            var ret = new TitleText
            {
                _titleKey = titleKey,
                _titleText = goRoot.GetComponent<Text>(),
            };
            ret.UpdateText();
            return ret;
        }

        private static bool ValidateTemplateStructure(
            GameObject objTemplate)
        {
            if (objTemplate == null)
            {
                return false;
            }
            return objTemplate.GetComponent<Text>() is not null;
        }

        internal void UpdateText()
        {
            if (this._titleText is not null
                && ModAssetProvider.TryGetModText(
                    this._titleKey, out var title))
            {
                this._titleText.text = title;
            }
        }
    }
}
