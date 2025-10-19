using System.Collections.ObjectModel;
using System.Reflection;

using NamedPipeServer;

using RevitTestLibrary.Common;

using Shouldly;

using Xunit.Runner.InProc.SystemConsole;

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

    [Fact]
    public void Foo()
    {
        AppDomain.CurrentDomain.Load(typeof(ConsoleRunnerInProcess).Assembly.GetName());
        
        xru.Initialize(new Dictionary<string,object>()
        {
            
        });
        const string assemblyFileName =
            @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\RevitTestAssembly\bin\Release R25\RevitTestAssembly.dll";

        var handler = new TestCommandHandler();

        var result = handler.Handle(assemblyFileName);

        result.ShouldBeTrue();
    }

    [Fact]
    public void Bar()
    {
        
        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(x => x.GetName().Name)
            .Where(x => x.Contains("xunit"))
            .ToArray();

        assembly.ShouldContain("xunit.v3.runner.inproc.console");
    }
}

public class Fuck : IDisposable
{
    public void Dispose()
    {
    }
}