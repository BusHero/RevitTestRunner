using Autodesk.Revit.UI;

using RevitTestLibrary.Common;

namespace RevitTestRunner.xunitv3.Tests;

public class RevitApiTests
{
    [Fact]
    public void UiApplicationNotNull()
    {
        // UIApplication? UiApplication = null;

        Assert.NotNull(xru.UiApplication);
    }
    
    [Fact]
    public void UiApplicationNull()
    {
        // UIApplication? UiApplication = null;
        // Assert.Null(xru.UiApplication);
    }
}