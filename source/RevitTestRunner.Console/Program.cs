using NamedPipeServer;

var server = new Server(
    "test-pipe",
    new TestCommandHandler());

Console.WriteLine("Starting server...");

await server.StartAsync();