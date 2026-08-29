using System.Collections.Generic;

namespace FG.Mods.YSYard.QoL.Models.Saves;

public class LevelPlayerState
{
    public int StoryId { get; set; }

    public Dictionary<int, string> TaskStates { get; set; } = [];

    public List<int> PlayedTasks { get; set; } = [];
}
