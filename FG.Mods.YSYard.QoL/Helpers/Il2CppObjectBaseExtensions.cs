using Il2CppInterop.Runtime.InteropTypes;
using System;

namespace FG.Mods.YSYard.QoL.Helpers;

internal static class Il2CppObjectBaseExtensions
{
    internal static System.IDisposable AsManagedDisposable(this Il2CppObjectBase obj)
    {
        var iDisposer = obj.TryCast<Il2CppSystem.IDisposable>()
            ?? throw new InvalidCastException();
        return new DisposeWrapper(iDisposer);
    }

    private class DisposeWrapper(Il2CppSystem.IDisposable obj)
        : System.IDisposable
    {
        public void Dispose() => obj.Dispose();
    }
}
