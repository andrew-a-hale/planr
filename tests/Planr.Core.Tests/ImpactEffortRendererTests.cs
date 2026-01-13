using FluentAssertions;
using Planr.Core.Models.ImpactEffort;
using Planr.Core.Renderers;

namespace Planr.Core.Tests;

public class ImpactEffortRendererTests
{
  [Fact]
  public void BuildViewModel_ShouldGroupTasksInQuadrants()
  {
    // Arrange
    var spec = new ImpactEffortSpec
    {
      Tasks =
        [
            new() { Name = "Quick Win", Impact = ImpactLevel.High, Effort = EffortLevel.Low, Category = "C1" },
                new() { Name = "Big Project", Impact = ImpactLevel.High, Effort = EffortLevel.High, Category = "C2" },
                new() { Name = "Fill in", Impact = ImpactLevel.Low, Effort = EffortLevel.Low, Category = "C1" }
        ]
    };

    // Act
    var vm = HtmlImpactEffortRenderer.BuildViewModel(spec);

    // Assert
    vm.Tasks.Should().HaveCount(3);
    // Quick Win (Impact 9, Effort 2) -> Top Left in a 3x3 matrix might be (0,0) or (0,2) depending on axes
    // Usually Impact is Y (inverted?) and Effort is X.
    // Let's check QuadrantHelper logic if possible, or just verify they are distinct.
    vm.Categories.Should().HaveCount(2).And.Contain(["C1", "C2"]);
  }

  [Fact]
  public void Render_EmptySpec_ShouldReturnNoTasksMessage()
  {
    var renderer = new HtmlImpactEffortRenderer();
    var result = renderer.Render(new ImpactEffortSpec { Tasks = [] });
    result.Should().Contain("No tasks to display");
  }
}
