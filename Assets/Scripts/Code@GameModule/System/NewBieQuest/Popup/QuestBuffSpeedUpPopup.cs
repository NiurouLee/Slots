// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/28/15:58
// Ver : 1.0.0
// Description : QuestBuffSpeedUpPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine.UI;

namespace GameModule 
{
    [AssetAddress("UIQuestBuffSpeedUp")]
    public class QuestBuffSpeedUpPopup: Popup<QuestBuffSpeedUpPopupViewController>
    {
        [ComponentBinder("Root/TopGroup/TimerGroup/TimerText")]
        public TextMeshProUGUI timerText;

        [ComponentBinder("Root/BottomGroup/VideoButton")]
        public Button videoButton;

        public QuestBuffSpeedUpPopup(string address)
            :base(address)
        {
            contentDesignSize = ViewResolution.designSize;
        }
    }
    public class QuestBuffSpeedUpPopupViewController: ViewController<QuestBuffSpeedUpPopup>
    {
        protected override void SubscribeEvents()
        {
            view.videoButton.onClick.AddListener(OnVideoButtonClicked);
        }

        public override void OnViewEnabled()
        {
            EnableUpdate(2);
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventNewbiequestBoostAdPopup);
        }

        public void OnVideoButtonClicked()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventNewbiequestBoostAdWatch);
           
            if (AdController.Instance.ShouldShowRV(eAdReward.NewbieQuestBoost, false))
            {
                view.videoButton.interactable = false;
                AdController.Instance.TryShowRewardedVideo(eAdReward.NewbieQuestBoost, OnWatchRvFinished);
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
            }
        }

        protected async void OnWatchRvFinished(bool success, string reason)
        {
            if (success)
            {
                //Claim RV Buff;
                var bonus = AdController.Instance.adModel.GetRewardedVideoBonus(eAdReward.NewbieQuestBoost);
                if (bonus.Count > 0)
                {
                    var adBonus = AdController.Instance.adModel.GetAdBonus(bonus[0]);
                    if (adBonus != null)
                    {
                        var sClaimAdReward = await AdController.Instance.ClaimRvReward(eAdReward.NewbieQuestBoost,
                            adBonus.itemId1, (ulong) adBonus.itemCnt1);

                        if (sClaimAdReward != null)
                        {
                            await Client.Get<BuffController>().SyncBufferData();
                        }
                    }
                }
                view.Close();
            }
            else
            {
                view.videoButton.interactable = true;
            }
        }
        
        public override void Update()
        {
            var countDown = Client.Get<NewBieQuestController>().GetQuestCountDown();
            if (countDown <= 0)
            {
                view.Close();
                return;
            }

            view.timerText.text = XUtility.GetTimeText(countDown, true);
        }
    }
}