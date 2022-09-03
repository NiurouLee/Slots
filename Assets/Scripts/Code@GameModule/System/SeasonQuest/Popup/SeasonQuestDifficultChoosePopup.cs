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

using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIQuestSeasonOneDifficultyChoice")]
    public class SeasonQuestDifficultChoosePopup : Popup
    {
        [ComponentBinder("Root/MainGroup/EasyGroup/DifficultyButton")] 
        private Button easeChoose;
        
        [ComponentBinder("Root/MainGroup/MediumGroup/DifficultyButton")] 
        private Button mediumChoose;
        
        [ComponentBinder("Root/MainGroup/HardGroup/DifficultyButton")] 
        private Button hardChoose;  
        
        [ComponentBinder("Root/TopGroup/TitleGroup/NumberText")] 
        private Text phaseNumText;
        
        [ComponentBinder("Root/MainGroup/EasyGroup/IntergalText")] 
        private TMP_Text easyGroupCoinText;
        
        [ComponentBinder("Root/MainGroup/MediumGroup/IntergalText")] 
        private TMP_Text mediumGroupCoinText;
        
        [ComponentBinder("Root/MainGroup/HardGroup/IntergalText")] 
        private TMP_Text hardGroupCoinText;
        
        [ComponentBinder("Root/MainGroup/EasyGroup/StarText")] 
        private TMP_Text easyGroupStarText;
        
        [ComponentBinder("Root/MainGroup/MediumGroup/StarText")] 
        private TMP_Text mediumGroupStarText;
        
        [ComponentBinder("Root/MainGroup/HardGroup/StarText")] 
        private TMP_Text hardGroupStarText;

        private SGetSeasonQuestDifficultyRewards _sGetSeasonQuestDifficultyRewards;
        
        public SeasonQuestDifficultChoosePopup(string address)
            : base(address)
        {
            contentDesignSize = ViewResolution.designSize;
        }
        
        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
          
            easeChoose.onClick.AddListener(OnChooseEaseClicked);
            mediumChoose.onClick.AddListener(OnChooseMediumClicked);
            hardChoose.onClick.AddListener(OnChooseHardClicked);
            
            InitRewardUI();
        }

        protected void OnChooseEaseClicked()
        {
            ChooseDifficulty(1);
        }
        
        protected void OnChooseMediumClicked()
        {
            ChooseDifficulty(2);
        }
        
        protected void OnChooseHardClicked()
        {
            ChooseDifficulty(3);
        }

        protected void ChooseDifficulty(uint difficulty)
        {
            easeChoose.interactable = false;
            mediumChoose.interactable = false;
            hardChoose.interactable = false;

            Client.Get<SeasonQuestController>().ChooseDifficulty(difficulty, (success) =>
            {
                EventBus.Dispatch(new EventSeasonQuestDifficultyChose());
                Close();
            });
        }
         
        protected override void SetUpController(object inExtraData, object inExtraAsyncData = null)
        {
            if(inExtraAsyncData != null)
                _sGetSeasonQuestDifficultyRewards = inExtraAsyncData as SGetSeasonQuestDifficultyRewards;
            
        }

        protected void InitRewardUI()
        {
            TMP_Text[] text = {easyGroupCoinText, mediumGroupCoinText, hardGroupCoinText};
            TMP_Text[] startText = {easyGroupStarText, mediumGroupStarText, hardGroupStarText};

            for (var i = 0; i < text.Length; i++)
            {
                var coinItem = XItemUtility.GetItem(_sGetSeasonQuestDifficultyRewards.Rewards[i].Items,
                    Item.Types.Type.Coin);
                var starItem = XItemUtility.GetItem(_sGetSeasonQuestDifficultyRewards.Rewards[i].Items,
                    Item.Types.Type.SeasonQuestStar);
                text[i].text = coinItem.Coin.Amount.GetAbbreviationFormat();
                startText[i].text = starItem.SeasonQuestStar.Amount.GetAbbreviationFormat();
            }

            phaseNumText.text = (Client.Get<SeasonQuestController>().GetCurrentPhrase() + 1).ToString();
        }
        
       
    }
}