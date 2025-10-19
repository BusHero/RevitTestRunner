using NamedPipeServer;

using Nice3point.Revit.Toolkit.External;

using RevitTestRunner.Commands;

namespace RevitTestRunner;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
public class Application : ExternalApplication
{
    private readonly CancellationTokenSource _source = new CancellationTokenSource();

    public override void OnStartup()
    {
        Task.Run(async () =>
        {
            var server = new Server(
                "test-pipe",
                new TestCommandHandler());
            await server.StartAsync(_source.Token);
        });
        CreateRibbon();
    }

    public override void OnShutdown()
    {
        base.OnShutdown();
        _source.Cancel();
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands", "RevitTestRunner");

        panel.AddPushButton<StartupCommand>("Execute")
            .SetImage("/RevitTestRunner;component/Resources/Icons/RibbonIcon16.png")
            .SetLargeImage("/RevitTestRunner;component/Resources/Icons/RibbonIcon32.png");
    }
}