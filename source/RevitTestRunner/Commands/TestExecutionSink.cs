using Xunit.Runner.Common;
using Xunit.Sdk;

namespace RevitTestRunner.Commands;

internal sealed class TestExecutionSink : TestMessageSink, IDisposable
{
    private readonly MessageMetadataCache _metadataCache = new();

    public ManualResetEvent Finished { get; } = new(false);

    public List<string> Messages { get; } = [];

    public TestExecutionSink()
    {
        Execution.TestPassedEvent += args =>
        {
            var metadata = _metadataCache.TryGetTestCaseMetadata(args.Message);
            Messages.Add($"PASS: {metadata?.TestCaseDisplayName}");
        };
        Execution.TestFailedEvent += args =>
        {
            var metadata = _metadataCache.TryGetTestCaseMetadata(args.Message);
            Messages.Add($"FAIL: {metadata?.TestCaseDisplayName}");
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