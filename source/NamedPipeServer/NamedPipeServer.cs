using System.IO.Pipes;

namespace NamedPipeServer;

public class NamedPipeServer
{
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await using var pipeServer = new NamedPipeServerStream(
            "testpipe",
            PipeDirection.InOut);

        await pipeServer.WaitForConnectionAsync(cancellationToken);
    }
}