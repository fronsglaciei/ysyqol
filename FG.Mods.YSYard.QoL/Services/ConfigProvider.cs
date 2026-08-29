using BepInEx.Configuration;

namespace FG.Mods.YSYard.QoL.Services;

internal static class ConfigProvider
{
    private const string SECTION_ADVENTURE_MODE = "AdventureMode";

    private const string SECTION_HOTEL_MODE = "HotelMode";

    private const string SECTION_QUICK_SAVE_FEATURE = "QuickSaveFeature";

    private const string SECTION_EXTRA = "Extra";

    internal static ConfigEntry<bool> MouseWheelDownToProgress { get; private set; }

    internal static ConfigEntry<bool> MouseWheelUpToBacklog { get; private set; }

    internal static ConfigEntry<bool> MouseRightUpToCloseBacklog { get; private set; }

    internal static ConfigEntry<bool> ForceInstantText { get; private set; }

    internal static ConfigEntry<string> TextFontName { get; private set; }

    internal static ConfigEntry<long> APMaxPoint { get; private set; }

    internal static ConfigEntry<bool> SkipAutoOpenVisitedPlayGuides { get; private set; }

    internal static ConfigEntry<bool> SkipBloodMoonAnimations { get; private set; }

    internal static ConfigEntry<bool> UseRestaurantFeatures { get; private set; }

    internal static ConfigEntry<bool> UseQuickSaveLoad { get; private set; }

    internal static ConfigEntry<bool> SkipConfirmQuickLoad { get; private set; }

    internal static ConfigEntry<bool> UseModTranslations { get; private set; }

    internal static void Init(ConfigFile configFile)
    {
        MouseWheelDownToProgress = configFile.Bind(
            SECTION_ADVENTURE_MODE,
            nameof(MouseWheelDownToProgress),
            true,
            "マウスホイールを下方向にスクロールするとテキストを読み進めます.\nバニラ環境でのマウス左クリック, またはスペースキー押下と同等の挙動をとります.\ntrueで有効, falseで無効です.");

        MouseWheelUpToBacklog = configFile.Bind(
            SECTION_ADVENTURE_MODE,
            nameof(MouseWheelUpToBacklog),
            true,
            "マウスホイールを上方向にスクロールするとバックログウィンドウを開きます.\nバニラ環境でバックログボタンをクリックしたときと同等の挙動をとります.\ntrueで有効, falseで無効です.");

        MouseRightUpToCloseBacklog = configFile.Bind(
            SECTION_ADVENTURE_MODE,
            nameof(MouseRightUpToCloseBacklog),
            true,
            "マウス右ボタンをクリックするとバックログウィンドウを閉じます.\nバニラ環境でバックログウィンドウの閉じるボタンをクリックしたときと同等の挙動をとります.\ntrueで有効, falseで無効です.");

        ForceInstantText = configFile.Bind(
            SECTION_ADVENTURE_MODE,
            nameof(ForceInstantText),
            false,
            "テキスト送り速度を無視して, 強制的に即時表示します.\n付随する画像等の演出も即時表示します.\ntrueで有効, falseで無効です.");

        TextFontName = configFile.Bind(
            SECTION_ADVENTURE_MODE,
            nameof(TextFontName),
            string.Empty,
            "テキスト表示に使用するフォントを, OSにインストールされたフォントから取得します.\n何も設定されていない, または設定からフォントが取得できない場合はバニラ環境のフォントが使用されます.\n注意:\n  設定されたフォントでテキストに含まれる文字が表示可能かのチェックは行いません.\n  例えば, アルファベットと記号しか含まれないフォントを設定しても英語以外は表示できません.\n有効な設定例: Yu Gothic UI");

        APMaxPoint = configFile.Bind(
            SECTION_HOTEL_MODE,
            nameof(APMaxPoint),
            100L,
            new ConfigDescription(
                "行動力(AP)の上限値をこの設定値で上書きします.\nバニラ環境ではAPの上限値は5です.",
                new AcceptableValueRange<long>(5L, long.MaxValue - 1)));

        SkipAutoOpenVisitedPlayGuides = configFile.Bind(
            SECTION_HOTEL_MODE,
            nameof(SkipAutoOpenVisitedPlayGuides),
            true,
            "一度でも表示したことがあるガイドが自動で表示されないようにします.\nガイドが表示済みであるという判定は, 当MODが独自に作成するセーブファイルに保存されるため, MODを導入したばかりの環境では機能しない場合があります.\ntrueで有効, falseで無効です.");

        SkipBloodMoonAnimations = configFile.Bind(
            SECTION_HOTEL_MODE,
            nameof(SkipBloodMoonAnimations),
            false,
            "至高神託選択前のアニメーションをスキップします.\ntrueで有効, falseで無効です.");

        UseRestaurantFeatures = configFile.Bind(
            SECTION_HOTEL_MODE,
            nameof(UseRestaurantFeatures),
            true,
            "レストランのメニュー選択画面に機能を追加します.\ntrueで有効, falseで無効です.");

        UseQuickSaveLoad = configFile.Bind(
            SECTION_QUICK_SAVE_FEATURE,
            nameof(UseQuickSaveLoad),
            true,
            "クイックセーブ＆ロード機能をゲームに追加します.\n設定が有効なとき, ゲーム内UIにQS, QLボタンが追加され, それぞれのボタンを押すことで対応する機能が利用できます.\ntrueで有効, falseで無効です.");

        SkipConfirmQuickLoad = configFile.Bind(
            SECTION_QUICK_SAVE_FEATURE,
            nameof(SkipConfirmQuickLoad),
            false,
            "クイックロード実行時に確認メッセージが表示されないようにします.\ntrueで有効, falseで無効です.");

        UseModTranslations = configFile.Bind(
            SECTION_EXTRA,
            nameof(UseModTranslations),
            false,
            "過去に非公式日本語化MOD(Yog-Sothoth's Yard Unofficial Japanese Translation Mod)として配布されていた機能は, 当MODに移植されました.\n言語設定で日本語が選択されているとき, 非公式の翻訳でテキストを置換します.\nシステムおよびストーリーに登場する固有名詞が公式日本語訳と異なりますのでご注意ください.\ntrueで有効, falseで無効です.");
    }
}
