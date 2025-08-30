using System.IO;
using System.Reflection;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

using Nice3point.Revit.Toolkit.External;

using Xunit;

namespace RevitTestRunner.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        Result = ActualExecute();
    }

    private Result ActualExecute()
    {
        try
        {
            const string dllPath =
                @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\RevitTestRunner.Tests\bin\Debug\net8.0\RevitTestRunner.Tests.dll";

            if (!File.Exists(dllPath))
            {
                TaskDialog.Show("Test Harness", "Test DLL not found.");
                return Result.Failed;
            }

            var testAssembly = Assembly.LoadFrom(dllPath);
            // TODO: Load real test assembly instead
            var testMethods = testAssembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(FactAttribute), false).Length != 0
                            || m.GetCustomAttributes(typeof(TheoryAttribute), false).Length != 0);

            var passed = 0;
            var failed = 0;

            foreach (var method in testMethods)
            {
                try
                {
                    var instance = method.IsStatic ? null : Activator.CreateInstance(method.DeclaringType);
                    method.Invoke(instance, null);
                    passed++;
                }
                catch (Exception ex)
                {
                    failed++;
                    TaskDialog.Show("Test Failed", $"{method.Name}: {ex.InnerException?.Message ?? ex.Message}");
                }
            }

            TaskDialog.Show("Test Results", $"{passed} passed, {failed} failed");
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Harness Error", ex.ToString());
            return Result.Failed;
        }
    }
}