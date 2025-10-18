using System.Diagnostics;

var revitPath = @"C:\Program Files\Autodesk\Revit 2025\Revit.exe";
var addinCmd = "RunTestsCommand";

var process = Process.Start(revitPath, $"/language ENU");
process.WaitForExit();