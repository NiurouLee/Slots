using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using Debug = UnityEngine.Debug;
using GameModule;
using System.Reflection;
using System.Text;
using Object = System.Object;

public class CreateMachineEditor : EditorWindow
{
    
    [MenuItem("AssetBundle/关卡适配编辑")]
    static void ShowEditorWindow()
    {
        MachineLayoutHelper window =
            (MachineLayoutHelper) EditorWindow.GetWindow(typeof(MachineLayoutHelper), false,
                "Find References", true);
        window.Show();
    }
    private class ClassToggle
    {
        public string name;
        public Type type;
        public bool toggle;
        public int index;
    }
    private const string MenuItemName = "Tools/生成新机器代码";
    private const string GameModuleNameSpaceInit = "GameModule.MachineContextBuilder";
    private const string GameModuleNameSpace = "GameModule.";
    private const string ScriptPath = "/Scripts/Code@GameModule/Machine/";

    private List<List<string>> ListContextOverrideFunctions = new List<List<string>>
    {
        //context
        new List<string>{
            "SetUpLogicStepProxy",
            "BindingWheelView",
            "AttachTopPanel",
            "BindingExtraView",
            "BindingJackpotView",
            "SetUpMachineState",
            "SetUpCommonMachineState",
            "SetUpWheelActiveState",
            "GetSequenceElementConstructor",
            "GetElementExtraInfoProvider",
        },
        new List<string>(){},
        new List<string>(){},
        new List<string>(){},
        new List<string>(){},
    };
    
    private Dictionary<int, List<Type>> MachineTypes;
    private Dictionary<string, ClassToggle> DictClassToggle;
    private List<string> IgnoreFuncList;
    private int NewMachineId = 0;
    private int SelectToolBarId = 0;

    private int TYPE_LOGIC = 1;

    Assembly assembly;
    [MenuItem(MenuItemName, false, 25)]
    public static void CreateSlotTemplate()
    {
        CreateMachineEditor window =
            (CreateMachineEditor)EditorWindow.GetWindow(typeof(CreateMachineEditor), false,
                "创建新机器", true);
        window.Init();
        window.ShowModalUtility();
    }
    public void Init()
    {
        if (DictClassToggle == null)
        {
            MachineTypes = new Dictionary<int, List<Type>>();
            DictClassToggle = new Dictionary<string, ClassToggle>();
        }

        GetContextType(0);
        GetLogicType(TYPE_LOGIC);
        GetPopUpTypes(2);
        GetStateType(3);
        GetUtilityType(4);
    }

    private List<Type> GetContextType(int index)
    {
        if (!MachineTypes.ContainsKey(index))
        {
            var MachineContextBuilderTypes = new List<Type>();
            MachineContextBuilderTypes.Add(typeof(MachineContextBuilder));
            MachineTypes[index] = MachineContextBuilderTypes;
        }
        return MachineTypes[index];
    }
    private List<Type> GetLogicType(int index)
    {
        if (!MachineTypes.ContainsKey(index))
        {
            var LogicTypes = new List<Type>();
            LogicTypes = new List<Type>();
            LogicTypes.Add(typeof(MachineSetUpProxy));
            LogicTypes.Add(typeof(NextSpinPrepareProxy));
            LogicTypes.Add(typeof(RoundStartProxy));
            LogicTypes.Add(typeof(SubRoundStartProxy));
            LogicTypes.Add(typeof(WheelsSpinningProxy));
            LogicTypes.Add(typeof(WheelStopSpecialEffectProxy));
            LogicTypes.Add(typeof(EarlyHighLevelWinEffectProxy));
            LogicTypes.Add(typeof(WinLineBlinkProxy));
            LogicTypes.Add(typeof(ControlPanelWinUpdateProxy));
            LogicTypes.Add(typeof(SpecialBonusProxy));
            LogicTypes.Add(typeof(LateHighLevelWinEffectProxy));
            LogicTypes.Add(typeof(BonusProxy));
            LogicTypes.Add(typeof(ReSpinLogicProxy));
            LogicTypes.Add(typeof(SubRoundFinishProxy));
            LogicTypes.Add(typeof(FreeGameProxy));
            LogicTypes.Add(typeof(RoundFinishProxy));
            MachineTypes[index] = LogicTypes;
        }
        return MachineTypes[index];
    }

