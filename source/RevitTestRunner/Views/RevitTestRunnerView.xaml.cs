using RevitTestRunner.ViewModels;

namespace RevitTestRunner.Views;

public sealed partial class RevitTestRunnerView
{
    public RevitTestRunnerView(RevitTestRunnerViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}