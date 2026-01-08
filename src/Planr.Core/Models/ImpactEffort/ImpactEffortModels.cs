using System.Text.Json.Serialization;

namespace Planr.Core.Models.ImpactEffort;

public class ImpactEffortSpec
{
    [JsonPropertyName("config")]
    public ImpactEffortConfig Config { get; set; } = new();

    [JsonPropertyName("tasks")]
    public List<ImpactEffortTask> Tasks { get; set; } = new();
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

