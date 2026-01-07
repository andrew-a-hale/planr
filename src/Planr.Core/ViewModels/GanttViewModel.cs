using Planr.Core.Models;

namespace Planr.Core.ViewModels;

public class GanttViewModel
{
    public List<WeekMarker> Weeks { get; set; } = new();
    public List<ProjectGroup> ProjectGroups { get; set; } = new();
    public List<LegendItem> Legend { get; set; } = new();
    public GanttConfig Config { get; set; } = new();
}

public class LegendItem
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class WeekMarker
{
    public double LeftPercent { get; set; }
    public double WidthPercent { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class ProjectGroup
{
    public string Name { get; set; } = string.Empty;
    public List<TaskViewModel> Tasks { get; set; } = new();
}

public class TaskViewModel
{
    public string Name { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public double LeftPercent { get; set; }
    public double WidthPercent { get; set; }
    public string Color { get; set; } = string.Empty;
    public List<double> WeekMarks { get; set; } = new();
}