    private List<Type> GetPopUpTypes(int index)
    {
        if (!MachineTypes.ContainsKey(index))
        {
            var PopupTypes = new List<Type>();
            PopupTypes.Add(typeof(FreeSpinStartPopUp));
            PopupTypes.Add(typeof(FreeSpinFinishPopUp));
            PopupTypes.Add(typeof(FreeSpinReTriggerPopUp));
            PopupTypes.Add(typeof(ReSpinStartPopUp));
            PopupTypes.Add(typeof(ReSpinFinishPopUp));
            MachineTypes[index] = PopupTypes;
        }
        return MachineTypes[index];
    }

    private List<Type> GetStateType(int index)
    {
        if (!MachineTypes.ContainsKey(index))
        {
            var StateTypes = new List<Type>();
            StateTypes.Add(typeof(BetState));
            StateTypes.Add(typeof(AdStrategyState));
            StateTypes.Add(typeof(FreeSpinState));
            StateTypes.Add(typeof(AutoSpinState));
            StateTypes.Add(typeof(ReSpinState));
            StateTypes.Add(typeof(JackpotInfoState));
            StateTypes.Add(typeof(WinState));
            StateTypes.Add(typeof(WheelsActiveState));
            StateTypes.Add(typeof(ExtraState));
            StateTypes.Add(typeof(WheelState));
            MachineTypes[index] = StateTypes;
        }
        return MachineTypes[index];
    }

    private List<Type> GetUtilityType(int index)
    {
        if (!MachineTypes.ContainsKey(index))
        {
            var UtilityTypes = new List<Type>();
            UtilityTypes.Add(typeof(ElementExtraInfoProvider));
            UtilityTypes.Add(typeof(SequenceElementConstructor));
            MachineTypes[index] = UtilityTypes;
        }
        return MachineTypes[index];
    }
    private void ReloadSlotAssemble()
    {
        assembly = null;
        assembly = System.Reflection.Assembly.Load("GameModuleAssembly");
        DictClassToggle.Clear();
    }

    private Type GetSlotType(string typeName)
    {
        if (assembly == null)
        {
            assembly = System.Reflection.Assembly.Load("GameModuleAssembly");
        }
        return assembly.GetType(typeName);
    }

    private void NewMachine()
    {
        if (NewMachineId == 0 || NewMachineId <= 11000)
        {
            ShowTips("请输入新关卡ID");
            return;
        }
        bool isOk = EditorUtility.DisplayDialog("提示", string.Format("确定要生成关卡({0})的所有选择的类和函数吗？", NewMachineId), "确认", "取消");
        if (!isOk)
            return;
        foreach (var key in DictClassToggle)
        {
            if (key.Value.toggle)
            {
                NewClass(key.Value);   
            }
        }
        AssetDatabase.Refresh();
        ReloadSlotAssemble();
    }

    private void ShowTips(string content)
    {
        EditorUtility.DisplayDialog("提示", content, "好");
    }
    
