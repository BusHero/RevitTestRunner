using System.IO.Pipes;
using System.Security.Principal;

using AutoFixture.Xunit3;

using NamedPipeServer;

using NSubstitute;

using Shouldly;

namespace RevitTestRunner.Console.Tests;

public class ServerTests
{
    [Theory, AutoData]
    public async Task ClientConnectsToServer(string pipeName)
    {
        var task = StartServerAsync(
            Substitute.For<ITestCommandHandler>(),
            pipeName, 
            TestContext.Current.CancellationToken);

        using var source = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        var pipeClient = await CreateAndConnectNamedPipe(pipeName, source.Token);

        using var reader = new StreamReader(pipeClient, leaveOpen: true);
        var text = await reader.ReadLineAsync(TestContext.Current.CancellationToken);

        text.ShouldBe("PING");
    }

    [Theory, AutoData]
    public async Task ConnectionToTheWrongPipeFails(string pipeName, string wrongPipeName)
    {
        var task = StartServerAsync(
            Substitute.For<ITestCommandHandler>(),
            pipeName, 
            TestContext.Current.CancellationToken);

        using var source = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        await CreateAndConnectNamedPipe(wrongPipeName, source.Token)
            .ShouldThrowAsync<OperationCanceledException>();
    }

    [Theory, AutoData]
    public async Task ServerWaitsForFirstMessageToBeRead(string pipeName)
    {
        var task = StartServerAsync(
            Substitute.For<ITestCommandHandler>(),
            pipeName, 
            TestContext.Current.CancellationToken);

        using var source = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        _ = await CreateAndConnectNamedPipe(pipeName, source.Token);

        await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);

        task.IsCompleted.ShouldBeFalse();
    }

    [Theory, AutoData]
    public async Task ServerWaitsToReceiveCommand(string pipeName)
    {
        var task = StartServerAsync(
            Substitute.For<ITestCommandHandler>(),
            pipeName, 
            TestContext.Current.CancellationToken);

        using var source = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        var pipeClient = await CreateAndConnectNamedPipe(pipeName, source.Token);
        using var reader = new StreamReader(pipeClient, leaveOpen: true);
        await reader.ReadLineAsync(source.Token);

        await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);

        task.IsCompleted.ShouldBeFalse();
    }

    [Theory, AutoData]
    public async Task SendCommandToServer(string pipeName)
    {
        _ = StartServerAsync(
            Substitute.For<ITestCommandHandler>(),
            pipeName, 
            TestContext.Current.CancellationToken);

        using var source = new CancellationTokenSource(
            TimeSpan.FromSeconds(1));

        var pipeClient = await CreateAndConnectNamedPipe(pipeName, source.Token);
        using var reader = new StreamReader(pipeClient, leaveOpen: true);
        await using var writer = new StreamWriter(pipeClient, leaveOpen: true);
        await reader.ReadLineAsync(source.Token);

        using var writeOperationSource = new CancellationTokenSource(
            TimeSpan.FromSeconds(2));
        await writer.WriteLineAsync("PONG");
        await writer.FlushAsync(writeOperationSource.Token);
        var text = await reader.ReadLineAsync(source.Token);
        text.ShouldBe("PING");
    }

    [Theory, AutoData]
    public async Task ProcessTestCommand(
        string pipeName,
        string fileName)
    {
        var testCommandHandler = Substitute.For<ITestCommandHandler>();
        _ = StartServerAsync(
            testCommandHandler,
            pipeName,
            TestContext.Current.CancellationToken);

        using var source = new CancellationTokenSource(
            TimeSpan.FromSeconds(1));

        var pipeClient = await CreateAndConnectNamedPipe(pipeName, source.Token);
        using var reader = new StreamReader(pipeClient, leaveOpen: true);
        await using var writer = new StreamWriter(pipeClient, leaveOpen: true);
        await reader.ReadLineAsync(source.Token);

        using var writeOperationSource = new CancellationTokenSource(
            TimeSpan.FromSeconds(2));
        await writer.WriteLineAsync(fileName);
        await writer.FlushAsync(writeOperationSource.Token);

        testCommandHandler.Received(1).Handle(fileName);
    }

    private static async Task<NamedPipeClientStream> CreateAndConnectNamedPipe(
        string pipeName,
        CancellationToken token)
    {
        var pipeClient = new NamedPipeClientStream(
            ".",
            pipeName,
            PipeDirection.InOut,
            PipeOptions.None,
            TokenImpersonationLevel.Impersonation);

        await pipeClient.ConnectAsync(token);

        return pipeClient;
    }

    private static async Task StartServerAsync(ITestCommandHandler testCommandHandler,
        string pipeName,
        CancellationToken cancellationToken)
    {
        await using var pipeServer = new Server(pipeName, testCommandHandler);
        await pipeServer.StartAsync(cancellationToken);
    }
}