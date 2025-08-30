using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using RevitTestRunner.ViewModels;
using RevitTestRunner.Views;

namespace RevitTestRunner.Commands;

/// <summary>
///     External command entry point
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new RevitTestRunnerViewModel();
        var view = new RevitTestRunnerView(viewModel);
        view.ShowDialog();
    }
}