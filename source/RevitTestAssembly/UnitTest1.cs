using Autodesk.Revit.UI;

using RevitTestLibrary.Common;

namespace RevitTestAssembly;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var application = xru.Data["UIApplication"] as UIApplication;
        
        Assert.NotNull(application);
    }
}