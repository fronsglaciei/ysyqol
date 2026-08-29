using System;

namespace FG.Mods.YSYard.QoL.Models.Saves;

public class QuickSaveData
{
    public Guid SaveId { get; set; }

    public int GameId { get; set; }

    public int TimeId { get; set; }

    public int TimeType { get; set; }

    public int RoundState { get; set; }

    public string GameParam { get; set; } = string.Empty;

    public LevelPlayerState LevelPlayerState { get; set; }

    public HotelEventManagerState HotelEventManagerState { get; set; }

    public AuctionState AuctionState { get; set; }

    public DateTime TimeStamp { get; set; }

    public string FormatVersion { get; set; } = "1.0.0";
}
