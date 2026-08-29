using FG.Mods.YSYard.QoL.Services;
using Plot;
using System;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal class PlotWindowWriterFontSetter : IDisposable
{
    private Text _text;

    private PlotWindowWriterFontSetter() { }

    internal static PlotWindowWriterFontSetter Create(PlotWindow pw)
    {
        if (pw is null)
        {
            return null;
        }
        //if (!pw.SayDialogs.TryGetValue(0, out var dialog))
        if (!pw.befl.TryGetValue(0, out var dialog))
        {
            return null;
        }
        //var writerText = dialog.GetWriter()?.textUI;
        var writerText = dialog.lsy()?.bdfl;
        if (writerText is null)
        {
            return null;
        }

        var ret = new PlotWindowWriterFontSetter
        {
            _text = writerText,
        };
        ret.OnTextFontNameChanged(null, null);
        ConfigProvider.TextFontName.SettingChanged
            += ret.OnTextFontNameChanged;
        return ret;
    }

    private void OnTextFontNameChanged(object _, EventArgs __)
    {
        var fontName = ConfigProvider.TextFontName.Value;
        if (string.IsNullOrEmpty(fontName)
            || this._text.font.name == fontName)
        {
            return;
        }

        var font = FontManager.CreateOSFont(
            fontName, this._text.font.fontSize);
        if (font is null)
        {
            return;
        }

        this._text.font = font;
    }

    public void Dispose() =>
        ConfigProvider.TextFontName.SettingChanged
            -= this.OnTextFontNameChanged;
}
