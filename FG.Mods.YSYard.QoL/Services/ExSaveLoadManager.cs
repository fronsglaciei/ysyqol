using FG.Mods.YSYard.QoL.Helpers;
using FG.Mods.YSYard.QoL.Models.Saves;
using HotelModule;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Runtime.Serialization.Formatters.Binary;
using SaveLoadSystem;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace FG.Mods.YSYard.QoL.Services;

internal static class ExSaveLoadManager
{
    private const string FILENAME_QSAVE = "qsave.json";

    private const string FILENAME_EXGLOBAL = "exglobal.json";

    private static readonly JsonSerializerOptions _jopts = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true,
    };

    internal static GameParam GameParamCache { get; private set; } = null;

    internal static QuickSaveData QuickSaveDataCache { get; private set; } = null;

    internal static ExGlobalSaveData ExGlobalSaveDataCache { get; private set; } = null;

    internal static void QuickSave()
    {
        // reference: SaveLoadSystem.SaveLoadManager.Save

        var gp = new GameParam();
        gp.Init();
        gp.Save();
#if DEBUG
        File.WriteAllText(
            Path.Combine(PathProvider.PluginDirectory, "on_save_gameparam.txt"),
            GetDebugString(gp));
#endif
        //var round = HotelManager.Instance.mHotelRoundManager.roundData;
        var round = rp.bhfn.yji.ykd;
        //var rState = HotelManager.Instance.mHotelRoundManager.CurRoundState;
        var rState = rp.bhfn.yji.bhfs;

        var ms = new Il2CppSystem.IO.MemoryStream();
        var gpString = string.Empty;
        using (ms.AsManagedDisposable())
        {
            var bf = new BinaryFormatter();
            bf.Serialize(ms, gp);
            var size = ms.Position;
            ms.Seek(0, Il2CppSystem.IO.SeekOrigin.Begin);
            var il2cppBuf = ms.GetBuffer();
            var dstBuf = new byte[il2cppBuf.Length];
            il2cppBuf.CopyTo(dstBuf, 0);
            gpString = System.Convert.ToBase64String(dstBuf.Take((int)size).ToArray());
        }
        if (string.IsNullOrWhiteSpace(gpString))
        {
            Plugin.Log.LogError($"Quick-Save failed! Failed to serialize {nameof(GameParam)}.");
            return;
        }

        var path = Path.Combine(PathProvider.ExSaveDirectory, FILENAME_QSAVE);
        File.WriteAllText(path, JsonSerializer.Serialize(new QuickSaveData
        {
            SaveId = System.Guid.NewGuid(),
            //GameId = HotelManager.Instance.GameID,
            GameId = rp.bhfn.yje,
            //TimeId = round.TimeID,
            TimeId = round.xvy,
            //TimeType = (int)round.Time,
            TimeType = (int)round.xwa,
            RoundState = (int)rState,
            GameParam = gpString,
            LevelPlayerState = ExLevelManager.CurrentPlayerState,
            HotelEventManagerState = ExHotelEventManager.CurrenState,
            AuctionState = ExAuctionManager.CurrentState,
            TimeStamp = System.DateTime.Now,
        }, _jopts));
    }

    internal static bool QuickLoadCache()
    {
        var path = Path.Combine(PathProvider.ExSaveDirectory, FILENAME_QSAVE);
        if (!File.Exists(path))
        {
            Plugin.Log.LogError($"Quick-Load failed! Failed to get {FILENAME_QSAVE}.");
            return false;
        }
        var qsd = JsonSerializer.Deserialize<QuickSaveData>(File.ReadAllText(path));
        if (qsd is null)
        {
            Plugin.Log.LogError($"Quick-Load failed! Failed to deserialize {nameof(QuickSaveData)}.");
            return false;
        }

        var il2cppBuf = new Il2CppStructArray<byte>(
                System.Convert.FromBase64String(qsd.GameParam));
        var ms = new Il2CppSystem.IO.MemoryStream(il2cppBuf);
        GameParam gp = null;
        using (ms.AsManagedDisposable())
        {
            var bf = new BinaryFormatter();
            gp = bf.Deserialize(ms).TryCast<GameParam>();
        }
        if (gp == null)
        {
            Plugin.Log.LogError($"Quick-Load failed! Failed to deserialize {nameof(GameParam)}.");
            return false;
        }
        File.WriteAllText(
            Path.Combine(PathProvider.PluginDirectory, "on_load_gameparam.txt"),
            GetDebugString(gp));

        QuickSaveDataCache = qsd;
        GameParamCache = gp;
        return true;
    }

    internal static void ApplyCacheToGame()
    {
        if (QuickSaveDataCache is null || GameParamCache is null)
        {
            throw new System.InvalidOperationException();
        }

        var slm = SaveLoadManager.Instance;
        if (slm.gameParam is null)
        {
            Plugin.Log.LogError($"Quick-Save failed! {nameof(GameParam)} is not constructed.");
            return;
        }
        //HotelManager.Instance.GameID = QuickSaveDataCache.GameId;
        rp.bhfn.yje = QuickSaveDataCache.GameId;
        slm.gameParam.Init();
        var dstParams = slm.gameParam.SaveParams;
        var srcParams = GameParamCache.SaveParams;
        for (var i = 0; i < dstParams.Count; i++)
        {
            var found = false;
            foreach (var src in srcParams)
            {
                if (dstParams[i].GetIl2CppType() == src.GetIl2CppType())
                {
                    dstParams[i] = src;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                dstParams[i].Init();
            }
        }
        slm.gameParam.Load();
    }

    private static string GetDebugString(GameParam gameParam)
    {
        var sb = new StringBuilder();

        foreach (var saveParam in gameParam.SaveParams)
        {
            switch (saveParam.GetIl2CppType().Name)
            {
                case nameof(HotelSaveParam):
                    var hsp = saveParam.Cast<HotelSaveParam>();
                    sb.AppendLine($"[{nameof(HotelSaveParam)}]");
                    sb.AppendLine($"  {nameof(HotelSaveParam.hotelRank)}: {hsp.hotelRank}");
                    sb.AppendLine($"  {nameof(HotelSaveParam.hotelStrategyType)}: {hsp.hotelStrategyType}");
                    if (hsp.restUnitParam == null)
                    {
                        break;
                    }
                    sb.AppendLine($"  {nameof(HotelSaveParam.restUnitParam)}:");
                    foreach (var unit in hsp.restUnitParam)
                    {
                        sb.AppendLine($"    [{unit.unitBaseId}]:");
                        sb.AppendLine($"      {nameof(UnitParam.money)}: {unit.money}");
                        sb.AppendLine($"      {nameof(UnitParam.startRound)}: {unit.startRound}");
                        sb.AppendLine($"      {nameof(UnitParam.lastRound)}: {unit.lastRound}");
                        sb.AppendLine(unit.items?.ToStr(nameof(UnitParam.items), 6));
                        sb.AppendLine($"      {nameof(UnitParam.hasCollectedRent)}: {unit.hasCollectedRent}");
                        sb.AppendLine($"      {nameof(UnitParam.firstName)}: {unit.firstName}");
                        sb.AppendLine($"      {nameof(UnitParam.lastName)}: {unit.lastName}");
                    }
                    break;
                case nameof(IllustrationSaveParam):
                    var isp = saveParam.Cast<IllustrationSaveParam>();
                    sb.AppendLine($"[{nameof(IllustrationSaveParam)}]");
                    sb.AppendLine(
                        isp.illustrationFinished?.ToStr(nameof(IllustrationSaveParam.illustrationFinished), 2));
                    break;
                case nameof(ArtifactSaveParam):
                    var asp = saveParam.Cast<ArtifactSaveParam>();
                    sb.AppendLine($"[{nameof(ArtifactSaveParam)}]");
                    sb.AppendLine($"  {nameof(ArtifactSaveParam.currentArtifactExp)}: {asp.currentArtifactExp}");
                    sb.AppendLine($"  {nameof(ArtifactSaveParam.currentExpIndex)}: {asp.currentExpIndex}");
                    sb.AppendLine($"  {nameof(ArtifactSaveParam.currentMonthID)}: {asp.currentMonthID}");
                    sb.AppendLine($"  {nameof(ArtifactSaveParam.currentTargetArtifactExp)}: {asp.currentTargetArtifactExp}");
                    sb.AppendLine($"  {nameof(ArtifactSaveParam.autoOpenDivinationWindow)}: {asp.autoOpenDivinationWindow}");
                    sb.AppendLine($"  {nameof(ArtifactSaveParam.canDivination)}: {asp.canDivination}");
                    sb.AppendLine($"  {nameof(ArtifactSaveParam.alreadyDivination)}: {asp.alreadyDivination}");
                    sb.AppendLine(asp.normalArtifacts?.ToStr(nameof(ArtifactSaveParam.normalArtifacts), 2));
                    sb.AppendLine(asp.normalArtifactStars?.ToStr(nameof(ArtifactSaveParam.normalArtifactStars), 2));
                    if (asp.normalEffect != null)
                    {
                        sb.AppendLine($"  {nameof(ArtifactSaveParam.normalEffect)}:");
                        foreach (var normalEff in asp.normalEffect)
                        {
                            sb.AppendLine($"    [{normalEff.ID}]:");
                            sb.AppendLine(
                                normalEff.lastExecuteTime?.ToStr(nameof(RelicEffectSaveParam.lastExecuteTime), 6));
                            sb.AppendLine(
                                normalEff.triggerCount?.ToStr(nameof(RelicEffectSaveParam.triggerCount), 6));
                        }
                    }
                    sb.AppendLine(asp.seniorArtifacts?.ToStr(nameof(ArtifactSaveParam.seniorArtifacts), 2));
                    sb.AppendLine(asp.seniorArtifactStars?.ToStr(nameof(ArtifactSaveParam.seniorArtifactStars), 2));
                    if (asp.seniorEffect != null)
                    {
                        sb.AppendLine($"  {nameof(ArtifactSaveParam.seniorEffect)}:");
                        foreach (var seniorEff in asp.seniorEffect)
                        {
                            sb.AppendLine($"    [{seniorEff.ID}]:");
                            sb.AppendLine(
                                seniorEff.lastExecuteTime?.ToStr(nameof(RelicEffectSaveParam.lastExecuteTime), 6));
                            sb.AppendLine(
                                seniorEff.triggerCount?.ToStr(nameof(RelicEffectSaveParam.triggerCount), 6));
                        }
                    }
                    sb.AppendLine(asp.itemIds?.ToStr(nameof(ArtifactSaveParam.itemIds), 2));
                    if (asp.itemRelicEffect != null)
                    {
                        sb.AppendLine($"  {nameof(ArtifactSaveParam.itemRelicEffect)}:");
                        foreach (var itemEff in asp.itemRelicEffect)
                        {
                            sb.AppendLine($"    [{itemEff.ID}]:");
                            sb.AppendLine(
                                itemEff.lastExecuteTime?.ToStr(nameof(RelicEffectSaveParam.lastExecuteTime), 6));
                            sb.AppendLine(
                                itemEff.triggerCount?.ToStr(nameof(RelicEffectSaveParam.triggerCount), 6));
                        }
                    }
                    sb.AppendLine($"  {nameof(ArtifactSaveParam.divinationLangID)}: {asp.divinationLangID}");
                    sb.AppendLine(asp.guestEffectIds?.ToStr(nameof(ArtifactSaveParam.guestEffectIds), 2));
                    sb.AppendLine(asp.guestEffectMoments?.ToStr(nameof(ArtifactSaveParam.guestEffectMoments), 2));
                    sb.AppendLine(asp.guestGroupIds?.ToStr(nameof(ArtifactSaveParam.guestGroupIds), 2));
                    sb.AppendLine(asp.guestNums?.ToStr(nameof(ArtifactSaveParam.guestNums), 2));
                    break;
                case nameof(HotelAllBuildingParam):
                    var habp = saveParam.Cast<HotelAllBuildingParam>();
                    sb.AppendLine($"[{nameof(HotelAllBuildingParam)}]");
                    sb.AppendLine($"  {nameof(HotelAllBuildingParam.currentFloor)}: {habp.currentFloor}");
                    sb.AppendLine($"  {nameof(HotelAllBuildingParam.currentBuildingID)}: {habp.currentBuildingID}");
                    sb.AppendLine($"  {nameof(HotelAllBuildingParam.lastMultiFloorID)}: {habp.lastMultiFloorID}");
                    if (habp.HotelBuildingParams != null)
                    {
                        sb.AppendLine($"  {nameof(HotelAllBuildingParam.HotelBuildingParams)}:");
                        foreach (var bp in habp.HotelBuildingParams)
                        {
                            sb.AppendLine($"    [{bp.id}]:");
                            sb.AppendLine($"      {nameof(HotelBuildingParam.level)}: {bp.level}");
                            if (bp.checkInUnits != null)
                            {
                                sb.AppendLine($"      {nameof(HotelBuildingParam.checkInUnits)}:");
                                foreach (var unit in bp.checkInUnits)
                                {
                                    sb.AppendLine($"        [{unit.unitBaseId}]:");
                                    sb.AppendLine($"          {nameof(UnitParam.money)}: {unit.money}");
                                    sb.AppendLine($"          {nameof(UnitParam.startRound)}: {unit.startRound}");
                                    sb.AppendLine($"          {nameof(UnitParam.lastRound)}: {unit.lastRound}");
                                    sb.AppendLine(unit.items?.ToStr(nameof(UnitParam.items), 10));
                                    sb.AppendLine($"          {nameof(UnitParam.hasCollectedRent)}: {unit.hasCollectedRent}");
                                    sb.AppendLine($"          {nameof(UnitParam.firstName)}: {unit.firstName}");
                                    sb.AppendLine($"          {nameof(UnitParam.lastName)}: {unit.lastName}");
                                }
                            }
                            if (bp.NPCList != null)
                            {
                                sb.AppendLine($"      {nameof(HotelBuildingParam.NPCList)}:");
                                foreach (var unit in bp.NPCList)
                                {
                                    sb.AppendLine($"        [{unit.unitBaseId}]:");
                                    sb.AppendLine($"          {nameof(UnitParam.money)}: {unit.money}");
                                    sb.AppendLine($"          {nameof(UnitParam.startRound)}: {unit.startRound}");
                                    sb.AppendLine($"          {nameof(UnitParam.lastRound)}: {unit.lastRound}");
                                    sb.AppendLine(unit.items?.ToStr(nameof(UnitParam.items), 10));
                                    sb.AppendLine($"          {nameof(UnitParam.hasCollectedRent)}: {unit.hasCollectedRent}");
                                    sb.AppendLine($"          {nameof(UnitParam.firstName)}: {unit.firstName}");
                                    sb.AppendLine($"          {nameof(UnitParam.lastName)}: {unit.lastName}");
                                }
                            }
                            if (bp.characterList != null)
                            {
                                sb.AppendLine($"      {nameof(HotelBuildingParam.characterList)}");
                                foreach (var unit in bp.characterList)
                                {
                                    sb.AppendLine($"        [{unit.unitBaseId}]:");
                                    sb.AppendLine($"          {nameof(UnitParam.money)}: {unit.money}");
                                    sb.AppendLine($"          {nameof(UnitParam.startRound)}: {unit.startRound}");
                                    sb.AppendLine($"          {nameof(UnitParam.lastRound)}: {unit.lastRound}");
                                    sb.AppendLine(unit.items?.ToStr(nameof(UnitParam.items), 10));
                                    sb.AppendLine($"          {nameof(UnitParam.hasCollectedRent)}: {unit.hasCollectedRent}");
                                    sb.AppendLine($"          {nameof(UnitParam.firstName)}: {unit.firstName}");
                                    sb.AppendLine($"          {nameof(UnitParam.lastName)}: {unit.lastName}");
                                }
                            }
                            sb.AppendLine($"      {nameof(HotelBuildingParam.currentTheme)}: {bp.currentTheme}");
                        }
                    }
                    if (habp.FloorSaveParams != null)
                    {
                        sb.AppendLine($"  {nameof(HotelAllBuildingParam.FloorSaveParams)}:");
                        foreach (var fsp in habp.FloorSaveParams)
                        {
                            sb.AppendLine($"    [{fsp.floorID}]:");
                            sb.AppendLine($"      {nameof(FloorSaveParam.isTriggerAutoCollectRentUI)}: {fsp.isTriggerAutoCollectRentUI}");
                            if (fsp.pernamentNPCs != null)
                            {
                                sb.AppendLine($"      {nameof(FloorSaveParam.pernamentNPCs)}:");
                                foreach (var unit in fsp.pernamentNPCs)
                                {
                                    sb.AppendLine($"        [{unit.unitBaseId}]:");
                                    sb.AppendLine($"          {nameof(UnitParam.money)}: {unit.money}");
                                    sb.AppendLine($"          {nameof(UnitParam.startRound)}: {unit.startRound}");
                                    sb.AppendLine($"          {nameof(UnitParam.lastRound)}: {unit.lastRound}");
                                    sb.AppendLine(unit.items?.ToStr(nameof(UnitParam.items), 10));
                                    sb.AppendLine($"          {nameof(UnitParam.hasCollectedRent)}: {unit.hasCollectedRent}");
                                    sb.AppendLine($"          {nameof(UnitParam.firstName)}: {unit.firstName}");
                                    sb.AppendLine($"          {nameof(UnitParam.lastName)}: {unit.lastName}");
                                }
                            }
                        }
                    }
                    sb.AppendLine(
                        habp.UnlockThemeBuilding?.ToStr(nameof(HotelAllBuildingParam.UnlockThemeBuilding), 2));
                    break;
                case nameof(RoundSaveParam):
                    var rsp = saveParam.Cast<RoundSaveParam>();
                    sb.AppendLine($"[{nameof(RoundSaveParam)}]");
                    sb.AppendLine($"  {nameof(RoundSaveParam.rounID)}: {rsp.rounID}");
                    sb.AppendLine($"  {nameof(RoundSaveParam.roundPhase)}: {rsp.roundPhase}");
                    break;
                case nameof(AllAttributeSaveParam):
                    var aasp = saveParam.Cast<AllAttributeSaveParam>();
                    sb.AppendLine($"[{nameof(AllAttributeSaveParam)}]");
                    if (aasp.AttributeParams != null)
                    {
                        sb.AppendLine($"  {nameof(AllAttributeSaveParam.AttributeParams)}:");
                        foreach (var ap in aasp.AttributeParams)
                        {
                            sb.AppendLine($"    {ap.attributeId}: {ap.value}");
                        }
                    }
                    break;
                case nameof(BagSaveParam):
                    var bsp = saveParam.Cast<BagSaveParam>();
                    sb.AppendLine($"[{nameof(BagSaveParam)}]");
                    sb.AppendLine(bsp.bagItemIdList?.ToStr(nameof(BagSaveParam.bagItemIdList), 2));
                    sb.AppendLine(bsp.itemCountList?.ToStr(nameof(BagSaveParam.itemCountList), 2));
                    sb.AppendLine(bsp.alchemyValueList?.ToStr(nameof(BagSaveParam.alchemyValueList), 2));
                    sb.AppendLine(bsp.dayList?.ToStr(nameof(BagSaveParam.dayList), 2));
                    sb.AppendLine(bsp.roundStateList?.ToStr(nameof(BagSaveParam.roundStateList), 2));
                    sb.AppendLine(bsp.timeDayOrNightList?.ToStr(nameof(BagSaveParam.timeDayOrNightList), 2));
                    break;
                case nameof(HotelBuffSaveParam):
                    var hbsp = saveParam.Cast<HotelBuffSaveParam>();
                    sb.AppendLine($"[{nameof(HotelBuffSaveParam)}]");
                    sb.AppendLine(hbsp.buffIDList?.ToStr(nameof(HotelBuffSaveParam.buffIDList), 2));
                    sb.AppendLine(hbsp.startRoundList?.ToStr(nameof(HotelBuffSaveParam.startRoundList), 2));
                    sb.AppendLine(hbsp.roundCountList?.ToStr(nameof(HotelBuffSaveParam.roundCountList), 2));
                    break;
                case nameof(InteractiveSaveParam):
                    var itrSP = saveParam.Cast<InteractiveSaveParam>();
                    sb.AppendLine($"[{nameof(InteractiveSaveParam)}]");
                    sb.AppendLine(
                        itrSP.interactiveSceneNpcId?.ToStr(nameof(InteractiveSaveParam.interactiveSceneNpcId), 2));
                    sb.AppendLine(
                        itrSP.interactiveTime?.ToStr(nameof(InteractiveSaveParam.interactiveTime), 2));
                    sb.AppendLine(
                        itrSP.existInteractiveSceneNpcId?.ToStr(nameof(InteractiveSaveParam.existInteractiveSceneNpcId), 2));
                    break;
                case nameof(MissionSaveParam):
                    var msp = saveParam.Cast<MissionSaveParam>();
                    sb.AppendLine($"[{nameof(MissionSaveParam)}]");
                    sb.AppendLine(msp.missionIDList?.ToStr(nameof(MissionSaveParam.missionIDList), 2));
                    sb.AppendLine(msp.acceptRoundIDList?.ToStr(nameof(MissionSaveParam.acceptRoundIDList), 2));
                    sb.AppendLine(msp.completeHistory?.ToStr(nameof(MissionSaveParam.completeHistory), 2));
                    sb.AppendLine(msp.storyPlayedList?.ToStr(nameof(MissionSaveParam.storyPlayedList), 2));
                    break;
                case nameof(SettleSaveParam):
                    var ssp = saveParam.Cast<SettleSaveParam>();
                    sb.AppendLine($"[{nameof(SettleSaveParam)}]");
                    sb.AppendLine(ssp.settleTurnOverDay?.ToStr(nameof(SettleSaveParam.settleTurnOverDay), 2));
                    sb.AppendLine(ssp.settleTurnOverMoney?.ToStr(nameof(SettleSaveParam.settleTurnOverMoney), 2));
                    sb.AppendLine(ssp.bonusTurnOverIncomeDay?.ToStr(nameof(SettleSaveParam.bonusTurnOverIncomeDay), 2));
                    sb.AppendLine(ssp.bonusTurnOverIncomeMoney?.ToStr(nameof(SettleSaveParam.bonusTurnOverIncomeMoney), 2));
                    sb.AppendLine(ssp.settleTrafficDay?.ToStr(nameof(SettleSaveParam.settleTrafficDay), 2));
                    sb.AppendLine(ssp.settleTrafficValue?.ToStr(nameof(SettleSaveParam.settleTrafficValue), 2));
                    sb.AppendLine(ssp.turnoverMonth?.ToStr(nameof(SettleSaveParam.turnoverMonth), 2));
                    sb.AppendLine(ssp.trafficDataMonth?.ToStr(nameof(SettleSaveParam.trafficDataMonth), 2));
                    sb.AppendLine(ssp.settleSlaughterIncomeDay?.ToStr(nameof(SettleSaveParam.settleSlaughterIncomeDay), 2));
                    sb.AppendLine(ssp.settleSlaughterIncomeMoney?.ToStr(nameof(SettleSaveParam.settleSlaughterIncomeMoney), 2));
                    sb.AppendLine($"  {nameof(SettleSaveParam.checkInHotelBase)}: {ssp.checkInHotelBase}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.checkInHotelFromRestaurant)}: {ssp.checkInHotelFromRestaurant}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.checkInHotelFromSummon)}: {ssp.checkInHotelFromSummon}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.restaurantDayMoney)}: {ssp.restaurantDayMoney}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.dailyPlayerShopSellItemMoney)}: {ssp.dailyPlayerShopSellItemMoney}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.totalPlayerShopSellItemMoney)}: {ssp.totalPlayerShopSellItemMoney}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.totalShopSpend)}: {ssp.totalShopSpend}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.totalFoodSellNum)}: {ssp.totalFoodSellNum}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.totalNpcTaskCompleteNum)}: {ssp.totalNpcTaskCompleteNum}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.maxPlayerBidingItemNum)}: {ssp.maxPlayerBidingItemNum}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.lastTrafficNum)}: {ssp.lastTrafficNum}");
                    sb.AppendLine(ssp.settleRestIncomeDay?.ToStr(nameof(SettleSaveParam.settleRestIncomeDay), 2));
                    sb.AppendLine(ssp.settleRestIncomeMoney?.ToStr(nameof(SettleSaveParam.settleRestIncomeMoney), 2));
                    sb.AppendLine(ssp.settleSpendDay?.ToStr(nameof(SettleSaveParam.settleSpendDay), 2));
                    sb.AppendLine(ssp.settleSpendMoney?.ToStr(nameof(SettleSaveParam.settleSpendMoney), 2));
                    sb.AppendLine(ssp.spendMonth?.ToStr(nameof(SettleSaveParam.spendMonth), 2));
                    sb.AppendLine(ssp.roomMonth?.ToStr(nameof(SettleSaveParam.roomMonth), 2));
                    sb.AppendLine(ssp.handbookProgressMonth?.ToStr(nameof(SettleSaveParam.handbookProgressMonth), 2));
                    sb.AppendLine(ssp.digDeepMonth?.ToStr(nameof(SettleSaveParam.digDeepMonth), 2));
                    sb.AppendLine(ssp.alchemyLevelMonth?.ToStr(nameof(SettleSaveParam.alchemyLevelMonth), 2));
                    sb.AppendLine(ssp.restIncomeMonth?.ToStr(nameof(SettleSaveParam.restIncomeMonth), 2));
                    sb.AppendLine($"  {nameof(SettleSaveParam.curMonth)}: {ssp.curMonth}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.lastGuestCleans)}: {ssp.lastGuestCleans}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.lastHouseWorkCleans)}: {ssp.lastHouseWorkCleans}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.lastTotalCleanness)}: {ssp.lastTotalCleanness}");
                    sb.AppendLine($"  {nameof(SettleSaveParam.cleannessChange)}: {ssp.cleannessChange}");
                    break;
                case nameof(SanCheckSaveParam):
                    var sanSP = saveParam.Cast<SanCheckSaveParam>();
                    sb.AppendLine($"[{nameof(SanCheckSaveParam)}]");
                    sb.AppendLine($"  {nameof(SanCheckSaveParam.sanPlot1IstTrigger)}: {sanSP.sanPlot1IstTrigger}");
                    sb.AppendLine($"  {nameof(SanCheckSaveParam.sanPlot2IstTrigger)}: {sanSP.sanPlot2IstTrigger}");
                    sb.AppendLine($"  {nameof(SanCheckSaveParam.sanZeroEnding)}: {sanSP.sanZeroEnding}");
                    break;
                case nameof(AlchemySaveParam):
                    var alcSP = saveParam.Cast<AlchemySaveParam>();
                    sb.AppendLine($"[{nameof(AlchemySaveParam)}]");
                    sb.AppendLine($"  {nameof(AlchemySaveParam.currentAlchemyLevel)}: {alcSP.currentAlchemyLevel}");
                    sb.AppendLine(
                        alcSP.probabilityCorrectionSlotList?.ToStr(nameof(AlchemySaveParam.probabilityCorrectionSlotList), 2));
                    sb.AppendLine(
                        alcSP.probabilityCorrectionValueList?.ToStr(nameof(AlchemySaveParam.probabilityCorrectionValueList), 2));
                    sb.AppendLine(alcSP.unlockedRecipeList?.ToStr(nameof(AlchemySaveParam.unlockedRecipeList), 2));
                    sb.AppendLine(alcSP.recipeLimitList?.ToStr(nameof(AlchemySaveParam.recipeLimitList), 2));
                    sb.AppendLine(alcSP.summonUnitRarityList?.ToStr(nameof(AlchemySaveParam.summonUnitRarityList), 2));
                    sb.AppendLine(alcSP.summonUnityNumList?.ToStr(nameof(AlchemySaveParam.summonUnityNumList), 2));
                    break;
                case nameof(StatisticsSaveParam):
                    var stsSP = saveParam.Cast<StatisticsSaveParam>();
                    sb.AppendLine($"[{nameof(StatisticsSaveParam)}]");
                    sb.AppendLine(stsSP.itemIdList?.ToStr(nameof(StatisticsSaveParam.itemIdList), 2));
                    sb.AppendLine(stsSP.itemGetCount?.ToStr(nameof(StatisticsSaveParam.itemGetCount), 2));
                    sb.AppendLine(stsSP.itemAlchemyCountIdList?.ToStr(nameof(StatisticsSaveParam.itemAlchemyCountIdList), 2));
                    sb.AppendLine(stsSP.itemAlchemyCount?.ToStr(nameof(StatisticsSaveParam.itemAlchemyCount), 2));
                    sb.AppendLine(stsSP.transItemIdList?.ToStr(nameof(StatisticsSaveParam.transItemIdList), 2));
                    sb.AppendLine(stsSP.transItemGetCount?.ToStr(nameof(StatisticsSaveParam.transItemGetCount), 2));
                    sb.AppendLine(stsSP.relicIdList?.ToStr(nameof(StatisticsSaveParam.relicIdList), 2));
                    sb.AppendLine(stsSP.relicSelectCount?.ToStr(nameof(StatisticsSaveParam.relicSelectCount), 2));
                    sb.AppendLine(stsSP.relicLevelList?.ToStr(nameof(StatisticsSaveParam.relicLevelList), 2));
                    sb.AppendLine(stsSP.npcHandBookIdList?.ToStr(nameof(StatisticsSaveParam.npcHandBookIdList), 2));
                    sb.AppendLine(stsSP.npcSlaughterCount?.ToStr(nameof(StatisticsSaveParam.npcSlaughterCount), 2));
                    sb.AppendLine(stsSP.npcIdList?.ToStr(nameof(StatisticsSaveParam.npcIdList), 2));
                    sb.AppendLine(stsSP.npcCheckInCount?.ToStr(nameof(StatisticsSaveParam.npcCheckInCount), 2));
                    sb.AppendLine(stsSP.cookbookIdList?.ToStr(nameof(StatisticsSaveParam.cookbookIdList), 2));
                    sb.AppendLine(stsSP.cookbookCookingNumList?.ToStr(nameof(StatisticsSaveParam.cookbookCookingNumList), 2));
                    sb.AppendLine(stsSP.foodIdList?.ToStr(nameof(StatisticsSaveParam.foodIdList), 2));
                    sb.AppendLine(stsSP.foodSellNumList?.ToStr(nameof(StatisticsSaveParam.foodSellNumList), 2));
                    if (stsSP.statisticsRecipeUnlockParams != null)
                    {
                        sb.AppendLine($"  {nameof(StatisticsSaveParam.statisticsRecipeUnlockParams)}:");
                        foreach (var srup in stsSP.statisticsRecipeUnlockParams)
                        {
                            sb.AppendLine($"    [{srup.recipeId}]:");
                            sb.AppendLine(
                                srup.dosageIdList?.ToStr(nameof(StatisticsRecipeUnlockParam.dosageIdList), 6));
                        }
                    }
                    break;
                case nameof(RestaurantSaveParam):
                    var restSP = saveParam.Cast<RestaurantSaveParam>();
                    sb.AppendLine($"[{nameof(RestaurantSaveParam)}]");
                    sb.AppendLine(restSP.MenuIdList?.ToStr(nameof(RestaurantSaveParam.MenuIdList), 2));
                    sb.AppendLine(restSP.MenuLevelList?.ToStr(nameof(RestaurantSaveParam.MenuLevelList), 2));
                    sb.AppendLine(restSP.MenuFoodIdList?.ToStr(nameof(RestaurantSaveParam.MenuFoodIdList), 2));
                    sb.AppendLine(restSP.MenuFoodNumList?.ToStr(nameof(RestaurantSaveParam.MenuFoodNumList), 2));
                    sb.AppendLine(restSP.MenuAutoSetList?.ToStr(nameof(RestaurantSaveParam.MenuAutoSetList), 2));
                    sb.AppendLine(restSP.FoodIdList?.ToStr(nameof(RestaurantSaveParam.FoodIdList), 2));
                    sb.AppendLine(restSP.RecipeIdList?.ToStr(nameof(RestaurantSaveParam.RecipeIdList), 2));
                    sb.AppendLine(restSP.RecipeIsLevelUpList?.ToStr(nameof(RestaurantSaveParam.RecipeIsLevelUpList), 2));
                    if (restSP.RecipeUnlockItems != null)
                    {
                        sb.AppendLine($"  {nameof(RestaurantSaveParam.RecipeUnlockItems)}:");
                        foreach (var unlock in restSP.RecipeUnlockItems)
                        {
                            sb.AppendLine($"    [{unlock.recipeItemID}]:");
                            sb.AppendLine(
                                unlock.unlockItems?.ToStr(nameof(RestRecipeUnlockItem.unlockItems), 6));
                        }
                    }
                    break;
                case nameof(ShopSaveParam):
                    var shopSP = saveParam.Cast<ShopSaveParam>();
                    sb.AppendLine($"[{nameof(ShopSaveParam)}]");
                    if (shopSP.ShopSellItemSaveParams != null)
                    {
                        sb.AppendLine($"  {nameof(ShopSaveParam.ShopSellItemSaveParams)}:");
                        foreach (var siSP in shopSP.ShopSellItemSaveParams)
                        {
                            sb.AppendLine($"    [{siSP.CommodityID}]");
                            sb.AppendLine($"      {nameof(ShopSellItemSaveParam.ItemId)}: {siSP.ItemId}");
                            sb.AppendLine($"      {nameof(ShopSellItemSaveParam.Num)}: {siSP.Num}");
                            sb.AppendLine($"      {nameof(ShopSellItemSaveParam.CurrentIdx)}: {siSP.CurrentIdx}");
                            sb.AppendLine(
                                siSP.AlreadySaleList?.ToStr(nameof(ShopSellItemSaveParam.AlreadySaleList), 6));
                        }
                    }
                    sb.AppendLine($"  {nameof(ShopSaveParam.RefreshCount)}: {shopSP.RefreshCount}");
                    break;
                case nameof(PlotSaveParam):
                    var psp = saveParam.Cast<PlotSaveParam>();
                    sb.AppendLine($"[{nameof(PlotSaveParam)}]");
                    sb.AppendLine(psp.playedStoryIDList?.ToStr(nameof(PlotSaveParam.playedStoryIDList), 2));
                    break;
                case nameof(CaveExploreSaveParam):
                    var cesp = saveParam.Cast<CaveExploreSaveParam>();
                    sb.AppendLine($"[{nameof(CaveExploreSaveParam)}]");
                    sb.AppendLine($"  {nameof(CaveExploreSaveParam.layer)}: {cesp.layer}");
                    sb.AppendLine($"  {nameof(CaveExploreSaveParam.digLevel)}: {cesp.digLevel}");
                    sb.AppendLine($"  {nameof(CaveExploreSaveParam.progress)}: {cesp.progress}");
                    sb.AppendLine($"  {nameof(CaveExploreSaveParam.digCount)}: {cesp.digCount}");
                    sb.AppendLine(cesp.caveExploreReward?.ToStr(nameof(CaveExploreSaveParam.caveExploreReward), 2));
                    break;
                case nameof(AdventureSaveParam):
                    var advSP = saveParam.Cast<AdventureSaveParam>();
                    sb.AppendLine($"[{nameof(AdventureSaveParam)}]");
                    sb.AppendLine($"  {nameof(AdventureSaveParam.level)}: {advSP.level}");
                    sb.AppendLine($"  {nameof(AdventureSaveParam.startRound)}: {advSP.startRound}");
                    sb.AppendLine($"  {nameof(AdventureSaveParam.adventureStamina)}: {advSP.adventureStamina}");
                    sb.AppendLine($"  {nameof(AdventureSaveParam.routeID)}: {advSP.routeID}");
                    sb.AppendLine($"  {nameof(AdventureSaveParam.addProgress)}: {advSP.addProgress}");
                    sb.AppendLine($"  {nameof(AdventureSaveParam.adventureAreaId)}: {advSP.adventureAreaId}");
                    sb.AppendLine($"  {nameof(AdventureSaveParam.adventuring)}: {advSP.adventuring}");
                    sb.AppendLine($"  {nameof(AdventureSaveParam.lastMainFood)}: {advSP.lastMainFood}");
                    sb.AppendLine(advSP.exploreAreaIdList?.ToStr(nameof(AdventureSaveParam.exploreAreaIdList), 2));
                    sb.AppendLine(advSP.exploreAreaValueList?.ToStr(nameof(AdventureSaveParam.exploreAreaValueList), 2));
                    sb.AppendLine(
                        advSP.exploreAreaProgressRewardList?.ToStr(nameof(AdventureSaveParam.exploreAreaProgressRewardList), 2));
                    sb.AppendLine(advSP.bagItemIdList?.ToStr(nameof(AdventureSaveParam.bagItemIdList), 2));
                    sb.AppendLine(advSP.itemCountList?.ToStr(nameof(AdventureSaveParam.itemCountList), 2));
                    sb.AppendLine(advSP.alchemyValueList?.ToStr(nameof(AdventureSaveParam.alchemyValueList), 2));
                    sb.AppendLine(advSP.dayList?.ToStr(nameof(AdventureSaveParam.dayList), 2));
                    sb.AppendLine(advSP.timeDayOrNightList?.ToStr(nameof(AdventureSaveParam.timeDayOrNightList), 2));
                    sb.AppendLine(advSP.lastSelectFood?.ToStr(nameof(AdventureSaveParam.lastSelectFood), 2));
                    sb.AppendLine(advSP.rewardIds?.ToStr(nameof(AdventureSaveParam.rewardIds), 2));
                    sb.AppendLine(advSP.areaProgressRewardIdList?.ToStr(nameof(AdventureSaveParam.areaProgressRewardIdList), 2));
                    break;
            }
        }

        return sb.ToString();
    }

    private static string ToStr<T>(
        this Il2CppSystem.Collections.Generic.List<T> list,
        string attrName, int indent)
    {
        var joined = string.Join(", ", list.ToManagedList());
        var spaces = string.Join(string.Empty, Enumerable.Range(0, indent).Select(_ => ' '));
        return $"{spaces}{attrName}: {joined}";
    }

    internal static void ExGlobalSave(System.Action<ExGlobalSaveData> updater)
    {
        updater?.Invoke(ExGlobalSaveDataCache);
        var json = JsonSerializer.Serialize(ExGlobalSaveDataCache);
        File.WriteAllText(
            Path.Combine(PathProvider.ExSaveDirectory, FILENAME_EXGLOBAL),
            json);
    }

    internal static void ExGlobalLoad(System.Action<ExGlobalSaveData> updater)
    {
        var path = Path.Combine(PathProvider.ExSaveDirectory, FILENAME_EXGLOBAL);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            var obj = JsonSerializer.Deserialize<ExGlobalSaveData>(json);
            ExGlobalSaveDataCache = obj;
            updater?.Invoke(ExGlobalSaveDataCache);
        }
        else
        {
            ExGlobalSaveDataCache = new();
            ExGlobalSave(null);
        }
    }
}
