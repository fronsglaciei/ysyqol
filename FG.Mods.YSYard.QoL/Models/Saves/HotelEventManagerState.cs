using System.Collections.Generic;

namespace FG.Mods.YSYard.QoL.Models.Saves;

public class HotelEventManagerState
{
    public HotelEventState CurrentEvent { get; set; }

    public List<HotelEventState> EventQueue { get; set; } = [];
}
