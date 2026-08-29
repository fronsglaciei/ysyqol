using System.Collections.Generic;

namespace FG.Mods.YSYard.QoL.Models.Saves;

public class AuctionState
{
    public int AuctionId { get; set; }

    public int RoundState { get; set; }

    public List<int> InitialRelicIds { get; set; } = [];

    public List<int> SelectedRelicIds { get; set; } = [];

    public List<int> ConsumedRelicIds { get; set; } = [];

    public int SubmitItemId { get; set; }

    public int SubmitItemIndex { get; set; }

    public int BiddingItemId { get; set; }

    public int BiddingIndex { get; set; }

    public List<int> PoolItemIds { get; set; } = [];

    public List<int> PoolItemBidPrices { get; set; } = [];

    public int PoolItemCount { get; set; }

    public List<int> SettleItemIds { get; set; } = [];
}
