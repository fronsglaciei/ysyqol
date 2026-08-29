using UnityEngine;

namespace FG.Mods.YSYard.QoL.Helpers;

internal static class TransformExtensions
{
    internal static Transform DigFind(this Transform tf, string name)
    {
        var cnt = tf.GetChildCount();
        for (var i = 0; i < cnt; i++)
        {
            var child = tf.GetChild(i);
            if (child.name.Contains(name))
            {
                return child;
            }
            var recursed = DigFind(child, name);
            if (recursed is not null)
            {
                return recursed;
            }
        }
        return null;
    }
}
