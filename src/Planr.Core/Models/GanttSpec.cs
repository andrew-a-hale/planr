using System.Text.Json.Serialization;

namespace Planr.Core.Models;

public class GanttSpec
{
    [JsonPropertyName("config")]
    public GanttConfig Config { get; set; } = new();

    [JsonPropertyName("tasks")]
    public List<GanttTask> Tasks { get; set; } = new();
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
