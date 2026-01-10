using Planr.Core.Models.Spider;

namespace Planr.Core.ViewModels.Spider;

public class SpiderViewModel
{
  public List<SpiderItem> Items { get; set; } = [];
  public List<SpiderCategory> Categories { get; set; } = [];
  public List<SpiderSeries> Series { get; set; } = [];
  public SpiderConfig Config { get; set; } = new();
  public double CenterValue { get; set; } = -1;
  public double MaxValue { get; set; } = 4;
}

public class SpiderCategory
{
  public string Name { get; set; } = string.Empty;
  public string TruncatedName { get; set; } = string.Empty;
  public double Angle { get; set; }
  public double AngleRadians { get; set; }
  public double AngleRadiansAdjusted { get; set; }
  public double OuterX { get; set; }
  public double OuterY { get; set; }
  public double LabelX { get; set; }
  public double LabelY { get; set; }
}

public class SpiderSeries
{
  public string Name { get; set; } = string.Empty;
  public string Color { get; set; } = string.Empty;
  public List<SpiderPoint> Points { get; set; } = [];
}

public class SpiderPoint
{
  public double X { get; set; }
  public double Y { get; set; }
  public double? Value { get; set; }
  public string CategoryName { get; set; } = string.Empty;
}

public static class SpiderChartHelper
{
  public const double CenterValue = -1;
  public const double MaxValue = 4;
  public const int MaxLabelLength = 20;

  public static double ValueToPercent(double? value)
  {
    if (value == null)
      return 0;

    double clampedValue = Math.Clamp(value.Value, CenterValue, MaxValue);
    double range = MaxValue - CenterValue;
    return (clampedValue - CenterValue) / range * 100;
  }

  public static (double x, double y) PercentToCoordinates(double percent, double angleDegrees, double centerPercent = 50)
  {
    double angleRadians = (angleDegrees - 90) * (Math.PI / 180);
    double radiusPercent = percent * 0.45;
    double x = centerPercent + radiusPercent * Math.Cos(angleRadians);
    double y = centerPercent + radiusPercent * Math.Sin(angleRadians);
    return (x, y);
  }

  public static string BuildPolygonPoints(List<SpiderPoint> points)
  {
    var pointStrings = points.Select(p => $"{p.X},{p.Y}");
    return string.Join(" ", pointStrings);
  }

  public static string TruncateLabel(string label)
  {
    if (string.IsNullOrEmpty(label) || label.Length <= MaxLabelLength)
      return label;

    return string.Concat(label.AsSpan(0, MaxLabelLength - 3), "...");
  }

  public static void ValidateSpec(SpiderSpec spec)
  {
    if (spec.Config?.SeriesNames == null || spec.Config.SeriesNames.Count < 2)
    {
      throw new InvalidOperationException("At least 2 series names must be provided in config.");
    }

    if (spec.Items == null || spec.Items.Count == 0)
    {
      return;
    }

    var seriesNames = spec.Config.SeriesNames.Distinct().ToList();
    var duplicateNames = spec.Config.SeriesNames.GroupBy(n => n).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
    if (duplicateNames.Count != 0)
    {
      throw new InvalidOperationException($"Duplicate series names found: {string.Join(", ", duplicateNames)}. Series names must be unique.");
    }

    foreach (var seriesName in seriesNames)
    {
      bool hasNull = spec.Items.Any(i => !i.SeriesValues.TryGetValue(seriesName, out var val) || val == null);
      bool hasValue = spec.Items.Any(i => i.SeriesValues.TryGetValue(seriesName, out var val) && val != null);

      if (hasNull && hasValue)
      {
        var missingItems = spec.Items
            .Where(i => !i.SeriesValues.TryGetValue(seriesName, out var val) || val == null)
            .Select(i => i.Category)
            .ToList();

        throw new InvalidOperationException(
            $"Series '{seriesName}' has missing/null values for categories: {string.Join(", ", missingItems)}. " +
            "All values for a series must be provided or all must be null."
        );
      }
    }
  }
}
