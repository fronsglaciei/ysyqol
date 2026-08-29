namespace FG.Mods.YSYard.QoL.Services;

internal static class ExHotelIllustrationManager
{
    internal static void SetIllustrationFinished(int id)
    {
        //var him = HotelIllustrationManager.Instance;
        var him = er.bgth;

        //for (var i = 0; i < __instance.illustrations.Count; i++)
        for (var i = 0; i < him.vom.Count; i++)
        {
            //if (__instance.illustrations[i].IllustrationID == id)
            if (him.vom[i].xkj == id)
            {
                //__instance.illustrationFinished.Add(__instance.illustrations[i]);
                him.von.Add(him.vom[i]);
                //__instance.illustrations.RemoveAt(i);
                him.vom.RemoveAt(i);
                break;
            }
        }
        //foreach (var kvp in __instance.illustrationsByUI)
        foreach (var kvp in him.vok)
        {
            //if (kvp.Value.IllustrationID == id)
            if (kvp.Value.xkj == id)
            {
                //__instance.illustrationsByUI.Remove(kvp.Key);
                him.vok.Remove(kvp.Key);
                break;
            }
        }
    }
}
