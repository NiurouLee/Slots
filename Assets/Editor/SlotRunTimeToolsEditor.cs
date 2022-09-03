using System;
using UnityEditor;
using UnityEngine;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using SimpleJson;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;


class JsonHelper
{
    private const string INDENT_STRING = "    ";

    public static string FormatJson(string str)
    {
        var indent = 0;
        var quoted = false;
        var sb = new StringBuilder();
        for (var i = 0; i < str.Length; i++)
        {
            var ch = str[i];
            switch (ch)
            {
                case '{':
                case '[':
                    sb.Append(ch);
                    if (!quoted)
                    {
                        sb.AppendLine();
                        Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
                    }

                    break;
                case '}':
                case ']':
                    if (!quoted)
                    {
                        sb.AppendLine();
                        Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
                    }

                    sb.Append(ch);
                    break;
                case '"':
                    sb.Append(ch);
                    bool escaped = false;
                    var index = i;
                    while (index > 0 && str[--index] == '\\')
                        escaped = !escaped;
                    if (!escaped)
                        quoted = !quoted;
                    break;
                case ',':
                    sb.Append(ch);
                    if (!quoted)
                    {
                        sb.AppendLine();
                        Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
                    }

                    break;
                case ':':
                    sb.Append(ch);
                    if (!quoted)
                        sb.Append(" ");
                    break;
                default:
                    sb.Append(ch);
                    break;
            }
        }

        return sb.ToString();
    }
}

static class Extensions
{
    public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
    {
        foreach (var i in ie)
        {
            action(i);
        }
    }
}


[CustomEditor(typeof(SlotRunTimeTools))]
public class SlotRumTimeToolsEditor : Editor
{
    private SlotRunTimeTools tools;

    private bool easingConfigFoldOut;
    private bool customPropertyFoldOut;
    private bool winEffectPropertyFoldOut;

    private AnimationCurve curve;
    private Material material;

    void OnEnable()
    {
        // Find the "Hidden/Internal-Colored" shader, and cache it for use.
        material = new Material(Shader.Find("Hidden/Internal-Colored"));
    }


