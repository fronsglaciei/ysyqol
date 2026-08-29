using System.Collections.Generic;

namespace FG.Mods.YSYard.QoL.Installer
{
    internal static class DynamicExtensions
    {
        internal static bool TryGetValue<T, U>(
            this Dictionary<T, dynamic> obj, T key, out U value)
        {
            value = default;
            if (obj.TryGetValue(key, out var dynVal)
                && dynVal is U tmpVal)
            {
                value = tmpVal;
                return true;
            }
            return false;
        }
    }
}
