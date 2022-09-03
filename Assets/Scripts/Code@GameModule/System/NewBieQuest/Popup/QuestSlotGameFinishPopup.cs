// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/05/19:28
// Ver : 1.0.0
// Description : QuestSlotGameFinishPopup.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIQuestFinishSlot")]
    public class QuestSlotGameFinishPopup:Popup<QuestSlotGameFinishViewController>
    {
        [ComponentBinder("ContinueButton")] 
        public Button continueButton;  
        
        [ComponentBinder("IconContainer")] 
        public Transform iconContainer;

        public QuestSlotGameFinishPopup(string address)
            : base(address)
        {
            
        }
        
        public override Vector3 CalculateScaleInfo()
        {
            if (ViewManager.Instance.IsPortrait)
            {
                return Vector3.one * 0.9f;
            }

            return Vector3.one;
        }

    }

    public class QuestSlotGameFinishViewController : ViewController<QuestSlotGameFinishPopup>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        public override async Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>();
            var currentQuest = Client.Get<NewBieQuestController>().GetCurrentQuest();
            extraAssetNeedToLoad.Add($"SlotIcon{currentQuest.AssetId}Group");

            await base.LoadExtraAsyncAssets();

            var assetReference = GetAssetReference($"SlotIcon{currentQuest.AssetId}Group");
            var icon = assetReference.InstantiateAsset<GameObject>();
            icon.transform.SetParent(view.iconContainer,false);
        }
        
        public void OnContinueButtonClicked()
        {
            SoundController.PlayButtonClick();
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(QuestPopup),"SlotFinish")));
            view.Close();
        }
    }
}