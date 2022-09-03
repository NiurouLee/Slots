// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/16/17:30
// Ver : 1.0.0
// Description : MachineBannerView.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameModule
{
    public class MachineBannerView:View<MachineBannerViewController>
    {
        [ComponentBinder("Loading")] public Transform loadingImage;
        [ComponentBinder("DownloadGroup")] public Transform downloadGroup;
        [ComponentBinder("AttachPoint")] public Transform attachPoint;
        [ComponentBinder("LockGroup")] public Transform lockGroup;
        [ComponentBinder("LayoutGroup")] public Transform layoutGroup;
        [ComponentBinder("JPGroup")] public Transform jpGroup;
        [ComponentBinder("TitlePoint")] public Transform titlePoint;
        [ComponentBinder("LevelText")] public TextMeshProUGUI unlockLevelText;
        [ComponentBinder("ProgressBar")] public Slider progressBar;
        [ComponentBinder("ProgressBG")] public Image progressBg;
        [ComponentBinder("ProgressText")] public TextMeshProUGUI progressText;
        [ComponentBinder("Hot")] public Transform hotTag;
        [ComponentBinder("New")] public Transform newTag;
        [ComponentBinder("DownloadIcon")] public Transform downloadIcon;
        [ComponentBinder("LockIcon")] public Transform lockIcon;
        [ComponentBinder("Button")] public Button button;
        [ComponentBinder("LayoutGroup/TitlePoint/JPGroup/IntegralGroup/IntegralText")] public TextMeshProUGUI jackpotText;
    }
    public class MachineBannerViewController:ViewController<MachineBannerView>
    {
        public AssetReference bannerAssetReference;

         public AssetDependenciesDownloadOperation asyncOperation;

         public GameObject attachedIconGameObject;

         protected Advertisement advertisement;
 
         protected MachineLogicController machineLogicController;
          
         public override void OnViewDidLoad()
         {
             base.OnViewDidLoad();
             view.button.onClick.AddListener(OnBannerClicked);
             machineLogicController = Client.Get<MachineLogicController>();
         }
         
         protected void OnHighLightMachineEntrance(EventHighLightMachineEntrance evt)
         {
             if (!view.transform.gameObject.activeInHierarchy)
                 return;
             
             if (advertisement != null && evt.machineId == advertisement.Jump)
             {
                 var canvas = view.transform.gameObject.AddComponent<Canvas>();
                 view.transform.gameObject.AddComponent<GraphicRaycaster>();
                 
                 canvas.overrideSorting = true;
                 canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
                 canvas.sortingOrder = 1;
                 
                 EventBus.Dispatch(new EventOnMachineEntranceHighlight(view.transform));
             }
         }

         public void LoadIcon(Action finishCallback)
         {
             if (bannerAssetReference != null)
             {
                 bannerAssetReference.ReleaseOperation();
                 bannerAssetReference = null;
             }
             
             bannerAssetReference = AssetHelper.PrepareAsset<GameObject>("Banner" + advertisement.Jump,
                 reference =>
                 {
                     if (view.transform == null)
                     {
                         return;
                     }
                     if(reference != null)
                        finishCallback.Invoke();
                 });
             
             AssetHelper.DownloadDependencies($"Loading{advertisement.Jump}", null,true);
         }
         
         public override void Update()
         {
             if (asyncOperation != null)
             {
                 var opHandle = asyncOperation.GetOperationHandle();
                 if (opHandle.IsValid())
                 {
                     var status = opHandle.GetDownloadStatus();
                     view.progressBar.value = status.Percent;
                     view.progressBg.fillAmount = 1 - status.Percent;
                     view.progressText.text = $"{(int) (Math.Min(status.Percent, 0.99f) * 100)}%";
                 }
                 else
                 {
                     var assetId = advertisement.Jump;
                     if (!machineLogicController.NeedDownloadAsset(assetId))
                     {
                         view.downloadGroup.gameObject.SetActive(false);
                         view.downloadIcon.gameObject.SetActive(false);
                     //    DisableUpdate();
                         view.button.interactable = true;
                     }
                 }
             }
          

             if (view.titlePoint.gameObject.activeSelf)
             {
                 if (Time.frameCount % 5 == 0)
                 {
                     UpdateJackpot();
                 }
             }
         }

         protected void UpdateJackpot()
         {
             view.jackpotText.text = machineLogicController.GetLobbyJackpotValue(advertisement.Jump).GetCommaFormat();
         }
         
         protected override void SubscribeEvents()
         {
             base.SubscribeEvents();
             SubscribeEvent<EventMachineAssetDownloadFinished>(OnMachineAssetDownloadFinished);
             SubscribeEvent<EventStartDownloadMachineAsset>(OnStartDownloadMachineAsset);
             SubscribeEvent<EventMachineDownloadSizeUpdated>(OnMachineDownloadSizeUpdated);
          //   SubscribeEvent<EventHighLightMachineEntrance>(OnHighLightMachineEntrance);
         }

         protected void OnStartDownloadMachineAsset(EventStartDownloadMachineAsset evt)
         {
             if (!view.transform.gameObject.activeInHierarchy)
                 return;
             
             if (advertisement != null &&  advertisement.Jump == evt.machineAssetId)
             {
                 asyncOperation = machineLogicController.GetDownloadingAssetAsyncOperation(advertisement.Jump);
                 view.downloadIcon.gameObject.SetActive(true);
                 
                 if (asyncOperation != null)
                 {
                     view.button.interactable = false;
                     view.downloadGroup.gameObject.SetActive(true);
                     EnableUpdate();
                 }
             }
         }
         
         protected void OnMachineAssetDownloadFinished(EventMachineAssetDownloadFinished evt)
         {
             if (!view.transform.gameObject.activeInHierarchy)
                 return;
             
             if (advertisement != null && advertisement.Jump == evt.machineAssetId)
             {
                 view.downloadGroup.gameObject.SetActive(false);
                 view.downloadIcon.gameObject.SetActive(false);
                 view.button.interactable = true;
                 asyncOperation = null;
             }
         }
         
         public void OnBannerClicked()
         {
              
             XDebug.Log("IconClicked");
 
             var machineId = advertisement.Jump; 
              

             if (view.lockIcon.gameObject.activeSelf)
             {
                 var unlockLevel = machineLogicController.GetUnlockLevel(advertisement.Jump);

                 view.unlockLevelText.text = "LEVEL " + unlockLevel;
               
                 if(!view.lockGroup.gameObject.activeSelf)
                     view.lockGroup.gameObject.SetActive(true);
                
                 var animator = view.lockGroup.GetComponent<Animator>();
                 
                 if (animator != null)
                 {
                     animator.Play("ShowLock",0,0);
                 }
                 return;
             }
             
             //大厅关卡的AssetID == machineId,这里先这些设置
             var assetId = advertisement.Jump;

             if (machineLogicController.NeedDownloadAsset(assetId))
             {
                 view.downloadGroup.gameObject.SetActive(true);
                 asyncOperation = machineLogicController.DownloadMachineAsset(assetId);
                 view.button.interactable = false;
                 EnableUpdate(0);
                 return;
             }
             
             Client.Get<MachineLogicController>().EnterGame(machineId, "", "Advertisement");
         }

         public void SetUpContent(Advertisement inAdvertisement)
         {
             advertisement = inAdvertisement;
             
             CleanUpOldContent();
             LoadIcon(() =>
             {
                
                 if (advertisement != inAdvertisement)
                 {
                     return;
                 }
                 
                 if (attachedIconGameObject != null)
                 {
                     GameObject.Destroy(attachedIconGameObject);
                     attachedIconGameObject = null;
                 }

                 if (bannerAssetReference == null || !bannerAssetReference.GetOperationHandle().IsDone)
                 {
                     return;
                 }

                 attachedIconGameObject = bannerAssetReference.InstantiateAsset<GameObject>();

                 attachedIconGameObject.transform.SetParent(view.attachPoint, false);

                 string machineId = advertisement.Jump;
                 var assetId = machineId;
                 var hasHotFlag = machineLogicController.HasHotFlag(machineId);
                 var hasNewFlag = machineLogicController.HasNewFlag(machineId);
                 view.loadingImage.gameObject.SetActive(false);
                 view.layoutGroup.gameObject.SetActive(true);
                 view.newTag.gameObject.SetActive(hasNewFlag);
                 view.hotTag.gameObject.SetActive(hasHotFlag);

                 if (machineLogicController.GetLobbyJackpotValue(advertisement.Jump) > 0)
                 {
                     view.titlePoint.gameObject.SetActive(true);
                     EnableUpdate();
                 }

                 view.button.interactable = true;

                 var images = attachedIconGameObject.GetComponentsInChildren<Image>(true);

                 if (images != null && images.Length > 0)
                 {
                     for (int i = 0; i < images.Length; i++)
                     {
                         images[i].raycastTarget = false;
                     }
                 }

                 var unlockLevel = machineLogicController.GetUnlockLevel(machineId);
                 var userLevel = Client.Get<UserController>().GetUserLevel();

                 if (userLevel < unlockLevel && !hasNewFlag && !hasHotFlag)
                 {
                     view.lockIcon.gameObject.SetActive(true);
                 }
                 else
                 {
                     //如果之前正在下载，那么需要继续显示下载进度
                     if (machineLogicController.NeedDownloadAsset(assetId))
                     {
                         asyncOperation = machineLogicController.GetDownloadingAssetAsyncOperation(assetId);
                         view.downloadIcon.gameObject.SetActive(true);

                         if (asyncOperation != null)
                         {
                             view.button.interactable = false;
                             view.downloadGroup.gameObject.SetActive(true);
                             EnableUpdate();
                         }
                     }
                 }
             });
         }

         protected void OnMachineDownloadSizeUpdated(EventMachineDownloadSizeUpdated evt)
         {
             var unlockLevel = machineLogicController.GetUnlockLevel(advertisement.Jump);
             var userLevel = Client.Get<UserController>().GetUserLevel();
             var hasHotFlag = machineLogicController.HasHotFlag(advertisement.Jump);
             var hasNewFlag = machineLogicController.HasNewFlag(advertisement.Jump);
             
             if (userLevel >= unlockLevel || hasNewFlag || hasHotFlag)
             {
                 view.downloadIcon.gameObject.SetActive(machineLogicController.NeedDownloadAsset(advertisement.Jump));
             }
         }
         
         
         public void CleanUpOldContent()
         {
             if (bannerAssetReference != null)
             {
                 bannerAssetReference.ReleaseOperation();
                 bannerAssetReference = null;
             }
             
             view.newTag.gameObject.SetActive(false);
             view.hotTag.gameObject.SetActive(false);
             
             view.loadingImage.gameObject.SetActive(true);
             view.downloadGroup.gameObject.SetActive(false);
             view.lockGroup.gameObject.SetActive(false);
             view.layoutGroup.gameObject.SetActive(false);
             
             view.downloadIcon.gameObject.SetActive(false);
             view.lockIcon.gameObject.SetActive(false);
            
             view.titlePoint.gameObject.SetActive(false);

             view.button.interactable = false;
             
             asyncOperation = null;
             
             DisableUpdate();
             
             if (attachedIconGameObject != null)
             {
                 GameObject.Destroy(attachedIconGameObject);
                 attachedIconGameObject = null;
             }
         }

         public override void OnViewDestroy()
         {
             if (bannerAssetReference != null)
             {
                 bannerAssetReference.ReleaseOperation();
                 bannerAssetReference = null;
             }
             
             base.OnViewDestroy();
         }
    }
}