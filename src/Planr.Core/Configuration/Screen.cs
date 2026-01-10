namespace Planr.Core.Configuration;

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
        Width.Narrow => $"Narrow ({WidthCss(width)})",
        Width.Standard => $"Standard ({WidthCss(width)})",
        Width.Wide => $"Wide ({WidthCss(width)})",
        Width.ExtraWide => $"Extra Wide ({WidthCss(width)})",
        Width.Fluid => $"Fluid ({WidthCss(width)})",
        _ => $"Wide ({WidthCss(width)})",
      };
}
