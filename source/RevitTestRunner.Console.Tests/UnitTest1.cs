using System.IO.Pipes;
using System.Security.Principal;
using System.Text;

using AutoFixture.Xunit3;

using NamedPipeServer;

using Shouldly;

namespace RevitTestRunner.Console.Tests;

public class UnitTest1
{
    [Theory, AutoData]
    public async Task OpenNamedPipeClient(string pipeName)
    {
        var pipeServer = new Server(pipeName);
        _ = pipeServer.StartAsync(TestContext.Current.CancellationToken);

        var pipeClient = new NamedPipeClientStream(
            ".",
            pipeName,
            PipeDirection.InOut,
            PipeOptions.None,
            TokenImpersonationLevel.Impersonation);
        using var source = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        await pipeClient.ConnectAsync(source.Token);
        //
        // var stream = new StreamString(pipeClient);
        //
        // using var streamReader = new StreamReader(pipeClient);
        // var foo = await streamReader.ReadLineAsync(source.Token);
        // foo.ShouldBe("I am the one true server!");
    }
}

public class StreamString(
    Stream ioStream)
{
    private readonly UnicodeEncoding _streamEncoding = new();

    public string ReadString()
    {
        var len = ioStream.ReadByte() * 256;
        len += ioStream.ReadByte();
        var inBuffer = new byte[len];
        ioStream.ReadExactly(inBuffer, 0, len);

        return _streamEncoding.GetString(inBuffer);
    }

    public int WriteString(string outString)
    {
        var outBuffer = _streamEncoding.GetBytes(outString);
        var len = outBuffer.Length;

        if (len > ushort.MaxValue)
        {
            len = ushort.MaxValue;
        }

        ioStream.WriteByte((byte)(len / 256));
        ioStream.WriteByte((byte)(len & 255));
        ioStream.Write(outBuffer, 0, len);
        ioStream.Flush();

        return outBuffer.Length + 2;
    }
}