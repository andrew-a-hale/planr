using Planr.Core.Models.ImpactEffort;

namespace Planr.Core.ViewModels.ImpactEffort;

public class ImpactEffortViewModel
{
    public List<MatrixQuadrant> Quadrants { get; set; } = new();
    public List<MatrixTask> Tasks { get; set; } = new();
    public ImpactEffortConfig Config { get; set; } = new();
    public List<string> Categories { get; set; } = new();
}

public class MatrixQuadrant
{
    public int Row { get; set; }
    public int Col { get; set; }
    public double LeftPercent { get; set; }
    public double TopPercent { get; set; }
    public List<MatrixTask> Tasks { get; set; } = new();
}

public class MatrixTask
{
    public string Ref { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public ImpactLevel Impact { get; set; }
    public EffortLevel Effort { get; set; }
    public double LeftPercent { get; set; }
    public double TopPercent { get; set; }
    public string CategoryColor { get; set; } = string.Empty;
}

public static class QuadrantHelper
{
    public static (int row, int col) GetQuadrantPosition(ImpactLevel impact, EffortLevel effort)
    {
        // Matrix: 3x3 with (0,0) at top-left
        // Row 0: High Impact, Row 1: Medium Impact, Row 2: Low Impact (inverted for standard quadrant layout)
        // Col 0: Low Effort, Col 1: Medium Effort, Col 2: High Effort

        int row = impact switch
        {
            ImpactLevel.High => 0, // Top row
            ImpactLevel.Medium => 1, // Middle row
            ImpactLevel.Low => 2, // Bottom row
            _ => 1,
        };

        int col = effort switch
        {
            EffortLevel.Low => 0,
            EffortLevel.Medium => 1,
            EffortLevel.High => 2,
            _ => 1,
        };

        return (row, col);
    }

    public static (double left, double top) GetTaskPosition(
        ImpactLevel impact,
        EffortLevel effort,
        int taskIndexInQuadrant = 0,
        int totalTasksInQuadrant = 1
    )
    {
        double quadrantSize = 33.33;
        double halfQuadrantSize = quadrantSize / 2;

        // Base position for each quadrant (center of each 3x3 cell)
        double baseLeft = effort switch
        {
            EffortLevel.Low => 0,
            EffortLevel.Medium => quadrantSize,
            EffortLevel.High => 2 * quadrantSize,
            _ => quadrantSize,
        };

        double baseTop = impact switch
        {
            ImpactLevel.High => 0, // Top row (inverted)
            ImpactLevel.Medium => quadrantSize, // Middle row
            ImpactLevel.Low => 2 * quadrantSize, // Bottom row (inverted)
            _ => quadrantSize,
        };

        // If only one task in quadrant, return center position
        if (totalTasksInQuadrant == 1)
        {
            return (baseLeft + halfQuadrantSize, baseTop + halfQuadrantSize);
        }

        // Distribute tasks within quadrant to prevent overlap
        // Create a grid within each quadrant to position tasks
        const double taskRadius = 2.5; // Task size as percentage (40px / 600px * 100)

        // Calculate available space within quadrant (with padding)
        double colsPerQuadrant = Math.Ceiling(Math.Sqrt(totalTasksInQuadrant));
        double rowsPerQuadrant = Math.Ceiling((double)totalTasksInQuadrant / colsPerQuadrant);

        int row = (taskIndexInQuadrant / (int)colsPerQuadrant) + 1;
        int col = (taskIndexInQuadrant % (int)colsPerQuadrant) + 1;

        double offsetX = col * quadrantSize / (colsPerQuadrant + 1);
        double offsetY = row * quadrantSize / (rowsPerQuadrant + 1);

        double finalLeft = baseLeft + offsetX;
        double finalTop = baseTop + offsetY;

        // Ensure tasks stay within bounds
        finalLeft = Math.Max(taskRadius, Math.Min(100 - taskRadius, finalLeft));
        finalTop = Math.Max(taskRadius, Math.Min(100 - taskRadius, finalTop));

        return (finalLeft, finalTop);
    }
}
