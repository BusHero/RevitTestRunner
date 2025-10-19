using JetBrains.Annotations;

namespace RevitTestLibrary.Common;

public static class xru
{
    public static Dictionary<string, object> Data { get; private set; }

    [UsedImplicitly]
    public static void Initialize(Dictionary<string, object> data)
    {
        Data = data;
    }
}