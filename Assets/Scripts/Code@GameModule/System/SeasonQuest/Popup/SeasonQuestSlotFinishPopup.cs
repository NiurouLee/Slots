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
    [AssetAddress("UIQuestSeasonOneFinishSlot")]
    public class SeasonQuestSlotFinishPopup:Popup<SeasonQuestSlotFinishViewController>
    {
        [ComponentBinder("ContinueButton")] 
        public Button continueButton;  
        
        [ComponentBinder("IconContainer")] 
        public Transform iconContainer;

        public SeasonQuestSlotFinishPopup(string address)
            : base(address)
        {
            contentDesignSize = ViewResolution.designSize;
        }

        public override string GetOpenAudioName()
        {
            return "General_GiftBoxOpen";
        }
    }

    public class SeasonQuestSlotFinishViewController : ViewController<SeasonQuestSlotFinishPopup>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        public override async Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>();
            var currentQuest = Client.Get<SeasonQuestController>().GetCurrentQuest();
            extraAssetNeedToLoad.Add($"SlotIcon{currentQuest.AssetId}Group");

            await base.LoadExtraAsyncAssets();

            var assetReference = GetAssetReference($"SlotIcon{currentQuest.AssetId}Group");
            var icon = assetReference.InstantiateAsset<GameObject>();
            icon.transform.SetParent(view.iconContainer,false);
        }
        
        public void OnContinueButtonClicked()
        {
            SoundController.PlayButtonClick();
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(SeasonQuestPopup),"SlotFinish")));
            view.Close();
        }
    }
}