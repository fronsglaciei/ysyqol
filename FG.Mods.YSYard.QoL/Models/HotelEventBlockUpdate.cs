using Example;
using HotelModule.Event;

namespace FG.Mods.YSYard.QoL.Models;

public class HotelEventBlockUpdate(NpcEvent EventData)
    //: HotelEventBase(EventData)
    : sk(EventData)
{
    internal void ExplicitFinish()
        //=> this.Finished = true;
        => this.yms = true;
}
