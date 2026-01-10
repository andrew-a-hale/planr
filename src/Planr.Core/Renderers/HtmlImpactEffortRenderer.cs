using Planr.Core.Configuration;

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
    var partialContent = Scriban
        .Template.Parse(GetTemplate("matrixPartial.html"))
        .Render(
            new
            {
              viewModel.Config,
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
    var layoutTemplate = Scriban.Template.Parse(GetTemplate("matrix.html"));
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
      // Get unique categories
      Categories = [.. spec.Tasks.Select(t => t.Category).Distinct().OrderBy(c => c)]
    };

    for (int row = 0; row < 3; row++)
    {
      for (int col = 0; col < 3; col++)
      {
        var quadrant = new ViewModels.ImpactEffort.MatrixQuadrant
        {
          Row = row,
          Col = col,
          LeftPercent = col * 33.333,
          TopPercent = row * 33.333,
        };

        vm.Quadrants.Add(quadrant);
      }
    }

    // Group tasks by quadrant first
    var tasksByQuadrant =
        new Dictionary<(int row, int col), List<Models.ImpactEffort.ImpactEffortTask>>();

    foreach (var task in spec.Tasks)
    {
      var (row, col) = ViewModels.ImpactEffort.QuadrantHelper.GetQuadrantPosition(
          task.Impact,
          task.Effort
      );

      var key = (row, col);
      if (!tasksByQuadrant.ContainsKey(key))
        tasksByQuadrant[key] = [];

      tasksByQuadrant[key].Add(task);
    }

    // Process tasks with quadrant-aware positioning
    foreach (var (quadrantKey, quadrantTasks) in tasksByQuadrant)
    {
      var (row, col) = quadrantKey;

      for (int i = 0; i < quadrantTasks.Count; i++)
      {
        var task = quadrantTasks[i];
        var (left, top) = ViewModels.ImpactEffort.QuadrantHelper.GetTaskPosition(
            task.Impact,
            task.Effort,
            i,
            quadrantTasks.Count
        );

        var matrixTask = new ViewModels.ImpactEffort.MatrixTask
        {
          Ref = task.Ref,
          Name = task.Name,
          Category = task.Category,
          Impact = task.Impact,
          Effort = task.Effort,
          LeftPercent = left,
          TopPercent = top,
          CategoryColor = ImpactEffortTheme.GetCategoryColor(
                task.Category,
                vm.Categories
            ),
        };

        vm.Tasks.Add(matrixTask);

        // Add to corresponding quadrant
        var quadrant = vm.Quadrants.FirstOrDefault(q => q.Row == row && q.Col == col);
        quadrant?.Tasks.Add(matrixTask);
      }
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
    // File: matrix.html
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