    void DrawCurve(CubicEasingConfig cubicEasingConfig)
    {
        GUILayout.BeginHorizontal(EditorStyles.helpBox);

        // Reserve GUI space with a width from 10 to 10000, and a fixed height of 200, and 
        // cache it as a rectangle.
        Rect layoutRectangle = GUILayoutUtility.GetRect(300, 300, 300, 300);
        layoutRectangle.x += (layoutRectangle.width - 300) * 0.5f;

        var gridWidth = 300;
        int easingStep = (int) cubicEasingConfig.easingStep;

        var gridCellNum = easingStep + 2;
        var gridCellWidth = (float) gridWidth / gridCellNum;

        var enterSteadyX = gridCellWidth + (float) cubicEasingConfig.enterSteadyRate * easingStep * gridCellWidth;

        if (Event.current.type == EventType.Repaint)
        {
            // If we are currently in the Repaint event, begin to draw a clip of the size of 
            // previously reserved rectangle, and push the current matrix for drawing.
            GUI.BeginClip(layoutRectangle);
            GL.PushMatrix();

            // Clear the current render buffer, setting a new background colour, and set our
            // material for rendering.
            GL.Clear(true, false, Color.black);
            material.SetPass(0);

            // Start drawing in OpenGL Quads, to draw the background canvas. Set the
            // colour black as the current OpenGL drawing colour, and draw a quad covering
            // the dimensions of the layoutRectangle.
            GL.Begin(GL.QUADS);
            GL.Color(Color.black);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(gridWidth, 0, 0);
            GL.Vertex3(gridWidth, layoutRectangle.height, 0);
            GL.Vertex3(0, layoutRectangle.height, 0);
            GL.End();

            // Start drawing in OpenGL Lines, to draw the lines of the grid.
            GL.Begin(GL.LINES);

            // Store measurement values to determine the offset, for scrolling animation,
            // and the line count, for drawing the grid.

            //网格线
            for (var i = 1; i < gridCellNum; i++)
            {
                if (i == 1 || i == (easingStep + 1))
                {
                    GL.Color(new Color(0.6f, 0.6f, 0.0f));
                }
                else
                {
                    GL.Color(new Color(0.5f, 0.5f, 0.5f));
                }

                GL.Vertex3(0, (i) * gridCellWidth, 0);
                GL.Vertex3(gridWidth, (i) * gridCellWidth, 0);

                GL.Vertex3((i) * gridCellWidth, 0, 0);
                GL.Vertex3((i) * gridCellWidth, gridWidth, 0);
            }

            //对角线
            GL.Vertex3(gridCellWidth, gridCellWidth, 0);
            GL.Vertex3(gridWidth - gridCellWidth, gridWidth - gridCellWidth, 0);

            GL.Color(new Color(1.0f, 0.0f, 0.0f));

            float lastY = (float) cubicEasingConfig.GetY(0);
            for (var x = 1; x <= 100; x++)
            {
                float y = (float) cubicEasingConfig.GetY(x / 100.0f);
                GL.Vertex3(gridCellWidth + (x - 1) * gridCellWidth * easingStep * 0.01f,
                    (gridCellWidth + lastY * gridCellWidth * easingStep), 0);
                GL.Vertex3(gridCellWidth + (x) * gridCellWidth * easingStep * 0.01f,
                    (gridCellWidth + y * gridCellWidth * easingStep), 0);
                lastY = y;
            }

            //进入匀速阶段线
            GL.Color(new Color(0.0f, 1.0f, 0.0f));
            GL.Vertex3(enterSteadyX, 0, 0);
            GL.Vertex3(enterSteadyX, gridWidth, 0);

            // End lines drawing.
            GL.End();

            // Pop the current matrix for rendering, and end the drawing clip.
            GL.PopMatrix();
            GUI.EndClip();
        }

        // End our horizontal 
        GUILayout.EndHorizontal();
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();

        tools = (SlotRunTimeTools) (target);

        if (EditorApplication.isPlaying)
        {
            var machineId = tools.GetMachineId();
            if (machineId > 0)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("加载配置"))
                {
                    var machineConfigAssets = LoadMachineConfigAssets(machineId);
                    tools.SetMachineConfig(machineId, machineConfigAssets);
                }

                if (tools.IsMachineConfigLoaded())
                {
                    if (GUILayout.Button("应用配置"))
                    {
                        tools.UpdateConfig();
                        tools.ApplyNewConfig();
                    }

                    if (GUILayout.Button("存储配置"))
                    {
                        tools.UpdateConfig();
                        SaveMachineConfig(tools.GetMachineId(), tools.machineConfig);
                    }
                }

                GUILayout.EndHorizontal();
            }

