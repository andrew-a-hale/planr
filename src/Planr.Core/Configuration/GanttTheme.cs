using Planr.Core.Models;

namespace Planr.Core.Configuration;

public static class GanttTheme
{
    public static Dictionary<Priority, string> PriorityColors { get; set; } =
        new()
        {
            { Priority.Critical, "#e74c3c" }, // Red
            { Priority.High, "#e67e22" }, // Orange
            { Priority.Medium, "#f1c40f" }, // Yellow
            { Priority.Low, "#3498db" }, // Blue
            { Priority.Lowest, "#2ecc71" }, // Green
        };

    public static string GetColor(Priority priority)
    {
        return PriorityColors.TryGetValue(priority, out var color) ? color : "#95a5a6";
    }
}
