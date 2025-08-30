using System.IO;
using System.Reflection;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

using Nice3point.Revit.Toolkit.External;

using RevitTestRunner.ViewModels;
using RevitTestRunner.Views;

using Xunit;
using Xunit.Runner.Common;
using Xunit.Sdk;

namespace RevitTestRunner.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    private RevitTestRunnerViewModel viewModel = new();

    public override void Execute()
    {
        // Result = ActualExecute();

        DoSomethingElse();

        var view = new RevitTestRunnerView(viewModel);
        view.ShowDialog();
    }

    private void DoSomethingElse()
    {
        var project = new XunitProject();

        project.Add(new XunitProjectAssembly(
            project,
            @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\RevitTestRunner.xunitv3.Tests\bin\Debug\net8.0\RevitTestRunner.xunitv3.Tests.dll",
            new AssemblyMetadata(3, ".netcoreapp")));

        var assembly = project.Assemblies.First();

        var controller = XunitFrontController.Create(assembly)
                         ?? throw new ArgumentException("not an xUnit.net test assembly: {0}",
                             assembly.AssemblyFileName);

        var settings = new FrontControllerFindAndRunSettings(
            TestFrameworkOptions.ForDiscovery(assembly.Configuration),
            TestFrameworkOptions.ForExecution(assembly.Configuration),
            assembly.Configuration.Filters);

        controller.FindAndRun(new CustomSink(viewModel), settings);
    }

    private Result ActualExecute()
    {
        try
        {
            const string dllPath =
                @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\RevitTestRunner.Tests\bin\Debug\net8.0\RevitTestRunner.Tests.dll";

            if (!File.Exists(dllPath))
            {
                TaskDialog.Show("Test Harness", "Test DLL not found.");
                return Result.Failed;
            }

            var testAssembly = Assembly.LoadFrom(dllPath);
            // TODO: Load real test assembly instead
            var testMethods = testAssembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(FactAttribute), false).Length != 0
                            || m.GetCustomAttributes(typeof(TheoryAttribute), false).Length != 0);

            var passed = 0;
            var failed = 0;

            foreach (var method in testMethods)
            {
                try
                {
                    var instance = method.IsStatic ? null : Activator.CreateInstance(method.DeclaringType);
                    method.Invoke(instance, null);
                    passed++;
                }
                catch (Exception ex)
                {
                    failed++;
                    TaskDialog.Show("Test Failed", $"{method.Name}: {ex.InnerException?.Message ?? ex.Message}");
                }
            }

            TaskDialog.Show("Test Results", $"{passed} passed, {failed} failed");
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Harness Error", ex.ToString());
            return Result.Failed;
        }
    }

    private class CustomSink(
        RevitTestRunnerViewModel viewModel) : IMessageSink, IRunnerLogger
    {
        public bool OnMessage(IMessageSinkMessage message)
        {
            viewModel.Messages.Add(message.ToString()!);

            return true;
        }

        public void LogMessage(StackFrameInfo stackFrame, string message)
            => viewModel.Messages.Add(message);

        public void LogImportantMessage(StackFrameInfo stackFrame, string message)
            => viewModel.Messages.Add(message);

        public void LogWarning(StackFrameInfo stackFrame, string message)
            => viewModel.Messages.Add(message);

        public void LogError(StackFrameInfo stackFrame, string message)
            => viewModel.Messages.Add(message);

        public void LogRaw(string message)
            => viewModel.Messages.Add(message);

        public void WaitForAcknowledgment()
        {
        }

        public object LockObject { get; }
    }
}