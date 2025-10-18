using Autodesk.Revit.UI;

namespace RevitTestRunner.xunitv3.Tests;

public class UnitTest1
{
    [Fact]
    public void PassingTest()
    {
        Assert.True(true);
    }
    
    [Fact]
    public void FailingTest()
    {
        Assert.True(true);
    }
}