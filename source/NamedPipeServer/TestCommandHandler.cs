using Xunit;
using Xunit.Runner.Common;
using Xunit.Runner.InProc.SystemConsole;
using Xunit.Runner.v3;
using Xunit.v3;

namespace NamedPipeServer;

public class TestCommandHandler : ITestCommandHandler
{
    public bool Handle(string path)
    {
        return RunXunitTests(path);
    }

    private static bool RunXunitTests(string assemblyFileName)
    {
        AppDomain.CurrentDomain.Load(typeof(ConsoleRunnerInProcess).Assembly.GetName());

        var project = new XunitProject();

        project.Add(new XunitProjectAssembly(
            project,
            assemblyFileName,
            new AssemblyMetadata(3, ".netcoreapp")));

        var assembly = project.Assemblies.First();

        var controller2 = Xunit3.ForDiscoveryAndExecution(
            assembly,
            testProcessLauncher: InProcessTestProcessLauncher.Instance);

        using var testExecutionSink = new TestExecutionSink();

        controller2.FindAndRun(
            testExecutionSink,
            new FrontControllerFindAndRunSettings(
                TestFrameworkOptions.ForDiscovery(assembly.Configuration),
                TestFrameworkOptions.ForExecution(assembly.Configuration)));

        testExecutionSink.Finished.WaitOne(); // block until tests finish

        return testExecutionSink.Passed;
    }
}