using System;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class TreasureRaidRankView : View<TreasureRaidRankViewController>
    {
        [ComponentBinder("waikuang/ProgressText")] private TextMeshProUGUI progressText;
        [ComponentBinder("waikuang/NoRank")] private Transform noRankTr;

        private Button goBtn;

        public void RefreshRank(uint myRank)
        {
            if (transform == null)
                return;

            bool bHasRank = myRank != 0;
            progressText.gameObject.SetActive(bHasRank);
            noRankTr.gameObject.SetActive(!bHasRank);
            if (bHasRank)
            {
                progressText.SetText($"{myRank}");
            }
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            goBtn = transform.GetComponent<Button>();
            goBtn.onClick.AddListener(OnRankBtnClicked);
        }

        private void OnRankBtnClicked()
        {
            if (viewController.activityTreasureRaid.GetServerNeedCurrentRoundID() > 1)
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidOpenrank);
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidRankPopup))));
            }
        }
    }

    public class TreasureRaidRankViewController : ViewController<TreasureRaidRankView>
    {
        public Activity_TreasureRaid activityTreasureRaid;

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            activityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            base.BindingView(inView, inExtraData, inExtraAsyncData);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventTreasureRaidRefreshRankView>(OnEventRefreshRankView);
        }

        private void OnEventRefreshRankView(EventTreasureRaidRefreshRankView evt)
        {
            view.RefreshRank(evt.myRank);
        }
    }
}