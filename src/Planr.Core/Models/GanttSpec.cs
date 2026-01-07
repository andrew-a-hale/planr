using System.Text.Json.Serialization;

namespace Planr.Core.Models;

public class GanttSpec
{
    [JsonPropertyName("style")]
    public string Style { get; set; } = "default";

    [JsonPropertyName("tasks")]
    public List<GanttTask> Tasks { get; set; } = new();
}
