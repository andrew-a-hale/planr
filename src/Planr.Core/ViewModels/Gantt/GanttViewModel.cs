using Planr.Core.Configuration;
using Planr.Core.Models.Gantt;

namespace Planr.Core.ViewModels.Gantt;

public class GanttViewModel
{
  public List<TimeBlock> TimeBlocks { get; set; } = [];
  public GanttConfig Config { get; set; } = new();
  public List<ProjectGroup> Projects { get; set; } = [];
  public List<LegendItem> Legend { get; set; } = [];
  public List<WeekMarker> Weeks { get; set; } = [];
  public List<TaskViewModel> Tasks { get; set; } = [];
  public List<ProjectGroup> ProjectGroups { get; set; } = [];
}

public class TimeBlock
{
  public string Project { get; set; } = string.Empty;
  public string TaskName { get; set; } = string.Empty;
  public DateOnly Start { get; set; }
  public DateOnly End { get; set; }
  public int Duration => End.DayNumber - Start.DayNumber + 1;
  public GanttPriority Priority { get; set; }
  public string PriorityCss => GanttTheme.GetPriorityCss(Priority);
  public string Color { get; set; } = string.Empty;
}

public class ProjectGroup
{
  public string Name { get; set; } = string.Empty;
  public List<TaskViewModel> Tasks { get; set; } = [];
  public string Color { get; set; } = string.Empty;
}

public class LegendItem
{
  public string Name { get; set; } = string.Empty;
  public int Value { get; set; }
  public string Color { get; set; } = string.Empty;
}

public class WeekMarker
{
  public double LeftPercent { get; set; }
  public double WidthPercent { get; set; }
  public string Label { get; set; } = string.Empty;
}

public class TaskViewModel
{
  public string Name { get; set; } = string.Empty;
  public DateOnly Start { get; set; }
  public DateOnly End { get; set; }
  public double LeftPercent { get; set; }
  public double WidthPercent { get; set; }
  public string Color { get; set; } = string.Empty;
  public string PriorityCss { get; set; } = string.Empty;
  public string Project { get; set; } = string.Empty;
  public GanttPriority Priority { get; set; }
  public List<double> WeekMarks { get; set; } = [];
}
