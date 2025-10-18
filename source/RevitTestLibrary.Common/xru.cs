using Autodesk.Revit.UI;

namespace RevitTestLibrary.Common;

public static class xru
{
    public static UIApplication? UiApplication { get; private set; }
    
    [UsedImplicitly]
    public static void Initialize(UIApplication uiApplication)
    {
        UiApplication = uiApplication;
    }
}