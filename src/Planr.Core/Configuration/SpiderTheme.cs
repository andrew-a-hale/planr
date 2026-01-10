using Planr.Core.Models.Spider;

namespace Planr.Core.Configuration;

public static class SpiderTheme
{
    public static List<(string Name, string Color)> GetSeriesColors(SpiderConfig config)
    {
        var seriesNames = config.SeriesNames ?? new List<string> { "First", "Second", "Third" };
        return seriesNames.Select((name, index) => (name, Colors.GetSeriesColor(index))).ToList();
    }
}
