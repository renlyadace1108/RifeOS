namespace RifeOS.SDK.Widgets;

public interface IWidget
{
    string Id { get; }
    string Title { get; }
    int GridSpanCol { get; }
    int GridSpanRow { get; }

    object CreateView();
    void Initialize();
    void Cleanup();
}