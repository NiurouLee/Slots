// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/10/17:31
// Ver : 1.0.0
// Description : MachineLayoutHelper.cs
// ChangeLog :
// **********************************************

using GameModule;
using UnityEditor;
using UnityEngine;

public class MachineLayoutHelper : EditorWindow
{
    private void OnGUI()
    {
        var machineScene = GameModule.ViewManager.Instance.GetSceneView<MachineScene>();

        if (machineScene == null)
        {
            return;
        }
        var layOutHelper = machineScene.viewController.GetMachineContext().contextBuilder.GetLayOutHelper();

        if (layOutHelper == null)
        {
            return;
        }

        var count = layOutHelper.GetLayOutElementCount();

        if (count == 0)
        {
            return;
        }

        for (var i = 0; i < count; i++)
        {
            EditorGUILayout.LabelField($"模块{i + 1}:");
            var layOutElement = layOutHelper.GetLayOutElement(i);

            layOutElement.heightInPixel = (int) EditorGUILayout.FloatField(
                "高度:",
                layOutElement.heightInPixel);

            layOutElement.anchorPoint =  EditorGUILayout.FloatField(
                "锚点[0,1]:",
                layOutElement.anchorPoint);

            layOutElement.bottomPaddingPercent =  EditorGUILayout.FloatField(
                "底部向上偏移可用空间百分比:",
                layOutElement.bottomPaddingPercent);

            layOutElement.bottomPaddingInPixel =  EditorGUILayout.FloatField(
                "底部向上偏移可用空间像素:",
                layOutElement.bottomPaddingInPixel);
        }

        layOutHelper.DoLayout();
    }
}