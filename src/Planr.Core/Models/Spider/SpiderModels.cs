using System.Text.Json.Serialization;
using Planr.Core.Configuration;

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

public class SpiderConfig
{
  [JsonPropertyName("title")]
  public string Title { get; set; } = "Spider Chart";

  [JsonPropertyName("seriesNames")]
  public List<string> SeriesNames { get; set; } = ["First", "Second", "Third"];

  [JsonIgnore]
  public Screen.Width ContainerMaxWidth { get; set; } = Screen.Width.Wide;

  [JsonPropertyName("containerMaxWidth")]
  public string ContainerMaxWidthCss => Screen.WidthCss(ContainerMaxWidth);
}
