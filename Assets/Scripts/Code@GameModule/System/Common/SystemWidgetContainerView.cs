// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/06/14:02
// Ver : 1.0.0
// Description : SystemWidgetContainerView.cs
// 系统功能入口容器类
// 点击入口可以进入系统功能，或者显示二级入口
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("SystemWidgetContainer")]
    public class SystemWidgetContainerView:View<SystemWidgetContainerViewController>
    {
        [ComponentBinder("WidgetListView")] 
        public RectTransform widgetListView;
        
        [ComponentBinder("Content")] 
        public RectTransform widgetContent;
        
        [ComponentBinder("DragLayer")] 
        public RectTransform dragLayer;

        [ComponentBinder("LowerButton")]
        public Button lowerButton;

        [ComponentBinder("SecondLevelWidgetAttachPoint")] 
        public RectTransform secondLevelWidgetAttachPoint;

        public SystemWidgetContainerView(string address)
            : base(address)
        {
            
        }
  
        public Vector2 GetWidgetSize()
        {
            return widgetListView.sizeDelta * transform.localScale.x;
        }

        public void SetMachineContext(MachineContext machineContext)
        {
            viewController.machineContext = machineContext;
        }

        public void CollectSystemWidget(Action finishCallback)
        {
            viewController.CollectSystemWidget(finishCallback);
        }
    }
    
    public class SystemWidgetContainerViewController : ViewController<SystemWidgetContainerView>
    {
        protected bool isDrag;
        protected Vector2 mouseOffset;
        protected Vector2 startPos;

        private List<ISystemWidget> _attachedSystemWidgetList;
        private ISystemSecondLevelWidget _secondLevelWidget;
       
        public string gameId;

        public MachineContext machineContext;

        public bool IsDrag => isDrag;
 
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            if (ViewManager.Instance.IsPortraitScene())
            {
                view.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
            else if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
            {
                var scale = ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                view.transform.localScale = new Vector3(scale, scale, scale);
            } 

            var dragDropEventCustomHandler = view.dragLayer.gameObject.AddComponent<DragDropEventCustomHandler>();
            dragDropEventCustomHandler.BindingBeginDragAction(OnBeginDrag);
            dragDropEventCustomHandler.BindingEndDragAction(OnEndDrag);
            dragDropEventCustomHandler.BindingDragAction(OnDrag);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSystemWidgetNeedAttach>(OnSystemWidgetNeedAttach);
            SubscribeEvent<EventSystemWidgetDetached>(OnSystemSystemWidgetDetached);
            SubscribeEvent<EventSystemWidgetActive>(OnSystemWidgetActive);

            view.lowerButton.onClick.AddListener(OnViewLowerButtonClicked);

            UpdateWidgetContentSize();
        }

        private void OnSystemWidgetActive(EventSystemWidgetActive evt)
        {
            if (view.transform)
            {
                view.transform.gameObject.SetActive(evt.Active);   
            }
        }

        protected void OnSystemSystemWidgetDetached(EventSystemWidgetDetached evt)
        {
            UpdateWidgetContentSize();
        }
        
        protected void OnViewLowerButtonClicked()
        {
            var scrollRect = view.widgetListView.GetComponent<ScrollRect>();
            if (scrollRect.verticalNormalizedPosition < 0.5)
            {
                float scrollStartPos = scrollRect.verticalNormalizedPosition;
                DOTween.To(() => scrollStartPos, (p) =>
                {
                    scrollRect.verticalNormalizedPosition = p;
                }, 1, 0.3f);
                view.lowerButton.transform.Find("Arrow").localScale = new Vector3(1, 1, 1);
            }
            else
            {
                float scrollStartPos = scrollRect.verticalNormalizedPosition;
                DOTween.To(() => scrollStartPos, (p) => { scrollRect.verticalNormalizedPosition = p; }, 0, 0.3f);
                view.lowerButton.transform.Find("Arrow").localScale = new Vector3(1, -1, 1);
            }
        }

        public void CollectSystemWidget(Action actionCallback)
        {
            EventBus.Dispatch(new EventOnCollectSystemWidget(this), () =>
            {
                EventBus.Dispatch(new EventOnSystemWidgetCollectedEnd(this));
                UpdateWidgetContentSize();

                SetWidgetContainerInitPosition();

                WaitNFrame(1, () =>
                {
                    var scrollRect = view.widgetListView.GetComponent<ScrollRect>();
                    scrollRect.verticalNormalizedPosition = 1;

                    actionCallback?.Invoke();
                });
            });
        }

        private Tween _turnPageTurn;

        public async Task FocusOnWidget(ISystemWidget systemWidgetView)
        {
            if (_attachedSystemWidgetList.Contains(systemWidgetView))
            {
                var widgetTransform = (RectTransform) systemWidgetView.GetWidgetTransform();

                var pos = ((RectTransform) widgetTransform.parent).anchoredPosition;
                var scrollRect = view.widgetListView.GetComponent<ScrollRect>();

                var viewportWidth = scrollRect.viewport.rect.size.y;
                var contentWidth = scrollRect.content.sizeDelta.y;

                if (_turnPageTurn != null)
                {
                    DOTween.Kill(_turnPageTurn);
                    _turnPageTurn = null;
                }

                if (contentWidth > viewportWidth)
                {
                    var normalizedScrollPosition = 1 - Mathf.Min(1,
                        Math.Max(0, (-pos.y - viewportWidth * 0.5f) / (contentWidth - viewportWidth)));


                    var startScrollPos = scrollRect.verticalNormalizedPosition;

                    _turnPageTurn = DOTween.To(() => startScrollPos,
                        (p) => { scrollRect.verticalNormalizedPosition = p; }, normalizedScrollPosition, 0.5f);
                    await WaitForSeconds(0.5f);
                }
            }
        }

        public void UpdateWidgetContentSize()
        {
            if (_secondLevelWidget != null && _secondLevelWidget.IsWidgetActive())
            {
                return;
            }
         
            var childCount = view.widgetContent.childCount;
            
            int activeCount = 0;
           
            for (var i = 0; i < childCount; i++)
            {
                bool activeSelf = view.widgetContent.GetChild(i).gameObject.activeSelf;
                if (activeSelf)
                {
                    activeCount++;
                }
            }

            if (activeCount <= 0)
            {
                view.transform.gameObject.SetActive(false);
                return;
            }
  
            view.transform.gameObject.SetActive(true);
            
            var visibleCount = Mathf.Min(activeCount,4);
            var width = view.widgetListView.sizeDelta.x;
            var sizeDelta = new Vector2(width, visibleCount * 120 + 30);
            view.widgetListView.sizeDelta = sizeDelta;
            
            view.dragLayer.sizeDelta = sizeDelta;
    
            ((RectTransform) view.lowerButton.transform).anchoredPosition = new Vector2(0,-visibleCount * 60 - 15);
            
            view.lowerButton.gameObject.SetActive(activeCount > 4);

            view.widgetListView.GetComponent<Image>().enabled = visibleCount > 1;
            
            _attachedSystemWidgetList.Sort((a, b) =>
            {
                return b.GetWidgetPriority() - a.GetWidgetPriority();
            });

            for (var i = 0; i < _attachedSystemWidgetList.Count; i++)
            {
                _attachedSystemWidgetList[i].GetWidgetTransform().parent.SetAsLastSibling();
            }
        }
        
        protected void OnSystemWidgetNeedAttach(EventSystemWidgetNeedAttach evt)
        {
            if (evt.systemWidget != null)
            {
                AddSystemWidget(evt.systemWidget);
                
                UpdateWidgetContentSize();

                WaitNFrame(1, async () =>
                {
                    FocusOnWidget(evt.systemWidget);
                });
            }
        }

        public void HideWidgetListView()
        {
            if (view.transform.position.x <= 0)
            {
                view.widgetListView.DOLocalMoveX(-view.GetWidgetSize().x, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    view.widgetListView.gameObject.SetActive(false);
                });
            }
            else
            {
                view.widgetListView.DOLocalMoveX(view.GetWidgetSize().x, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    view.widgetListView.gameObject.SetActive(false);
                });
            }
        }
        
        public void ShowWidgetListView()
        {
            var widgetSize = view.GetWidgetSize();
            
            var screenRect = GetScreenSafeRect(widgetSize);
            var safePosition = GetSafePosition(screenRect, view.transform.localPosition);
            
            if (view.transform.position.x <= 0)
            {
                view.transform.localPosition = safePosition;
                view.widgetListView.localPosition = new Vector3(-widgetSize.x,0);
                view.widgetListView.gameObject.SetActive(true);
                UpdateWidgetContentSize();
                view.widgetListView.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
            }
            else
            {
                view.transform.localPosition = safePosition;
                view.widgetListView.localPosition = new Vector3(widgetSize.x, 0);
                view.widgetListView.gameObject.SetActive(true);
                UpdateWidgetContentSize();
                view.widgetListView.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
            }
            
            view.dragLayer.sizeDelta = view.GetWidgetSize();
        }
        
        public void HideSecondLevelWidget()
        {
            if (_secondLevelWidget == null)
                return;
            
            var widgetSize = _secondLevelWidget.GetContentSize() * view.transform.localScale.x;
            
            if (view.transform.position.x <= 0)
            {
                view.secondLevelWidgetAttachPoint.DOLocalMoveX(-widgetSize.x, 0.5f).SetEase(Ease.InBack);
            }
            else
            {
                view.secondLevelWidgetAttachPoint.DOLocalMoveX(widgetSize.x, 0.5f).SetEase(Ease.InBack);
            }
        }
        
        public void ShowSecondLevelWidget()
        {
            if (_secondLevelWidget == null)
                return;

            var widgetSize = _secondLevelWidget.GetContentSize() * view.transform.localScale.x;

            var screenRect = GetScreenSafeRect(widgetSize);

            var safePosition = GetSafePosition(screenRect, view.transform.localPosition);
             
            if (view.transform.position.x <= 0)
            {
                view.transform.localPosition = safePosition; 
                view.secondLevelWidgetAttachPoint.localPosition = new Vector3( - widgetSize.x,0,0);
                view.secondLevelWidgetAttachPoint.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
            }
            else
            {
                view.transform.localPosition = safePosition;
                view.secondLevelWidgetAttachPoint.localPosition = new Vector3( widgetSize.x, 0,0);
                view.secondLevelWidgetAttachPoint.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
            }

            view.dragLayer.sizeDelta = _secondLevelWidget.GetContentSize() * view.transform.localScale.x;
        }
        
        public ISystemSecondLevelWidget GetSecondLevelWidget()
        {
            return _secondLevelWidget;
        }
        public async void AttachSecondLevelWidget(ISystemSecondLevelWidget inSecondLevelWidget)
        {
            inSecondLevelWidget.SetParent(view.secondLevelWidgetAttachPoint);
            inSecondLevelWidget.HideWidget();

            if (_secondLevelWidget != null)
            {
                DetachSecondLevelWidget(false);
            }
            else
            {
                HideWidgetListView();
            }

            await WaitForSeconds(0.5f);

            inSecondLevelWidget.ShowWidget();
            _secondLevelWidget = inSecondLevelWidget;
            
            _secondLevelWidget.AttachDragDropEvent(this);

            ShowSecondLevelWidget();
        }
        
        public async void DetachSecondLevelWidget(bool showWidgetList = true)
        {
            if (_secondLevelWidget != null)
            {
                HideSecondLevelWidget();

                await WaitForSeconds(0.5f);

                _secondLevelWidget.DestroyWidget();

                _secondLevelWidget = null;

                if (showWidgetList)
                {
                    ShowWidgetListView();
                }
            }
        }

        public override void OnViewDestroy()
        {
            base.OnViewDestroy();
            if (_attachedSystemWidgetList != null && _attachedSystemWidgetList.Count > 0)
            {
                for (var i = 0; i < _attachedSystemWidgetList.Count; i++)
                {
                    _attachedSystemWidgetList[i].DestroyWidget();
                }
            }
            
            if (_secondLevelWidget != null)
            {
                _secondLevelWidget.DestroyWidget();
            }
        }
        
        public void AddSystemWidget(ISystemWidget inSystemWidget)
        {
            var itemContainer = view.widgetContent.Find("ItemContainer");
            var newItemContainer = GameObject.Instantiate(itemContainer.gameObject, view.widgetContent);
         
            inSystemWidget.SetWidgetContainer(newItemContainer.transform);
            inSystemWidget.SetSystemWidgetContainerViewController(this);
              
            if (_attachedSystemWidgetList == null)
            {
                _attachedSystemWidgetList = new List<ISystemWidget>();
            }
            
            _attachedSystemWidgetList.Add(inSystemWidget);

            float lastResponseTime = Time.realtimeSinceStartup;
            newItemContainer.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!isDrag)
                {
                    var now = Time.realtimeSinceStartup;
                    if (now - lastResponseTime > 1)
                    {
                        inSystemWidget.OnWidgetClicked(this);
                        lastResponseTime = now;
                    }
                }
            });
            
            var dragDropEventCustomHandler = newItemContainer.AddComponent<DragDropEventCustomHandler>();
            dragDropEventCustomHandler.BindingBeginDragAction(OnBeginDrag);
            dragDropEventCustomHandler.BindingEndDragAction(OnEndDrag);
            dragDropEventCustomHandler.BindingDragAction(OnDrag);
            
            newItemContainer.gameObject.SetActive(true);
            
            UpdateWidgetContentSize();
        }

        public void SetWidgetContainerInitPosition()
        {
            bool isPortrait = ViewManager.Instance.IsPortrait;
            var referenceResolution = ViewResolution.referenceResolutionLandscape;
            
            float screenCutOut = 0;
            
            if (isPortrait)
            {
                referenceResolution = ViewResolution.referenceResolutionPortrait;
            }
            
#if UNITY_IOS
            if (!isPortrait)
            {
                screenCutOut = MachineConstant.widgetOffset;
            }
#endif
            var initY = 0.0f;
            if (ViewManager.Instance.IsPortraitScene())
            {
                var safeRect = GetScreenSafeRect(view.GetWidgetSize());
                initY  = safeRect.yMax * 0.7f;
            }
            
            view.transform.localPosition = new Vector2(-referenceResolution.x * 0.5f + view.GetWidgetSize().x * 0.5f + screenCutOut, initY);
        }
        
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            
            SetWidgetContainerInitPosition();
        }

        public  void RemoveSystemWidgetView(ISystemWidget systemWidget)
        {
            _attachedSystemWidgetList.Remove(systemWidget);
            systemWidget.HideWidget();
            systemWidget.DestroyWidget();
            UpdateWidgetContentSize();
        }
        
        public void OnBeginDrag(PointerEventData pointerEventData)
        {
            isDrag = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)view.transform.parent, pointerEventData.position, pointerEventData.pressEventCamera, out startPos);
            mouseOffset = (Vector2)(view.transform.localPosition) - startPos;
        }
        
        public void OnDrag(PointerEventData pointerEventData)
        {
            if (isDrag)
            {
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) view.transform.parent,
                    pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localCursor))
                    return;
                view.transform.localPosition = localCursor + mouseOffset;
            }
        }

        public Rect GetScreenSafeRect(Vector2 contentSize)
        {
            bool isPortrait = ViewManager.Instance.IsPortrait;
            
            var referenceResolution = ViewResolution.referenceResolutionLandscape;
            
            if (isPortrait)
            {
                referenceResolution = ViewResolution.referenceResolutionPortrait;
            }

            var controlPanelHeight = MachineConstant.controlPanelHeight;
            
            var machineUIHeight = MachineConstant.topPanelHeight + MachineConstant.controlPanelHeight;

            float screenCutOut = 0;
           
            if (isPortrait)
            {
                controlPanelHeight = MachineConstant.controlPanelVHeight;
                machineUIHeight = MachineConstant.topPanelVHeight + MachineConstant.controlPanelVHeight;
 
            }
            
#if UNITY_IOS
            if (!isPortrait)
            {
                screenCutOut = MachineConstant.widgetOffset;
            }
#endif         
            
            var safeRect = new Rect(-referenceResolution.x * 0.5f + contentSize.x * 0.5f + screenCutOut,
                -referenceResolution.y * 0.5f + controlPanelHeight + contentSize.y * 0.5f,
                referenceResolution.x - contentSize.x - screenCutOut * 2, referenceResolution.y - machineUIHeight - contentSize.y);
            
            return safeRect;
        }

        public Vector2 GetSafePosition(Rect safeRect, Vector2 localPosition)
        {
            var targetY = localPosition.y;
               
            if (localPosition.y > safeRect.yMax)
            {
                targetY = safeRect.yMax;
            }
            else if (localPosition.y < safeRect.yMin)
            {
                targetY = safeRect.yMin;
            }
                
            var targetX = localPosition.x;
                
            if (localPosition.x <= 0 )
            {
                targetX = safeRect.xMin;
            }
            else if (localPosition.x > 0)
            {
                targetX = safeRect.xMax;
            }
            
            return new Vector2(targetX, targetY);
        }
        
        public void OnEndDrag(PointerEventData pointerEventData)
        {
            if(!isDrag) return;

            isDrag = false;
             
            Vector2 contentSize = view.GetWidgetSize();
           
            //如果显示的二级菜单，根据二级菜单的位置，计算安全区域
            if (!view.widgetListView.gameObject.activeInHierarchy && _secondLevelWidget != null)
            {
                contentSize = _secondLevelWidget.GetContentSize() * view.transform.localScale.x;
            }
            
            var safeRect = GetScreenSafeRect(contentSize);
       
            var localPosition = view.transform.localPosition;
            
            view.transform.localPosition = GetSafePosition(safeRect, localPosition);
        }
    }
}