using Autodesk.Revit.Attributes;

using Nice3point.Revit.Toolkit.External;

using RevitTestRunner.ViewModels;
using RevitTestRunner.Views;

using Xunit;
using Xunit.Runner.Common;
using Xunit.v3;

namespace RevitTestRunner.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    private readonly RevitTestRunnerViewModel _viewModel = new();

    private const string ASSEMBLY_FILE_NAME =
        @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\RevitTestRunner.xunitv3.Tests\bin\Debug R25\RevitTestRunner.xunitv3.Tests.dll";

    public override void Execute()
    {
        RunXunitTests();

        var view = new RevitTestRunnerView(_viewModel);
        view.ShowDialog();
    }

    private void RunXunitTests()
    {
        var project = new XunitProject();

        project.Add(new XunitProjectAssembly(
            project,
            ASSEMBLY_FILE_NAME,
            new AssemblyMetadata(3, ".netcoreapp")));

        var assembly = project.Assemblies.First();

        var controller = XunitFrontController.Create(assembly)
                         ?? throw new ArgumentException("not an xUnit.net test assembly: {0}",
                             assembly.AssemblyFileName);

        using var testExecutionSink = new TestExecutionSink();

        controller.FindAndRun(
            testExecutionSink,
            new FrontControllerFindAndRunSettings(
                TestFrameworkOptions.ForDiscovery(assembly.Configuration),
                TestFrameworkOptions.ForExecution(assembly.Configuration)));

        testExecutionSink.Finished.WaitOne(); // block until tests finish

        _viewModel.Messages.AddRange(testExecutionSink.Messages);
    }
}