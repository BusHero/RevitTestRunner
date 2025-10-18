using System.IO.Pipes;

namespace NamedPipeServer;

public class Server(
    string pipeName)
{
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await using var pipeServer = new NamedPipeServerStream(
            pipeName,
            PipeDirection.InOut);

        await pipeServer.WaitForConnectionAsync(cancellationToken);
    }
}