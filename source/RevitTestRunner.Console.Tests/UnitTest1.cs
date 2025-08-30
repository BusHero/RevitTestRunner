using Xunit.Runner.Common;

namespace RevitTestRunner.Console.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var project = new XunitProject();

        project.Add(new XunitProjectAssembly(
            project,
            @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\RevitTestRunner.xunitv3.Tests\bin\Debug\net8.0\RevitTestRunner.xunitv3.Tests.dll",
            new AssemblyMetadata(3, ".netcoreapp")));

        var controller = XunitFrontController.Create(project.Assemblies.First());
    }
}