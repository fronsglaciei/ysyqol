using UGUIExtend;
using UnityEngine;

namespace FG.Mods.YSYard.QoL.Services;

internal static class MissingAssetProvider
{
    private const string EX_ANIM_TEMPLATE_PREFAB = "ExpressionAnim/2103/ExpressionAnim2103";

    private const int MISSING_ID_2291 = 2291;

    private const string MISSING_SPRITE_2103_22 = "uiatlas/expression/2103_22";

    private static Sprite _sprite2103_22;

    internal static bool TryGetExpressionAnimator(
        int unitBaseId, out ExpressionAnimator ea)
    {
        ea = null;
        if (unitBaseId != MISSING_ID_2291)
        {
            return false;
        }

        //var prefab = ResourcesManager.Instance.Load<GameObject>();
        var prefab = dg.bgsl.Load<GameObject>(EX_ANIM_TEMPLATE_PREFAB);
        if (prefab is null)
        {
            return false;
        }
        var inst = GameObject.Instantiate(prefab);
        if (prefab is null)
        {
            return false;
        }
        ea = inst.GetComponent<ExpressionAnimator>();
        if (ea is null)
        {
            return false;
        }
        var img = inst.transform
            .Find("mesh")?.Find("Body")?
            .GetComponent<AdvancedImage>();
        if (img is null)
        {
            return false;
        }

        if (_sprite2103_22 is null)
        {
            Init();
        }
        img.sprite = _sprite2103_22;

        return true;
    }

    private static void Init()
    {
        var sprites = Resources.LoadAll<Sprite>(MISSING_SPRITE_2103_22);
        if (sprites?.Length != 1)
        {
            return;
        }
        _sprite2103_22 = sprites[0];
    }
}
