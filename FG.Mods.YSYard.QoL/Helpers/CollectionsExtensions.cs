namespace FG.Mods.YSYard.QoL.Helpers;

internal static class CollectionsExtensions
{
    internal static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(
        this System.Collections.Generic.IEnumerable<T> enumerable)
    {
        var ret = new Il2CppSystem.Collections.Generic.List<T>();
        foreach (var item in enumerable)
        {
            ret.Add(item);
        }
        return ret;
    }

    internal static System.Collections.Generic.List<T> ToManagedList<T>(
        this Il2CppSystem.Collections.Generic.List<T> list)
    {
        var ret = new System.Collections.Generic.List<T>();
        foreach (var item in list)
        {
            ret.Add(item);
        }
        return ret;
    }
}
