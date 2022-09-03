// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/19/21:16
// Ver : 1.0.0
// Description : MachineUnlockView.cs
// ChangeLog :
// **********************************************

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UILevelUpUnlockSlotNotice")]
    public class MachineUnlockView : View
    {
        [ComponentBinder("SpinButton")] private Button spinButton;

        [ComponentBinder("SlotIconPoint")] private Transform slotIconPoint;

        [ComponentBinder("Root")] private RectTransform rootNode;

        private string machineId;
        private Vector3 hidePosition;

        public Vector2 horizontalStartPosition = new Vector2(380, 170);
        public Vector2 horizontalStopPosition = new Vector2(380, -70);

        public Vector2 verticalStartPosition = new Vector2(220, 170);
        public Vector2 verticalStopPosition = new Vector2(220, -60);
        public Vector3 verticalScale = new Vector3(0.8f, 0.8f, 0.8f);

        public Vector2 startPosition;
        public Vector2 stopPosition;

        public bool animationFinished = false;

        protected View slotIconView;

        public MachineUnlockView(string address)
            :base(address)
        {
            
        }
        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            spinButton.onClick.AddListener(OnPlayButtonClicked);
        }

        public void OnPlayButtonClicked()
        {
            Client.Get<MachineLogicController>().EnterGame(machineId, "", "MachineUnlock");
        }
        
        public async void ShowUnlockMachineTip(string inMachineId, bool isVertical)
        {
            animationFinished = false;
            if (isVertical)
            {
                startPosition = verticalStartPosition;
                stopPosition = verticalStopPosition;
                rootNode.localScale = verticalScale;
            }
            else
            {
                startPosition = horizontalStartPosition;
                stopPosition = horizontalStopPosition;
                rootNode.localScale = Vector3.one;
            }

            machineId = inMachineId;
            rootNode.anchoredPosition = startPosition;

            if (slotIconView != null)
            {
                slotIconView.Destroy();
            }
          
            await AssetHelper.PrepareAssetAsync<GameObject>("Loading" + inMachineId);
           
            slotIconView  = await CreateView<View>("Icon" + inMachineId, slotIconPoint);

          
            transform.gameObject.SetActive(true);
            DOTween.To(() => rootNode.anchoredPosition, (x => rootNode.anchoredPosition = x),
                stopPosition, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                DOVirtual.DelayedCall(3, HideUnlockMachineTip);
            });
        }

        public void HideUnlockMachineTip()
        {
            if (transform != null)
            {
                DOTween.To(() => rootNode.anchoredPosition, (x => rootNode.anchoredPosition = x),
                        startPosition, 0.25f)
                    .SetEase(Ease.InBack).OnComplete(
                        () =>
                        {
                            Hide();
                            animationFinished = true;
                        });
            }
        }
    }
}