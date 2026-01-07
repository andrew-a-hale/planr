using System.Text.Json.Serialization;

namespace Planr.Core.Models;

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
    public int Priority { get; set; } = 3;
}
