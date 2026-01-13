using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Planr.Core.Models.ImpactEffort;
using Planr.Core.Renderers;
using Planr.Core.Services;
using Planr.Wasm.Pages;

namespace Planr.Wasm.Tests;

public class ImpactEffortChartTests : BunitContext
{
  [Fact]
  public void ImpactEffortChart_ShouldRenderDefaultTasks()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<ImpactEffortSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);

    // Act
    var cut = Render<ImpactEffortChart>();

    // Assert
    cut.Find("h1").TextContent.Should().Be("Impact-Effort Matrix");
    // Check for table rows in the task editor (4 default tasks)
    cut.FindAll("table tbody tr").Should().HaveCount(4);
  }

  [Fact]
  public void AddTask_ShouldIncreaseTableRowCount()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<ImpactEffortSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);
    var cut = Render<ImpactEffortChart>();

    // Act
    var addButton = cut.Find("button.btn-primary");
    addButton.Click();

    // Assert
    cut.FindAll("table tbody tr").Should().HaveCount(5);
  }

  [Fact]
  public void ExportJson_ShouldInvokeDownload()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<ImpactEffortSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    persistence.ToJson(Arg.Any<ImpactEffortSpec>()).Returns("{\"test\":true}");

    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);

    var downloadModule = JSInterop.SetupVoid("downloadFile", "impact-effort-spec.json", "{\"test\":true}");
    var cut = Render<ImpactEffortChart>();

    // Act
    var exportButton = cut.FindAll("button").First(b => b.TextContent.Contains("Export JSON"));
    exportButton.Click();

    // Assert
    downloadModule.VerifyInvoke("downloadFile");
  }

  [Fact]
  public void ExportCsv_ShouldInvokeDownload()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<ImpactEffortSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    persistence.ExportImpactEffort(Arg.Any<List<ImpactEffortTask>>()).Returns("csv,data");

    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);

    var downloadModule = JSInterop.SetupVoid("downloadFile", "planr-impact-effort.csv", "csv,data", "text/csv");
    var cut = Render<ImpactEffortChart>();

    // Act
    var exportButton = cut.FindAll("button").First(b => b.TextContent.Contains("Export CSV"));
    exportButton.Click();

    // Assert
    downloadModule.VerifyInvoke("downloadFile");
  }
}
