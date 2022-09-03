// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/12/07/11:29
// Ver : 1.0.0
// Description : FiveOfKindView.cs
// ChangeLog :
// **********************************************

using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class FiveOfKindView : TransformHolder
    {
        [ComponentBinder("Root")]
        private RectTransform uiRoot;

        private bool needBreak = false;
        
        public FiveOfKindView(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this,transform);
            uiRoot.gameObject.SetActive(false);
        }

        public async void ShowFiveOfKind()
        {
            var moveX = uiRoot.sizeDelta.x;
 
            uiRoot.DOKill();
            
            uiRoot.gameObject.SetActive(true);
            var anchoredPosition = uiRoot.anchoredPosition;
            uiRoot.anchoredPosition = new Vector2(0, anchoredPosition.y);
            
            uiRoot.DOAnchorPosX(-moveX, 0.3f).SetEase(Ease.InBack);
            
            
            transform.gameObject.GetComponent<Animator>().Play("Open",-1,0);
            
            await context.WaitSeconds(3.5f);
            uiRoot.DOAnchorPosX(0,0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                uiRoot.gameObject.SetActive(false);
            });
        }

        public void HideFiveOfKind()
        {
            // var moveX = uiRoot.sizeDelta.x;
            // var distance = (-moveX - uiRoot.anchoredPosition.x);
            // float waitTime = distance / moveX * 0.3f;
            //
            // if (waitTime > 0)
            // {
            //     context.WaitSeconds(waitTime,
            //         () =>
            //         {
            //             uiRoot.DOAnchorPosX(0, 0.10f).SetEase(Ease.OutQuad).OnComplete(() =>
            //             {
            //                 uiRoot.gameObject.SetActive(false);
            //             });
            //         });
            // }
            // else
            // {
                uiRoot.DOAnchorPosX(0, 0.10f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    uiRoot.gameObject.SetActive(false);
                });
           // }
        }
    }
}