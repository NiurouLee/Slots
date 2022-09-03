//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-02 17:36
//  Ver : 1.0.0
//  Description : UIPaytablePopUp.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using DG.Tweening;
using SRF;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameModule
{
    public class UIPayTablePopUp:MachinePopUp
    {
        private bool canClick = true;
        private const int MAX_CONTAINER = 2;
        private int COUNT_PAGES;
        [ComponentBinder("BG")]
        private RectTransform _rectTransformBG;

        [ComponentBinder("MainGroup")]
        private RectTransform _rectMainGroup;
        private Vector2 vecContentSize;
        
        [ComponentBinder("View")]
        private Transform _transView;
        [ComponentBinder("PagesPool")]
        private Transform _transPagesPool;

        private Transform[] dequePages;
        private Transform[] dequeContainers;
        private int shiftPage = 0;
        public UIPayTablePopUp(Transform transform) :
            base(transform)
        {
            vecContentSize = _rectTransformBG.sizeDelta;
            if (_rectTransformBG.sizeDelta.x < _rectMainGroup.sizeDelta.x)
            {
                vecContentSize = _rectMainGroup.sizeDelta;
            }
            COUNT_PAGES = _transPagesPool.childCount;
            dequePages = new Transform[COUNT_PAGES];
            dequeContainers = new Transform[MAX_CONTAINER];
            for (int i = 0; i < MAX_CONTAINER; i++)
            {
                dequeContainers[i] = _transView.Find("Container" + i);
            }
            for (int i = 0; i < COUNT_PAGES; i++)
            {
                var child = _transPagesPool.GetChild(i);
                dequePages[i] = child;
            }
            for (int i = 0; i < MAX_CONTAINER; i++)
            {
                var page = dequePages[i];
                var container = dequeContainers[i];
                page.gameObject.SetActive(true);
                page.transform.SetParent(container, false);
                container.localPosition = new Vector3(vecContentSize.x*i, 0, 0);
            }
            _transPagesPool.gameObject.SetActive(false);
            
            EventBus.Dispatch(new EventPayTableShowed());

            //AdaptUI();
        }
        
        // protected void AdaptUI()
        // {
        //     if (ViewManager.Instance.IsPortrait)
        //     {
        //         var viewSize = ViewResolution.referenceResolutionPortrait;
        //         if (viewSize.y < ViewResolution.designSize.y)
        //         {
        //             var scale = viewSize.y / ViewResolution.designSize.y;
        //             transform.localScale = new Vector3(scale, scale, scale);
        //         }
        //     }
        //     else
        //     {
        //         var viewSize = ViewResolution.referenceResolutionLandscape;
        //         if (viewSize.x < ViewResolution.designSize.x)
        //         {
        //             var scale = viewSize.x / ViewResolution.designSize.x;
        //             transform.localScale = new Vector3(scale, scale, scale);
        //         }
        //     }
        // }

        [ComponentBinder("Root/BottomGroup/NextButton")]
        private void OnBtnNextClick()
        {
            SoundController.PlayButtonClick();
            OnPageScroll(true);
        }
        
        [ComponentBinder("Root/BottomGroup/PreviousButton")]
        private void OnBtnPreviousClick()
        {
            SoundController.PlayButtonClick();
            OnPageScroll(false);
        }
        
        [ComponentBinder("Root/BottomGroup/BackButton")]
        private void OnBtnBackClick()
        {
            SoundController.PlayButtonClick();
            Close();
            EventBus.Dispatch(new EventPayTableClosed());
        }

        private void OnPageScroll(bool isNext)
        {
            if (canClick)
            {
                canClick = false;
                SetNextPage(isNext);
                PlayMoveAnimation(isNext ? -1 : 1);
            }
        }

        private void ChangePageIndex(int delta)
        {
            shiftPage += delta;
            if (shiftPage<0)
            {
                shiftPage += COUNT_PAGES;
            }
            if (shiftPage>=COUNT_PAGES)
            {
                shiftPage %= COUNT_PAGES;
            }
        }

        private Transform GetOuterContainer()
        {
            for (int i = 0; i < dequeContainers.Length; i++)
            {
                if (Mathf.Abs(dequeContainers[i].localPosition.x)>0)
                {
                    return dequeContainers[i];
                }
            }

            return dequeContainers[0];
        }

        private void PlayMoveAnimation(int delta)
        {
            for (int i = 0; i < MAX_CONTAINER; i++)
            {
                var container = dequeContainers[i];
                var localPosition = container.localPosition;
                var nextPosX = localPosition.x + delta * vecContentSize.x;
                container.DOLocalMove(new Vector3(nextPosX, 0,0), 0.2f).onComplete += () =>
                {
                    canClick = true;
                };
            }   
        }

        private void SetNextPage(bool isNext)
        {
            RecyclePage();
            ChangePageIndex(isNext?1:-1);
            var outContainer = GetOuterContainer();
            var outPosX = isNext ? vecContentSize.x : -vecContentSize.x;
            outContainer.localPosition = new Vector3(outPosX, 0, 0);
            var showPage = GetNextPage();
            showPage.gameObject.SetActive(true);
            showPage.SetParent(outContainer.transform,false);
        }

        private void RecyclePage()
        {
            var pageContainer = GetOuterContainer();
            var outPage = pageContainer.GetChild(0);
            if (outPage)
            {
                outPage.gameObject.SetActive(false);
                outPage.SetParent(_transPagesPool,false);    
            }
        }

        private Transform GetNextPage()
        {
            return dequePages[shiftPage%COUNT_PAGES];   
        }
        
        public override async Task OnClose()
        {
            await Task.CompletedTask;
        }
        public override async void OnOpen()
        {
            await Task.CompletedTask;
        }

    }
}