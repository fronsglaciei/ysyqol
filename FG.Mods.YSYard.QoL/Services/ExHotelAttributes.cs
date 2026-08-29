using HotelModule;
using System;

namespace FG.Mods.YSYard.QoL.Services;

internal static class ExHotelAttributes
{
    private const int HOTEL_ATTRIBUTE_ID_AP = 921;

    private const long DEFAULT_AP_MAX_VALUE = 5L;

    private static rm _instance; // private static HotelAttributes _instance;

    private static bool _configHandlerAdded = false;

    internal static void Init(
        //HotelAttributes instance)
        rm instance)
    {
        _instance = instance;
        OnApMaxValueChanged(null, null);

        if (!_configHandlerAdded)
        {
            ConfigProvider.APMaxPoint.SettingChanged
                += OnApMaxValueChanged;
            _configHandlerAdded = true;
        }
    }

    private static void OnApMaxValueChanged(object _, EventArgs __)
    {
        //var attributes = _instance?.attributeDic;
        var attributes = _instance?.yiz;
        if (attributes is null)
        {
            return;
        }

        if (attributes.TryGetValue(HOTEL_ATTRIBUTE_ID_AP, out var valComb))
        {
            //valComb.max = Math.Max(
            valComb.vod = Math.Max(
                DEFAULT_AP_MAX_VALUE,
                ConfigProvider.APMaxPoint.Value);
        }
    }
}
