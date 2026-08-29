using System.IO;
using System.Reflection;

namespace FG.Mods.YSYard.QoL.Services;

internal static class PathProvider
{
    internal static string PluginDirectory { get; } =
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    internal static string ExSaveDirectory { get; } =
        CreateGetExSaveDirectory();

    private const string DIRNAME_EX_SAVE = "exsave";

    private static string CreateGetExSaveDirectory()
    {
        var path = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            DIRNAME_EX_SAVE);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }
}
