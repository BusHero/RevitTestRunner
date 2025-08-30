using Autodesk.Revit.Attributes;

using Nice3point.Revit.Toolkit.External;

using RevitTestRunner.ViewModels;
using RevitTestRunner.Views;

using Xunit;
using Xunit.Runner.Common;

namespace RevitTestRunner.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    private RevitTestRunnerViewModel viewModel = new();

    const string assemblyFileName =
        @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\RevitTestRunner.xunitv3.Tests\bin\Debug\net8.0\RevitTestRunner.xunitv3.Tests.dll";

    public override void Execute()
    {
        RunXunitTests();

        var view = new RevitTestRunnerView(viewModel);
        view.ShowDialog();
    }

    private void RunXunitTests()
    {
        var project = new XunitProject();

        project.Add(new XunitProjectAssembly(
            project,
            assemblyFileName,
            new AssemblyMetadata(3, ".netcoreapp")));

        var assembly = project.Assemblies.First();

        var controller = XunitFrontController.Create(assembly)
                         ?? throw new ArgumentException("not an xUnit.net test assembly: {0}",
                             assembly.AssemblyFileName);

        using var testExecutionSink = new TestExecutionSink();
        controller.Find(
            testExecutionSink,
            new FrontControllerFindSettings(
                TestFrameworkOptions.ForDiscovery(
                    assembly.Configuration)));
        controller.Run(
            testExecutionSink,
            new FrontControllerRunSettings(
                TestFrameworkOptions.ForExecution(assembly.Configuration),
                testExecutionSink.TestNames));

        testExecutionSink.Finished.WaitOne(); // block until tests finish

        viewModel.Messages.AddRange(testExecutionSink.Messages);
    }
}