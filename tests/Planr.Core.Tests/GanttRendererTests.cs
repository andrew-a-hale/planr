using FluentAssertions;
using Planr.Core.Models.Gantt;
using Planr.Core.Renderers;

namespace Planr.Core.Tests;

public class GanttRendererTests
{
  [Fact]
  public void BuildViewModel_ShouldAlignToMondays()
  {
    // Arrange
    var spec = new GanttSpec
    {
      Tasks =
            [
                new()
                {
                    Project = "P1",
                    Name = "T1",
                    Start = new DateOnly(2024, 1, 3), // Wednesday
                    End = new DateOnly(2024, 1, 5),   // Friday
                    Priority = GanttPriority.Medium
                }
            ]
    };

    // Act
    var vm = HtmlGanttRenderer.BuildViewModel(spec);

    // Assert
    // Jan 1st 2024 was a Monday
    vm.Weeks.Should().NotBeEmpty();
    vm.Weeks.First().Label.Should().Be("Jan 01");
  }

  [Fact]
  public void BuildViewModel_ShouldGroupTasksByProject()
  {
    // Arrange
    var spec = new GanttSpec
    {
      Tasks =
            [
                new() { Project = "A", Name = "T1", Start = new(2024, 1, 1), End = new(2024, 1, 2) },
                new() { Project = "B", Name = "T2", Start = new(2024, 1, 1), End = new(2024, 1, 2) },
                new() { Project = "A", Name = "T3", Start = new(2024, 1, 3), End = new(2024, 1, 4) }
            ]
    };

    // Act
    var vm = HtmlGanttRenderer.BuildViewModel(spec);

    // Assert
    vm.ProjectGroups.Should().HaveCount(2);
    vm.ProjectGroups.First(g => g.Name == "A").Tasks.Should().HaveCount(2);
    vm.ProjectGroups.First(g => g.Name == "B").Tasks.Should().HaveCount(1);
  }

  [Fact]
  public void BuildViewModel_ShouldCalculateCorrectPercentages()
  {
    // Arrange
    // Total range: 2024-01-01 (Mon) to 2024-01-15 (Mon) = 14 days
    var spec = new GanttSpec
    {
      Tasks =
            [
                new()
                {
                    Project = "P1",
                    Name = "T1",
                    Start = new DateOnly(2024, 1, 1),
                    End = new DateOnly(2024, 1, 8), // 7 days duration
                    Priority = GanttPriority.High
                }
            ]
    };

    // Act
    var vm = HtmlGanttRenderer.BuildViewModel(spec);

    // Assert
    var task = vm.ProjectGroups.First().Tasks.First();
    task.LeftPercent.Should().Be(0);
    task.WidthPercent.Should().Be(50); // 7 days / 14 days * 100
  }

  [Fact]
  public void Render_EmptySpec_ShouldReturnNoTasksMessage()
  {
    var renderer = new HtmlGanttRenderer();
    var result = renderer.Render(new GanttSpec { Tasks = [] });
    result.Should().Contain("No tasks to display");
  }

  [Fact]
  public void BuildViewModel_ShouldHandleMultiYearTasks()
  {
    var spec = new GanttSpec
    {
      Tasks =
            [
                new() { Project = "P1", Name = "Long Task", Start = new(2023, 12, 1), End = new(2024, 1, 31) }
            ]
    };

    var vm = HtmlGanttRenderer.BuildViewModel(spec);
    vm.Weeks.Should().Contain(w => w.Label.Contains("Dec"));
    vm.Weeks.Should().Contain(w => w.Label.Contains("Jan"));
  }
}
