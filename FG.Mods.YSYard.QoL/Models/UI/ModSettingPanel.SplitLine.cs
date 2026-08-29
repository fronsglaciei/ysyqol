using UnityEngine;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal partial class ModSettingPanel
{
    private class SplitLine
    {
        private SplitLine() { }

        internal static void CreateFromTemplate(
            GameObject slRefObj, Transform parent = null)
        {
            if (!ValidateTemplateStructure(slRefObj))
            {
                return;
            }

            var cloned = GameObject.Instantiate(slRefObj);
            var leRoot = cloned.AddComponent<LayoutElement>();
            leRoot.minWidth = 300f;
            leRoot.minHeight = 3f;
            if (parent is not null)
            {
                cloned.transform.SetParent(parent, false);
            }
        }

        private static bool ValidateTemplateStructure(
            GameObject objTemplate)
        {
            if (objTemplate == null)
            {
                return false;
            }

            return objTemplate.GetComponent<Image>() is not null;
        }
    }
}
