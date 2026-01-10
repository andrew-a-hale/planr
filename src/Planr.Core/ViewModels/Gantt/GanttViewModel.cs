using Planr.Core.Configuration;
using Planr.Core.Models.Gantt;

namespace Planr.Core.ViewModels.Gantt;

public class GanttViewModel
{
    public List<TimeBlock> TimeBlocks { get; set; } = new();
    public GanttConfig Config { get; set; } = new();
    public List<ProjectGroup> Projects { get; set; } = new();
    public List<LegendItem> Legend { get; set; } = new();
    public List<WeekMarker> Weeks { get; set; } = new();
    public List<TaskViewModel> Tasks { get; set; } = new();
    public List<ProjectGroup> ProjectGroups { get; set; } = new();
}

public class TimeBlock
{
    public string Project { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Duration => (int)Math.Ceiling((End - Start).TotalDays) + 1;
    public GanttPriority Priority { get; set; }
    public string PriorityCss => GanttTheme.GetPriorityCss(Priority);
    public string Color { get; set; } = string.Empty;
}

public class ProjectGroup
{
    public string Name { get; set; } = string.Empty;
    public List<TaskViewModel> Tasks { get; set; } = new();
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
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public double LeftPercent { get; set; }
    public double WidthPercent { get; set; }
    public string Color { get; set; } = string.Empty;
    public string PriorityCss { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public GanttPriority Priority { get; set; }
    public List<double> WeekMarks { get; set; } = new();
}
