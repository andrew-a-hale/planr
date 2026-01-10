using System.Text.Json.Serialization;
using Planr.Core.Configuration;

namespace Planr.Core.Models.ImpactEffort;

public class ImpactEffortSpec
{
  [JsonPropertyName("config")]
  public ImpactEffortConfig Config { get; set; } = new();

  [JsonPropertyName("tasks")]
  public List<ImpactEffortTask> Tasks { get; set; } = [];
}

public class ImpactEffortTask
{
  [JsonPropertyName("ref")]
  public string Ref { get; set; } = string.Empty;

  [JsonPropertyName("name")]
  public string Name { get; set; } = string.Empty;

  [JsonPropertyName("category")]
  public string Category { get; set; } = string.Empty;

  [JsonPropertyName("impact")]
  public ImpactLevel Impact { get; set; } = ImpactLevel.Medium;

  [JsonPropertyName("effort")]
  public EffortLevel Effort { get; set; } = EffortLevel.Medium;
}

public class ImpactEffortConfig
{
  [JsonPropertyName("title")]
  public string Title { get; set; } = "Impact-Effort Matrix";

  [JsonIgnore]
  public Screen.Width ContainerMaxWidth { get; set; } = Screen.Width.Wide;

  [JsonPropertyName("containerMaxWidth")]
  public string ContainerMaxWidthCss => Screen.WidthCss(ContainerMaxWidth);
}

public enum ImpactLevel
{
  Low,
  Medium,
  High,
}

public enum EffortLevel
{
  Low,
  Medium,
  High,
}
