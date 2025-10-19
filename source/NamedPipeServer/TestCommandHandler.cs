using Xunit;
using Xunit.Runner.Common;

namespace NamedPipeServer;

public class TestCommandHandler : ITestCommandHandler
{
    public bool Handle(string path)
    {
        return RunXunitTests(path);
    }

    private static bool RunXunitTests(string assemblyFileName)
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

        controller.FindAndRun(
            testExecutionSink,
            new FrontControllerFindAndRunSettings(
                TestFrameworkOptions.ForDiscovery(assembly.Configuration),
                TestFrameworkOptions.ForExecution(assembly.Configuration)));

        testExecutionSink.Finished.WaitOne(); // block until tests finish

        return testExecutionSink.Passed;
    }
}