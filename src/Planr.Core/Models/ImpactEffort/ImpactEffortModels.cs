using System.Text.Json.Serialization;

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

public class ImpactEffortConfig : ChartConfig { }

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
