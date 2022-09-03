using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Text;
using TMPro;
using Button = UnityEngine.UI.Button;

public class PrefabVariablesGenerator
{
    private static List<CSelectable> listAllControls;
    private static Dictionary<Transform, bool> dictAllControls;
    private static List<string> listVariableNameFilter;
    private static List<string> listButtonFuncNameFilter;
    private static List<string> listVariableBinderFilter;
    
    [MenuItem("Assets/收集Prefab变量到剪切板",false,0)]
    [MenuItem("GameObject/收集变量到剪切板",false,0)]
    private static void CollectGameObjectVariables(MenuCommand menuCommand)
    {
        InitCollections();
        CollectAllGameObject(Selection.activeTransform);
        CopyCodeToClipBoard();
    }
    
    private static void InitCollections()
    { 
        listAllControls = new List<CSelectable>();
        listVariableNameFilter = new List<string>();
        listButtonFuncNameFilter = new List<string>();   
        listVariableBinderFilter = new List<string>();
        dictAllControls = new Dictionary<Transform, bool>();
    }
    
    private static void CollectAllGameObject(Transform prefab)
    {
        StringBuilder stringBuilder = new StringBuilder();
        CollectVariables(prefab, in stringBuilder);
        if (listAllControls.Count == 0)
        {
            EditorUtility.DisplayDialog("警告", "请为需要收集的变量添加Variable Tag", "Ok");
        }
    }

    private static void CollectVariables(Transform transform, in StringBuilder stringBuilder)
    {
        int depth = 0;
        stringBuilder.Clear();
        CollectData(transform, stringBuilder,depth); 
    }

    private static void CollectData(Transform prefab, StringBuilder stringBuilder,int depth)
    {
        if (!prefab) return;
        var nameLen = prefab.name.Length;
        if (depth>0)
        {
            stringBuilder.Append(prefab.name);   
        }
        var childCount = prefab.childCount;
        if ((prefab.CompareTag("Variable") || depth == 0)&& !dictAllControls.ContainsKey(prefab))
        {
            bool hasTransForm = false;
            var abslutePath = depth == 0 ? prefab.name : stringBuilder.ToString();
            var componentType = typeof(Transform);
            if (prefab.GetComponent<Button>())
            {
                componentType = typeof(Button);
            }
            if (prefab.GetComponent<Text>())
            {
                componentType = typeof(Text);
            }
            if (prefab.GetComponent<TextMesh>())
            {
                componentType = typeof(TextMesh);
            }
            if (prefab.GetComponent<TextMeshPro>())
            {
                componentType = typeof(TextMeshPro);
            }
            if (prefab.GetComponent<TextMeshProUGUI>())
            {
                componentType = typeof(TextMeshProUGUI);
            }
            if (prefab.GetComponent<ScrollRect>())
            {
                componentType = typeof(ScrollRect);
            }
            if (prefab.GetComponent<Slider>())
            {
                componentType = typeof(Slider);
            }
            var item = new CSelectable(prefab.transform, abslutePath, componentType);
            listAllControls.Add(item);
            RemoveString(stringBuilder, nameLen);
            dictAllControls.Add(prefab,true);
        }
        if (childCount>0 && stringBuilder.Length>0)
        {
            stringBuilder.Append("/");
        }
        for (int i = 0; i < childCount; i++)
        {
            var child = prefab.GetChild(i);
            CollectData(child.transform,stringBuilder,depth+1);
        }

        if (childCount>0 && stringBuilder.Length>0)
        {
            RemoveString(stringBuilder, nameLen+1);
        }
    }

    private static void RemoveString(in StringBuilder stringBuilder, int rmvLength)
    {
        if (stringBuilder.Length>0)
        {
            stringBuilder.Remove(stringBuilder.Length-rmvLength, rmvLength);
        }
    }
    
    private static void CopyCodeToClipBoard()
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < listAllControls.Count; i++)
        {
            GenerateVariableCode(listAllControls[i], in stringBuilder);
        }

        var listButtons = GetComponents<Button>();
        for (int i = 0; i < listButtons.Count; i++)
        {
            GenerateButtonListenerCode(listButtons[i], in stringBuilder);
        }
        GUIUtility.systemCopyBuffer = stringBuilder.ToString();
    }

    private static List<CSelectable> GetComponents<T>() where T: Component
    {
        var listComponents = new List<CSelectable>();
        for (int i = 0; i < listAllControls.Count; i++)
        {
            var item = listAllControls[i];
            if (item.TypeName == typeof(T).Name)
            {
                listComponents.Add(item);
            }
        }
        return listComponents;
    }

    private static void GenerateVariableCode(CSelectable selectable, in StringBuilder stringBuilder)
    {
        string strVariableName = ToVariableName(listVariableNameFilter,selectable.Name);
        string strBinder = GetBinderPath(selectable);
        stringBuilder.Append($"\t\t[ComponentBinder(\"{strBinder}\")]\n");
        stringBuilder.Append($"\t\tpublic {selectable.TypeName} {strVariableName};\n\n");
    }

    private static void GenerateButtonListenerCode(CSelectable selectable, in StringBuilder stringBuilder)
    {
        string strVariableName = ToVariableName(listButtonFuncNameFilter,selectable.Name,false).Replace("Button", "");
        string strBinder = GetBinderPath(selectable);
        stringBuilder.Append($"\t\t[ComponentBinder(\"{strBinder}\")]\n");
        stringBuilder.Append($"\t\tprivate void OnBtn{strVariableName}Click()\n"+"\t\t{\n\n\t\t}\n");
    }

    private static string GetBinderPath(CSelectable selectable)
    {
        string strBinder = selectable.Name;
        if (listVariableBinderFilter.Contains(strBinder))
        {
            strBinder = selectable.AbsolutePath;
        }
        else
        {
            listVariableBinderFilter.Add(strBinder);
        }
        return strBinder;
    }
    private static string ToVariableName(List<string> filter, string input,bool capticalLower=true)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 0; index < input.Length; ++index)
        {
            char ch = input[index];
            if (index == 0)
            {
                stringBuilder.Append(capticalLower ? char.ToLower(ch) : char.ToUpper(ch));
            }
            else
            {
                stringBuilder.Append(ch);
            }
        }

        int tmpIndex = 0;
        string newName = stringBuilder.ToString();
        string tmpName = newName;
        while (filter.Contains(newName))
        {
            newName = tmpName + tmpIndex;
            tmpIndex++;
        }
        filter.Add(newName);
        return newName;
    }

    /// <summary>
    /// 遍历
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="Func"></param>
    public class CSelectable
    {
        public string Name;
        public string TypeName;
        public string AbsolutePath;
        
        public CSelectable(Transform go, string absolutePath, Type type)
        {
            Name = go.name;
            AbsolutePath = absolutePath;
            TypeName = type.Name;
        }
    }
}
