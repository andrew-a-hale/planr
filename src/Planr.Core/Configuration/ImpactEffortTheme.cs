namespace Planr.Core.Configuration;

public static class ImpactEffortTheme
{
    // Quadrant colors
    public static Dictionary<string, string> QuadrantColors { get; set; } =
        new()
        {
            { "Quick Wins", "#2ecc71" }, // Green
            { "Major Projects", "#e67e22" }, // Orange
            { "Fill-ins", "#3498db" }, // Blue
            { "Thankless Tasks", "#e74c3c" }, // Red
        };

    // Category colors - cycle through these for different categories
    public static List<string> CategoryColors { get; set; } =
        new()
        {
            "#9b59b6", // Purple
            "#1abc9c", // Turquoise
            "#f39c12", // Yellow
            "#34495e", // Dark Gray
            "#e91e63", // Pink
            "#795548", // Brown
            "#607d8b", // Blue Gray
            "#ff5722", // Deep Orange
        };

    public static string GetQuadrantColor(string quadrantName)
    {
        return QuadrantColors.TryGetValue(quadrantName, out var color) ? color : "#95a5a6";
    }

    public static string GetCategoryColor(string category, List<string> allCategories)
    {
        if (string.IsNullOrEmpty(category) || allCategories.Count == 0)
            return "#95a5a6";

        int index = allCategories.IndexOf(category);
        if (index < 0)
            return "#95a5a6";

        return CategoryColors[index % CategoryColors.Count];
    }
}

