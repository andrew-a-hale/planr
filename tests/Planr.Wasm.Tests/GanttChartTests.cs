using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Planr.Core.Models.Gantt;
using Planr.Core.Renderers;
using Planr.Core.Services;
using Planr.Wasm.Pages;

namespace Planr.Wasm.Tests;

public class GanttChartTests : BunitContext
{
  [Fact]
  public void GanttChart_ShouldRenderDefaultTasks()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<GanttSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);

    // Act
    var cut = Render<GanttChart>();

    // Assert
    cut.Find("h1").TextContent.Should().Be("Gantt Chart");
    // Check for table rows in the task editor
    cut.FindAll("table tbody tr").Should().HaveCount(2);
  }

  [Fact]
  public void AddTask_ShouldIncreaseTableRowCount()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<GanttSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);
    var cut = Render<GanttChart>();

    // Act
    var addButton = cut.Find("button.btn-primary");
    addButton.Click();

    // Assert
    cut.FindAll("table tbody tr").Should().HaveCount(3);
  }

  [Fact]
  public void GenerateChart_ShouldCallRenderer()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<GanttSpec>>();
    renderer.Render(Arg.Any<GanttSpec>(), Arg.Any<bool>()).Returns("<div id='test-chart'>Chart Content</div>");

    var persistence = Substitute.For<IChartPersistenceService>();
    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);
    var cut = Render<GanttChart>();

    // Act
    var generateButton = cut.Find("button.btn-success");
    generateButton.Click();

    // Assert
    renderer.Received(1).Render(Arg.Any<GanttSpec>(), true);
    cut.WaitForAssertion(() => cut.Markup.Should().Contain("Chart Content"));
  }

  [Fact]
  public void ExportJson_ShouldInvokeDownload()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<GanttSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    persistence.ToJson(Arg.Any<GanttSpec>()).Returns("{\"test\":true}");

    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);

    var downloadModule = JSInterop.SetupVoid("downloadFile", "planr-gantt.json", "{\"test\":true}");
    var cut = Render<GanttChart>();

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
    var renderer = Substitute.For<IChartRenderer<GanttSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    persistence.ExportGantt(Arg.Any<List<GanttTask>>()).Returns("csv,data");

    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);

    var downloadModule = JSInterop.SetupVoid("downloadFile", "planr-gantt.csv", "csv,data", "text/csv");
    var cut = Render<GanttChart>();

    // Act
    var exportButton = cut.FindAll("button").First(b => b.TextContent.Contains("Export CSV"));
    exportButton.Click();

    // Assert
    downloadModule.VerifyInvoke("downloadFile");
  }

  [Fact]
  public void RemoveTask_ShouldDecreaseTableRowCount()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<GanttSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);
    var cut = Render<GanttChart>();

    // Act
    var removeButton = cut.Find("button.btn-outline-danger");
    removeButton.Click();

    // Assert
    cut.FindAll("table tbody tr").Should().HaveCount(1);
  }

  [Fact]
  public void Toggles_ShouldChangeCollapseClasses()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<GanttSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);
    var cut = Render<GanttChart>();

    // Act & Assert Task Editor (Default Open)
    cut.Find("div.card:nth-of-type(1) div.collapse").ClassList.Should().Contain("show");
    cut.FindAll("button.btn-link").First(b => b.TextContent.Contains("Task Editor")).Click();
    cut.Find("div.card:nth-of-type(1) div.collapse").ClassList.Should().NotContain("show");

    // Act & Assert Chart Settings (Default Closed)
    cut.Find("div.card:nth-of-type(2) div.collapse").ClassList.Should().NotContain("show");
    cut.FindAll("button.btn-link").First(b => b.TextContent.Contains("Chart Settings")).Click();
    cut.Find("div.card:nth-of-type(2) div.collapse").ClassList.Should().Contain("show");
  }
}
