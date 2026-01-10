using Planr.Core.Configuration;
using Planr.Core.Models.Spider;
using Planr.Core.ViewModels.Spider;
using Scriban;

namespace Planr.Core.Renderers;

public static class HtmlSpiderRenderer
{
    public static string Render(Models.Spider.SpiderSpec spec, bool partial = false)
    {
        if (spec.Items == null || spec.Items.Count == 0)
        {
            return "<html><body><h1>No items to display</h1></body></html>";
        }

        SpiderViewModel viewModel;
        try
        {
            viewModel = BuildViewModel(spec);
        }
        catch (InvalidOperationException ex)
        {
            var errorCssContent = GetTemplate("core-charts.css");
            return $@"
 <style>{{ errorCssContent }}</style>
 <div class=""spider-chart"">
   <h1>{{ Config.Title }}</h1>
   <div class=""alert alert-danger"">
     <strong>Error:</strong> {ex.Message}
   </div>
 </div>";
        }

        var cssContent = GetTemplate("core-charts.css");

        var partialContent = global::Scriban
            .Template.Parse(GetTemplate("spiderPartial.html"))
            .Render(
                new
                {
                    Config = viewModel.Config,
                    Css = cssContent,
                    ViewModel = viewModel,
                },
                member => member.Name
            );

        if (partial)
        {
            return partialContent;
        }

        var layoutTemplate = global::Scriban.Template.Parse(GetTemplate("spider.html"));
        return layoutTemplate.Render(
            new { body = partialContent, Css = cssContent },
            member => member.Name
        );
    }

    private static SpiderViewModel BuildViewModel(Models.Spider.SpiderSpec spec)
    {
        var vm = new SpiderViewModel
        {
            Config = spec.Config ?? new Models.Spider.SpiderConfig(),
            CenterValue = SpiderChartHelper.CenterValue,
            MaxValue = SpiderChartHelper.MaxValue,
            Items = spec.Items ?? new List<SpiderItem>(),
        };

        SpiderChartHelper.ValidateSpec(spec);

        var dimensions = vm.Items.Select(i => i.Category).Distinct().ToList();
        double angleStep = 360.0 / dimensions.Count;

        for (int i = 0; i < dimensions.Count; i++)
        {
            double angle = i * angleStep;
            double angleRadians = angle * Math.PI / 180;
            double angleRadiansAdjusted = (angle - 90) * Math.PI / 180;
            double outerRadius = 45;
            double labelRadius = 54;

            var dimensionName = dimensions[i];

            vm.Dimensions.Add(new SpiderDimension
            {
                Name = dimensionName,
                TruncatedName = SpiderChartHelper.TruncateLabel(dimensionName),
                Angle = angle,
                AngleRadians = angleRadians,
                AngleRadiansAdjusted = angleRadiansAdjusted,
                OuterX = 50 + outerRadius * Math.Cos(angleRadiansAdjusted),
                OuterY = 50 + outerRadius * Math.Sin(angleRadiansAdjusted),
                LabelX = 50 + labelRadius * Math.Cos(angleRadiansAdjusted),
                LabelY = 50 + labelRadius * Math.Sin(angleRadiansAdjusted),
            });
        }

        var seriesList = SpiderTheme.GetSeriesColors(vm.Config);

        foreach (var (seriesName, color) in seriesList)
        {
            var series = new SpiderSeries
            {
                Name = seriesName,
                Color = color,
                Points = new List<SpiderPoint>(),
            };

            foreach (var dimension in vm.Dimensions)
            {
                var item = vm.Items.FirstOrDefault(i => i.Category == dimension.Name);
                if (item == null)
                    continue;

                double? value = item.SeriesValues.TryGetValue(seriesName, out var val) ? val : null;

                var percent = SpiderChartHelper.ValueToPercent(value);
                var (x, y) = SpiderChartHelper.PercentToCoordinates(percent, dimension.Angle);

                if (value != null)
                {
                    series.Points.Add(new SpiderPoint
                    {
                        X = x,
                        Y = y,
                        Value = value,
                        DimensionName = dimension.Name,
                    });
                }
            }

            if (series.Points.Count > 0)
            {
                vm.Series.Add(series);
            }
        }

        return vm;
    }

    private static string GetTemplate(string name)
    {
        var assembly = typeof(HtmlSpiderRenderer).Assembly;
        var resourceName = $"Planr.Core.Templates.{name}";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            var resources = string.Join(", ", assembly.GetManifestResourceNames());
            throw new FileNotFoundException(
                $"Template '{name}' not found. Searched for '{resourceName}'. Available: {resources}"
            );
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
