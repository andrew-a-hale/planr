using System.Text.Json.Serialization;

namespace Planr.Core.Models.Gantt;

public class GanttSpec
{
    [JsonPropertyName("config")]
    public GanttConfig Config { get; set; } = new();

    [JsonPropertyName("tasks")]
    public List<GanttTask> Tasks { get; set; } = new();
}

public class GanttTask
{
    [JsonPropertyName("project")]
    public string Project { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("start")]
    public DateTime Start { get; set; }

    [JsonPropertyName("end")]
    public DateTime End { get; set; }

    [JsonPropertyName("priority")]
    public Priority Priority { get; set; } = Priority.Medium;
}

public class GanttConfig
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = "Project Plan";

    [JsonPropertyName("labelWidth")]
    public int LabelWidth { get; set; } = 150;

    [JsonPropertyName("showLegend")]
    public bool ShowLegend { get; set; } = true;

    [JsonIgnore]
    public ScreenWidth ContainerMaxWidth { get; set; } = ScreenWidth.Wide;

    [JsonPropertyName("containerMaxWidth")]
    public string ContainerMaxWidthCss =>
        ContainerMaxWidth switch
        {
            ScreenWidth.Narrow => "800px",
            ScreenWidth.Standard => "1000px",
            ScreenWidth.Wide => "1200px",
            ScreenWidth.ExtraWide => "1400px",
            ScreenWidth.Fluid => "100%",
            _ => "1200px",
        };
}

public enum Priority
{
    Critical = 1,
    High = 2,
    Medium = 3,
    Low = 4,
    Lowest = 5,
}

