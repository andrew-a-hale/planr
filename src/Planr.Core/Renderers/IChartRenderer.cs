namespace Planr.Core.Renderers;

public interface IChartRenderer<TSpec>
{
    string Render(TSpec spec, bool partial = false);
}
