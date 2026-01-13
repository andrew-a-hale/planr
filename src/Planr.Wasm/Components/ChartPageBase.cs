using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Planr.Core.Configuration;
using Planr.Core.Renderers;
using Planr.Core.Services;

namespace Planr.Wasm.Components;

public abstract class ChartPageBase<TSpec> : ComponentBase where TSpec : class, new()
{
  [Inject] protected IChartRenderer<TSpec> Renderer { get; set; } = default!;
  [Inject] protected IChartPersistenceService PersistenceService { get; set; } = default!;
  [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

  protected TSpec Spec { get; set; } = new();
  protected string? ChartHtml { get; set; }
  protected string? ErrorMessage { get; set; }
  protected bool ShowItemEditor { get; set; } = true;
  protected bool ShowChartSettings { get; set; } = false;
  protected abstract string ChartTitle { get; }
  protected abstract string ChartDescription { get; }
  protected abstract string ExportFileName { get; }
  protected abstract Func<Task> GenerateChartAsync { get; }
  protected virtual void OnInitializedCore() { }
  protected override void OnInitialized()
  {
    InitializeDefaultData();
    OnInitializedCore();
  }

  protected abstract void InitializeDefaultData();
  protected void ToggleItemEditor() => ShowItemEditor = !ShowItemEditor;
  protected void ToggleChartSettings() => ShowChartSettings = !ShowChartSettings;
  protected string GetScreenWidthLabel(Screen.Width width) => Screen.WidthCssOption(width);

  protected async Task LoadFile(InputFileChangeEventArgs e)
  {
    ErrorMessage = null;
    try
    {
      var file = e.File;
      if (file != null)
      {
        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();

        if (file.Name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
          await ImportCsv(content);
        }
        else
        {
          var spec = PersistenceService.FromJson<TSpec>(content);
          if (spec != null)
          {
            Spec = spec;
          }
        }

        OnFileLoaded();
        await GenerateChartAsync();
      }
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Error parsing imported file: {ex.Message}";
    }
  }

  protected virtual Task ImportCsv(string csvContent) => Task.CompletedTask;

  protected virtual async Task ExportCsv() => await Task.CompletedTask;

  protected virtual void OnFileLoaded() { }

  protected async Task GenerateChart()
  {
    ErrorMessage = null;
    try
    {
      await GenerateChartAsync();
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Error generating chart: {ex.Message}";
    }
    ShowItemEditor = false;
  }

  protected async Task AddButtonClick(Func<Task> addAction)
  {
    await addAction();
    StateHasChanged();
  }
}
