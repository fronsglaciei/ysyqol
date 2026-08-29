using Foundation;
using System.Collections.Generic;
using System.Linq;

namespace FG.Mods.YSYard.QoL.Services;

internal static class ExRestaurantSystemManager
{
    internal static string ReportCurrentMenuIngredients()
    {
        //var rsm = ResutaurantSystemManager.Instance;
        var rsm = ya.bhie;

        var ingredients = new Dictionary<int, int>();
        foreach (var mi
            //in rsm.RestaurantMenuItemDic.Values)
            in rsm.zlc.Values)
        {
            //if (mi?.IsEmpty ?? false)
            if (mi?.bhib ?? false)
            {
                continue;
            }

            //var rri = rsm.GetRecipeItemByFoodItem(mi.foodItemId);
            var rri = rsm.jmf(mi.zkp);
            //if (rri?.Data is null)
            if (rri?.zku is null)
            {
                continue;
            }
            var rr = rri.zku;

            //if (rr.Dosage is null || rr.DosageNum is null
            if (rr.xsb is null || rr.xsc is null
                //|| rr.Dosage.Count != rr.DosageNum.Count
                || rr.xsb.Count != rr.xsc.Count)
            {
                continue;
            }

            for (var i = 0; i < rr.xsb.Count; i++)
            {
                var itemNum = rr.xsc[i];
                if (itemNum < 1)
                {
                    continue;
                }

                var itemId = rr.xsb[i];
                if (ingredients.ContainsKey(itemId))
                {
                    ingredients[itemId] += itemNum;
                }
                else
                {
                    ingredients[itemId] = itemNum;
                }
            }
        }

        return string.Join('\n', ingredients.Select(x =>
        {
            //var item = ItemManager.Instance.GetItem(x.Key);
            var item = hl.bgvr.GetItem(x.Key);
            if (item is null)
            {
                return string.Empty;
            }

            //var itemName = GameAPI.GetLanguageStr(item.NameID);
            var itemName = cy.cus(item.xle);
            if (string.IsNullOrEmpty(itemName))
            {
                return string.Empty;
            }

            return $"{itemName} x {x.Value}";

        }).Where(x => !string.IsNullOrEmpty(x)));
    }
}
