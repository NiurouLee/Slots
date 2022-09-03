// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/15/15:07
// Ver : 1.0.0
// Description : ApplicationExceptionHandler.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class ApplicationExceptionHandler : MonoBehaviour
{
    private int _maxExceptionCount = 200;

    private LinkedList<Tuple<string,string>> _collectExceptions;

    private Vector3 _scrollPosition;
    private bool _showExceptionView = false;
    
    private GUIStyle currentStyle = null;
    private GUIStyle logStyle = null;
    private GUIStyle openButtonStyle = null;
    private GUIStyle customButtonStyle = null;
    private GUIStyle gsAlterQuest;

    private bool _autoShowException = true;
    private bool _showStackTrace = false;

    private bool _showBundleDownloadInfo = false;

    private bool showBlockUI = true;

    public List<string> logFilterOnUI = new List<string>() {"Track", "LB","RB", "Lock", "D", "E", "E&S", "ALL", "All&S"};
   
    public int selectIndex = 4;
 
    public List<string> logFilters = new List<string>() {"[[ShowOnExceptionHandler]]"};

    private void InitStyles()
    {
        if (currentStyle == null)
        {
            currentStyle = new GUIStyle(GUI.skin.label);
            currentStyle.normal.textColor = Color.red;
            currentStyle.fontSize = 20;
        } 
        
        if (logStyle == null)
        {
            logStyle = new GUIStyle(GUI.skin.label);
            logStyle.normal.textColor = Color.yellow;
            logStyle.fontSize = 20;
        }

        if (openButtonStyle == null)
        {
            openButtonStyle = new GUIStyle(GUI.skin.label);
            openButtonStyle.normal.background = MakeRoundTex(5,  new Color(0.0f, 0.8f, 0.0f, 0.8f));
            openButtonStyle.fontSize = 25;
        }

        if (customButtonStyle == null)
        {
            customButtonStyle = new GUIStyle("button");
            customButtonStyle.fontSize = 25;
        }

        if (gsAlterQuest == null)
        {
            gsAlterQuest = new GUIStyle();
            gsAlterQuest.normal.background = MakeTex(2, 2, new Color(0.0f, 0.0f, 0.0f, 0.8f));
        }

    }
    
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
    
    private Texture2D MakeRoundTex(int radius, Color color)
    {
        Color[] pix = new Color[radius * radius];
        for (int i = 0; i < pix.Length; ++i)
        {
            var col = i / radius;
            var row = i % radius;

            var distance = (col - radius / 2) * (col - radius / 2) + (row - radius / 2) * (row - radius / 2);
            if (distance < radius * radius)
            {
                pix[i] = color;
            }
            else
            {
                pix[i] = new Color(0, 0, 0, 0);
            }
        }
        
        Texture2D result = new Texture2D(radius, radius);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }


    void Awake()
    {
        Application.logMessageReceived += HandleException;
        // m_Writer = new StreamWriter(Path.Combine(Application.dataPath, "unityexceptions.txt"));
        // m_Writer.AutoFlush = true;

        _collectExceptions = new LinkedList<Tuple<string,string>>();

        _autoShowException = PlayerPrefs.GetInt("_DebugAutoShowExceptionWindow", 1) > 0;
    }

    private void HandleException(string condition, string stackTrace, LogType type)
    {
       // Debug.Log(condition);

        if (condition != "NotImplementedException: Neutral region info")
        {
            if (type == LogType.Exception && !stackTrace.Contains("HandleNonSuccessAndDebuggerNotification"))
            {
                _collectExceptions.AddFirst(new Tuple<string, string>(stackTrace,"ExceptionStackTrace"));
                _collectExceptions.AddFirst(new Tuple<string, string>(condition, "Exception"));

                if (_autoShowException)
                    _showExceptionView = true;
                return;
            }
            
            if (type == LogType.Error)
            {
                _collectExceptions.AddFirst(new Tuple<string, string>(stackTrace, "ErrorStackTrace"));
                _collectExceptions.AddFirst(new Tuple<string, string>(condition, "Error"));
                return;
            }

            for (var i = 0; i < logFilters.Count; i++)
            {
                if (condition.Contains(logFilters[i]))
                {
                    if (condition.Contains("\nIL"))
                    {
                        stackTrace = condition.Substring(condition.IndexOf('\n')) + stackTrace;
                        condition = condition.Substring(0, condition.IndexOf('\n'));
                    }
                    
                    _collectExceptions.AddFirst(new Tuple<string, string>(stackTrace, "LogStackTrace"));
                    _collectExceptions.AddFirst(new Tuple<string, string>(condition.Replace(logFilters[i], ""), "Log"));
                  
                    break;
                }
            }
        }
    }

    void OnGUI()
    {
        //   GUILayout.BeginScrollView("")
        InitStyles();
 
        //GUILayout.Box("", customButtonStyle, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));

     //   GUI.backgroundColor = new Color(0, 0, 0, 0.8f);

     if (_showExceptionView)
     {
         GUILayout.BeginVertical(gsAlterQuest, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
     }
     else
     {
         GUILayout.BeginVertical();
     }

     GUILayout.BeginHorizontal();

        float buttonSizeWidth = 130;
        float buttonSizeHeight = 50;

        var ratio = Math.Max(Screen.width, Screen.height) / ViewResolution.referenceResolutionLandscape.x;

        buttonSizeWidth =  ratio * buttonSizeWidth;
        buttonSizeHeight = ratio * buttonSizeHeight;


        var buttonName = "打开";

        if (_showExceptionView)
        {
            buttonName = "关闭";
        }

        if (GUILayout.Button(buttonName, customButtonStyle, GUILayout.Width(buttonSizeWidth), GUILayout.Height(buttonSizeHeight)))
        {
            _showExceptionView = !_showExceptionView;
        }
        
        if (_showExceptionView)
        {
            buttonName = "自动显示";
        
            if (_autoShowException)
            {
                buttonName = "手动显示";
            }
        
            if (GUILayout.Button(buttonName, customButtonStyle, GUILayout.Width(buttonSizeWidth), GUILayout.Height(buttonSizeHeight)))
            {
                _autoShowException = !_autoShowException;
                PlayerPrefs.SetInt("_DebugAutoShowExceptionWindow", _autoShowException ? 1 : 0);
            }

            
            if (GUILayout.Button("复制", customButtonStyle,GUILayout.Width(buttonSizeWidth), GUILayout.Height(buttonSizeHeight)))
            {
                var stringBuffer = "";

                foreach (var str in _collectExceptions)
                {
                    if (MatchCurrentFilter(str.Item1, str.Item2))
                    {
                        stringBuffer += str.Item1;
                        stringBuffer += "\n";
                        stringBuffer += "######\n";
                    }
                }
                
                GUIUtility.systemCopyBuffer = stringBuffer;
            }
            
            if (GUILayout.Button("清除", customButtonStyle, GUILayout.Width(buttonSizeWidth), GUILayout.Height(buttonSizeHeight)))
                _collectExceptions.Clear();

            if (GUILayout.Button("SRDebug", customButtonStyle, GUILayout.Width(buttonSizeWidth),
                GUILayout.Height(buttonSizeHeight)))
            {
                SRDebug.Instance.ShowDebugPanel();
                _showExceptionView = false;
            }
               

            buttonName = "阻塞点击";
            
            if (showBlockUI)
            {
                buttonName = "不阻塞点击";
            }
            
            if (GUILayout.Button(buttonName, customButtonStyle, GUILayout.Width(buttonSizeWidth),
                GUILayout.Height(buttonSizeHeight)))
                showBlockUI = !showBlockUI;
        }

        GUILayout.EndHorizontal();

        if (_showExceptionView)
        {
            GUI.skin.verticalScrollbar.fixedWidth = ViewResolution.referenceResolutionLandscape.x * 0.03f;
            GUI.skin.verticalScrollbarDownButton.fixedWidth = ViewResolution.referenceResolutionLandscape.x * 0.03f;;
            GUI.skin.verticalScrollbarThumb.fixedWidth = ViewResolution.referenceResolutionLandscape.x * 0.03f;;
            GUI.skin.verticalScrollbarUpButton.fixedWidth = ViewResolution.referenceResolutionLandscape.x * 0.03f;;
         //   GUI.skin.scrollView = scrollViewStyle;
            
         GUILayout.BeginVertical();
         selectIndex = GUILayout.Toolbar(selectIndex, logFilterOnUI.ToArray(),customButtonStyle,GUILayout.Height(buttonSizeHeight));
            
            _scrollPosition = GUILayout.BeginScrollView(
                _scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height - buttonSizeHeight*2));

            foreach (var item in _collectExceptions)
            {
                if (MatchCurrentFilter(item.Item1, item.Item2))
                {
                    if (item.Item2 == "Log")
                        GUILayout.Label(item.Item1, logStyle);
                    else
                    {
                        GUILayout.Label(item.Item1, currentStyle);
                    }
                }
            }
            
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
        ShowBlockUI();
    }

    // {"Track", "Bundle", "Lock", "D", "E", "E&S", "All"}
    private bool MatchCurrentFilter(string logItem, string logTag)
    {
        switch (selectIndex)
        {
            case 0:
                return logTag == "Log" && logItem.Contains("TrackerPoint:");
            case 1:
                return logTag == "Log" && logItem.Contains("LocalBundle:");
            case 2:
                return logTag == "Log" && logItem.Contains("RemoteBundle:");
            case 3:    
                return logTag == "Log" && logItem.Contains("BlockingUserClick");
            case 4:
                return logTag == "Log" && !logItem.Contains("TrackerPoint:") && !logItem.Contains("LocalBundle:") && !logItem.Contains("RemoteBundle:");;
            case 5:
                return logTag == "Error" || logTag == "Exception";
            case 6:
                return logTag == "ErrorStackTrace" || logTag == "ExceptionStackTrace" || logTag == "Error" ||
                       logTag == "Exception";
            case 7:
                return !logTag.Contains("Stack");
            case 8:
                return true;
        }

        return false;
    }

    private Transform blockingUserClickUI;
    private void ShowBlockUI()
    {
        if (blockingUserClickUI == null)
        {
            blockingUserClickUI = GameObject.Find("Launcher/HighPriorityUIContainerCanvas").transform.Find("ExceptionHandlerUI");
            if (blockingUserClickUI == null)
            {
                var gameObject = new GameObject("ExceptionHandlerUI");
                gameObject.transform.SetParent(GameObject.Find("Launcher/HighPriorityUIContainerCanvas").transform, false);
                var rectTransform = gameObject.AddComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                var image = gameObject.AddComponent<Image>();
                image.color = new Color(0, 0, 0, 0);
                blockingUserClickUI = gameObject.transform;
            }
            blockingUserClickUI.SetAsLastSibling();
        }
        blockingUserClickUI.gameObject.SetActive(_showExceptionView && showBlockUI);
    }
}