// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/19/15:49
// Ver : 1.0.0
// Description : LobbySingleIconContainerView.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
     public class LobbySingleIconContainerView : View<LobbySingleIconContainerViewController>
    {
        [ComponentBinder("Loading")] public Transform loadingImage;
        [ComponentBinder("DownloadGroup")] public Transform downloadGroup;
        [ComponentBinder("AttachPoint")] public Transform attachPoint;
        [ComponentBinder("LockGroup")] public Transform lockGroup;
        [ComponentBinder("LayoutGroup")] public Transform layoutGroup;
        [ComponentBinder("Button")] public Button responseButton;
        [ComponentBinder("LevelText")] public TextMeshProUGUI unlockLevelText;
        [ComponentBinder("ProgressBar")] public Slider progressBar;
        [ComponentBinder("ProgressBG")] public Image progressBg;
        [ComponentBinder("ProgressText")] public TextMeshProUGUI progressText;
        [ComponentBinder("Hot")] public Transform hotTag;
        [ComponentBinder("New")] public Transform newTag;
        
        [ComponentBinder("DownloadIcon")] public Transform downloadIcon;
        [ComponentBinder("LockIcon")] public Transform lockIcon;
 
        public int iconIndex = -10;
    }
     
     public class LobbySingleIconContainerViewController:ViewController<LobbySingleIconContainerView>
     {
         private TaskCompletionSource<bool> downloadTask;
         
         public AssetReference iconAssetReference;

         public AssetDependenciesDownloadOperation asyncOperation;

         public GameObject attachedIconGameObject;
         
         protected MachineLogicController machineLogicController;
         
         protected string assetId;
         protected string machineId;

         protected int bannerListViewItemIndex = -10;
         
         public override void OnViewDidLoad()
         {
             base.OnViewDidLoad();
             view.responseButton.onClick.AddListener(OnIconClicked);
             
             machineLogicController = Client.Get<MachineLogicController>();
         }

         protected override void SubscribeEvents()
         {
             base.SubscribeEvents();
             SubscribeEvent<EventMachineAssetDownloadFinished>(OnMachineAssetDownloadFinished);
             SubscribeEvent<EventStartDownloadMachineAsset>(OnStartDownloadMachineAsset);
             SubscribeEvent<EventHighLightMachineEntrance>(OnHighLightMachineEntrance);
             SubscribeEvent<EventMachineDownloadSizeUpdated>(OnMachineDownloadSizeUpdated);
           //  SubscribeEvent<EventCollectGameIndexOnBannerViewList>(OnCollectGameIndexOnBannerViewList);
         }

         protected void OnMachineDownloadSizeUpdated(EventMachineDownloadSizeUpdated evt)
         {
             var unlockLevel = machineLogicController.GetUnlockLevel(machineId);
             var userLevel = Client.Get<UserController>().GetUserLevel();
             var hasHotFlag = machineLogicController.HasHotFlag(machineId);
             var hasNewFlag = machineLogicController.HasNewFlag(machineId);
             
             if (userLevel >= unlockLevel || hasNewFlag || hasHotFlag)
             {
                 view.downloadIcon.gameObject.SetActive(machineLogicController.NeedDownloadAsset(machineId));
             }
         }
         
         // protected void OnCollectGameIndexOnBannerViewList(EventCollectGameIndexOnBannerViewList evt)
         // {
         //     if (!view.transform.gameObject.activeInHierarchy)
         //         return;
         //     
         //     if (machineId == evt.machineId)
         //     {
         //         EventBus.Dispatch(new EventNoticeGameIndexOnBannerViewList(evt.machineId, bannerListViewItemIndex));
         //     }
         // }

         protected void OnHighLightMachineEntrance(EventHighLightMachineEntrance evt)
         {
             if (!view.transform.gameObject.activeInHierarchy)
                 return;
             
             if (evt.machineId == machineId)
             {
                 var canvas = view.transform.gameObject.AddComponent<Canvas>();
                 view.transform.gameObject.AddComponent<GraphicRaycaster>();
                 
                 canvas.overrideSorting = true;
                 canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
                 canvas.sortingOrder = 1;
                 
                 EventBus.Dispatch(new EventOnMachineEntranceHighlight(view.transform));
             }
         }

         protected void OnStartDownloadMachineAsset(EventStartDownloadMachineAsset evt)
         {
             // if (!view.transform.gameObject.activeInHierarchy)
             //     return;
             
             if(attachedIconGameObject == null)
                 return;
             
             if (assetId == evt.machineAssetId)
             {
                 asyncOperation = machineLogicController.GetDownloadingAssetAsyncOperation(assetId);
                 view.downloadIcon.gameObject.SetActive(true);
                 
                 if (asyncOperation != null)
                 {
                     view.responseButton.interactable = false;
                     view.downloadGroup.gameObject.SetActive(true);
                     EnableUpdate();
                 }
             }
         }
         
         protected void OnMachineAssetDownloadFinished(EventMachineAssetDownloadFinished evt)
         {
             // if (!view.transform.gameObject.activeInHierarchy)
             //     return;
             
             if(attachedIconGameObject == null)
                 return;

             if (assetId == evt.machineAssetId)
             {
                 //if (!machineLogicController.NeedDownloadAsset(assetId))
                 {
                     view.downloadGroup.gameObject.SetActive(false);
                     view.downloadIcon.gameObject.SetActive(!evt.isSuccess);
                     DisableUpdate();
                     view.responseButton.interactable = true;
                     asyncOperation = null;
                 }
             }
         }

         public void LoadIcon(Action finishCallback)
         {
             if (iconAssetReference != null)
             {
                 iconAssetReference.ReleaseOperation();
                 iconAssetReference = null;
             }

             iconAssetReference = AssetHelper.PrepareAsset<GameObject>($"Icon{assetId}",
                 reference =>
                 {
                     if (view.transform == null)
                     {
                         return;
                     }
                     
                     if (reference != null)
                     {
                         finishCallback.Invoke();
                     }
                 });

             AssetHelper.DownloadDependencies($"Loading{assetId}", null,true);
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
                     view.progressText.text = $"{(int) (Math.Min(status.Percent,0.99f) * 100)}%";
                 }
                 else
                 {
                     if (!machineLogicController.NeedDownloadAsset(assetId))
                     {
                         view.downloadGroup.gameObject.SetActive(false);
                         view.downloadIcon.gameObject.SetActive(false);
                         DisableUpdate();
                         view.responseButton.interactable = true;
                     }
                 }
             }
             else
             {
                 DisableUpdate();
             }
         }
         
         protected void OnIconClicked()
         {
             XDebug.Log("IconClicked");
             
             if (view.lockIcon.gameObject.activeSelf)
             {
                 var unlockLevel = machineLogicController.GetUnlockLevel(assetId);
                 view.unlockLevelText.text = "LEVEL " + unlockLevel;
               
                 if(!view.lockGroup.gameObject.activeSelf)
                     view.lockGroup.gameObject.SetActive(true);

                 var animator = view.lockGroup.GetComponent<Animator>();
                 animator.keepAnimatorControllerStateOnDisable = true;

                 if (animator != null)
                 {
                     animator.Play("ShowLock",0,0);
                 }
                
                 return;
             }
             
             //大厅关卡的AssetID == machineId,这里先这些设置
             
             if (machineLogicController.NeedDownloadAsset(assetId))
             {
                 machineLogicController.DownloadMachineAsset(assetId);
                 view.responseButton.interactable = false;
                 return;
             }
             
             Client.Get<MachineLogicController>().EnterGame(machineId, "", "Lobby");
         }

         public  void UpdateContent(int inBannerListViewItemIndex, int iconIndex)
         {
             //小优化，如果内容没法发生变化就不更新内容了
             if (bannerListViewItemIndex == inBannerListViewItemIndex && iconIndex == view.iconIndex &&
                 attachedIconGameObject != null)
             {
                 return;
             }
             
             bannerListViewItemIndex = inBannerListViewItemIndex;
             
             view.iconIndex = iconIndex;
              
             machineId = machineLogicController.GetGameIdByIndex(view.iconIndex);
             assetId = machineId;
             
             CleanUpOldContent();

             LoadIcon(() =>
             {
                 if (iconIndex != view.iconIndex)
                 {
                     return;
                 }

                 if (attachedIconGameObject != null)
                 {
                     GameObject.Destroy(attachedIconGameObject);
                     attachedIconGameObject = null;
                 }

                 if (iconAssetReference == null || !iconAssetReference.GetOperationHandle().IsDone)
                 {
                     return;
                 }
                 
                 attachedIconGameObject = iconAssetReference.InstantiateAsset<GameObject>();
                 attachedIconGameObject.transform.SetParent(view.attachPoint, false);
                 
                 view.loadingImage.gameObject.SetActive(false);
                 
                 var hasComingSoonFlag = machineLogicController.HasComingSoonFlag(machineId);

                 if (attachedIconGameObject != null)
                 {
                     var comingSoonIcon = attachedIconGameObject.transform.Find("Root/IconComingSoon");
                     
                     if (comingSoonIcon != null)
                     {
                         comingSoonIcon.gameObject.SetActive(hasComingSoonFlag);
                     }
                 }

                 if (hasComingSoonFlag)
                 {
                     return;
                 }
                 
                 var hasHotFlag = machineLogicController.HasHotFlag(machineId);
                 var hasNewFlag = machineLogicController.HasNewFlag(machineId);

                
                 view.layoutGroup.gameObject.SetActive(true);
                 view.newTag.gameObject.SetActive(hasNewFlag);
                 view.hotTag.gameObject.SetActive(hasHotFlag);

                 view.responseButton.interactable = true;

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

                 if (userLevel < unlockLevel && !hasHotFlag && !hasNewFlag)
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
                             view.responseButton.interactable = false;
                             view.downloadGroup.gameObject.SetActive(true);
                             EnableUpdate();
                         }
                     }
                 }
             });
         }
         public void CleanUpOldContent()
         {
             if (iconAssetReference != null)
             {
                 iconAssetReference.ReleaseOperation();
                 iconAssetReference = null;
             }
             
             view.newTag.gameObject.SetActive(false);
             view.hotTag.gameObject.SetActive(false);
             
             view.loadingImage.gameObject.SetActive(true);
             view.downloadGroup.gameObject.SetActive(false);
             view.lockGroup.gameObject.SetActive(false);
             view.layoutGroup.gameObject.SetActive(false);
             
             view.downloadIcon.gameObject.SetActive(false);
             view.lockIcon.gameObject.SetActive(false);

             view.responseButton.interactable = false;
             
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
             if (iconAssetReference != null)
             {
                 iconAssetReference.ReleaseOperation();
                 iconAssetReference = null;
             }
             
             base.OnViewDestroy();
         }
     } 
}