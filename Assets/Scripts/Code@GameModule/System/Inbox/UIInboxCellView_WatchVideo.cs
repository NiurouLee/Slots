using System.Collections.Generic;
using DragonPlus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIInboxCellView_WatchVideo : UIInboxCellView
    {
        [ComponentBinder("DetailGroup/IntegralGroup/IntegralText")]
        public TMP_Text tmpTextIntegral;

        [ComponentBinder("WatchGroup/WatchButton")]
        public Button button;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            AdRewardedVideoPlacementMonitor.Bind(button.gameObject, eAdReward.InboxRV.ToString());
        }

        public override void Set(InboxItem itemData)
        {
            base.Set(itemData);
            if (tmpTextIntegral != null)
            {
                var count = ADSController.GetFirstBonusCount(eAdReward.InboxRV);
                var coin = ADSController.GetRewardCoin(count);
                tmpTextIntegral.text = coin.GetCommaFormat();
            }

            if (button != null)
            {
                button.interactable = true;
            }
        }

        private int WeightedRandom(int[] values, int[] weights)
        {
            if (values == null || weights == null || values.Length != weights.Length)
            {
                return 0;
            }

            var length = values.Length;
            int total = 0;
            for (int i = 0; i < length; i++)
            {
                total += weights[i];
            }

            var random = Random.Range(0, total);
            for (int i = 0; i < length; i++)
            {
                var weight = weights[i];
                if (random <= weight)
                {
                    return values[i];
                }

                random = random - weight;
            }

            return 0;
        }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnButtonClick);
                AdRewardedVideoPlacementMonitor.Bind(button.gameObject, eAdReward.InboxRV.ToString());
            }
        }

        protected void OnButtonClick()
        {
            if (!ADSController.ShouldShowRV(eAdReward.InboxRV))
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
                if (button != null)
                {
                    button.interactable = true;
                }
            }
            else
            {
                if (button != null)
                {
                    button.interactable = false;
                }

                ShowRV();
            }
        }

        private void ShowRV()
        {
            var count = ADSController.GetFirstBonusCount(eAdReward.InboxRV);
            var reward = ADSController.GetRewardCoin(count);
            var arg2 = ADSController.GetArg2(eAdReward.InboxRV);
            if (arg2 == null || arg2.Count == 0 || arg2.Count % 2 != 0)
            {
                return;
            }

            var length = arg2.Count / 2;
            var ratios = new int[length];
            var weights = new int[length];
            for (int i = 0; i < length; i++)
            {
                ratios[i] = arg2[i * 2];
                weights[i] = arg2[i * 2 + 1];
            }

            var ratio = WeightedRandom(ratios, weights);
            var coin = (ulong) ((double) reward * (double) ratio / 100);

            ADSController.TryShowRewardedVideoWithCollectRewardPopup(
                eAdReward.InboxRV,
                coin,
                coin,
                eAdReward.InboxRV.ToString(),
                async (b) =>
                {
                    EventBus.Dispatch(new EventInBoxItemUpdated());

                    if (button != null)
                    {
                        button.interactable = true;
                    }

                    XDebug.Log("111111111111111111 inbox rv finish");

                    ADSController.ShowGetMorePopup(eAdReward.InboxRV, reward, () =>
                    {
                        XDebug.Log("111111111111111111 GetMorePopup click");
                        ShowRV();
                    });
                });
        }
    }
}