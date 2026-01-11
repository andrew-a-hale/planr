namespace Planr.Core.Configuration;

public static class Colors
{
  // Category colors
  public static readonly string Purple = "#9b59b6";
  public static readonly string Turquoise = "#1abc9c";
  public static readonly string Amber = "#f39c12";
  public static readonly string DarkGray = "#34495e";
  public static readonly string Pink = "#e91e63";
  public static readonly string Brown = "#795548";
  public static readonly string BlueGray = "#607d8b";
  public static readonly string DeepOrange = "#ff5722";

  // Priority colors
  public static readonly string Red = "#e74c3c";
  public static readonly string Orange = "#e67e22";
  public static readonly string Gold = "#f1c40f";
  public static readonly string Blue = "#3498db";
  public static readonly string Green = "#2ecc71";

  // Default color
  public static readonly string LightGray = "#95a5a6";

  // Extended palette for spider chart series (up to 10)
  public static readonly string[] SeriesPalette =
  [
    Red,
    Turquoise,
    Amber,
    Purple,
    Blue,
    DeepOrange,
    Pink,
    Green,
    Orange,
    DarkGray,
  ];

  public static string GetSeriesColor(int index)
  {
    if (index < 0)
      return LightGray;
    return SeriesPalette[index % SeriesPalette.Length];
  }
}
