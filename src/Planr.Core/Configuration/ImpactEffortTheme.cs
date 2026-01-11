namespace Planr.Core.Configuration;

public static class ImpactEffortTheme
{
  // Category colors - cycle through these for different categories
  public static List<string> CategoryColors { get; set; } =
    [
      Colors.Purple,
      Colors.Turquoise,
      Colors.Amber,
      Colors.DarkGray,
      Colors.Pink,
      Colors.Brown,
      Colors.BlueGray,
      Colors.DeepOrange,
    ];

  public static string GetCategoryColor(string category, List<string> allCategories)
  {
    if (string.IsNullOrEmpty(category) || allCategories.Count == 0)
      return Colors.LightGray;

    int index = allCategories.IndexOf(category);
    if (index < 0)
      return Colors.LightGray;

    return CategoryColors[index % CategoryColors.Count];
  }
}
