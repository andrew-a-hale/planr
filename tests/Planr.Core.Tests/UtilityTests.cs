using FluentAssertions;
using Planr.Core.Configuration;
using Planr.Core.ViewModels.Spider;
using Xunit;

namespace Planr.Core.Tests;

public class UtilityTests
{
    [Fact]
    public void Colors_GetSeriesColor_ShouldCycleAndHandleNegative()
    {
        // Cycle
        var first = Colors.GetSeriesColor(0);
        var overflow = Colors.GetSeriesColor(Colors.SeriesPalette.Length);
        overflow.Should().Be(first);

        // Negative
        Colors.GetSeriesColor(-1).Should().Be(Colors.LightGray);
    }

    [Fact]
    public void ImpactEffortTheme_GetCategoryColor_ShouldCycle()
    {
        var categories = new List<string> { "C1", "C2", "C3" };
        
        var color1 = ImpactEffortTheme.GetCategoryColor("C1", categories);
        var color2 = ImpactEffortTheme.GetCategoryColor("C2", categories);
        
        color1.Should().NotBe(color2);
        
        // Overflow
        var manyCategories = Enumerable.Range(0, 20).Select(i => $"C{i}").ToList();
        var firstColor = ImpactEffortTheme.GetCategoryColor("C0", manyCategories);
        var overflowColor = ImpactEffortTheme.GetCategoryColor($"C{ImpactEffortTheme.CategoryColors.Count}", manyCategories);
        
        overflowColor.Should().Be(firstColor);
    }

    [Fact]
    public void SpiderChartHelper_TruncateLabel_ShouldWork()
    {
        SpiderChartHelper.TruncateLabel("Short").Should().Be("Short");
        
        var longLabel = new string('A', 30);
        var truncated = SpiderChartHelper.TruncateLabel(longLabel);
        
        truncated.Should().HaveLength(SpiderChartHelper.MaxLabelLength);
        truncated.Should().EndWith("...");
    }

    [Theory]
    [InlineData(null, 0.0)]
    [InlineData(-2.0, 0.0)] // Below center
    [InlineData(5.0, 100.0)] // Above max
    [InlineData(1.5, 50.0)] // Mid point (-1 to 4 range)
    public void SpiderChartHelper_ValueToPercent_ShouldCalculateCorrectly(double? value, double expected)
    {
        SpiderChartHelper.ValueToPercent(value).Should().Be(expected);
    }
}
