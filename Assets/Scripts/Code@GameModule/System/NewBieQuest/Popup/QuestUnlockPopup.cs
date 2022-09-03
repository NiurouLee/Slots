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
using DragonU3DSDK.Network.BI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIQuestUnlock")]
    public class QuestUnlockPopup : Popup
    {
        [ComponentBinder("Root/MainGroup/Text")] 
        private Text integralText;

        [ComponentBinder("Root/BottomGroup/PlayButton")] 
        private Button confirmButton;  
        
        [ComponentBinder("IconContainer")] 
        private Button iconContainer;
         
        public QuestUnlockPopup(string address)
            : base(address)
        {
          // contentDesignSize = new Vector2(1200,768);
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            
            integralText.text =  Client.Get<NewBieQuestController>().GetTotalPrize().GetCommaFormat();
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);

            Client.Get<NewBieQuestController>().IsNewUnlocked = false;

        }

        public override Vector3 CalculateScaleInfo()
        {
            if (ViewManager.Instance.IsPortrait)
            {
                return new Vector3(0.9f, 0.9f, 0.9f);
            }
            
            return Vector3.one;
        }

        protected  void OnConfirmButtonClicked()
        {
            SoundController.PlayButtonClick();
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventGuideTransferQuest);
            
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(QuestPopup),"Guide")));
            Close();
        }
    }
}