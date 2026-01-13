using Planr.Core.Models.Gantt;
using Planr.Core.Models.ImpactEffort;
using Planr.Core.Models.Spider;

namespace Planr.Core.Services;

public interface IChartPersistenceService
{
  // JSON
  string ToJson<TSpec>(TSpec spec);
  TSpec? FromJson<TSpec>(string json);

  // CSV
  string ExportGantt(List<GanttTask> tasks);
  List<GanttTask> ImportGantt(string csvContent);

  string ExportImpactEffort(List<ImpactEffortTask> tasks);
  List<ImpactEffortTask> ImportImpactEffort(string csvContent);

  string ExportSpider(SpiderConfig config, List<SpiderItem> items);
  (SpiderConfig Config, List<SpiderItem> Items) ImportSpider(string csvContent);
}
