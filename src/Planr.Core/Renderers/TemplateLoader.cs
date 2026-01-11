using System.Reflection;

namespace Planr.Core.Renderers;

public static class TemplateLoader
{
  public static string LoadTemplate(string name)
  {
    var assembly = Assembly.GetExecutingAssembly();
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
