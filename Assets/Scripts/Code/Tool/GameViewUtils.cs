#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class GameViewUtils
{
    static object gameViewSizesInstance;
    static MethodInfo getGroup;
    static List<string> allText;
    static GameViewSizeGroupType currentGroupType;

    static GameViewUtils()
    {
        // gameViewSizesInstance  = ScriptableSingleton<GameViewSizes>.instance;
        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singleType.GetProperty("instance");
        getGroup = sizesType.GetMethod("GetGroup");
        gameViewSizesInstance = instanceProp.GetValue(null, null);
        allText = new List<string>();
        currentGroupType = GameViewSizeGroupType.Android;
        InitGroupText(true);
    }

    public enum GameViewSizeType
    {
        AspectRatio,
        FixedResolution
    }

    public static void SetSize(int index)
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        selectedSizeIndexProp.SetValue(gvWnd, index, null);
    }

    public static int GetCurrentIndex()
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        return Convert.ToInt32(selectedSizeIndexProp.GetValue(gvWnd, null).ToString());
    }

    public static float GetGameViewRatio()
    {
        System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
        System.Type type = assembly.GetType("UnityEditor.PlayModeView");

        Vector2 size = (Vector2) type.GetMethod(
            "GetMainPlayModeViewTargetSize",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Static
        ).Invoke(null, null);
        float screenWidth = Math.Max(size.x, size.y);
        float screenHeight = Math.Min(size.x, size.y);

        return screenWidth / screenHeight;
    }

    public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width,
        int height, string text)
    {
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupTyge);
        // group.AddCustomSize(new GameViewSize(viewSizeType, width, height, text);

        var group = GetGroup(sizeGroupType);
        var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
        var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        var ctor = gvsType.GetConstructor(new Type[] {typeof(int), typeof(int), typeof(int), typeof(string)});
        var newSize = ctor.Invoke(new object[] {(int) viewSizeType, width, height, text});
        addCustomSize.Invoke(group, new object[] {newSize});
    }

    public static bool SizeExists(GameViewSizeGroupType sizeGroupType, string text)
    {
        return FindSize(sizeGroupType, text) != -1;
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, string text)
    {
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
        // string[] texts = group.GetDisplayTexts();
        // for loop...

        var group = GetGroup(sizeGroupType);
        var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
        var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
        for (int i = 0; i < displayTexts.Length; i++)
        {
            string display = displayTexts[i];
            if (display == text)
                return i;
        }

        return -1;
    }

    public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        return FindSize(sizeGroupType, width, height) != -1;
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        // goal:
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
        // int sizesCount = group.GetBuiltinCount() + group.GetCustomCount();
        // iterate through the sizes via group.GetGameViewSize(int index)

        var group = GetGroup(sizeGroupType);
        var groupType = group.GetType();
        var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
        var getCustomCount = groupType.GetMethod("GetCustomCount");
        int sizesCount = (int) getBuiltinCount.Invoke(group, null) + (int) getCustomCount.Invoke(group, null);
        var getGameViewSize = groupType.GetMethod("GetGameViewSize");
        var gvsType = getGameViewSize.ReturnType;
        var widthProp = gvsType.GetProperty("width");
        var heightProp = gvsType.GetProperty("height");
        var indexValue = new object[1];

        for (int i = 0; i < sizesCount; i++)
        {
            indexValue[0] = i;
            var size = getGameViewSize.Invoke(group, indexValue);
            int sizeWidth = (int) widthProp.GetValue(size, null);
            int sizeHeight = (int) heightProp.GetValue(size, null);
            if (sizeWidth == width && sizeHeight == height)
                return i;
        }

        return -1;
    }

    static object GetGroup(GameViewSizeGroupType type)
    {
        return getGroup.Invoke(gameViewSizesInstance, new object[] {(int) type});
    }

    public static GameViewSizeGroupType GetCurrentGroupType()
    {
        var getCurrentGroupTypeProp = gameViewSizesInstance.GetType().GetProperty("currentGroupType");
        return (GameViewSizeGroupType) (int) getCurrentGroupTypeProp.GetValue(gameViewSizesInstance, null);
    }

    static void InitGroupText(bool force = false)
    {
        var cur = GetCurrentGroupType();
        if (cur != currentGroupType || force)
        {
            currentGroupType = cur;
            var group = GetGroup(currentGroupType);
            var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
            var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
            allText.Clear();
            for (int i = 0; i < displayTexts.Length; i++)
            {
                allText.Add(displayTexts[i]);
            }
        }
    }

    public static void SwitchOrientation(bool portrait)
    {
        var selectIdx = GetCurrentIndex();
        if (selectIdx < 0)
        {
            return;
        }

        InitGroupText(selectIdx >= allText.Count);
        if (selectIdx >= allText.Count)
        {
            return;
        }

        var curDisplayName = allText[selectIdx];
        string[] curDisplayNameSplit = curDisplayName.Split(' ');
        string resultText = null;
        for (var i = 0; i < allText.Count; i++)
        {
            if (selectIdx != i)
                if (allText[i].Contains(curDisplayNameSplit[0]))
                {
                    resultText = allText[i];
                    break;
                }
        }

        if (resultText == null)
        {
            return;
        }
#if UNITY_ANDROID
        string text = string.Empty;
        if (portrait)
        {
            if (resultText.Contains("Portrait"))
            {
                text = resultText;
            }
        }
        else
        {
            if (resultText.Contains("Landscape"))
            {
                text = resultText;
            }
        }

        var index = FindSize(GameViewSizeGroupType.Android, text);
        if (index != -1)
            SetSize(index);
#elif UNITY_IOS
    //todo
#endif
    }
}
#endif