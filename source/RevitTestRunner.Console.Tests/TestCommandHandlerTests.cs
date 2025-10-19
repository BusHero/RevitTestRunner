using NamedPipeServer;

using Shouldly;

namespace RevitTestRunner.Console.Tests;

public class TestCommandHandlerTests
{
    [Fact]
    public void TestPassingAssembly()
    {
        const string assemblyFileName =
            @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\PassingTestAssembly\bin\Release\net8.0\PassingTestAssembly.dll";

        var handler = new TestCommandHandler();

        var result = handler.Handle(assemblyFileName);

        result.ShouldBeTrue();
    }

    [Fact]
    public void TestFailingAssembly()
    {
        const string assemblyFileName =
            @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\FailingTestAssembly\bin\Release\net8.0\FailingTestAssembly.dll";

        var handler = new TestCommandHandler();

        var result = handler.Handle(assemblyFileName);

        result.ShouldBeFalse();
    }
}