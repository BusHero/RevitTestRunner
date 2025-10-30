using System.IO.Pipes;

namespace NamedPipeServer;

public class Server(
    string pipeName,
    ITestCommandHandler testCommandHandler)
{
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await using var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut);

                Console.WriteLine("Waiting for connection...");
                await pipeServer.WaitForConnectionAsync(cancellationToken);
                Console.WriteLine("Connection established.");

                await using var writer = new StreamWriter(pipeServer, leaveOpen: true);
                using var reader = new StreamReader(pipeServer, leaveOpen: true);
                writer.AutoFlush = true;

                await writer.WriteLineAsync("PING");

                while (!cancellationToken.IsCancellationRequested)
                {
                    var line = await reader.ReadLineAsync(cancellationToken);
                    testCommandHandler.Handle(line!);
                    await writer.WriteLineAsync("PING");
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Connection closed.");
            }
        }
    }
}