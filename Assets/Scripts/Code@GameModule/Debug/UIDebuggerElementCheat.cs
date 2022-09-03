#if UNITY_EDITOR || !PRODUCTION_PACKAGE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DG.Tweening;
using GameModule;
using UnityEngine;
using UnityEngine.UI;

[System.AttributeUsage(AttributeTargets.Method)]
public class UIDebuggerElementCheatAttribute : Attribute
{
    public string mName = string.Empty;

    public UIDebuggerElementCheatAttribute(string name)
    {
        mName = name;
    }
}
public class UIDebuggerElementCheatItem
{
    public string name;
    public Action action;
    public Button btn;
}
public class UIDebuggerElementCheat:UIDebuggerElement
{
    [ComponentBinder("ScrollViewQuick")] 
    private RectTransform tranGroupQuick;
    [ComponentBinder("ScrollViewQuick/Viewport/Content/BtnTempletQuick")] 
    private Button btnQuickTemplate;
    [ComponentBinder("ScrollViewQuick/dragBar")] 
    private Transform dragBar;

    private Tweener _tweenerQuickShow;
    private bool isQuickShow = false;
    protected string[] mListCheatId;
    protected bool isInited = false;
    protected List<string> listCheatIdPool = new List<string>();
    protected bool isQueueCheatId = false;
    private Dictionary<string, UIDebuggerElementCheatItem> listItems = new Dictionary<string, UIDebuggerElementCheatItem>();
    public UIDebuggerElementCheat(string inButtonText = "defaultBtnText", int inPriority = 10, Action inButtonCallback = null):base(inButtonText, inPriority, inButtonCallback)
    {
        buttonCallback = OnBtnShowCheat;
    }
    private static UIDebuggerElementCheat instance;
    public static UIDebuggerElementCheat Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UIDebuggerElementCheat("Cheat",0);
            }

            return instance;
        }
    }
    public override void InitContext(Transform inDebuggerTransform)
    {
        base.InitContext(inDebuggerTransform);
        tranGroupQuick.localScale = new Vector3(1, 0, 1);
        FindAttributes();
        isInited = true;
        btnQuickTemplate.gameObject.SetActive(false);
        SetDragPointAndObject(tranGroupQuick.transform,dragBar);
        
        SubscribeEvent<EventEnterMachineScene>(EnterMachineCallback);
    }
    public void EnterMachineCallback(EventEnterMachineScene eventSceneSwitchEnd)
    {
        tranGroupQuick.pivot = new Vector2(0.5f,0.5f);
        if (!isQuickShow)
        {
            isQuickShow = true;
            tranGroupQuick.localScale = Vector3.one;
        }
        SetPositionInSafeRect(tranGroupQuick.sizeDelta,tranGroupQuick);
    }
    private void OnBtnShowCheat()
    {
        if (_tweenerQuickShow != null)
        {
            _tweenerQuickShow.Kill();
        }
        
        tranGroupQuick.pivot = GetContainerPivot();
        var btnPosition = btnElement.transform.position;
        tranGroupQuick.position = btnPosition;
        
        float scaleEnd;
        if (isQuickShow)
        {
            scaleEnd = 0;
        }
        else
        {
            scaleEnd = 1;
        }

        isQuickShow = !isQuickShow;

        _tweenerQuickShow = DOTween.To(() => tranGroupQuick.localScale.y,
            num => { tranGroupQuick.localScale = new Vector3(1, num, 1); }, scaleEnd, 0.1f);
    }
    
    public async void RefreshGameCheatId(string[] listCheatId)
    {
        while (!isInited)
        {
            await Task.Delay(10);
        }


        if (mListCheatId != null)
        {
            for (int i = 0; i < mListCheatId.Length; i++)
            {
                if (!string.IsNullOrEmpty(mListCheatId[i]))
                {
                    RemoveItem(mListCheatId[i]);
                }
            }
        }

        mListCheatId = listCheatId;

        if (mListCheatId != null)
        {
            var userController = Client.Get<UserController>();
            for (int i = 0; i < mListCheatId.Length; i++)
            {
                string cheatName = mListCheatId[i];
                if (!string.IsNullOrEmpty(cheatName))
                {
                    bool isPlayerItem = false;
                    if (cheatName.Contains("Cheat_playerId_"))
                    {
                        ulong playerId = 0;
                        if (ulong.TryParse(cheatName.Split('_')[2], out playerId))
                        {
                            if (userController.GetUserId() != playerId)
                            {
                                continue;
                            }
                            else
                            {
                                isPlayerItem = true;
                            }
                        }
                    }

                    UIDebuggerElementCheatItem item = new UIDebuggerElementCheatItem();
                    item.name = cheatName;
                    item.action = () => { BtnOnMachineCheat(item.name); };
                    AddItem(item);
                    if (isPlayerItem)
                    {
                        item.btn.transform.SetAsFirstSibling();
                    }
                }
            }
        }
    }
    private void FindAttributes()
    {
        // if (listItems.Count > 0)
        // {
        //     for (int i = listItems.Count - 1; i >= 0; i--)
        //     {
        //         var element = listItems.ElementAt(i);
        //         RemoveItem(element.Key);
        //     }
        // }
        
        listItems.Clear();
        Type typeMy = this.GetType();
        var listMethodInfo = typeMy.GetMethods();
        Type typeAttr = typeof(UIDebuggerElementCheatAttribute);
        for (int i = 0; i < listMethodInfo.Length; i++)
        {
            MethodInfo methodInfo = listMethodInfo[i];
            var objAttr = methodInfo.GetCustomAttributes(typeAttr, false);
            UIDebuggerElementCheatAttribute attr = null;
            if (objAttr.Length > 0)
            {
                attr = objAttr[0] as UIDebuggerElementCheatAttribute;
            }

            if (attr != null)
            {
                UIDebuggerElementCheatItem item = new UIDebuggerElementCheatItem();
                item.name = attr.mName;
                item.action = () => { methodInfo.Invoke(this, null); };
                AddItem(item);
            }
        }
    }
    
    

    public void AddItem(UIDebuggerElementCheatItem item)
    {
        if (!listItems.ContainsKey(item.name))
        {
            listItems[item.name] = item;
            Button btnT = btnQuickTemplate;

            Button btnNew = GameObject.Instantiate(btnT, btnT.transform.parent);
            Text lblTitle = btnNew.transform.Find("lblTitle").GetComponent<Text>();
            lblTitle.text = item.name;
            btnNew.onClick.AddListener(() => { item.action?.Invoke(); });
            btnNew.gameObject.SetActive(true);
            item.btn = btnNew;
        }
    }

    public UIDebuggerElementCheatItem AddItem(string name, Action action)
    {
        UIDebuggerElementCheatItem uiDebuggerItem = new UIDebuggerElementCheatItem();
        uiDebuggerItem.name = name;
        uiDebuggerItem.action = action;
        AddItem(uiDebuggerItem);
        return uiDebuggerItem;
    }

    public void RemoveItem(string name)
    {
        UIDebuggerElementCheatItem item = null;
        if (listItems.TryGetValue(name, out item))
        {
            GameObject.Destroy(item.btn.gameObject);
            listItems.Remove(name);
        }
    }
    public void BtnOnMachineCheat(string cheatId)
    {
        if (isQueueCheatId)
        {
            listCheatIdPool.Add(cheatId);
        }
        else
        {
            if (listCheatIdPool.Count > 0)
            {
                listCheatIdPool.Clear();
            }

            listCheatIdPool.Add(cheatId);
        }

        Debug.LogError($"======加入cheatId:{cheatId}");
    }
    public string GetActiveCheatId()
    {
        if (listCheatIdPool.Count > 0)
        {
            string cheatId = listCheatIdPool[0];
            listCheatIdPool.RemoveAt(0);

            Debug.LogError($"======获取cheatId:{cheatId}");
            return cheatId;
        }
        else
        {
            return String.Empty;
        }
    }
    
    [UIDebuggerElementCheatAttribute("打开CheatId队列")]
    public void EnableQueueCheatId()
    {
        isQueueCheatId = true;
        Debug.LogError($"======打开CheatId队列");
    }

    [UIDebuggerElementCheatAttribute("关闭并清空CheatId队列")]
    public void DisableQueueCheatId()
    {
        isQueueCheatId = false;
        listCheatIdPool.Clear();
        Debug.LogError($"======关闭并清空CheatId队列");
    }
}
#endif