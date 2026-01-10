namespace Planr.Core.Models;

public class Screen
{
    public enum Width
    {
        Narrow,
        Standard,
        Wide,
        ExtraWide,
        Fluid,
    }

    public static string WidthCss(Width width) =>
        width switch
        {
            Width.Narrow => "800px",
            Width.Standard => "1000px",
            Width.Wide => "1200px",
            Width.ExtraWide => "1400px",
            Width.Fluid => "100%",
            _ => "1200px",
        };

    public static string WidthCssOption(Width width) =>
        width switch
        {
            Width.Narrow => $"Narrow ({Screen.WidthCss(width)})",
            Width.Standard => $"Standard ({Screen.WidthCss(width)})",
            Width.Wide => $"Wide ({Screen.WidthCss(width)})",
            Width.ExtraWide => $"Extra Wide ({Screen.WidthCss(width)})",
            Width.Fluid => $"Fluid ({Screen.WidthCss(width)})",
            _ => $"Wide ({Screen.WidthCss(width)})",
        };
}
