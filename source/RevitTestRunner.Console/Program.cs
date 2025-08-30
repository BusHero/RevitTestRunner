using Xunit;
using Xunit.Runner.Common;

using IMessageSinkMessage = Xunit.Sdk.IMessageSinkMessage;

var project = new XunitProject();

project.Add(new XunitProjectAssembly(
    project,
    @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\RevitTestRunner.xunitv3.Tests\bin\Debug\net8.0\RevitTestRunner.xunitv3.Tests.dll",
    new AssemblyMetadata(3, ".netcoreapp")));

var assembly = project.Assemblies.First();

await using var controller = XunitFrontController.Create(assembly)
                             ?? throw new ArgumentException("not an xUnit.net test assembly: {0}",
                                 assembly.AssemblyFileName);
var settings = new FrontControllerFindAndRunSettings(
    TestFrameworkOptions.ForDiscovery(assembly.Configuration),
    TestFrameworkOptions.ForExecution(assembly.Configuration),
    assembly.Configuration.Filters);

controller.FindAndRun(new CustomSink(), settings);

return;

internal class CustomSink : Xunit.Sdk.IMessageSink
{
    public bool OnMessage(IMessageSinkMessage message)
    {
        Console.WriteLine(message);
        return true;
    }
}