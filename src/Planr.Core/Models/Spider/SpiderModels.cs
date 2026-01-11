using System.Text.Json.Serialization;

namespace Planr.Core.Models.Spider;

public class SpiderSpec
{
  [JsonPropertyName("config")]
  public SpiderConfig Config { get; set; } = new();

  [JsonPropertyName("items")]
  public List<SpiderItem> Items { get; set; } = [];
}

public class SpiderItem
{
  [JsonPropertyName("category")]
  public string Category { get; set; } = string.Empty;

  [JsonPropertyName("seriesValues")]
  public Dictionary<string, double?> SeriesValues { get; set; } = [];
}

public class SpiderConfig : ChartConfig
{
  [JsonPropertyName("seriesNames")]
  public List<string> SeriesNames { get; set; } = ["First", "Second", "Third"];
}
