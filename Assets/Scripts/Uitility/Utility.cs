using UnityEngine;

public static class Utility
{
    public static string SetStringColor(Color color, string str)
    {
        return "<color=" + ColorToHex(color) + ">" + str + "</color>";
    }

    public static string SetStringColor(ColorType colorType, string str)
    {
        return "<color=" + ColorToHex(GetColor(colorType)) + ">" + str + "</color>";
    }

    public static Color GetColor(ColorType type)
    {
        return type switch
        {
            ColorType.None => Color.white,
            ColorType.Negative => new Color(0.83f, 0.28f, 0.25f),
            ColorType.Positive => new Color(0.1059f, 0.7176f, 0.0314f),
            _ => Color.black
        };
    }

    public static string ColorToHex(Color color)
    {
        int r = Mathf.Clamp(Mathf.FloorToInt(color.r * 255), 0, 255);
        int g = Mathf.Clamp(Mathf.FloorToInt(color.g * 255), 0, 255);
        int b = Mathf.Clamp(Mathf.FloorToInt(color.b * 255), 0, 255);
        return $"#{r:X2}{g:X2}{b:X2}";
    }
}
