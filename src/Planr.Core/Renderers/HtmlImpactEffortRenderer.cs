using Planr.Core.Configuration;
using Planr.Core.Models.ImpactEffort;
using Planr.Core.ViewModels.ImpactEffort;
using Scriban;

namespace Planr.Core.Renderers;

public static class HtmlImpactEffortRenderer
{
    public static string Render(Models.ImpactEffort.ImpactEffortSpec spec, bool partial = false)
    {
        if (spec.Tasks == null || spec.Tasks.Count == 0)
        {
            return "<html><body><h1>No tasks to display</h1></body></html>";
        }

        var viewModel = BuildViewModel(spec);

        // Get shared CSS content
        var cssContent = GetTemplate("core-charts.css");

        // Always render partial first with CSS included
        var partialContent = global::Scriban.Template
            .Parse(GetTemplate("gridPartial.html"))
            .Render(
                new
                {
                    Config = viewModel.Config,
                    Css = cssContent,
                    ViewModel = viewModel,
                },
                member => member.Name
            );

        if (partial)
        {
            return partialContent;
        }

        // Wrap in full layout with CSS included
        var layoutTemplate = global::Scriban.Template.Parse(GetTemplate("grid.html"));
        return layoutTemplate.Render(
            new { body = partialContent, Css = cssContent },
            member => member.Name
        );
    }

    private static ViewModels.ImpactEffort.ImpactEffortViewModel BuildViewModel(
        Models.ImpactEffort.ImpactEffortSpec spec
    )
    {
        var vm = new ViewModels.ImpactEffort.ImpactEffortViewModel
        {
            Config = spec.Config ?? new Models.ImpactEffort.ImpactEffortConfig(),
        };

        // Get unique categories
        vm.Categories = spec.Tasks.Select(t => t.Category).Distinct().OrderBy(c => c).ToList();

        // Build quadrants
        var quadrantNames = new Dictionary<(int row, int col), string>
        {
            { (0, 0), "Fill-ins" }, // Low Impact, Low Effort
            { (0, 1), "Fill-ins" }, // Low Impact, Medium Effort
            { (0, 2), "Thankless Tasks" }, // Low Impact, High Effort
            { (1, 0), "Fill-ins" }, // Medium Impact, Low Effort
            { (1, 1), "Thankless Tasks" }, // Medium Impact, Medium Effort
            { (1, 2), "Thankless Tasks" }, // Medium Impact, High Effort
            { (2, 0), "Quick Wins" }, // High Impact, Low Effort
            { (2, 1), "Major Projects" }, // High Impact, Medium Effort
            { (2, 2), "Major Projects" }, // High Impact, High Effort
        };

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                var quadrantName = quadrantNames[(row, col)];
                var quadrant = new ViewModels.ImpactEffort.GridQuadrant
                {
                    Name = quadrantName,
                    Row = row,
                    Col = col,
                    LeftPercent = col * 33.333,
                    TopPercent = row * 33.333,
                    Color = ImpactEffortTheme.GetQuadrantColor(quadrantName),
                };

                // Add description
                quadrant.Description = quadrantName switch
                {
                    "Quick Wins" => "High Impact, Low Effort",
                    "Major Projects" => "High Impact, High Effort",
                    "Fill-ins" => "Low Impact, Low Effort",
                    "Thankless Tasks" => "Low Impact, High Effort",
                    _ => "",
                };

                vm.Quadrants.Add(quadrant);
            }
        }

        // Process tasks
        foreach (var task in spec.Tasks)
        {
            var (row, col) = ViewModels.ImpactEffort.QuadrantHelper.GetQuadrantPosition(
                task.Impact,
                task.Effort
            );
            var (left, top) = ViewModels.ImpactEffort.QuadrantHelper.GetTaskPosition(
                task.Impact,
                task.Effort
            );

            var gridTask = new ViewModels.ImpactEffort.GridTask
            {
                Ref = task.Ref,
                Name = task.Name,
                Category = task.Category,
                Impact = task.Impact,
                Effort = task.Effort,
                LeftPercent = left,
                TopPercent = top,
                QuadrantColor = ImpactEffortTheme.GetQuadrantColor(
                    ViewModels.ImpactEffort.QuadrantHelper.GetQuadrantName(task.Impact, task.Effort)
                ),
                CategoryColor = ImpactEffortTheme.GetCategoryColor(task.Category, vm.Categories),
            };

            vm.Tasks.Add(gridTask);

            // Add to corresponding quadrant
            var quadrant = vm.Quadrants.FirstOrDefault(q => q.Row == row && q.Col == col);
            quadrant?.Tasks.Add(gridTask);
        }

        // Build legend
        var quadrantGroups = vm.Quadrants.GroupBy(q => q.Name).Select(g => g.First());
        foreach (var quadrant in quadrantGroups)
        {
            vm.Legend.Add(
                new ViewModels.ImpactEffort.LegendItem
                {
                    Name = $"{quadrant.Name} ({quadrant.Description})",
                    Color = quadrant.Color,
                    Type = "quadrant",
                }
            );
        }

        // Add category colors to legend
        for (int i = 0; i < vm.Categories.Count; i++)
        {
            var category = vm.Categories[i];
            vm.Legend.Add(
                new ViewModels.ImpactEffort.LegendItem
                {
                    Name = category,
                    Color = ImpactEffortTheme.GetCategoryColor(category, vm.Categories),
                    Type = "category",
                }
            );
        }

        return vm;
    }

    private static string GetTemplate(string name)
    {
        // Use the assembly where this class is defined
        var assembly = typeof(HtmlImpactEffortRenderer).Assembly;

        // Resource name pattern: DefaultNamespace.Folders.Filename
        // Project Namespace: Planr.Core
        // Folder: Templates
        // File: grid.html
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

