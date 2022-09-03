//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-17 20:46
//  Ver : 1.0.0
//  Description : DailyMissionWin.cs
//  ChangeLog :
//  **********************************************

using System;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class DailyMissionPageItemNormal: DailyMissionPageItemBase
    {
        public Action<int, Action> actionCompleteNow;
        public DailyMissionPageItemNormal()
        {

        }

        [ComponentBinder("GetButton")]
        private async void OnBtnCompleteClicked()
        {
            var popup = await PopupStack.ShowPopup<DailyMissionPurchaseNotice>(null);
            popup.Initialize(nMissionIndex, normalMission.CompleteNeedCostDiamond());
            popup.actionCompleteNow += actionCompleteNow;
            popup.SubscribeFinishAction(() =>
            {
                RefreshMission();
                RefreshUI();
            });
        }
        
        public async void FlyGiftBox(Vector3 worldPos, ViewController viewController)
        {
            var container = animatorGiftBox.transform.parent;
            var startAnchorPos = animatorGiftBox.transform.localPosition;
            ModifyGiftBoxSortingOrder(true);
            XUtility.PlayAnimation(animatorGiftBox, "Fly");
            await XUtility.WaitSeconds(0.4f,viewController);
            SoundController.PlaySfx("CashCrazy_GiftBox_Appear");
            await XUtility.WaitSeconds(0.18f,viewController);
            var targetWorldPos = new Vector3(worldPos.x, worldPos.y, worldPos.z);
            Vector3 targetLocalPos = container.InverseTransformPoint(targetWorldPos);
            animatorGiftBox.transform.DOLocalMove(targetLocalPos, 0.59f);
            await XUtility.WaitSeconds(0.8f,viewController);
            SoundController.PlaySfx("CashCrazy_GiftBox_Open");
            await XUtility.WaitSeconds(2f,viewController);
            ModifyGiftBoxSortingOrder(false);
            animatorGiftBox.transform.gameObject.SetActive(false);
            animatorGiftBox.transform.localPosition = new Vector3(startAnchorPos.x, startAnchorPos.y);
        }
        public void ModifyGiftBoxSortingOrder(bool toTop)
        {
            if (toTop)
            {
                canvasGiftBox.gameObject.SetActive(true);
                canvasGiftBox.overrideSorting = true;
                canvasGiftBox.sortingOrder = 8;
                canvasGiftBox.sortingLayerID = SortingLayer.NameToID("SystemPopup");
                return;
            }
            canvasGiftBox.overrideSorting = false;
            canvasGiftBox.sortingOrder = 1;
            canvasGiftBox.sortingLayerID = SortingLayer.NameToID("UI");
        }
    }
}
