using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Text;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class CodeGenerateHelperWindow : EditorWindow
{
    private List<Object> _objectInPrefab;

    public string className;
    public  CodeGenerateHelperWindow()
    {
        _objectInPrefab = new List<Object>();
        _objectInPrefab.Add(null);
    }
    
    [MenuItem("GameObject/生成代码",false,0)]
  
    private static void CollectGameObjectVariables(MenuCommand menuCommand)
    {
        CodeGenerateHelperWindow window =
            (CodeGenerateHelperWindow) EditorWindow.GetWindow(typeof(CodeGenerateHelperWindow), false,
                "Popup代码生成", true);
        window.Show();

        if (Selection.gameObjects != null && Selection.gameObjects.Length > 0 && Selection.gameObjects[0] != null)
        {
            window.className = Selection.gameObjects[0].name;
        }
    }
    
    private void OnGUI()
    {
        int variableCount = _objectInPrefab.Count;

        EditorGUILayout.LabelField("Popup类名:");
        className = EditorGUILayout.TextField(className);
        
        EditorGUILayout.LabelField("变量:");
     //   EditorGUILayout.BeginHorizontal();
        
        bool hasEmpty = false;
      
        for (var i = 0; i < variableCount; i++)
        {
            _objectInPrefab[i] = EditorGUILayout.ObjectField(_objectInPrefab[i], typeof(Object), true);

            if (_objectInPrefab[i] is GameObject go)
            {
                _objectInPrefab[i] = go.transform;
            }
            
            if (_objectInPrefab[i] == null)
            {
                hasEmpty = true;
            }
        }
        
        if (!hasEmpty)
        {
            _objectInPrefab.Add(null);
        }
        
        if (GUILayout.Button("生成PopupCode"))
        {
            GeneratePopUpCode();
        }
        
        if (GUILayout.Button("生成ViewCode"))
        {
            GenerateViewCode();
        }
        if (GUILayout.Button("生成变量绑定代码"))
        {
            GenerateVariableCode();
        }
    }

    private string GetPath(Object classVariable)
    {
        var component = classVariable as Component;

        if (component == null)
        {
            if (classVariable is GameObject go)
            {
                component = go.transform;
            }
        }

        if (component != null)
        {
            var path = component.transform.name;
            var parent = component.transform.parent;
            
            while (parent != null)
            {
                if (parent.name.Equals("Canvas (Environment)"))
                {
                    parent = null;
                }
                else
                {
                    if (parent.parent != null && !parent.parent.name.Equals("Canvas (Environment)"))
                    {
                        path = parent.name + "/" + path;
                        parent = parent.parent;
                    }
                    else
                    {
                        parent = null;
                    }
                }
            }
            return path;
        }
        return null;
    }

    private void GenerateVariableCode()
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (var i = 0; i < _objectInPrefab.Count; i++)
        {
            if (_objectInPrefab[i] != null)
            {
                var path =  GetPath(_objectInPrefab[i]);
                if (path != null)
                {
                    stringBuilder.Append($"\t\t[ComponentBinder(\"{path}\")]\n");
                   
                    var variableName = _objectInPrefab[i].name;
                   
                    variableName = Char.ToLowerInvariant(variableName[0]) + variableName.Substring(1);
                   
                    stringBuilder.Append($"\t\tpublic {_objectInPrefab[i].GetType().Name} {variableName};\n");   
                    stringBuilder.Append("\n"); 
                }
            }
        }
        GUIUtility.systemCopyBuffer = stringBuilder.ToString();
    }
    private void GenerateViewCode()
    { 
        StringBuilder stringBuilder = new StringBuilder();
 
        string assetAddress = className;
        stringBuilder.Append("using UnityEngine;\nusing UnityEngine.UI;\n");
        stringBuilder.Append("namespace GameModule \n{\n");
        
        stringBuilder.Append($"\t[AssetAddress(\"{assetAddress}\")]\n");
        
        if (className.Substring(0, 2) == "UI")
        {
            className = className.Substring(2);
        }

        stringBuilder.Append($"\tpublic class {className}: View<{className}Controller>\n");
        stringBuilder.Append("\t{\n");
        
        for (var i = 0; i < _objectInPrefab.Count; i++)
        {
            if (_objectInPrefab[i] != null)
            {
                var path =  GetPath(_objectInPrefab[i]);
                if (path != null)
                {
                    stringBuilder.Append($"\t\t[ComponentBinder(\"{path}\")]\n");
                   
                    var variableName = _objectInPrefab[i].name;
                   
                    variableName = Char.ToLowerInvariant(variableName[0]) + variableName.Substring(1);
                   
                    stringBuilder.Append($"\t\tpublic {_objectInPrefab[i].GetType().Name} {variableName};\n");   
                    stringBuilder.Append("\n"); 
                }
            }
        }
        
        stringBuilder.Append($"\t\tpublic {className}(string address)\n:base(address)");
        stringBuilder.Append("\t\t{\n");
        stringBuilder.Append("\t\t}\n");
         
        stringBuilder.Append("\t}\n");
        
        stringBuilder.Append($"\tpublic class {className}Controller: ViewController<{className}>\n");
        stringBuilder.Append("\t{\n");
        GenerateButtonResponseCode(stringBuilder);
        stringBuilder.Append("\t}\n");
        stringBuilder.Append("}");
        
        GUIUtility.systemCopyBuffer = stringBuilder.ToString();
    }
    
    private void GeneratePopUpCode()
    { 
        StringBuilder stringBuilder = new StringBuilder();
 
        string assetAddress = className;
        stringBuilder.Append("using UnityEngine;\nusing UnityEngine.UI;\n");
        stringBuilder.Append("namespace GameModule \n{\n");
        
        stringBuilder.Append($"\t[AssetAddress(\"{assetAddress}\")]\n");
        
        if (!className.EndsWith("Popup"))
        {
            className += "Popup";
        }

        if (className.Substring(0, 2) == "UI")
        {
            className = className.Substring(2);
        }

        stringBuilder.Append($"\tpublic class {className}: Popup<{className}ViewController>\n");
        stringBuilder.Append("\t{\n");
        
        for (var i = 0; i < _objectInPrefab.Count; i++)
        {
            if (_objectInPrefab[i] != null)
            {
               var path =  GetPath(_objectInPrefab[i]);
               if (path != null)
               {
                   stringBuilder.Append($"\t\t[ComponentBinder(\"{path}\")]\n");
                   
                   var variableName = _objectInPrefab[i].name;
                   
                   variableName = Char.ToLowerInvariant(variableName[0]) + variableName.Substring(1);
                   
                   stringBuilder.Append($"\t\tpublic {_objectInPrefab[i].GetType().Name} {variableName};\n");   
                   stringBuilder.Append("\n"); 
               }
            }
        }
        
        stringBuilder.Append($"\t\tpublic {className}(string address)\n\t\t:base(address)\n");
        stringBuilder.Append("\t\t{\n");
        stringBuilder.Append("\t\t}\n");
         
        stringBuilder.Append("\t}\n");
        
        stringBuilder.Append($"\tpublic class {className}ViewController: ViewController<{className}>\n");
        stringBuilder.Append("\t{\n");

        GenerateButtonResponseCode(stringBuilder);
        
        stringBuilder.Append("\t}\n");
        stringBuilder.Append("}");
        
        GUIUtility.systemCopyBuffer = stringBuilder.ToString();
    }

    public void GenerateButtonResponseCode(StringBuilder stringBuilder)
    {
        List<Button> buttons = new List<Button>();
      
        for (var i = 0; i < _objectInPrefab.Count; i++)
        {
            if (_objectInPrefab[i] is Button button)
            {
                buttons.Add(button);
            }
        }

        if (buttons.Count > 0)
        {
            stringBuilder.Append("\t\tprotected override void SubscribeEvents()\n");
            stringBuilder.Append("\t\t{\n");
            
            for (var i = 0; i < buttons.Count; i++)
            {
                var buttonName = buttons[i].name;
                var buttonVariableName = Char.ToLowerInvariant(buttonName[0]) + buttonName.Substring(1);
                stringBuilder.Append($"\t\t\tview.{buttonVariableName}.onClick.AddListener(On{buttonName}Clicked);\n");
            }
            
            stringBuilder.Append("\t\t}\n");
            
            for (var i = 0; i < buttons.Count; i++)
            {
                var buttonName = buttons[i].name;
                stringBuilder.Append($"\t\t\tpublic void On{buttonName}Clicked()\n");
                stringBuilder.Append("\t\t\t{\n\n");
                stringBuilder.Append("\t\t\t}\n");
            }
        }
      
    }
}