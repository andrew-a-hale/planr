using Planr.Core.Configuration;

namespace Planr.Core.Renderers;

public class HtmlImpactEffortRenderer : IChartRenderer<Models.ImpactEffort.ImpactEffortSpec>
{
  public string Render(Models.ImpactEffort.ImpactEffortSpec spec, bool partial = false)
  {
    if (spec.Tasks == null || spec.Tasks.Count == 0)
    {
      return "<html><body><h1>No tasks to display</h1></body></html>";
    }

    var viewModel = BuildViewModel(spec);

    var cssContent = TemplateLoader.LoadTemplate("core-charts.css");

    // Always render partial first with CSS included
    var partialContent = Scriban
        .Template.Parse(TemplateLoader.LoadTemplate("matrixPartial.html"))
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

    // Wrap in shared layout
    var layoutTemplate = Scriban.Template.Parse(TemplateLoader.LoadTemplate("chart-wrapper.html"));
    return layoutTemplate.Render(
        new { body = partialContent, viewModel.Config.Title },
        member => member.Name
    );
  }

  internal static ViewModels.ImpactEffort.ImpactEffortViewModel BuildViewModel(
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
}
