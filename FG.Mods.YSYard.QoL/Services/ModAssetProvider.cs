using FG.Defs.YSYard.QoL;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using UnityEngine;

namespace FG.Mods.YSYard.QoL.Services;

internal static class ModAssetProvider
{
    private const string ASSETNAME_MODCONST = "FG.Mods.YSYard.QoL.Assets.constants.json";

    private const string ASSETNAME_CONTROLS = "FG.Mods.YSYard.QoL.Assets.controls.png";

    private static readonly ModConstants _consts;

    private static readonly byte[] _conrolsImageCache;

    private static Texture2D _texControls;

    private static TlData _tlData = new();

    private static Sprite _showModSettingsSprite;
    internal static Sprite ShowModSettingsSprite
    {
        get
        {
            if (!TryLoadControlsTexture())
            {
                return null;
            }
            if (_showModSettingsSprite is null)
            {
                if (!_consts.ControlsTextureRegions
                    .TryGetValue("ShowModSettings", out var region))
                {
                    return null;
                }

                _showModSettingsSprite = Sprite.Create(
                    _texControls,
                    new Rect(
                        region.X, region.Y,
                        region.W, region.H),
                    new Vector2(0.5f, 0.5f),
                    100f, 0, SpriteMeshType.FullRect);
                _showModSettingsSprite.hideFlags = HideFlags.HideAndDontSave;
            }
            return _showModSettingsSprite;
        }
    }

    private static Sprite _hideModSettingsSprite;
    internal static Sprite HideModSettingsSprite
    {
        get
        {
            if (!TryLoadControlsTexture())
            {
                return null;
            }
            if (_hideModSettingsSprite is null)
            {
                if (!_consts.ControlsTextureRegions
                    .TryGetValue("HideModSettings", out var region))
                {
                    return null;
                }

                _hideModSettingsSprite = Sprite.Create(
                    _texControls,
                    new Rect(
                        region.X, region.Y,
                        region.W, region.H),
                    new Vector2(0.5f, 0.5f),
                    100f, 0, SpriteMeshType.FullRect);
                _hideModSettingsSprite.hideFlags = HideFlags.HideAndDontSave;
            }
            return _hideModSettingsSprite;
        }
    }

    private static Sprite _quickSaveSprite;
    internal static Sprite QuickSaveSprite
    {
        get
        {
            if (!TryLoadControlsTexture())
            {
                return null;
            }
            if (_quickSaveSprite is null)
            {
                if (!_consts.ControlsTextureRegions
                    .TryGetValue("QuickSave", out var region))
                {
                    return null;
                }

                _quickSaveSprite = Sprite.Create(
                    _texControls,
                    new Rect(
                        region.X, region.Y,
                        region.W, region.H),
                    new Vector2(0.5f, 0.5f),
                    100f, 0, SpriteMeshType.FullRect);
                _quickSaveSprite.hideFlags = HideFlags.HideAndDontSave;
            }
            return _quickSaveSprite;
        }
    }

    private static Sprite _quickSaveMainSprite;
    internal static Sprite QuickSaveMainSprite
    {
        get
        {
            if (!TryLoadControlsTexture())
            {
                return null;
            }
            if (_quickSaveMainSprite is null)
            {
                if (!_consts.ControlsTextureRegions
                    .TryGetValue("QuickSaveMain", out var region))
                {
                    return null;
                }

                _quickSaveMainSprite = Sprite.Create(
                    _texControls,
                    new Rect(
                        region.X, region.Y,
                        region.W, region.H),
                    new Vector2(0.5f, 0.5f),
                    100f, 0, SpriteMeshType.FullRect);
                _quickSaveMainSprite.hideFlags = HideFlags.HideAndDontSave;
            }
            return _quickSaveMainSprite;
        }
    }

    private static Sprite _quickLoadSprite;
    internal static Sprite QuickLoadSprite
    {
        get
        {
            if (!TryLoadControlsTexture())
            {
                return null;
            }
            if (_quickLoadSprite is null)
            {
                if (!_consts.ControlsTextureRegions
                    .TryGetValue("QuickLoad", out var region))
                {
                    return null;
                }

                _quickLoadSprite = Sprite.Create(
                    _texControls,
                    new Rect(
                        region.X, region.Y,
                        region.W, region.H),
                    new Vector2(0.5f, 0.5f),
                    100f, 0, SpriteMeshType.FullRect);
                _quickLoadSprite.hideFlags = HideFlags.HideAndDontSave;
            }
            return _quickLoadSprite;
        }
    }

