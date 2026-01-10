using Planr.Core.Models.Gantt;

namespace Planr.Core.Configuration;

public static class GanttTheme
{
  public static Dictionary<GanttPriority, string> PriorityColors { get; set; } =
      new()
      {
            { GanttPriority.Critical, Colors.Red },
            { GanttPriority.High, Colors.Orange },
            { GanttPriority.Medium, Colors.Gold },
            { GanttPriority.Low, Colors.Blue },
            { GanttPriority.Lowest, Colors.Green },
      };

  public static string GetColor(GanttPriority priority)
  {
    return PriorityColors.TryGetValue(priority, out var color) ? color : Colors.LightGray;
  }

  public static string GetPriorityCss(GanttPriority priority) =>
      priority switch
      {
        GanttPriority.Critical => "critical",
        GanttPriority.High => "high",
        GanttPriority.Medium => "medium",
        GanttPriority.Low => "low",
        GanttPriority.Lowest => "lowest",
        _ => "medium",
      };
}
