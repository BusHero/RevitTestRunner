using Xunit.Runner.Common;
using Xunit.Sdk;

namespace RevitTestRunner.Commands;

internal sealed class TestExecutionSink : TestMessageSink, IDisposable
{
    public ManualResetEvent Finished { get; } = new(false);

    public List<string> Messages { get; } = [];

    private List<ITestCaseDiscovered> TestCases { get; } = [];

    public List<string> TestNames => TestCases.Select(x => x.Serialization).ToList();

    public TestExecutionSink()
    {
        Execution.TestPassedEvent += args =>
        {
            var message = TestCases
                .First(x => x.TestCaseUniqueID == args.Message.TestCaseUniqueID);
            
            Messages.Add($"PASS: {message.TestCaseDisplayName}");
        };
        Execution.TestFailedEvent += args
            => Messages.Add($"FAIL: {args.Message.Output}\n{string.Join("\n", args.Message.Messages)}");
        Execution.TestAssemblyFinishedEvent += _ => Finished.Set();

        Discovery.TestCaseDiscoveredEvent += args => TestCases.Add(args.Message);
        Discovery.DiscoveryCompleteEvent += _ => Finished.Set();
    }

    public void Dispose() => Finished.Dispose();
}