    Vector2 scrollPos;
    private void OnGUI()
    {
        Init();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("请输入新机器ID：");
        var LastNewMachineId = NewMachineId;
        NewMachineId = EditorGUILayout.IntField(NewMachineId);
        if (GUILayout.Button("创建", GUILayout.Width(100)))
        {
            var filePath = $"Assets/RemoteAssets/Machine/LazyLoad/Machine{NewMachineId}";
            if (Directory.Exists(filePath))
            {
                CopyMachineRes(filePath);
                NewMachine();   
            }
            else
            {
                NewMachine(); 
                EditorUtility.DisplayDialog("机器资源不存在", "请输入正确的机器ID", "Ok");
            }
        }
        if (GUILayout.Button("刷新GameModule库", GUILayout.Width(100)))
        {
            ReloadSlotAssemble();
        }
        EditorGUILayout.EndHorizontal();
        if(LastNewMachineId != NewMachineId)
        {
            var newType = GetSlotType(GameModuleNameSpaceInit + NewMachineId);
            if (newType != null)
            {
                DictClassToggle.Clear();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("请选择需要继承的类：");
        EditorGUILayout.BeginHorizontal();
        SelectToolBarId = GUILayout.Toolbar(SelectToolBarId, new[] {"机器Context","逻辑Proxy", "弹板PopUp","State", "工具类"});
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        DrawToolbar();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    private void CopyMachineRes(string assetPath)
    {
        string clickedPath = assetPath;
        string clickedPathFull = Path.Combine(Directory.GetCurrentDirectory(), clickedPath);
        FileAttributes attr = File.GetAttributes(clickedPathFull);
        bool isDirectory = attr.HasFlag(FileAttributes.Directory);
        string parentDirectory = Path.GetDirectoryName(clickedPathFull);
        StringBuilder sourceFolder = new StringBuilder(clickedPathFull);
        string workDirectory = Path.GetFileName(Path.GetDirectoryName(Application.dataPath));
        sourceFolder.Replace($"/{workDirectory}/Assets", "/FortuneXRes/Assets");

        string shell = $"rsync -av --delete {sourceFolder}/ {clickedPathFull}/";
        Debug.Log($"shell :{shell}");
        ShellHelper.ShellRequest req = ShellHelper.ProcessCommand(shell, "");


        EditorUtility.DisplayProgressBar("", "Wait", 0);
         
        req.onDone += () =>
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Done", "......", "Ok");
            AssetDatabase.Refresh();
        };
    }

    private List<string> ListDefaultExtendClass = new List<string>
    {
        "MachineContextBuilder",
        "ExtraState"
    };
    private void DrawToolbar()
    {
        for (int i = 0; i < ListDefaultExtendClass.Count; i++)
        {
            if (!DictClassToggle.ContainsKey(ListDefaultExtendClass[i]))
            {
                var toggleItem = new ClassToggle();
                toggleItem.name = ListDefaultExtendClass[i];
                for (int j = 0; j < MachineTypes.Count; j++)
                {
                    var types = MachineTypes[j];
                    for (int k = 0; k < types.Count; k++)
                    {
                        if (types[k].Name.Contains(toggleItem.name))
                        {
                            toggleItem.type = types[k];
                            toggleItem.toggle = true;
                            toggleItem.index = j;
                            break;;
                        }
                    }
                }

                DictClassToggle.Add(ListDefaultExtendClass[i],toggleItem);
            }
        }
        List<Type> sourceData = MachineTypes[SelectToolBarId];
        for (var i = 0; i < sourceData.Count; i++)
        {
            var item = sourceData[i];
            var className = item.Name;
            if (!DictClassToggle.ContainsKey(className))
            {
                var toggleItem = new ClassToggle();
                toggleItem.index = SelectToolBarId;
                toggleItem.name = className;
                toggleItem.type = item;
                DictClassToggle.Add(className,toggleItem);
            }
            EditorGUI.indentLevel++;
            if (ListDefaultExtendClass.Contains(className))
            {
                DictClassToggle[className].toggle = true;
            }
            DictClassToggle[className].toggle = EditorGUILayout.ToggleLeft(item.Name, DictClassToggle[className].toggle);
            EditorGUI.indentLevel--;
        }
    }
    
    private bool IsIgnoreFunc(string funName)
    {
        if(IgnoreFuncList == null)
        {
            IgnoreFuncList = new List<string>();
            IgnoreFuncList.Add("GetHashCode");
            IgnoreFuncList.Add("Equals");
            IgnoreFuncList.Add("ToString");
            IgnoreFuncList.Add("SetActive");
        }
        return IgnoreFuncList.Contains(funName);
    }
    private string GetTypePath(int index)
    {
        switch (index)
        {
            case 1:
                return "Proxy";
            case 2:
                return "PopUp";
            case 3:
                return "State";
            case 4:
                return "Utility";
        }
        return "";
    }
    private string GetClassNormalHead(Type t)
    {
        StringBuilder str = new StringBuilder();
        str.Append("using System.Collections.Generic;\nusing UnityEngine;\nusing System;\nusing GameModule;\n\nnamespace GameModule\n{\n\t");
        str.Append(string.Format("public class {0} : {1}\n\t{{\n",t.Name + NewMachineId, t.Name));

        return str.ToString();
    }
    private void NewClass(ClassToggle item)
    {
        if (NewMachineId <= 11000 || item.type == null)
        {
            return;
        }
        var strClassName = GameModuleNameSpace + item.name + NewMachineId;
        var newType = GetSlotType(strClassName);
        var strFileName = strClassName.Replace(GameModuleNameSpace, "");
        var sourcePath = Application.dataPath + ScriptPath + "Machine" + NewMachineId + "/" + GetTypePath(item.index);
        var fullPathName = string.Format("{0}/{1}.cs", sourcePath,strFileName);
        StringBuilder strb = new StringBuilder();
        if (File.Exists(fullPathName) || newType != null)
        {
            return;
        }

        strb.Append(GetClassNormalHead(item.type));
        strb.Append(GenContruct(item.type));

        var lstFunctions = ListContextOverrideFunctions[item.index];
        if (lstFunctions.Count>0)
        {
            var methods = item.type.GetMethods(BindingFlags.Instance|BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var m in methods)
            {
                if (m.IsVirtual && !IsIgnoreFunc(m.Name) && lstFunctions.Contains(m.Name))
                {
                    strb.Append(GenFunction(m)); 
                }
            }
        }
        strb.Append("\n\t}\n}");

        if (!Directory.Exists(sourcePath))
        {
            Directory.CreateDirectory(sourcePath);
        }
        File.WriteAllText(fullPathName, strb.ToString());
    }
    private string GenContruct(Type t)
    {
        var methods = t.GetConstructors();
        if(methods != null && methods.Length > 0)
        {
            string str = "\t\tpublic ";
            var m = methods[0];
            str += t.Name + NewMachineId + "(";
            var param = m.GetParameters();
            string baseParam = "";
            for(var i=0; i< param.Length; i++)
            {
                str += string.Format("{0} {1},", GetTypeString(param[i].ParameterType),param[i].Name);
                baseParam += string.Format("{0},", param[i].Name);
            }
            if (baseParam.Length > 0)
            {
                str = str.Remove(str.Length - 1);
                baseParam = baseParam.Remove(baseParam.Length - 1);
                str += string.Format(")\n\t\t:base({0})\n\t\t{{\n\n\t\n\t\t}}\n", baseParam);
            }
            else
            {
                str += string.Format(")\n\t\t{{\n\n\t\t}}\n", baseParam);
            }

            return str;
        }
        return "";
    }
    private string GetTypeString(Type t)
    {
        switch (t.Name)
        {
            case "System.Void":
            case "Void":
                return "void";
            case "System.Boolean":
            case "Boolean":
                return "bool";
            case "System.Byte":
            case "Byte":
                return "byte";
            case "System.SByte":
            case "SByte":
                return "sbyte";
            case "System.Char":
            case "Char":
                return "char";
            case "System.Decimal":
            case "Decimal":
                return "decimal";
            case "System.Double":
            case "Double":
                return "double";
            case "System.Single":
            case "Single":
                return "float";
            case "System.Int32":
            case "Int32":
                return "int";
            case "System.UInt32":
            case "UInt32":
                return "uint";
            case "System.Int64":
            case "Int64":
                return "long";
            case "System.UInt64":
            case "UInt64":
                return "ulong";
            case "System.Object":
            case "Object":
                return "object";
            case "System.Object[]":
            case "Object[]":
                return "object[]";
            case "System.Int16":
            case "Int16":
                return "short";
            case "System.UInt16":
            case "UInt16":
                return "ushort";
            case "System.String":
            case "String":
                return "string";
        }
        if (t.IsGenericType)
        {
            var types = t.GetGenericArguments();
            if (t.Name.Contains("List"))
            {
                string ret = "List<{0}> ";
                string str = GetTypeString(types[0]);
                ret = string.Format(ret, str);
                return ret;
            }
            else if (t.Name.Contains("Dictionary"))
            {
                string ret = "Dictionary<{0},{1}> ";
                string key = GetTypeString(types[0]);
                string val = GetTypeString(types[1]);
                ret = string.Format(ret, key,val);
                return ret;
            }
            else if (t.Name.Contains("Action"))
            {
                var len = types.Length;
                string ret = "Action<";
                for(var i = 0; i < len; i++)
                {
                    string key = GetTypeString(types[i]);
                    ret += key + ",";
                }
                ret = ret.Remove(ret.Length - 1);
                ret += ">";
                return ret;
            }
            else
            {
                Debug.LogError("未实现的范型，请实现！" + t.Name);
            }
        }
        return t.Name;
    }
    
    private string GenFunction(MethodInfo m)
    {
        string str = "\t\t"+(m.IsPublic ? "public" : "protected")+" override " + GetTypeString(m.ReturnType) + " ";
        str += m.Name + "(";
        var param = m.GetParameters();
        string baseParam = "";
        for (var i = 0; i < param.Length; i++)
        {
            str += string.Format("{0} {1},", GetTypeString(param[i].ParameterType), param[i].Name);
            baseParam += string.Format("{0},", param[i].Name);
        }
        if (baseParam.Length > 1)
        {
            str = str.Remove(str.Length - 1);
            baseParam = baseParam.Remove(baseParam.Length - 1);
        }

        bool shouldReturn = false;
        var ret = GetWheelsData(m);
        if (!string.IsNullOrEmpty(ret))
        {
            shouldReturn = true;
            str += ret;
        }
        ret = GetLogicFuncBody(m, "Proxy");
        if (!string.IsNullOrEmpty(ret))
        {
            shouldReturn = true;
            str += ret;
        }
        ret = GetElementExtraProvier(m, "ElementExtraInfoProvider");
        if (!string.IsNullOrEmpty(ret))
        {
            shouldReturn = true;
            str += ret;
        }
        ret = GetSetUpMachineState(m);
        if (!string.IsNullOrEmpty(ret))
        {
            shouldReturn = true;
            str += ret;
        }
        ret = GetSetUpWheelActiveState(m, "WheelsActiveState");
        if (!string.IsNullOrEmpty(ret))
        {
            shouldReturn = true;
            str += ret;
        }

        ret = GetSetUpCommonMachineState(m, "State");
        if (!string.IsNullOrEmpty(ret))
        {
            shouldReturn = true;
            str += ret;
        }

        if (shouldReturn) return str;

        if (m.IsAbstract)
        {
            if(m.ReturnType != typeof(void))
            {
                str += ")\n\t\t{\n\t\t\treturn default;\n\t\t}\n";
            }
            else
            {
                str += ")\n\t\t{\n\t\t\t\n\t\t}\n";
            }  
        }
        else
        {
            if(m.ReturnType != typeof(void))
            {
                str += string.Format(")\n\t\t{{\n\t\t\treturn base.{0}({1});\n\t\t}}\n", m.Name, baseParam);
            }
            else
            {
                str += string.Format(")\n\t\t{{\n\t\t\tbase.{0}({1});\n\t\t}}\n", m.Name, baseParam);
            }  
        }
        return str;
    }

    private Dictionary<string, string> DictLogicMapping = new Dictionary<string, string>
    {
        {"MachineSetUpProxy", "LogicStepType.STEP_MACHINE_SETUP"},
        {"NextSpinPrepareProxy", "LogicStepType.STEP_NEXT_SPIN_PREPARE"},
        {"RoundStartProxy", "LogicStepType.STEP_ROUND_START"},
        {"SubRoundStartProxy", "LogicStepType.STEP_SUBROUND_START"},
        {"WheelsSpinningProxy", "LogicStepType.STEP_WHEEL_SPINNING"},
        {"WheelStopSpecialEffectProxy", "LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT"},
        {"EarlyHighLevelWinEffectProxy", "LogicStepType.STEP_EARLY_HIGH_LEVEL_WIN_EFFECT"},
        {"WinLineBlinkProxy", "LogicStepType.STEP_WIN_LINE_BLINK"},
        {"ControlPanelWinUpdateProxy", "LogicStepType.STEP_CONTROL_PANEL_WIN_UPDATE"},
        {"SpecialBonusProxy", "LogicStepType.STEP_SPECIAL_BONUS"},
        {"LateHighLevelWinEffectProxy", "LogicStepType.STEP_LATE_HIGH_LEVEL_WIN_EFFECT"},
        {"BonusProxy", "LogicStepType.STEP_BONUS"},
        {"ReSpinLogicProxy", "LogicStepType.STEP_RE_SPIN"},
        {"SubRoundFinishProxy", "LogicStepType.STEP_SUBROUND_FINISH"},
        {"FreeGameProxy", "LogicStepType.STEP_FREE_GAME"},
        {"RoundFinishProxy", "LogicStepType.STEP_ROUND_FINISH"},
    };
    private string GetLogicFuncBody(MethodInfo m, string suffix)
    {
        if (m.Name.Contains("SetUpLogicStepProxy"))
        {
            StringBuilder ret = new StringBuilder();
            ret.Append(")\n\t\t{\n\t\t\tvar logicProxy = base.SetUpLogicStepProxy();");
            foreach (var item in DictClassToggle)
            {
                if (item.Key.Contains(suffix))
                {
                    if (item.Value.toggle)
                    {
                        ret.Append($"\n\t\t\tlogicProxy[{DictLogicMapping[item.Key]}] = typeof({item.Value.name}{NewMachineId});");       
                    }
                }
            }
            ret.Append("\n\t\t\treturn logicProxy;\n\t\t}\n");
            return ret.ToString();
        }
        return String.Empty;
    }
    private string GetElementExtraProvier(MethodInfo m, string suffix)
    {
        if (m.Name.Contains("GetElementExtraInfoProvider"))
        {
            bool isToggle = false;
            StringBuilder ret = new StringBuilder();
            foreach (var item in DictClassToggle)
            {
                if (item.Key.Contains(suffix))
                {
                    if (item.Value.toggle)
                    {
                        isToggle = true;
                        ret.Append(")\n\t\t{\n\t\t\treturn new ElementExtraInfoProvider"+NewMachineId+"();\n\t\t}\n");
                    }
                }
            }

            if (isToggle)
            {
                return ret.ToString();
            }
        }
        return String.Empty;
    }
    
    private string GetSetUpWheelActiveState(MethodInfo m, string suffix="")
    {
        if (m.Name.Contains("SetUpWheelActiveState"))
        {         
            StringBuilder ret = new StringBuilder();
            ret.Append(")\n\t\t{\n\t\t\t");
            var isOverride = false;
            foreach (var item in DictClassToggle)
            {
                if (item.Key.Contains(suffix))
                {
                    if (item.Value.toggle)
                    {
                        isOverride = true;
                        ret.Append("machineState.Add<WheelsActiveState"+NewMachineId+">();\n\t\t}\n");
                        break;
                    }
                }
            }

            if (!isOverride)
            {
                ret.Append("machineState.Add<WheelsActiveState>();\n\t\t}\n");   
            }
            return ret.ToString();
        }
        return String.Empty;
    }
    private string GetSetUpMachineState(MethodInfo m, string suffix="")
    {
        if (m.Name.Contains("SetUpMachineState"))
        {
            StringBuilder ret = new StringBuilder();
            ret.Append(")\n\t\t{\n\t\t\tmachineState.Add<ExtraState"+NewMachineId+">();\n\t\t}\n");
            return ret.ToString();
        }
        return String.Empty;
    }
    
    private string GetSetUpCommonMachineState(MethodInfo m, string suffix="")
    {
        if (m.Name.Contains("SetUpCommonMachineState"))
        {

            List<string> filter = new List<string> {"WheelsActiveState", "ExtraState"};
            StringBuilder ret = new StringBuilder();
            ret.Append(")\n\t\t{\n\t\t\t");
            foreach (var item in DictClassToggle)
            {
                if (item.Key.Contains(suffix) && !filter.Contains(item.Key))
                {
                    if (item.Value.toggle)
                    {
                        ret.Append("\n\t\t\tmachineState.Add<"+item.Key+NewMachineId+">();");
                    }
                    else
                    {
                        ret.Append("\n\t\t\tmachineState.Add<"+item.Key+">();");
                    }
                }
            }
            ret.Append("\n\t\t\tSetUpWheelActiveState(machineState);");
            ret.Append("\n\t\t}\n");
            return ret.ToString();
        }
        return String.Empty;
    }
    
    private string GetWheelsData(MethodInfo m, string suffix="")
    {
        if (m.Name.Contains("BindingWheelView"))
        {
            var wheelsPath = "MachineContext/Wheels";
            var filePath = $"Assets/RemoteAssets/Machine/LazyLoad/Machine{NewMachineId}/Prefab/MachineScene{NewMachineId}.prefab";
            GameObject machineGo = AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject)) as GameObject;
            if (machineGo)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(")\n\t\t{\n");
                var wheelGo = machineGo.transform.Find(wheelsPath).transform;
                for (int i = 0; i <  wheelGo.childCount; i++)
                {
                    var childGo = wheelGo.GetChild(i);
                    var wheelTrans = childGo.name+"Trans";
                    var wheelName = childGo.name+"Wheel";
                    stringBuilder.Append(
                        "\n\t\t\tvar "+wheelTrans+" = machineContext.transform.Find(\"Wheels/"+childGo.name+"\");");
                    stringBuilder.Append(
                        "\n\t\t\tvar "+wheelName+" = machineContext.view.Add<Wheel>("+wheelTrans+");");
                    stringBuilder.Append(
                        "\n\t\t\t"+wheelName+".BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>());");
                    stringBuilder.Append(
                        "\n\t\t\t"+wheelName+".SetUpWinLineAnimationController<WinLineAnimationController>();\n");
                }
                stringBuilder.Append("\n\t\t}\n");
                return stringBuilder.ToString();
            }
        }
        return String.Empty;
    }
}