    private static Sprite _quickLoadMainSprite;
    internal static Sprite QuickLoadMainSprite
    {
        get
        {
            if (!TryLoadControlsTexture())
            {
                return null;
            }
            if (_quickLoadMainSprite is null)
            {
                if (!_consts.ControlsTextureRegions
                    .TryGetValue("QuickLoadMain", out var region))
                {
                    return null;
                }

                _quickLoadMainSprite = Sprite.Create(
                    _texControls,
                    new Rect(
                        region.X, region.Y,
                        region.W, region.H),
                    new Vector2(0.5f, 0.5f),
                    100f, 0, SpriteMeshType.FullRect);
                _quickLoadMainSprite.hideFlags = HideFlags.HideAndDontSave;
            }
            return _quickLoadMainSprite;
        }
    }

    static ModAssetProvider()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var streamModconst = assembly.GetManifestResourceStream(ASSETNAME_MODCONST)
            ?? throw new NullReferenceException($"Failed to load {ASSETNAME_MODCONST}");
        using (streamModconst)
        {
            using var sr = new StreamReader(streamModconst);
            var json = sr.ReadToEnd();
            _consts = JsonSerializer.Deserialize<ModConstants>(json);
        }

        var streamControls = assembly.GetManifestResourceStream(ASSETNAME_CONTROLS)
            ?? throw new NullReferenceException($"Failed to load {ASSETNAME_CONTROLS}");
        using (streamControls)
        {
            using var ms = new MemoryStream();
            streamControls.CopyTo(ms);
            _conrolsImageCache = ms.ToArray();
        }
    }

    internal static bool TryGetModText(string key, out string text)
    {
        text = string.Empty;
        if (!_consts.Texts.TryGetValue(key, out var modText))
        {
            return false;
        }

        //text = GameSettingManager.Instance.GameLanguage switch
        //{
        //    GameSettingManager.LanguageEnum.SIMPLECHINESE
        //        => modText.SimpleChinese,
        //    GameSettingManager.LanguageEnum.TRADITIONALCHINESE
        //        => modText.TraditionalChinese,
        //    GameSettingManager.LanguageEnum.ENGILISH
        //        => modText.English,
        //    GameSettingManager.LanguageEnum.JANPANESE
        //        => modText.Japanese,
        //    _ => string.Empty
        //};
        text = ds.bgsv.vjz switch
        {
            ds.dr.SIMPLECHINESE => modText.SimpleChinese,
            ds.dr.TRADITIONALCHINESE => modText.TraditionalChinese,
            ds.dr.ENGILISH => modText.English,
            ds.dr.JANPANESE => modText.Japanese,
            _ => string.Empty
        };
        return !string.IsNullOrEmpty(text);
    }

    internal static void LoadTranslations()
    {
        var tlPath = Path.Combine(PathProvider.PluginDirectory, ModConstants.FILENAME_TL_DATA);
        if (!File.Exists(tlPath))
        {
            Plugin.Log.LogError($"Failed to load {tlPath}. File not found.");
            return;
        }

        var json = File.ReadAllText(tlPath);
        var obj = JsonSerializer.Deserialize<TlData>(json);
        if (obj is null)
        {
            Plugin.Log.LogError($"Failed to load {tlPath}. JSON is invalid.");
            return;
        }
        _tlData = obj;

        ConfigProvider.UseModTranslations.SettingChanged
            += ReloadOriginalTranslations;
    }

    internal static bool TryGetLanguageTranslation(int key, out string languageTranslation)
        => _tlData.Languages.TryGetValue(key, out languageTranslation);

    internal static bool TryGetLanguageTalkTranslation(int key, out string languageTalkTranslation)
        => _tlData.LanguageTalks.TryGetValue(key, out languageTalkTranslation);

    private static bool TryLoadControlsTexture()
    {
        if (_texControls is not null)
        {
            return true;
        }

        _texControls = new(256, 256, TextureFormat.RGBA32, false);
        if (!ImageConversion.LoadImage(_texControls, new(_conrolsImageCache)))
        {
            _texControls = null;
            return false;
        }
        return true;
    }

    private static void ReloadOriginalTranslations(object _, EventArgs __)
    {
        // skip when ysytrans is used
        if (ConfigProvider.UseModTranslations.Value)
        {
            return;
        }

        //LanguageManager.mItemArray = null;
        hm.wao = null;
        //LanguageManager.Load();
        hm.Load();
        //LanguageTalkManager.mItemArray = null;
        hn.war = null;
        //LanguageTalkManager.Load();
        hn.Load();
    }
}
