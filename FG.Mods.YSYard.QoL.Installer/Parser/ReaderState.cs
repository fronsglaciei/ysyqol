namespace FG.Mods.YSYard.QoL.Installer.Parser
{
    internal enum ReaderState
    {
        Start,
        Property,
        Object,
        Conditional,
        Finished,
        Closed
    }
}
