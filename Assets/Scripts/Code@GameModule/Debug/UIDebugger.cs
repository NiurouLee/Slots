#if UNITY_EDITOR || !PRODUCTION_PACKAGE
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Code;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Storage;
using GameModule;
using ILRuntime.Runtime;
using LitJson;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ErrorCode = DragonU3DSDK.Network.API.Protocol.ErrorCode;


// public enum UIDebugerType
// {
//     Lobby,
//     Game,
//     Quick,
// }

[System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
public class UIDebuggerAttribute : Attribute
{
    public string mName = string.Empty;
    public int priority;

    public UIDebuggerAttribute(string name, int inPriority = 10)
    {
        mName = name;
        priority = inPriority;
    }
}

public class UIDebugger:LogicController
{
#if UNITY_EDITOR
    public static bool sandboxIapEnabled = true;
#else
    public static bool sandboxIapEnabled = false;
#endif

    public static bool isSilent = false;
 
    public List<UIDebuggerElement> debuggerElementList;
    // public static List<string> elementArrayList = new List<string>()
    // {
    //     "Cheat",
    //     "数据重播",
    //     "设置金币数量",
    //     "打开SRDebug",
    //     "变快",
    //     "变慢",
    //     "游戏内重启",
    //     "显示/隐藏FPS",
    //     "开启/关闭模拟支付",
    //     "上传本地存档",
    //     "播放测试插屏广告",
    //     "播放测试激励广告",
    //     "测试打开广告遮罩",
    //     "打印玩家GroupId",
    //     "开关 API log",
    //     "拉取服务器上的Storage数据",
    //     "打印本地Storage Common数据",
    //     "开关左上角的玩家信息",
    //     "打印版本相关信息",
    // };

    public UIDebugger(Client client)
        : base(client)
    {
       
    }

    private Transform transform;
    private GameObject gameObject;
    private Canvas canvas;
    private bool isInited = false;

    [ComponentBinder("GroupDisplay")] protected CanvasGroup GroupDisplay;

    [ComponentBinder("GroupDisplay/btnShow")] protected Button btnShow;

    [ComponentBinder("GroupDisplay/sliderAlpha")] private Slider sliderAlpha;
    
    [ComponentBinder("GroupDisplay/ElementShowGroup")] private RectTransform elementShowGroup;
    

    protected void InitElementArray()
    {
        debuggerElementList = new List<UIDebuggerElement>();
        
        Type typeMy = this.GetType();
        var listMethodInfo = typeMy.GetMethods();
        var listFieldInfo = typeMy.GetFields();
        Type typeAttr = typeof(UIDebuggerAttribute);
        
        for (int i = 0; i < listMethodInfo.Length; i++)
        {
            MethodInfo methodInfo = listMethodInfo[i];
            var objAttr = methodInfo.GetCustomAttributes(typeAttr, false);
            UIDebuggerAttribute attr = null;
            if (objAttr.Length > 0)
            {
                attr = objAttr[0] as UIDebuggerAttribute;
            }

            if (attr != null)
            {
                var normalDebuggerElement = new UIDebuggerElement(attr.mName, attr.priority, () => { methodInfo.Invoke(this,  null); });
                normalDebuggerElement.InitContext(transform);
                debuggerElementList.Add(normalDebuggerElement);
            }
        }
        
        for (int i = 0; i < listFieldInfo.Length; i++)
        {
            FieldInfo fieldInfo = listFieldInfo[i];
            var objAttr = fieldInfo.GetCustomAttributes(typeAttr, false);
            UIDebuggerAttribute attr = null;
            if (objAttr.Length > 0)
            {
                attr = objAttr[0] as UIDebuggerAttribute;
            }

            if (attr != null)
            {
                var specialDebuggerElement = (UIDebuggerElement)fieldInfo.GetValue(this);
                specialDebuggerElement.InitContext(transform);
                debuggerElementList.Add(specialDebuggerElement);
            }
        }
        
        debuggerElementList.Sort((a, b) =>
        {
            return  a.priority - b.priority;
        });
        
        ArrayElementsPosition();
    }

    public override void CleanUp()
    {
        canvas = GameObject.Find("Launcher/HighPriorityUIContainerCanvas").GetComponent<Canvas>();
        if (canvas.transform.Find("UIDebuger"))
        {
            gameObject = canvas.transform.Find("UIDebuger").gameObject;
            GameObject.Destroy(gameObject);
        }

        base.CleanUp();
    }

    protected override void SubscribeEvents()
    {
        base.SubscribeEvents();
        SubscribeEvent<EventSceneSwitchEnd>(SceneSwitchCallback, 1);
        SubscribeEvent<EventSilentSceneSwitchEnd>(SceneSwitchCallback, 1);
    }

    public void ArrayElementsPosition()
    {
        int totalElementCount = debuggerElementList.Count;
        float width = (float)Math.Ceiling(Math.Pow(totalElementCount,0.5f));
        float height = (float)Math.Ceiling(totalElementCount / width);
        float midWidth = (width - 1) / 2;
        float midHeight = (height - 1) / 2;
        float distanceX = 100;
        float distanceY = 100;
        int elementArrayIndex = 0;
       // debuggerElementList.Keys.ToList();
        for (int i = 0; i < debuggerElementList.Count; i++)
        {
              int nowWidth = elementArrayIndex % (int)width;
                int nowHeight = elementArrayIndex / (int)width;
                float tempPosX = (nowWidth - midWidth) * distanceX;
                float tempPosY = (nowHeight - midHeight) * -distanceY;
                var elementLocalPosition = new Vector3(tempPosX,tempPosY,0);
                debuggerElementList[i].SetElementPosition(elementLocalPosition);
                elementArrayIndex++;
            
        }
        elementShowGroup.sizeDelta = new Vector2(width*distanceX + 50,height*distanceY + 50);
    }
    protected async void Init()
    {
        canvas = GameObject.Find("Launcher/HighPriorityUIContainerCanvas").GetComponent<Canvas>();
        if (canvas.transform.Find("UIDebuger"))
        {
            gameObject = canvas.transform.Find("UIDebuger").gameObject;
            GameObject.Destroy(gameObject);
        }

        gameObject = await Addressables.InstantiateAsync("UIDebuger", canvas.transform).Task;
        gameObject.name = "UIDebuger";

        transform = gameObject.transform;

        //transform.parent = canvas.transform;

        ComponentBinder.BindingComponent(this, transform);

        btnShow.onClick.AddListener(OnBtnShowClick);
        //btnClose.onClick.AddListener(OnBtnCloseClick);
        //btnShowQuick.onClick.AddListener(OnBtnShowQuick);
        sliderAlpha.onValueChanged.AddListener(OnSliderAlphaValueChange);
        sliderAlpha.value = PlayerPrefs.GetFloat("UIDebuger_sliderAlpha", 1);

        var dropEventCustomHandler = btnShow.AddComponent<DragDropEventCustomHandler>();
        dropEventCustomHandler.BindingDragAction(OnDrag);
        dropEventCustomHandler.BindingBeginDragAction(OnBeginDrag);
        dropEventCustomHandler.BindingEndDragAction(OnEndDrag);

        elementShowGroup.gameObject.SetActive(false);
        InitElementArray();
     

        isInited = true;

        isSilent = PlayerPrefs.GetInt("Silent", 0) == 1;
    }

    protected bool isDrag = false;
    protected Vector2 mouseOffset;
    protected Vector2 startPos;

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        isDrag = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) GroupDisplay.transform.parent,
            pointerEventData.position, pointerEventData.pressEventCamera, out startPos);
        mouseOffset = (Vector2) (GroupDisplay.transform.localPosition) - startPos;
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        if (isDrag)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) GroupDisplay.transform.parent,
                pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localCursor))
                return;
            GroupDisplay.transform.localPosition = localCursor + mouseOffset;
        }
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        if (!isDrag) return;

        isDrag = false;
        SetPositionInSafeRect();
    }

    public void SceneSwitchCallback(Action handleEndCallback, EventSilentSceneSwitchEnd eventSceneSwitchEnd,
        IEventHandlerScheduler scheduler)
    {
        handleEndCallback?.Invoke();
        if (eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY &&
            eventSceneSwitchEnd.lastSceneType == SceneType.TYPE_LOADING)
        {
            Init();
            return;
        }

        if(isInited)
            SetPositionInSafeRect();
    }

    public void SceneSwitchCallback(Action handleEndCallback, EventSceneSwitchEnd eventSceneSwitchEnd,IEventHandlerScheduler scheduler)
    {
        handleEndCallback?.Invoke();
        if (eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY &&
            eventSceneSwitchEnd.lastSceneType == SceneType.TYPE_LOADING)
        {
            Init();
            return;
        }

        if(isInited)
            SetPositionInSafeRect();
    }
    public void SetPositionInSafeRect()
    {
        Vector2 contentSize = ((RectTransform) btnShow.transform).sizeDelta;
        
        var safeRect = GetScreenSafeRect(contentSize);

        var localPosition = GroupDisplay.transform.localPosition;
        var safePosition = GetSafePosition(safeRect, localPosition);
        // safePosition.x += contentSize.x * 0.8f;
        // safePosition.y -= contentSize.y * 2;
        GroupDisplay.transform.localPosition = safePosition;
        // GroupDisplay.transform.localPosition = localPosition;
    }

    private Rect GetScreenSafeRect(Vector2 contentSize)
    {
        bool isPortrait = ViewManager.Instance.IsPortrait;

        var referenceResolution = ViewResolution.referenceResolutionLandscape;

        if (isPortrait)
        {
            referenceResolution = ViewResolution.referenceResolutionPortrait;
        }

        var controlPanelHeight = MachineConstant.controlPanelHeight;

        var machineUIHeight = MachineConstant.topPanelHeight + MachineConstant.controlPanelHeight;

        if (isPortrait)
        {
            controlPanelHeight = MachineConstant.controlPanelVHeight;
            machineUIHeight = MachineConstant.topPanelVHeight + MachineConstant.controlPanelVHeight;
        }

        var safeRect = new Rect(-referenceResolution.x * 0.5f + contentSize.x * 0.5f,
            -referenceResolution.y * 0.5f + controlPanelHeight + contentSize.y * 0.5f,
            referenceResolution.x - contentSize.x, referenceResolution.y - machineUIHeight - contentSize.y);

        return safeRect;
    }

    private Vector2 GetSafePosition(Rect safeRect, Vector2 localPosition)
    {
        var targetY = localPosition.y;

        if (localPosition.y > safeRect.yMax)
        {
            targetY = safeRect.yMax;
        }
        else if (localPosition.y < safeRect.yMin)
        {
            targetY = safeRect.yMin;
        }

        var targetX = localPosition.x;

        if (localPosition.x < safeRect.xMin)
        {
            targetX = safeRect.xMin;
        }
        else if (localPosition.x > safeRect.xMax)
        {
            targetX = safeRect.xMax;
        }

        return new Vector2(targetX, targetY);
    }

    private void OnSliderAlphaValueChange(float value)
    {
        GroupDisplay.alpha = value;
        PlayerPrefs.SetFloat("UIDebuger_sliderAlpha", value);
    }
    


    private void OnBtnShowClick()
    {
        if (!isDrag)
        {
            elementShowGroup.pivot = GetContainerPivot();
            elementShowGroup.position = GroupDisplay.transform.position;
            elementShowGroup.gameObject.SetActive(!elementShowGroup.gameObject.activeSelf);
        }
    }
    public virtual Vector2 GetContainerPivot()
    {
        if (GroupDisplay != null)
        {
            var btnPosition = GroupDisplay.transform.position;
            var tempPivot = new Vector2();
            if (btnPosition.x >= 0)
            {
                tempPivot.x = 1;
            }
            else
            {
                tempPivot.x = 0;
            }

            if (btnPosition.y >= 0)
            {
                tempPivot.y = 1;
            }
            else
            {
                tempPivot.y = 0;
            }

            return tempPivot;
        }
        return Vector2.zero;
    }


    


    //protected string activeCheatId = string.Empty;




    [UIDebugger("Cheat",1)] 
    public UIDebuggerElementCheat cheatElement = UIDebuggerElementCheat.Instance;
    
    [UIDebugger("数据重播",2)] 
    public UIDebuggerElementSpinRecord spinRecordElement = UIDebuggerElementSpinRecord.Instance;
    
    [UIDebugger("设置金币数量",3)] 
    public UIDebuggerElementSetCoins setCoinsElement = UIDebuggerElementSetCoins.Instance;
   
    [UIDebugger("变快",4)]
    public void ChangeTimeScale()
    {
        Time.timeScale = Time.timeScale > 1 ? 1 : 10;
    }


    [UIDebugger("变慢",5)]
    public void ChangeTimeSlow()
    {
        Time.timeScale = Time.timeScale < 1 ? 1 : 0.01f;
    }
     

    [UIDebugger("游戏内重启",6)]
    public void InGameRestart()
    {
        EventBus.Dispatch(new EventRequestGameRestart());
    }


    [UIDebugger("显示/隐藏FPS",7)]
    public void HideShowFps()
    {
        Camera.main.gameObject.GetComponent<FPSDisplay>().enabled =
            !Camera.main.gameObject.GetComponent<FPSDisplay>().enabled;
    }

    [UIDebugger("开启/关闭模拟支付",8)]
    public void OpenCloseSandboxIap()
    {
        sandboxIapEnabled = !sandboxIapEnabled;
    }


    [UIDebugger("上传本地存档")]
    public void UploadStorage()
    {
        StorageManager.Instance.SaveToLocal();
        StorageManager.Instance.LocalVersion++;
        StorageManager.Instance.SyncForceRemote = true;
    }


  //  [UIDebugger("播放测试插屏广告")]
    public void PlayTestAdInterstitial()
    {
        if (!AdController.Instance.ShouldShowInterstitial(eAdInterstitial.E_Background, false))
        {
            CommonNoticePopup.ShowCommonNoticePopUp("ad interstitial loading...");
        }
        else
        {
            AdController.Instance.TryShowInterstitial(eAdInterstitial.E_Background);
        }
    }

   // [UIDebugger("播放测试激励广告")]
    public void PlayTestAdReward()
    {
        //广告的按钮本身需要绑定 AdRewardedVideoPlacementMonitor.Bind();

        if (!AdController.Instance.ShouldShowRV(eAdReward.Free, false))
        {
            CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
        }
        else
        {
            AdController.Instance.TryShowRewardedVideo(eAdReward.Free, (b, s) =>
            {
                Debug.Log($"PlayTestAdReward state : {b}");
                if (b)
                {
                    //PlayTestAdRewardRequestBonus(eAdReward.Free);
                }
            });
        }
    }


   // [UIDebugger("测试打开广告遮罩")]
    public void OpenAdBlock()
    {
        EventBus.Dispatch(new EventAdStart());
    }

    [UIDebugger("玩家GroupId")]
    public void GetPlayerGroupId()
    {
        Debug.Log($"====GetPlayerGroupId:{AdConfigManager.Instance.MetaData.GroupId}");
    }

    [UIDebugger("开关 API log")]
    public void SwitchAPILog()
    {
        APIManagerGameModule.Instance.isShowResponseJSON = !APIManagerGameModule.Instance.isShowResponseJSON;
    }

   // [UIDebugger("拉取服务器上的Storage数据")]
    public void GetServerStorage()
    {
        StorageManager.Instance.GetOrCreateProfile((isSuccess) =>
        {
            if (isSuccess)
            {
                CommonNoticePopup.ShowCommonNoticePopUp("Get Server Storage Success");
                Debug.Log(
                    $"====StorageCommon:{JsonMapper.ToJson(StorageManager.Instance.GetStorage<StorageCommon>())}");
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("Get Server Storage Fail");
            }
        });
    }


    [UIDebugger("Storage Common数据")]
    public void PrintStorageCommon()
    {
        Debug.Log($"====StorageCommon:{JsonMapper.ToJson(StorageManager.Instance.GetStorage<StorageCommon>())}");
    }


    [UIDebugger("玩家信息")]
    public void PrintAdModelAdRewardConfigs()
    {
        var objShow = GameObject.Find("ShowPlayerInfo");
        if (objShow != null)
        {
            var showPlayerInfo = objShow.GetComponent<ShowPlayerInfo>();
            showPlayerInfo.IsShow = !showPlayerInfo.IsShow;
        }
    }


    [UIDebugger("版本信息")]
    public void PrintVersionInfo()
    {
        Input.multiTouchEnabled = false;
        Debug.LogWarning(
            $"====RootFolderVersion:{BundleFolderSetting.BundleRootFolderName}  ResVersion:{ResVersionManager.ResVersion}  ServerConnectVersion:{ServerVersion.ServerConnectVersion}");
    }
    
    [UIDebugger("安静模式")]
    public void Silent模式()
    {
        isSilent = !isSilent;
        PlayerPrefs.GetInt("Silent", isSilent ? 1 : 0);
    }


    // private async Task PlayTestAdRewardRequestBonus(eAdReward place_id)
    // {
    //     CSpinXClaimAdReward cSpinXClaimAdReward = new CSpinXClaimAdReward();
    //     cSpinXClaimAdReward.PlaceId = (int)place_id;
    //     List<int> rewardeds = AdController.Instance.adModel.GetRewardedVideoBonus(place_id);
    //     foreach (var p in rewardeds)
    //     {
    //         AdBonus bonu;
    //         if (AdController.Instance.adModel.AdBonusConfigs.TryGetValue(p, out bonu))
    //         {
    //             cSpinXClaimAdReward.ItemIdList.Add(bonu.ItemId1);
    //             cSpinXClaimAdReward.ItemCountList.Add(bonu.ItemCnt1);
    //         }
    //     }
    //     var handler = await APIManagerFixHot.Instance.SendAsync<CSpinXClaimAdReward, SSpinXClaimAdReward>(cSpinXClaimAdReward);
    //     if (handler.ErrorCode == ErrorCode.Success)
    //     {
    //         Debug.Log("CSpinXClaimAdReward success");
    //     }
    //     else
    //     {
    //         Debug.Log($"CSpinXClaimAdReward fail : {handler.ErrorCode}");
    //     }
    // }
}
#endif