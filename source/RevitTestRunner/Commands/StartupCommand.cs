using System.IO;
using System.Reflection;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

using Nice3point.Revit.Toolkit.External;

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
            const string dllPath = @"C:\Users\Petru\projects\revit-projects\RevitTestRunner\source\RevitTestRunner.Tests\bin\Debug\net8.0\RevitTestRunner.Tests.dll";

            if (!File.Exists(dllPath))
            {
                TaskDialog.Show("Test Harness", "Test DLL not found.");
                return Result.Failed;
            }

            var asm = Assembly.LoadFrom(dllPath);
            var wallHelperType = asm.GetType("RevitTestRunner.Tests.WallHelper");
            var method = wallHelperType?.GetMethod("GetWallTypeName", BindingFlags.Public | BindingFlags.Static);

            if (method == null)
            {
                TaskDialog.Show("Test Harness", "Test method not found.");
                return Result.Failed;
            }

            var result = method.Invoke(null, null)?.ToString() ?? "(null)";
            TaskDialog.Show("Test Harness", $"Test executed: {result}");
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            TaskDialog.Show("RevitTestRunner", ex.Message);
            return Result.Failed;
        }
    }
}