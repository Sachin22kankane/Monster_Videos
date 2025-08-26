
#if UNITY_EDITOR
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
#endif

public class GameViewUtils
{
#if UNITY_EDITOR
    static object gameViewSizesInstance;
    static MethodInfo getGroup;
    internal static int screenIndex = 4, minIndex, maxIndex; // Because have 16 indexes in my list.
    private static int gameViewProfilesCount;

    static  GameViewUtils()
    {
        // gameViewSizesInstance  = ScriptableSingleton<GameViewSizes>.instance;
        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singleType.GetProperty("instance");
        getGroup = sizesType.GetMethod("GetGroup");
        gameViewSizesInstance = instanceProp.GetValue(null, null);
    }

    private enum GameViewSizeType
    {
        AspectRatio, FixedResolution
    }

    internal static void SetSize(int index)
    {
           var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        var SizeSelectionCallback = gvWndType.GetMethod("SizeSelectionCallback",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        SizeSelectionCallback.Invoke(gvWnd, new object[] { index, null });
    }

    static object GetGroup(GameViewSizeGroupType type)
    {
        return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
    }

    internal static void SetMinMaxIndex(int min, int max)
    {
        screenIndex = minIndex = min;
        maxIndex = max;

    }
    
    [MenuItem("Tools/GameViewSize/Previous %&Q")]
    private static void SetPrevious()
    {
        GetViewListSize();

        screenIndex--;

        Debug.Log(screenIndex);
        SetSize(screenIndex);
    }

    [MenuItem("Tools/GameViewSize/Next  %&E")]
    private static void SetNext()
    {
        GetViewListSize();
        screenIndex++;

        Debug.Log(screenIndex);
        SetSize(screenIndex);
    }

    internal static void GetViewListSize()
    {
        var group = GetGroup(GameViewSizeGroupType.Android);
        var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
        gameViewProfilesCount = (getDisplayTexts.Invoke(group, null) as string[]).Length;
    }

    internal static string GetViewDimentions(int index)
    {
        var group = GetGroup(GameViewSizeGroupType.Android);
        var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
        return (getDisplayTexts.Invoke(group, null) as string[])[index];
    }
#endif
}