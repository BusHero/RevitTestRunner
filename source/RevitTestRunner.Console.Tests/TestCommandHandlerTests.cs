using CliWrap;

using NamedPipeServer;

using Shouldly;

namespace RevitTestRunner.Console.Tests;

public class TestCommandHandlerTests
{
    private const string CONFIGURATION = "net8.0";

    [Fact]
    public async Task TestPassingAssembly()
    {
        var assemblyFileName = await BuildProject("PassingTestAssembly", CONFIGURATION);

        var handler = new TestCommandHandler();

        var result = handler.Handle(assemblyFileName);

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task TestFailingAssembly()
    {
        var assemblyFileName = await BuildProject("FailingTestAssembly", CONFIGURATION);

        var handler = new TestCommandHandler();

        var result = handler.Handle(assemblyFileName);

        result.ShouldBeFalse();
    }

    private static async Task<string> BuildProject(string projectName, string configuration)
    {
        var workingDirPath = Directory.GetParent(AppContext.BaseDirectory)!
            .Parent!
            .Parent!
            .Parent!
            .Parent!
            .FullName;

        await Cli.Wrap("dotnet")
            .WithArguments(x => x
                .Add(["build", projectName])
                .Add(["--configuration", configuration]))
            .WithWorkingDirectory("")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(System.Console.WriteLine))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(System.Console.WriteLine))
            .WithWorkingDirectory(workingDirPath)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        return Path.Combine(
            workingDirPath,
            projectName,
            "bin",
            "Release",
            configuration,
            $"{projectName}.dll");
    }
}