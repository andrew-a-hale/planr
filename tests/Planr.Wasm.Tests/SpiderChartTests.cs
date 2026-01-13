using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Planr.Core.Models.Spider;
using Planr.Core.Renderers;
using Planr.Core.Services;
using Planr.Wasm.Pages;

namespace Planr.Wasm.Tests;

public class SpiderChartTests : BunitContext
{
  [Fact]
  public void SpiderChart_ShouldRenderDefaultItems()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<SpiderSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);

    // Act
    var cut = Render<SpiderChart>();

    // Assert
    cut.Find("h1").TextContent.Should().Be("Spider Chart");
    // Check for table rows in the item editor (5 default dimensions)
    cut.FindAll("table tbody tr").Should().HaveCount(5);
  }

  [Fact]
  public void AddItem_ShouldIncreaseTableRowCount()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<SpiderSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);
    var cut = Render<SpiderChart>();

    // Act
    var addButton = cut.Find("button.btn-primary");
    addButton.Click();

    // Assert
    cut.FindAll("table tbody tr").Should().HaveCount(6);
  }

  [Fact]
  public void AddSeries_ShouldIncreaseColumnCount()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<SpiderSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);
    var cut = Render<SpiderChart>();

    // Act - Open settings first to find the add series button
    var settingsToggles = cut.FindAll("button.btn-link");
    var settingsToggle = settingsToggles.First(b => b.TextContent.Contains("Chart Settings"));
    settingsToggle.Click();

    var addSeriesButton = cut.Find("button.btn-outline-success");
    addSeriesButton.Click();

    // Assert - Headers should increase (Category + 4 series + Action = 6)
    cut.FindAll("table thead th").Should().HaveCount(6);
  }

  [Fact]
  public void ExportJson_ShouldInvokeDownload()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<SpiderSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    persistence.ToJson(Arg.Any<SpiderSpec>()).Returns("{\"test\":true}");

    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);

    var downloadModule = JSInterop.SetupVoid("downloadFile", "spider-spec.json", "{\"test\":true}");
    var cut = Render<SpiderChart>();

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
    var renderer = Substitute.For<IChartRenderer<SpiderSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    persistence.ExportSpider(Arg.Any<SpiderConfig>(), Arg.Any<List<SpiderItem>>()).Returns("csv,data");

    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);

    var downloadModule = JSInterop.SetupVoid("downloadFile", "planr-spider.csv", "csv,data", "text/csv");
    var cut = Render<SpiderChart>();

    // Act
    var exportButton = cut.FindAll("button").First(b => b.TextContent.Contains("Export CSV"));
    exportButton.Click();

    // Assert
    downloadModule.VerifyInvoke("downloadFile");
  }

  [Fact]
  public void RemoveItem_ShouldDecreaseTableRowCount()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<SpiderSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);
    var cut = Render<SpiderChart>();

    // Act
    var removeButton = cut.Find("button.btn-outline-danger");
    removeButton.Click();

    // Assert
    cut.FindAll("table tbody tr").Should().HaveCount(4);
  }

  [Fact]
  public void RenamingSeries_ShouldUpdateAllItemDictionaries()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<SpiderSpec>>();
    var persistence = Substitute.For<IChartPersistenceService>();
    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);
    var cut = Render<SpiderChart>();

    // Open settings
    var settingsToggle = cut.FindAll("button.btn-link").First(b => b.TextContent.Contains("Chart Settings"));
    settingsToggle.Click();

    // Act - Rename "First" to "NewName"
    var firstSeriesInput = cut.FindAll("input.form-control").First(i => i.GetAttribute("value") == "First");
    firstSeriesInput.Change("NewName");
    
    // Re-find to avoid UnknownEventHandlerIdException after re-render
    firstSeriesInput = cut.FindAll("input.form-control").First(i => i.GetAttribute("value") == "NewName");
    firstSeriesInput.Blur(); // Triggers OnSeriesNameChanged

    // Assert
    // Check first item in the table
    var firstRowInputs = cut.FindAll("table tbody tr:first-child input[type='number']");
    firstRowInputs.Should().HaveCount(3);
    
    // We can also verify by clicking Generate and checking the spec passed to renderer
    var generateButton = cut.Find("button.btn-success");
    generateButton.Click();
    
    renderer.Received(1).Render(Arg.Is<SpiderSpec>(s => 
        s.Config.SeriesNames.Contains("NewName") && 
        !s.Config.SeriesNames.Contains("First") &&
        s.Items.All(i => i.SeriesValues.ContainsKey("NewName"))
    ), true);
  }

  [Fact]
  public void GenerateChart_WithError_ShouldDisplayAlert()
  {
    // Arrange
    var renderer = Substitute.For<IChartRenderer<SpiderSpec>>();
    renderer.When(r => r.Render(Arg.Any<SpiderSpec>(), Arg.Any<bool>()))
            .Do(x => throw new InvalidOperationException("Test Error"));
            
    var persistence = Substitute.For<IChartPersistenceService>();
    Services.AddSingleton(renderer);
    Services.AddSingleton(persistence);
    var cut = Render<SpiderChart>();

    // Act
    var generateButton = cut.Find("button.btn-success");
    generateButton.Click();

    // Assert
    cut.WaitForElement("div.alert-danger").TextContent.Should().Contain("Test Error");
  }
}
