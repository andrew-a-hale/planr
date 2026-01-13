using FluentAssertions;
using Planr.Core.Models.Gantt;
using Planr.Core.Models.ImpactEffort;
using Planr.Core.Models.Spider;
using Planr.Core.Services;

namespace Planr.Core.Tests;

public class PersistenceTests
{
  private readonly ChartPersistenceService _service = new();

  [Fact]
  public void GanttJson_RoundTrip_ShouldMaintainData()
  {
    // Arrange
    var spec = new GanttSpec
    {
      Tasks =
        [
            new() { Project = "P1", Name = "T1", Start = new(2024, 1, 1), End = new(2024, 1, 2), Priority = GanttPriority.High }
        ],
      Config = new GanttConfig { Title = "Gantt Test" }
    };

    // Act
    var json = _service.ToJson(spec);
    var imported = _service.FromJson<GanttSpec>(json);

    // Assert
    imported.Should().NotBeNull();
    imported.Config.Title.Should().Be("Gantt Test");
    imported.Tasks.Should().BeEquivalentTo(spec.Tasks);
  }

  [Fact]
  public void ImpactEffortJson_RoundTrip_ShouldMaintainData()
  {
    // Arrange
    var spec = new ImpactEffortSpec
    {
      Tasks =
        [
            new() { Ref = "01", Name = "T1", Category = "C1", Impact = ImpactLevel.High, Effort = EffortLevel.Low }
        ],
      Config = new ImpactEffortConfig { Title = "IE Test" }
    };

    // Act
    var json = _service.ToJson(spec);
    var imported = _service.FromJson<ImpactEffortSpec>(json);

    // Assert
    imported.Should().NotBeNull();
    imported.Config.Title.Should().Be("IE Test");
    imported.Tasks.Should().BeEquivalentTo(spec.Tasks);
  }

  [Fact]
  public void SpiderJson_RoundTrip_ShouldMaintainData()
  {
    // Arrange
    var spec = new SpiderSpec
    {
      Items =
        [
            new() { Category = "C1", SeriesValues = new() { { "S1", 1.5 }, { "S2", null } } }
        ],
      Config = new SpiderConfig { Title = "Spider Test", SeriesNames = ["S1", "S2"] }
    };

    // Act
    var json = _service.ToJson(spec);
    var imported = _service.FromJson<SpiderSpec>(json);

    // Assert
    imported.Should().NotBeNull();
    imported.Config.Title.Should().Be("Spider Test");
    imported.Items.Should().BeEquivalentTo(spec.Items);
  }

  [Fact]
  public void GanttCsv_RoundTrip_ShouldMaintainData()
  {
    // Arrange
    var tasks = new List<GanttTask>
        {
            new()
            {
                Project = "Project Alpha",
                Name = "Task 1",
                Start = new DateOnly(2024, 1, 1),
                End = new DateOnly(2024, 1, 10),
                Priority = GanttPriority.High
            }
        };

    // Act
    var csv = _service.ExportGantt(tasks);
    var imported = _service.ImportGantt(csv);

    // Assert
    imported.Should().BeEquivalentTo(tasks);
  }

  [Fact]
  public void ImpactEffortCsv_RoundTrip_ShouldMaintainData()
  {
    // Arrange
    var tasks = new List<ImpactEffortTask>
        {
            new() { Ref = "01", Name = "T1", Category = "C1", Impact = ImpactLevel.High, Effort = EffortLevel.Low }
        };

    // Act
    var csv = _service.ExportImpactEffort(tasks);
    var imported = _service.ImportImpactEffort(csv);

    // Assert
    imported.Should().BeEquivalentTo(tasks);
  }

  [Fact]
  public void SpiderCsv_RoundTrip_ShouldMaintainData()
  {
    // Arrange
    var config = new SpiderConfig { SeriesNames = ["S1", "S2"] };
    var items = new List<SpiderItem>
        {
            new() { Category = "C1", SeriesValues = new() { { "S1", 1.5 }, { "S2", 2.0 } } }
        };

    // Act
    var csv = _service.ExportSpider(config, items);
    var (Config, Items) = _service.ImportSpider(csv);

    // Assert
    Config.SeriesNames.Should().BeEquivalentTo(config.SeriesNames);
    Items.Should().BeEquivalentTo(items);
  }

  [Fact]
  public void FromJson_MalformedJson_ShouldThrow()
  {
    Action act = () => _service.FromJson<GanttSpec>("not json");
    act.Should().Throw<System.Text.Json.JsonException>();
  }

  [Fact]
  public void ImportGantt_MalformedCsv_ShouldThrow()
  {
    Action act = () => _service.ImportGantt("invalid,csv,data\n1,2,3");
    act.Should().Throw<CsvHelper.HeaderValidationException>();
  }
}
