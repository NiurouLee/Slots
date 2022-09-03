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
    [AssetAddress("UIQuestSeasonOneNotice")]
    public class SeasonQuestSeasonStartPopup : Popup<SeasonQuestSeasonStartPopupViewController>
    {
        [ComponentBinder("PlayButton")] public Button playButton;
        [ComponentBinder("TimerText")] public TMP_Text timerText;

        public SeasonQuestSeasonStartPopup(string address)
            : base(address)
        {

        }
    }

    public class SeasonQuestSeasonStartPopupViewController : ViewController<SeasonQuestSeasonStartPopup>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.playButton.onClick.AddListener(OnConfirmButtonClicked);

            view.timerText.text = XUtility.GetTimeText(Client.Get<SeasonQuestController>().GetQuestCountDown()).ToUpper();
            Client.Get<SeasonQuestController>().seasonQuestNewUnlocked = false;
        }

        public override void Update()
        {
            var countDown = Client.Get<SeasonQuestController>().GetQuestCountDown();

            if (countDown <= 0)
            {
                view.Close();
            }
            else
            {
                view.timerText.text = XUtility.GetTimeText(Client.Get<SeasonQuestController>().GetQuestCountDown())
                    .ToUpper();
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            EnableUpdate(2);
        }

        protected  void OnConfirmButtonClicked()
        {
            view.playButton.interactable = false;
            
            SoundController.PlayButtonClick();
            
            //  BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventGuideTransferQuest);
            
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(SeasonQuestPopup),"Season_Start")));
            
            WaitForSeconds(1, () =>
            {
                view.Close();
            });
        }
    }
}