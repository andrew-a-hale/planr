using CsvHelper;
using CsvHelper.Configuration;
using Planr.Core.Models.Gantt;
using Planr.Core.Models.ImpactEffort;
using Planr.Core.Models.Spider;
using System.Globalization;
using System.Text.Json;

namespace Planr.Core.Services;

public class ChartPersistenceService : IChartPersistenceService
{
  private readonly CsvConfiguration _csvConfig = new(CultureInfo.InvariantCulture)
  {
    HasHeaderRecord = true,
    PrepareHeaderForMatch = args => args.Header.ToLower(),
  };

  private readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    WriteIndented = true
  };

  // JSON Implementation
  public string ToJson<TSpec>(TSpec spec) => JsonSerializer.Serialize(spec, _jsonOptions);

  public TSpec? FromJson<TSpec>(string json) => JsonSerializer.Deserialize<TSpec>(json, _jsonOptions);

  // CSV Implementation
  public string ExportGantt(List<GanttTask> tasks)
  {
    using var writer = new StringWriter();
    using var csv = new CsvWriter(writer, _csvConfig);
    csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = ["yyyy-MM-dd"];
    csv.Context.TypeConverterOptionsCache.GetOptions<DateOnly>().Formats = ["yyyy-MM-dd"];
    csv.WriteRecords(tasks);
    return writer.ToString();
  }

  public List<GanttTask> ImportGantt(string csvContent)
  {
    using var reader = new StringReader(csvContent);
    using var csv = new CsvReader(reader, _csvConfig);
    csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = ["yyyy-MM-dd"];
    csv.Context.TypeConverterOptionsCache.GetOptions<DateOnly>().Formats = ["yyyy-MM-dd"];
    return [.. csv.GetRecords<GanttTask>()];
  }

  public string ExportImpactEffort(List<ImpactEffortTask> tasks)
  {
    using var writer = new StringWriter();
    using var csv = new CsvWriter(writer, _csvConfig);
    csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = ["yyyy-MM-dd"];
    csv.Context.TypeConverterOptionsCache.GetOptions<DateOnly>().Formats = ["yyyy-MM-dd"];
    csv.WriteRecords(tasks);
    return writer.ToString();
  }

  public List<ImpactEffortTask> ImportImpactEffort(string csvContent)
  {
    using var reader = new StringReader(csvContent);
    using var csv = new CsvReader(reader, _csvConfig);
    csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = ["yyyy-MM-dd"];
    csv.Context.TypeConverterOptionsCache.GetOptions<DateOnly>().Formats = ["yyyy-MM-dd"];
    return [.. csv.GetRecords<ImpactEffortTask>()];
  }

  public string ExportSpider(SpiderConfig config, List<SpiderItem> items)
  {
    using var writer = new StringWriter();
    using var csv = new CsvWriter(writer, _csvConfig);

    // Write Header
    csv.WriteField("Category");
    foreach (var seriesName in config.SeriesNames)
    {
      csv.WriteField(seriesName);
    }
    csv.NextRecord();

    // Write Rows
    foreach (var item in items)
    {
      csv.WriteField(item.Category);
      foreach (var seriesName in config.SeriesNames)
      {
        csv.WriteField(item.SeriesValues.TryGetValue(seriesName, out var val) ? val : null);
      }
      csv.NextRecord();
    }

    return writer.ToString();
  }

  public (SpiderConfig Config, List<SpiderItem> Items) ImportSpider(string csvContent)
  {
    using var reader = new StringReader(csvContent);
    using var csv = new CsvReader(reader, _csvConfig);

    csv.Read();
    csv.ReadHeader();
    var headers = csv.HeaderRecord;

    if (headers == null || headers.Length < 1)
      throw new Exception("Invalid CSV header");

    var seriesNames = headers.Skip(1).ToList();
    var config = new SpiderConfig { SeriesNames = seriesNames };
    var items = new List<SpiderItem>();

    while (csv.Read())
    {
      var item = new SpiderItem
      {
        Category = csv.GetField(0) ?? "Unknown",
        SeriesValues = []
      };

      for (int i = 1; i < headers.Length; i++)
      {
        var seriesName = headers[i];
        if (csv.TryGetField<double?>(i, out var value))
        {
          item.SeriesValues[seriesName] = value;
        }
        else
        {
          item.SeriesValues[seriesName] = null;
        }
      }
      items.Add(item);
    }

    return (config, items);
  }
}