            if (tools.IsMachineConfigLoaded())
            {
                easingConfigFoldOut = EditorGUILayout.Foldout(easingConfigFoldOut, "缓动编辑:");

                if (easingConfigFoldOut)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUI.indentLevel += 1;


                    EditorGUILayout.BeginHorizontal();
                    tools.selectEasingType =
                        EditorGUILayout.Popup("添加新的缓动节奏:", tools.selectEasingType, new[] {"Cubic"});
                    if (GUILayout.Button("+", GUILayout.Width(50)))
                    {
                        if (tools.selectEasingType == 0)
                        {
                            tools.easingConfig.Add(new CubicEasingConfig());
                            tools.selectIndex = tools.easingConfig.Count - 1;
                        }
                    }

                    EditorGUILayout.EndHorizontal();

                    string[] options = new string[tools.easingConfig.Count];

                    for (var i = 0; i < tools.easingConfig.Count; i++)
                    {
                        options[i] = "节奏[" + (i + 1) + "]";
                    }

                    EditorGUILayout.BeginHorizontal();
                    tools.selectIndex = EditorGUILayout.Popup("当前编辑缓动节奏:", tools.selectIndex, options);

                    if (GUILayout.Button("-", GUILayout.Width(50)))
                    {
                        if (tools.easingConfig.Count > 0)
                        {
                            tools.easingConfig.RemoveAt(tools.selectIndex);
                            tools.selectIndex = Math.Max(0, tools.selectIndex - 1);

                            //更新Panel的EasingIndex 索引，避免由于删除配置组，造成非法的数据
                            for (var i = 0; i < tools.panelEasingIndexList.Count; i++)
                            {
                                if (tools.panelEasingIndexList[i].NormalEasingIndex >= tools.easingConfig.Count)
                                {
                                    tools.panelEasingIndexList[i].NormalEasingIndex = tools.easingConfig.Count - 1;
                                }

                                if (tools.panelEasingIndexList[i].FreeEasingIndex >= tools.easingConfig.Count)
                                {
                                    tools.panelEasingIndexList[i].FreeEasingIndex = tools.easingConfig.Count - 1;
                                }

                                if (tools.panelEasingIndexList[i].LinkEasingIndex >= tools.easingConfig.Count)
                                {
                                    tools.panelEasingIndexList[i].LinkEasingIndex = tools.easingConfig.Count - 1;
                                }

                                if (tools.panelEasingIndexList[i].ExtraEasingIndex >= tools.easingConfig.Count)
                                {
                                    tools.panelEasingIndexList[i].ExtraEasingIndex = tools.easingConfig.Count - 1;
                                }
                            }
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                    if (tools.easingConfig?.Count > 0)
                    {
                        ShowEasingConfig(tools.easingConfig[tools.selectIndex]);
                    }

                    if (tools.panelEasingIndexList.Count > 0)
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField("轮盘缓动选择:");
                        for (var i = 0; i < tools.panelEasingIndexList.Count; i++)
                        {
                            ShowPanelEasingConfig(tools.panelEasingIndexList[i], i, options);
                        }

                        EditorGUILayout.Separator();
                    }

                    EditorGUI.indentLevel -= 1;
                    EditorGUILayout.EndVertical();
                }

                customPropertyFoldOut = EditorGUILayout.Foldout(customPropertyFoldOut, "自定义参数编辑:");

                if (customPropertyFoldOut)
                {
                    ShowBoolProperty(tools.customBooleanProperty);
                    ShowDoubleProperty(tools.customDoubleProperty);
                }


                EditorGUILayout.Separator();

                winEffectPropertyFoldOut = EditorGUILayout.Foldout(winEffectPropertyFoldOut, "赢钱效果配置:");

                if (winEffectPropertyFoldOut)
                {
                    for (var i = 0; i < tools.winEffectConfigs.Count; i++)
                    {
                        ShowWinEffectConfig(tools.winEffectConfigs[i]);
                    }
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }

    public void ShowWinEffectConfig(WinEffectConfig effectConfig)
    {
        EditorGUILayout.LabelField("Win Level [" + effectConfig.winLevel + "]");
       
        effectConfig.audioName = EditorGUILayout.TextField("音效:", effectConfig.audioName);
        effectConfig.stopAudioName = EditorGUILayout.TextField("尾音:", effectConfig.stopAudioName);
        effectConfig.freeAudioName = EditorGUILayout.TextField("Free音效:", effectConfig.freeAudioName);
        effectConfig.freeStopAudioName= EditorGUILayout.TextField("Free尾音:", effectConfig.freeStopAudioName);
    }

    public void ShowEasingConfig(EasingConfig easingConfig)
    {
        if (easingConfig is CubicEasingConfig)
        {
            var cubicEasingConfig = easingConfig as CubicEasingConfig;


            DrawCurve(cubicEasingConfig);

            EditorGUILayout.Separator();
            cubicEasingConfig.control1X =
                EditorGUILayout.Slider("缓动控制点1X:", (float) cubicEasingConfig.control1X, 0.0f, 1.0f);
            cubicEasingConfig.control1Y =
                EditorGUILayout.Slider("缓动控制点1Y:", (float) cubicEasingConfig.control1Y, -1.0f, 1.0f);
            EditorGUILayout.Separator();
            cubicEasingConfig.control2X =
                EditorGUILayout.Slider("缓动控制点2X:", (float) cubicEasingConfig.control2X, 0.0f, 1.0f);
            cubicEasingConfig.control2Y =
                EditorGUILayout.Slider("缓动控制点2Y:", (float) cubicEasingConfig.control2Y, 0.0f, 2.0f);
            EditorGUILayout.Separator();
            cubicEasingConfig.easingStep =
                EditorGUILayout.IntSlider("缓动移动距离:", (int) cubicEasingConfig.easingStep, 1, 30);
            cubicEasingConfig.easingDuration = Math.Max(0.1,
                EditorGUILayout.DoubleField("缓动整体时长:", cubicEasingConfig.easingDuration));

            cubicEasingConfig.enterSteadyRate =
                EditorGUILayout.Slider("进入匀数阶段的时间:", (float) cubicEasingConfig.enterSteadyRate, 0.1f, 0.9f);

            cubicEasingConfig.UpdateConfig();
            double steadySpeed = cubicEasingConfig.GetSpeed(cubicEasingConfig.enterSteadyRate);

            EditorGUILayout.LabelField("匀速阶段速度:                       [" + steadySpeed + "]");

            EditorGUILayout.Separator();

            cubicEasingConfig.steadyLeastStep =
                EditorGUILayout.IntField("匀速阶段基础步数:", (int) cubicEasingConfig.steadyLeastStep);
            cubicEasingConfig.steadyIncreaseStep =
                EditorGUILayout.IntField("匀速阶段每列递增步数:", (int) cubicEasingConfig.steadyIncreaseStep);
            cubicEasingConfig.drumIncreaseStep =
                EditorGUILayout.IntField("Drum模式每列递增步数:", (int) cubicEasingConfig.drumIncreaseStep);
            cubicEasingConfig.drumSpeedMultiplier =
                EditorGUILayout.DoubleField("Drum模式速度加乘倍数:", cubicEasingConfig.drumSpeedMultiplier);
            cubicEasingConfig.earlyStopSpeedMultiplier = EditorGUILayout.DoubleField("EarlyStop速度加乘倍数:",
                cubicEasingConfig.earlyStopSpeedMultiplier);
        }
    }

    public void ShowPanelEasingConfig(PanelEasingConfig panelEasingConfig, int index, string[] options)
    {
        panelEasingConfig.foldOut = EditorGUILayout.Foldout(panelEasingConfig.foldOut, "轮盘" + (index + 1));
        if (panelEasingConfig.foldOut)
        {
            panelEasingConfig.NormalEasingIndex =
                EditorGUILayout.Popup("Normal:", panelEasingConfig.NormalEasingIndex, options);
            panelEasingConfig.FreeEasingIndex =
                EditorGUILayout.Popup("Free:", panelEasingConfig.FreeEasingIndex, options);
            panelEasingConfig.LinkEasingIndex =
                EditorGUILayout.Popup("Link:", panelEasingConfig.LinkEasingIndex, options);
            panelEasingConfig.ExtraEasingIndex =
                EditorGUILayout.Popup("Extra:", panelEasingConfig.ExtraEasingIndex, options);
        }
    }

    public void ShowBoolProperty(Dictionary<string, bool> boolProperties)
    {
        if (boolProperties != null && boolProperties.Count > 0)
        {
            var copy = new Dictionary<string, bool>(boolProperties);
            foreach (var property in copy)
            {
                boolProperties[property.Key] = EditorGUILayout.Toggle(property.Key + ": ", property.Value);
            }
        }
    }

    public void ShowDoubleProperty(Dictionary<string, double> doubleProperties)
    {
        if (doubleProperties != null && doubleProperties.Count > 0)
        {
            var copy = new Dictionary<string, double>(doubleProperties);

            foreach (var property in copy)
            {
                doubleProperties[property.Key] = EditorGUILayout.DoubleField(property.Key + ": ", property.Value);
            }
        }
    }

    public JsonObject LoadMachineConfigAssets(int subjectId)
    {
        var machineConfigAsset =
            AssetDatabase.LoadAssetAtPath($"Assets/Slot/Slot_{subjectId}/Config/SlotConfig_{subjectId}.txt",
                typeof(TextAsset)) as TextAsset;
        if (machineConfigAsset != null)
        {
            var machineConfig = SimpleJson.SimpleJson.DeserializeObject(machineConfigAsset.text) as JsonObject;

            return machineConfig;
        }

        return null;
    }

    public void SaveMachineConfig(int subjectId, JsonObject machineConfig)
    {
        var text = JsonHelper.FormatJson(machineConfig.ToString());
        var machineConfigAsset =
            AssetDatabase.LoadAssetAtPath($"Assets/Slot/Slot_{subjectId}/Config/SlotConfig_{subjectId}.txt",
                typeof(TextAsset)) as TextAsset;
        File.WriteAllText(AssetDatabase.GetAssetPath(machineConfigAsset), text);
        EditorUtility.SetDirty(machineConfigAsset);
    }
}