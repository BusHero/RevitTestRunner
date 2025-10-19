// using NamedPipeServer;
//
// var server = new Server(
//     "test-pipe",
//     new TestCommandHandler());
//
// Console.WriteLine("Starting server...");
//
// await server.StartAsync();

using System.IO.Pipes;
using System.Security.Principal;

using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
var pipeClient = new NamedPipeClientStream(
    ".",
    "test-pipe",
    PipeDirection.InOut,
    PipeOptions.None,
    TokenImpersonationLevel.Impersonation);

await pipeClient.ConnectAsync(tokenSource.Token);
// using var reader = new StreamReader(pipeClient, leaveOpen: true);
await using var writer = new StreamWriter(pipeClient, leaveOpen: true);
writer.AutoFlush = true;

using var source = new CancellationTokenSource(TimeSpan.FromSeconds(2));
// await reader.ReadLineAsync(source.Token);

await writer.WriteLineAsync(
    @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\PassingTestAssembly\bin\Release\net8.0\PassingTestAssembly.dll");


await writer.WriteLineAsync(
    @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\FailingTestAssembly\bin\Release\net8.0\FailingTestAssembly.dll");
