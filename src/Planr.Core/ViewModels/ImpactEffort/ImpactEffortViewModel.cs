using Planr.Core.Models.ImpactEffort;

namespace Planr.Core.ViewModels.ImpactEffort;

public class ImpactEffortViewModel
{
    public List<GridQuadrant> Quadrants { get; set; } = new();
    public List<GridTask> Tasks { get; set; } = new();
    public List<LegendItem> Legend { get; set; } = new();
    public ImpactEffortConfig Config { get; set; } = new();
    public List<string> Categories { get; set; } = new();
}

public class GridQuadrant
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Col { get; set; }
    public double LeftPercent { get; set; }
    public double TopPercent { get; set; }
    public List<GridTask> Tasks { get; set; } = new();
}

public class GridTask
{
    public string Ref { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public ImpactLevel Impact { get; set; }
    public EffortLevel Effort { get; set; }
    public double LeftPercent { get; set; }
    public double TopPercent { get; set; }
    public string QuadrantColor { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
}

public class LegendItem
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "quadrant" or "category"
}

public static class QuadrantHelper
{
    public static (int row, int col) GetQuadrantPosition(ImpactLevel impact, EffortLevel effort)
    {
        // Grid: 3x3 with (0,0) at top-left
        // Row 0: Low Impact, Row 1: Medium Impact, Row 2: High Impact
        // Col 0: Low Effort, Col 1: Medium Effort, Col 2: High Effort
        
        int row = impact switch
        {
            ImpactLevel.Low => 0,
            ImpactLevel.Medium => 1,
            ImpactLevel.High => 2,
            _ => 1
        };
        
        int col = effort switch
        {
            EffortLevel.Low => 0,
            EffortLevel.Medium => 1,
            EffortLevel.High => 2,
            _ => 1
        };
        
        return (row, col);
    }

    public static string GetQuadrantName(int row, int col)
    {
        return (row, col) switch
        {
            (0, 0) => "Low Priority",
            (0, 1) => "Low Priority", 
            (0, 2) => "Thankless Tasks",
            (1, 0) => "Fill-ins",
            (1, 1) => "Thankless Tasks",
            (1, 2) => "Thankless Tasks",
            (2, 0) => "Quick Wins",
            (2, 1) => "Major Projects",
            (2, 2) => "Major Projects",
            _ => "Unknown"
        };
    }

    public static string GetQuadrantName(ImpactLevel impact, EffortLevel effort)
    {
        if (impact == ImpactLevel.High && effort == EffortLevel.Low)
            return "Quick Wins";
        if (impact == ImpactLevel.High && effort == EffortLevel.Medium)
            return "Major Projects";
        if (impact == ImpactLevel.High && effort == EffortLevel.High)
            return "Major Projects";
        if (impact == ImpactLevel.Medium && effort == EffortLevel.Low)
            return "Fill-ins";
        if (impact == ImpactLevel.Medium && effort == EffortLevel.Medium)
            return "Thankless Tasks";
        if (impact == ImpactLevel.Medium && effort == EffortLevel.High)
            return "Thankless Tasks";
        if (impact == ImpactLevel.Low && effort == EffortLevel.Low)
            return "Low Priority";
        if (impact == ImpactLevel.Low && effort == EffortLevel.Medium)
            return "Low Priority";
        if (impact == ImpactLevel.Low && effort == EffortLevel.High)
            return "Thankless Tasks";
            
        return "Unknown";
    }

    public static (double left, double top) GetTaskPosition(ImpactLevel impact, EffortLevel effort)
    {
        // Position task in center of each 3x3 cell
        // Each cell is 33.33% of container
        // Add 16.67% (half cell) to center within the cell
        // Add small random offset to avoid overlap
        
        double left = effort switch
        {
            EffortLevel.Low => 16.67,
            EffortLevel.Medium => 50.00,
            EffortLevel.High => 83.33,
            _ => 50.00
        };
        
        double top = impact switch
        {
            ImpactLevel.Low => 16.67,
            ImpactLevel.Medium => 50.00,
            ImpactLevel.High => 83.33,
            _ => 50.00
        };
        
        return (left, top);
    }
}