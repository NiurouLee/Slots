// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/05/19:07
// Ver : 1.0.0
// Description : QuestFinishPopup.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIQuestSeasonOneStart")]
    public class SeasonQuestStartPopup : Popup<SeasonQuestStartPopupViewController>
    {
        [ComponentBinder("IntegralText")] 
        private TextMeshProUGUI _integralText;

        [ComponentBinder("StartButton")] 
        private Button _startButton;  
         
        [ComponentBinder("Root/MainGroup/ConditionCell1/ContentText")] 
        private TextMeshProUGUI _descriptionText1;
        
        [ComponentBinder("Root/MainGroup/ConditionCell2/ContentText")] 
        private TextMeshProUGUI _descriptionText2;
        
        [ComponentBinder("Root/MainGroup/ConditionCell3/ContentText")] 
        private TextMeshProUGUI _descriptionText3;
         
        [ComponentBinder("IconContainer")] 
        public Transform iconContainer;

 
        public SeasonQuestStartPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1357, 768);
        }
 
        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            
            _startButton.onClick.AddListener(OnStartButtonClicked);
            
            InitQuestDescriptionUI();
            
            if (iconContainer.childCount > 0)
            {
                int childCount = iconContainer.childCount;

                for (var i = childCount - 1; i >= 0; i--)
                {
                    var child = iconContainer.GetChild(i);
                    GameObject.Destroy(child);
                }
            }
        }
        protected void InitQuestDescriptionUI()
        {
            var missions = Client.Get<SeasonQuestController>().GetCurrentMission();

            var descriptionTexts = new []{_descriptionText1, _descriptionText2, _descriptionText3};
           
            for (var i = 0; i < 3; i++)
            {
                if (i < missions.Count)
                {
                    descriptionTexts[i].text = missions[i].GetContentDescText().Replace("\n"," ");
                }
                else
                {
                    descriptionTexts[i].transform.parent.gameObject.SetActive(false);
                }
            }
        }
        protected void OnStartButtonClicked()
        {
            SoundController.PlayButtonClick();
            Close();
        }
    }

    public class SeasonQuestStartPopupViewController : ViewController<SeasonQuestStartPopup>
    {
        public override Task LoadExtraAsyncAssets()
        {
            var quest = Client.Get<SeasonQuestController>().GetCurrentQuest();
           
            extraAssetNeedToLoad = new List<string>();
            extraAssetNeedToLoad.Add( $"SlotIcon{quest.AssetId}Group");
           
            return base.LoadExtraAsyncAssets();
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            var quest = Client.Get<SeasonQuestController>().GetCurrentQuest();

            var assetReference = GetAssetReference($"SlotIcon{quest.AssetId}Group");

            if (assetReference != null)
            {
                var machineIcon = assetReference.InstantiateAsset<GameObject>();
                var iconAnimator = machineIcon.GetComponentInChildren<Animator>();
                iconAnimator.enabled = false;
                machineIcon.transform.SetParent(view.iconContainer, false);
            }
        }
    }
}