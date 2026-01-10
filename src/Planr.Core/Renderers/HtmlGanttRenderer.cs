using Planr.Core.Configuration;
using Planr.Core.Models.Gantt;

namespace Planr.Core.Renderers;

public static class HtmlGanttRenderer
{
  public static string Render(GanttSpec spec, bool partial = false)
  {
    if (spec.Tasks == null || spec.Tasks.Count == 0)
    {
      return "<html><body><h1>No tasks to display</h1></body></html>";
    }

    var viewModel = BuildViewModel(spec);

    // Get shared CSS content
    var cssContent = GetTemplate("core-charts.css");

    // Always render partial first with CSS included
    var partialContent = Scriban
        .Template.Parse(GetTemplate("ganttPartial.html"))
        .Render(
            new
            {
              viewModel.Config,
              Css = cssContent,
              viewModel.Legend,
              viewModel.Weeks,
              viewModel.ProjectGroups,
            },
            member => member.Name
        );

    if (partial)
    {
      return partialContent;
    }

    // Wrap in full layout with CSS included
    var layoutTemplate = Scriban.Template.Parse(GetTemplate("gantt.html"));
    return layoutTemplate.Render(
        new { body = partialContent, Css = cssContent },
        member => member.Name
    );
  }

  private static ViewModels.Gantt.GanttViewModel BuildViewModel(GanttSpec spec)
  {
    // Calculate global timeline
    var minDate = spec.Tasks.Min(t => t.Start);
    var maxDate = spec.Tasks.Max(t => t.End);

    // Align to full weeks (Monday to Monday)
    int daysSinceMonday = ((int)minDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
    minDate = minDate.AddDays(-daysSinceMonday);

    int daysUntilNextMonday = ((int)DayOfWeek.Monday - (int)maxDate.DayOfWeek + 7) % 7;
    if (daysUntilNextMonday == 0)
      daysUntilNextMonday = 7;
    maxDate = maxDate.AddDays(daysUntilNextMonday);

    var totalDuration = (maxDate - minDate).TotalSeconds;
    if (totalDuration <= 0)
      totalDuration = 1; // Prevent division by zero

    var vm = new ViewModels.Gantt.GanttViewModel
    {
      Config = spec.Config ?? new GanttConfig(),
    };

    // Populate Legend
    foreach (var kvp in GanttTheme.PriorityColors)
    {
      vm.Legend.Add(
          new ViewModels.Gantt.LegendItem
          {
            Name = kvp.Key.ToString(),
            Value = (int)kvp.Key,
            Color = kvp.Value,
          }
      );
    }

    // Ensure sorted by priority value
    vm.Legend = [.. vm.Legend.OrderBy(x => x.Value)];

    // Generate Weeks
    var oneWeekSeconds = 7.0 * 24 * 60 * 60;
    var weekWidthPercent = oneWeekSeconds / totalDuration * 100;

    var currentDate = minDate;
    while (currentDate < maxDate)
    {
      if (currentDate.DayOfWeek == DayOfWeek.Monday)
      {
        var offset = (currentDate - minDate).TotalSeconds;
        var leftPercent = offset / totalDuration * 100;

        vm.Weeks.Add(
            new ViewModels.Gantt.WeekMarker
            {
              LeftPercent = leftPercent,
              WidthPercent = weekWidthPercent,
              Label = currentDate.ToString("MMM dd"),
            }
        );
      }
      currentDate = currentDate.AddDays(1);
    }

    // Generate Groups and Tasks
    var groupedTasks = spec.Tasks.GroupBy(t => t.Project).OrderBy(g => g.Key);

    foreach (var group in groupedTasks)
    {
      var projectGroup = new ViewModels.Gantt.ProjectGroup { Name = group.Key };

      foreach (var task in group.OrderBy(t => t.Start))
      {
        var offset = (task.Start - minDate).TotalSeconds;
        var duration = (task.End - task.Start).TotalSeconds;

        var leftPercent = offset / totalDuration * 100;
        var widthPercent = duration / totalDuration * 100;
        var weeks = (task.End - task.Start).TotalDays / 7.0;

        var weekMarks = new List<double>();
        // Find first Monday strictly after start date
        var nextMonday = task.Start.AddDays(
            ((int)DayOfWeek.Monday - (int)task.Start.DayOfWeek + 7) % 7
        );
        if (nextMonday <= task.Start)
          nextMonday = nextMonday.AddDays(7);

        while (nextMonday < task.End)
        {
          var markOffset = (nextMonday - task.Start).TotalSeconds;
          // Calculate percentage relative to the task width, not total width
          var markPercent = markOffset / duration * 100;
          weekMarks.Add(markPercent);
          nextMonday = nextMonday.AddDays(7);
        }

        projectGroup.Tasks.Add(
            new ViewModels.Gantt.TaskViewModel
            {
              Name = task.Name,
              Priority = task.Priority,
              LeftPercent = leftPercent,
              WidthPercent = widthPercent,
              Color = GanttTheme.GetColor(task.Priority),
              WeekMarks = weekMarks,
            }
        );
      }

      vm.ProjectGroups.Add(projectGroup);
    }

    return vm;
  }

  private static string GetTemplate(string name)
  {
    // Use the assembly where this class is defined
    var assembly = typeof(HtmlGanttRenderer).Assembly;

    // Resource name pattern: DefaultNamespace.Folders.Filename
    // Project Namespace: Planr.Core
    // Folder: Templates
    // File: gantt.html
    var resourceName = $"Planr.Core.Templates.{name}";

    using var stream = assembly.GetManifestResourceStream(resourceName);
    if (stream == null)
    {
      var resources = string.Join(", ", assembly.GetManifestResourceNames());
      throw new FileNotFoundException(
          $"Template '{name}' not found. Searched for '{resourceName}'. Available: {resources}"
      );
    }

    using var reader = new StreamReader(stream);
    return reader.ReadToEnd();
  }
}
