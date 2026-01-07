using Planr.Core.Models;
using Planr.Core.ViewModels;
using Scriban;

namespace Planr.Core.Renderers;

public static class HtmlGanttRenderer
{
    private const string Name = "gantt_partial.html";

    public static string Render(GanttSpec spec, bool partial = false)
    {
        if (spec.Tasks == null || spec.Tasks.Count == 0)
        {
            return "<html><body><h1>No tasks to display</h1></body></html>";
        }

        var viewModel = BuildViewModel(spec);

        // Always render partial first
        var partialContent = Template
            .Parse(GetTemplate(Name))
            .Render(viewModel, member => member.Name);

        if (partial)
        {
            return partialContent;
        }

        // Wrap in full layout
        var layoutTemplate = Template.Parse(GetTemplate("gantt.html"));
        return layoutTemplate.Render(new { body = partialContent }, member => member.Name);
    }

    private static GanttViewModel BuildViewModel(GanttSpec spec)
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

        var vm = new GanttViewModel();

        // Generate Weeks
        var oneWeekSeconds = 7.0 * 24 * 60 * 60;
        var weekWidthPercent = (oneWeekSeconds / totalDuration) * 100;

        var currentDate = minDate;
        while (currentDate < maxDate)
        {
            if (currentDate.DayOfWeek == DayOfWeek.Monday)
            {
                var offset = (currentDate - minDate).TotalSeconds;
                var leftPercent = (offset / totalDuration) * 100;

                vm.Weeks.Add(
                    new WeekMarker
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
            var projectGroup = new ProjectGroup { Name = group.Key };

            foreach (var task in group.OrderBy(t => t.Start))
            {
                var offset = (task.Start - minDate).TotalSeconds;
                var duration = (task.End - task.Start).TotalSeconds;

                var leftPercent = (offset / totalDuration) * 100;
                var widthPercent = (duration / totalDuration) * 100;
                var weeks = (task.End - task.Start).TotalDays / 7.0;

                projectGroup.Tasks.Add(
                    new TaskViewModel
                    {
                        Name = task.Name,
                        Priority = task.Priority,
                        LeftPercent = leftPercent,
                        WidthPercent = widthPercent,
                        Color = GetPriorityColor(task.Priority),
                        DurationWeeks = weeks,
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

    private static string GetPriorityColor(int priority)
    {
        return priority switch
        {
            1 => "#e74c3c", // Critical/Highest (Red)
            2 => "#e67e22", // High (Orange)
            3 => "#f1c40f", // Medium (Yellow)
            4 => "#3498db", // Low (Blue)
            5 => "#2ecc71", // Lowest (Green)
            _ => "#95a5a6", // Unknown (Grey)
        };
    }
}
