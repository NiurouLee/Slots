using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace GameModule
{
    public class ShopEntranceView11301 : TransformHolder
    {
        private Transform BoxText;
        private Transform Tips;
        private bool IsOpen = false;
        public bool goldIsFlying = false;

        public ShopEntranceView11301(Transform inTransform) : base(inTransform)
        {
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            var pointEvent = transform.GetComponent<PointerEventCustomHandler>();
            if (pointEvent == null)
                pointEvent = transform.gameObject.AddComponent<PointerEventCustomHandler>();
            pointEvent.BindingPointerClick(ClickBoxShopCallBack);
            BoxText = transform.Find("TextBG/Text");
            Tips = transform.Find("Tips");
            IsOpen = false;
        }

        public async void ClickBoxShopCallBack(PointerEventData pointerEventData)
        {
            if (Constant11301.IsShowMapFeature) return;
            if (context.state.Get<AutoSpinState>().IsAutoSpin) return;
            if (Constant11301.IsSpining) return;
            if (context.state.Get<FreeSpinState11301>().IsTriggerFreeSpin) return;
            if (context.state.Get<ReSpinState11301>().ReSpinTriggered) return;
            if (goldIsFlying) return;
            if (IsOpen) return;
            IsOpen = true;
            AudioUtil.Instance.PlayAudioFx("ShopOpen");
            XDebug.Log("正常点击商城入口打开商城");
            context.view.Get<TransitionView11301>().UpdateShopMaskShow(true);
            var UIShop = PopUpManager.Instance.ShowPopUp<UIShopPopUp11301>($"UIShop{context.assetProvider.AssetsId}");
            UIShop.InitShopInfos();
            var collectItems = context.state.Get<ExtraState11301>().GetCollectItems();
            if (collectItems >= Constant11301.MaxTokenNum)
            {
                UIShop.ShowTipsToAutoHide(UIShop.TokenTips);
                UIShop.FullMask.gameObject.SetActive(true);
            }


            UIShop.SetPopUpCloseAction(() =>
            {
                IsOpen = false;
                AudioUtil.Instance.StopMusic();
            });
        }

        /// <summary>
        /// 刷新代币限制值
        /// </summary>
        public void RefreshTokenLimitValue(long num)
        {
            if (num < Constant11301.WarnTokenNum)
                return;
            if (num >= Constant11301.WarnTokenNum && num < Constant11301.MaxTokenNum)
            {
                Tips.transform.Find("Text1").gameObject.SetActive(true);
                Tips.transform.Find("Text2").gameObject.SetActive(false);
            }
            else if (num >= Constant11301.MaxTokenNum)
            {
                Tips.transform.Find("Text1").gameObject.SetActive(false);
                Tips.transform.Find("Text2").gameObject.SetActive(true);
            }

            Constant11301.ShowTipsToAutoHide(Tips);
        }

        /// <summary>
        /// 设置宝箱的代币数量
        /// </summary>
        public void SetBoxTokensNum(long num)
        {
            if (num >= Constant11301.MaxTokenNum)
                num = Constant11301.MaxTokenNum;
            BoxText.GetComponent<TextMeshPro>().text = "" + num.GetCommaFormat();
        }
    }
}