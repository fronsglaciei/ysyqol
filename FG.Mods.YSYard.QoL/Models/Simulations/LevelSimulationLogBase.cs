namespace FG.Mods.YSYard.QoL.Models.Simulations;

internal class LevelSimulationLogBase(int taskId)
    : ILevelSimulationLog
{
    public int TaskId { get; } = taskId;
}
