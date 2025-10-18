using System.IO.Pipes;

namespace NamedPipeServer;

public class Server(
    string pipeName,
    ITestCommandHandler testCommandHandler) : IAsyncDisposable, IDisposable
{
    private readonly NamedPipeServerStream _pipeServer = new(pipeName, PipeDirection.InOut);

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _pipeServer.WaitForConnectionAsync(cancellationToken);

        await using var writer = new StreamWriter(_pipeServer, leaveOpen: true);
        using var reader = new StreamReader(_pipeServer, leaveOpen: true);
        writer.AutoFlush = true;

        await writer.WriteLineAsync("PING");

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            testCommandHandler.Handle(line!);
            await writer.WriteLineAsync("PING");
        }
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _pipeServer.DisposeAsync();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _pipeServer.Dispose();
    }
}