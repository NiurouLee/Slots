// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
//
// namespace GameModule.UI
// {
//     public class RewardKeyDialog : BaseDialog
//     {
//         private TextMeshProUGUI vipTxt;
//         private TextMeshProUGUI baseTxt;
//         private TextMeshProUGUI baseWasTxt;
//         private TextMeshProUGUI deltaTxt;
//         private Button collectBtn;
//         private Image lineImg;
//
//         public RewardKeyDialog(string url) : base(url)
//         {
//
//         }
//
//         public override void BindingViewVariable()
//         {
//             base.BindingViewVariable();
//
//             vipTxt = transform.Find("VipTxt").GetComponent<TextMeshProUGUI>();
//             baseTxt = transform.Find("BaseTxt").GetComponent<TextMeshProUGUI>();
//             baseWasTxt = transform.Find("BaseTxt1").GetComponent<TextMeshProUGUI>();
//             deltaTxt = transform.Find("DeltaTxt").GetComponent<TextMeshProUGUI>();
//             collectBtn = transform.Find("Collect").GetComponent<Button>();
//             lineImg = transform.Find("BaseXian").GetComponent<Image>();
//
//             collectBtn.onClick.AddListener(OnCollectClick);
//
//             UpdateContent();
//
//             if (ViewManager.Instance.IsPortrait)
//             {
//                 //Transform ui = transform.Find("UI");
//                 this.transform.localScale = new Vector3(0.8f, 0.8f, 1);
//             }
//         }
//
//         private void UpdateContent()
//         {
//             FeatureItemRewardKeyChipsSettlement rewardkeyChipSettlement = Client.Inbox.rewardkeyChipSettlement;
//             vipTxt.text = rewardkeyChipSettlement.vipMultiple + "X";
//             baseTxt.text = rewardkeyChipSettlement.GetBaseDescText();
//             deltaTxt.text = rewardkeyChipSettlement.GetDescText();
//
//             Vector2 pos = baseTxt.transform.localPosition;
//             pos.x = baseTxt.preferredWidth / 2 + 40;
//             baseTxt.transform.localPosition = pos;
//             
//             lineImg.transform.localPosition = pos + new Vector2(13, 0);
//             lineImg.rectTransform.sizeDelta = new Vector2(baseTxt.preferredWidth, 7);
//             
//             baseWasTxt.transform.localPosition = baseTxt.transform.localPosition - new Vector3(baseTxt.preferredWidth, 0, 0);
//         }
//
//         private void OnCollectClick()
//         {
//             collectBtn.interactable = false;
//             FeatureItemRewardKeyChipsSettlement rewardkeyChipSettlement = Client.Inbox.rewardkeyChipSettlement;
//             EventBus.Dispatch(new EventBalanceUpdate(rewardkeyChipSettlement.settlement.Delta, "RewardKey", true, true));
//             FlyUtils.Fly(collectBtn.transform, () => { OnCloseClickedNoSound(); Client.Inbox.ReleaseEndCallBack(); });
//         }
//     }
// }