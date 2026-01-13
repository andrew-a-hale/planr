using FluentAssertions;
using Planr.Core.Models.Spider;
using Planr.Core.Renderers;

namespace Planr.Core.Tests;

public class SpiderRendererTests
{
  [Fact]
  public void BuildViewModel_ShouldCalculateCorrectAngles()
  {
    // Arrange
    var spec = new SpiderSpec
    {
      Config = new SpiderConfig { SeriesNames = ["Current", "Target"] },
      Items =
            [
                new() { Category = "C1", SeriesValues = new Dictionary<string, double?> { { "Current", 1 }, { "Target", 2 } } },
                new() { Category = "C2", SeriesValues = new Dictionary<string, double?> { { "Current", 2 }, { "Target", 3 } } },
                new() { Category = "C3", SeriesValues = new Dictionary<string, double?> { { "Current", 3 }, { "Target", 4 } } }
            ]
    };

    // Act
    var vm = HtmlSpiderRenderer.BuildViewModel(spec);

    // Assert
    vm.Categories.Should().HaveCount(3);
    vm.Categories[0].Angle.Should().Be(0);
    vm.Categories[1].Angle.Should().Be(120);
    vm.Categories[2].Angle.Should().Be(240);
  }

  [Fact]
  public void BuildViewModel_ShouldThrowIfSeriesNamesMissing()
  {
    // Arrange
    var spec = new SpiderSpec
    {
      Config = new SpiderConfig { SeriesNames = ["OnlyOne"] },
      Items = [new() { Category = "C1" }]
    };

    // Act
    Action act = () => HtmlSpiderRenderer.BuildViewModel(spec);

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("At least 2 series names must be provided in config.");
  }

  [Fact]
  public void BuildViewModel_ShouldHandleNullValuesConsistently()
  {
    // Arrange
    var spec = new SpiderSpec
    {
      Config = new SpiderConfig { SeriesNames = ["S1", "S2"] },
      Items =
            [
                new() { Category = "C1", SeriesValues = new Dictionary<string, double?> { { "S1", 1 }, { "S2", null } } },
                new() { Category = "C2", SeriesValues = new Dictionary<string, double?> { { "S1", 2 }, { "S2", null } } }
            ]
    };

    // Act
    var vm = HtmlSpiderRenderer.BuildViewModel(spec);

    // Assert
    vm.Series.Should().HaveCount(1); // Only S1 should be rendered
    vm.Series[0].Name.Should().Be("S1");
  }

  [Fact]
  public void Render_EmptySpec_ShouldReturnNoItemsMessage()
  {
    var renderer = new HtmlSpiderRenderer();
    var result = renderer.Render(new SpiderSpec { Items = [] });
    result.Should().Contain("No items to display");
  }
}
