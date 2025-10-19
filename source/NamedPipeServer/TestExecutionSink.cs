using Xunit.Runner.Common;

namespace NamedPipeServer;

internal sealed class TestExecutionSink : TestMessageSink, IDisposable
{
    private readonly MessageMetadataCache _metadataCache = new();

    public ManualResetEvent Finished { get; } = new(false);

    private List<string> Messages { get; } = [];

    public bool Passed { get; set; }

    public bool Failed { get; set; }

    public TestExecutionSink()
    {
        Execution.TestPassedEvent += args =>
        {
            var metadata = _metadataCache.TryGetTestCaseMetadata(args.Message);
            var message = $"PASS: {metadata?.TestCaseDisplayName}";
            Messages.Add(message);
            Console.WriteLine(message);
            Passed = true;
        };
        Execution.TestFailedEvent += args =>
        {
            var metadata = _metadataCache.TryGetTestCaseMetadata(args.Message);
            var message = $"FAIL: {metadata?.TestCaseDisplayName}";
            Messages.Add(message);
            Console.WriteLine(message);
            Failed = true;
        };
        Execution.TestAssemblyFinishedEvent += _ => Finished.Set();
        Execution.TestAssemblyStartingEvent += args => _metadataCache.Set(args.Message);
        Execution.TestCaseStartingEvent += args => _metadataCache.Set(args.Message);
        Execution.TestClassStartingEvent += args => _metadataCache.Set(args.Message);
        Execution.TestCollectionStartingEvent += args => _metadataCache.Set(args.Message);
        Execution.TestStartingEvent += args => _metadataCache.Set(args.Message);
        Execution.TestMethodStartingEvent += args => _metadataCache.Set(args.Message);
    }

    public void Dispose() => Finished.Dispose();
}