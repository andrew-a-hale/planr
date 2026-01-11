using System.Text.Json.Serialization;
using Planr.Core.Configuration;

namespace Planr.Core.Models;

public abstract class ChartConfig
{
  [JsonPropertyName("title")]
  public string Title { get; set; } = "Chart";

  [JsonIgnore]
  public Screen.Width ContainerMaxWidth { get; set; } = Screen.Width.Wide;

  [JsonPropertyName("containerMaxWidth")]
  public string ContainerMaxWidthCss => Screen.WidthCss(ContainerMaxWidth);
}
