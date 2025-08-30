using Nice3point.Revit.Toolkit.External;
using RevitTestRunner.Commands;

namespace RevitTestRunner;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
        CreateRibbon();
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands", "RevitTestRunner");

        panel.AddPushButton<StartupCommand>("Execute")
            .SetImage("/RevitTestRunner;component/Resources/Icons/RibbonIcon16.png")
            .SetLargeImage("/RevitTestRunner;component/Resources/Icons/RibbonIcon32.png");
    }
}