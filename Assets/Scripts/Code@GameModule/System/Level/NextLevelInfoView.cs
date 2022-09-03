// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/20/17:05
// Ver : 1.0.0
// Description : NextLevelInfoView.cs
// ChangeLog :
// **********************************************

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    [AssetAddress("UILevelUpBubble")]
    public class NextLevelInfoView: View
    {
        [ComponentBinder("LevelExpNumberText")]
        public TextMeshProUGUI levelExpNumberText;

        [ComponentBinder("IntegralText")]
        public TextMeshProUGUI integralText;
        
        [ComponentBinder("Root")]
        public RectTransform rootNode;
        
        [ComponentBinder("Content")]
        public RectTransform contentNode;
        
        public Vector2 horizontalPosition = new Vector2(320, -80);
        public Vector2 verticalPosition = new Vector2(230, -60);
        public Vector3 verticalScale = new Vector3(0.7f,0.7f,0.7f);


        public long showActionId = 0;
        
        public NextLevelInfoView(string address)
        :base(address)
        {
            
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            if (ViewManager.Instance.IsPortrait)
            {
                rootNode.anchoredPosition = verticalPosition;
                rootNode.localScale = verticalScale;
            }
            else
            {
                rootNode.anchoredPosition = horizontalPosition;
            }

            contentNode.anchoredPosition = new Vector2(0, 300);
            
            var eventCustomHandler = transform.gameObject.AddComponent<SelectEventCustomHandler>();
            
            eventCustomHandler.BindingDeselectedAction(eventData =>
            {
                HideLevelInfoView();
            });
        }
        
        public void UpdateViewInfo()
        {
            var userLevelInfo = Client.Get<UserController>().GetUserLevelInfo();

            levelExpNumberText.text = userLevelInfo.ExpNextLevel.GetCommaFormat();
            integralText.text = userLevelInfo.LevelReward.GetCommaFormat();
        }

        public void ShowLevelInfoView()
        {
            UpdateViewInfo();
            Show();

            showActionId++;

            if (showActionId > 1000)
            {
                showActionId = 0;
            }

            long localShowAction = showActionId;
           
            DOTween.To(() => contentNode.anchoredPosition, (x => contentNode.anchoredPosition = x), Vector2.zero, 0.5f)
                .SetEase(Ease.OutBack).OnComplete(
                    () =>
                    {
                        DOVirtual.DelayedCall(3, ()=>
                        {
                            if(showActionId == localShowAction)
                                HideLevelInfoView();
                        });
                    });
             
            EventSystem.current.SetSelectedGameObject(transform.gameObject);
        }

        private Tween hideTween;
        public void HideLevelInfoView()
        {
            if (transform && hideTween == null)
            {
                hideTween = DOTween.To(() => contentNode.anchoredPosition, (x => contentNode.anchoredPosition = x),
                        new Vector2(0, 300), 0.25f)
                    .SetEase(Ease.InBack).OnComplete(
                        () =>
                        {
                            Hide();
                            hideTween = null;
                        });
            }
        }
    }
}