using System.Text.Json.Serialization;

namespace Planr.Core.Models.Gantt;

public class GanttSpec
{
  [JsonPropertyName("config")]
  public GanttConfig Config { get; set; } = new();

  [JsonPropertyName("tasks")]
  public List<GanttTask> Tasks { get; set; } = [];
}

public class GanttTask
{
  [JsonPropertyName("project")]
  public string Project { get; set; } = string.Empty;

  [JsonPropertyName("name")]
  public string Name { get; set; } = string.Empty;

  [JsonPropertyName("start")]
  public DateOnly Start { get; set; }

  [JsonPropertyName("end")]
  public DateOnly End { get; set; }

  [JsonPropertyName("priority")]
  public GanttPriority Priority { get; set; } = GanttPriority.Medium;
}

public class GanttConfig : ChartConfig
{
  [JsonPropertyName("labelWidth")]
  public int LabelWidth { get; set; } = 150;

  [JsonPropertyName("showLegend")]
  public bool ShowLegend { get; set; } = true;
}

public enum GanttPriority
{
  Critical = 1,
  High = 2,
  Medium = 3,
  Low = 4,
  Lowest = 5,
}
