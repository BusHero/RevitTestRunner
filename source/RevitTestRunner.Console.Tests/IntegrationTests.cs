using System.IO.Pipes;
using System.Security.Principal;

namespace RevitTestRunner.Console.Tests;

public class IntegrationTests
{
    [Fact]
    public async Task Foo()
    {
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        var pipeClient = new NamedPipeClientStream(
            ".",
            "test-pipe",
            PipeDirection.InOut,
            PipeOptions.None,
            TokenImpersonationLevel.Impersonation);

        await pipeClient.ConnectAsync(tokenSource.Token);
        using var reader = new StreamReader(pipeClient, leaveOpen: true);
        await using var writer = new StreamWriter(pipeClient, leaveOpen: true);
        writer.AutoFlush = true;
        
        await reader.ReadLineAsync(TestContext.Current.CancellationToken);

        await writer.WriteLineAsync(
            @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\PassingTestAssembly\bin\Release\net8.0\PassingTestAssembly.dll");
        await writer.WriteLineAsync(
            @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\PassingTestAssembly\bin\Release\net8.0\PassingTestAssembly.dll");
        await writer.WriteLineAsync(
            @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\PassingTestAssembly\bin\Release\net8.0\PassingTestAssembly.dll");
    }
}