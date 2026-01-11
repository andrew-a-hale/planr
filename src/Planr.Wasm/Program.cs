using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Planr.Core.Models.Gantt;
using Planr.Core.Models.ImpactEffort;
using Planr.Core.Models.Spider;
using Planr.Core.Renderers;
using Planr.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
  BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

builder.Services.AddScoped<IChartRenderer<GanttSpec>, HtmlGanttRenderer>();
builder.Services.AddScoped<IChartRenderer<ImpactEffortSpec>, HtmlImpactEffortRenderer>();
builder.Services.AddScoped<IChartRenderer<SpiderSpec>, HtmlSpiderRenderer>();

await builder.Build().RunAsync();
