using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class CompanyIdentityCatalog
{
    public readonly struct FontOption
    {
        public FontOption(string key, string label, string osFontName, FontStyles style)
        {
            Key = key;
            Label = label;
            OsFontName = osFontName;
            Style = style;
        }

        public string Key { get; }
        public string Label { get; }
        public string OsFontName { get; }
        public FontStyles Style { get; }
    }

    public readonly struct IconOption
    {
        public IconOption(string key, string label, string fileName)
        {
            Key = key;
            Label = label;
            FileName = fileName;
        }

        public string Key { get; }
        public string Label { get; }
        public string FileName { get; }
    }

    public static readonly FontOption[] FontOptions =
    {
        new FontOption("default", "Default", "", FontStyles.Bold),
        new FontOption("technical", "Technical", "Consolas", FontStyles.Bold),
        new FontOption("editorial", "Editorial", "Georgia", FontStyles.Bold),
        new FontOption("corporate", "Corporate", "Arial", FontStyles.Bold)
    };

    public static readonly Color[] ColorOptions =
    {
        Hex("#0EA5E9"),
        Hex("#10B981"),
        Hex("#F59E0B"),
        Hex("#EF4444"),
        Hex("#8B5CF6"),
        Hex("#1C2333")
    };

    public static readonly IconOption[] IconOptions =
    {
        new IconOption("pixflow", "Pixflow", "pixflow.jpg"),
        new IconOption("anthropic", "Anthropic", "anthropic.png")
    };

    private static readonly Dictionary<string, TMP_FontAsset> FontCache = new Dictionary<string, TMP_FontAsset>();
    private static readonly Dictionary<string, Sprite> IconCache = new Dictionary<string, Sprite>();

    public static string DefaultFontKey => FontOptions[0].Key;
    public static Color DefaultColor => ColorOptions[0];
    public static string DefaultIconKey => IconOptions[0].Key;

    public static string ColorToHex(Color color)
    {
        return "#" + ColorUtility.ToHtmlStringRGB(color);
    }

    public static Color ParseColor(string hex, Color fallback)
    {
        return ColorUtility.TryParseHtmlString(hex, out var color) ? color : fallback;
    }

    public static FontOption GetFontOption(string key)
    {
        foreach (var option in FontOptions)
        {
            if (option.Key == key)
            {
                return option;
            }
        }

        return FontOptions[0];
    }

    public static IconOption GetIconOption(string key)
    {
        foreach (var option in IconOptions)
        {
            if (option.Key == key)
            {
                return option;
            }
        }

        return IconOptions[0];
    }

    public static void ApplyToCompanyText(TextMeshProUGUI text, string fontKey, Color color)
    {
        if (text == null)
        {
            return;
        }

        var option = GetFontOption(fontKey);
        var fontAsset = GetFontAsset(option);
        if (fontAsset != null)
        {
            text.font = fontAsset;
        }

        text.fontStyle = option.Style;
        text.color = color;
    }

    public static Sprite LoadCompanyIcon(string iconKey)
    {
        var option = GetIconOption(iconKey);
        if (IconCache.TryGetValue(option.Key, out var cached))
        {
            return cached;
        }

        var path = System.IO.Path.Combine(Application.dataPath, "../MyAssets/Rivals", option.FileName);
        var sprite = LoadSprite(path);
        IconCache[option.Key] = sprite;
        return sprite;
    }

    public static Sprite LoadSprite(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            return null;
        }

        try
        {
            var data = System.IO.File.ReadAllBytes(path);
            var texture = new Texture2D(2, 2);
            if (!texture.LoadImage(data))
            {
                Object.Destroy(texture);
                return null;
            }

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Could not load company icon from {path}: {e.Message}");
            return null;
        }
    }

    private static TMP_FontAsset GetFontAsset(FontOption option)
    {
        if (string.IsNullOrEmpty(option.OsFontName))
        {
            return null;
        }

        if (FontCache.TryGetValue(option.Key, out var cached))
        {
            return cached;
        }

        try
        {
            var font = Font.CreateDynamicFontFromOSFont(option.OsFontName, 90);
            if (font == null)
            {
                return null;
            }

            var asset = TMP_FontAsset.CreateFontAsset(font);
            FontCache[option.Key] = asset;
            return asset;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Could not create TMP font asset for {option.OsFontName}: {e.Message}");
            return null;
        }
    }

    private static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var color);
        return color;
    }
}
