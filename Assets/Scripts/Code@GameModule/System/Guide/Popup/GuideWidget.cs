// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/18/14:31
// Ver : 1.0.0
// Description : LevelGuideWidget.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    [AssetAddress("LevelGuide")]
    public class GuideWidget:View<GuideWidgetViewController>
    {
        [ComponentBinder("Root")] 
        public RectTransform rootRect;
        
        [ComponentBinder("Root")] 
        public Canvas rootCanvas;
        
        [ComponentBinder("DragLayer")] 
        public RectTransform dragLayer;

        public GuideWidget(string address)
            : base(address)
        {
            
        }
        
        public Vector2 GetWidgetSize()
        {
            return rootRect.sizeDelta;
        }
    }

    public class GuideWidgetViewController : ViewController<GuideWidget>
    {
        protected bool isDrag;
        protected Vector2 mouseOffset;
        protected Vector2 startPos;
        protected bool isLeft=true;
        
        private List<GuideLevelTaskView> listTaskViews;
        private string[] strTasks = {"Times", "ReachLevel3", "ReachLevel4"};
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            var dragDropEventCustomHandler = view.dragLayer.gameObject.AddComponent<DragDropEventCustomHandler>();
            dragDropEventCustomHandler.BindingBeginDragAction(OnBeginDrag);
            dragDropEventCustomHandler.BindingEndDragAction(OnEndDrag);
            dragDropEventCustomHandler.BindingDragAction(OnDrag);   
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            bool enterMachineGuideExist = Client.Get<GuideController>().GetEnterMachineGuide() != null;
            
            InitGuideTasks(enterMachineGuideExist ? 1f: 0);
        
            UpdateWidgetPosition();
            
            if (HasEnterMachineGuide())
            {
                ModifyWidgetCanvas(true);   
            }

            if (enterMachineGuideExist)
            {
                var animator = view.transform.GetComponent<Animator>();
                if (animator)
                {
                    animator.Play("GuideOne");
                }
            }
        }

        private void InitGuideTasks(float delayTimeUpdateDirection)
        {
            listTaskViews = new List<GuideLevelTaskView>();
            
            for (int i = 0; i < 3; i++)
            {
                var taskView = view.AddChild<GuideLevelTaskView>(view.transform.Find($"Root/UIGuideTask{i + 1}"));
                taskView.viewController.InitGuide(strTasks[i]);
              
                if (delayTimeUpdateDirection == 0)
                {
                    taskView.viewController.UpdateDirection(true);
                }

                listTaskViews.Add(taskView);
            }

            if (delayTimeUpdateDirection > 0)
            {
                WaitForSeconds(delayTimeUpdateDirection, () =>
                {
                    for (int i = 0; i < listTaskViews.Count; i++)
                    {
                        listTaskViews[i].viewController.UpdateDirection(true);
                    }
                });
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventLevelGuideUpdate>(OnLevelGuideUpdate);
        }

        private void OnLevelGuideUpdate(EventLevelGuideUpdate evt)
        {
            if (Client.Get<GuideController>().GetCurrentGuide() == null)
            {
                view.Destroy();
                return;
            }

            if (!HasEnterMachineGuide())
            {
                ModifyWidgetCanvas(false);
            }
            view.Show();
           
            for (int i = 0; i < listTaskViews.Count; i++)
            {
                listTaskViews[i].viewController.InitGuide(strTasks[i]);
            }
        }

        private bool HasEnterMachineGuide()
        {
            return Client.Get<GuideController>().GetEnterMachineGuide() != null;
        }
        
        private void OnBeginDrag(PointerEventData pointerEventData)
        {
            isDrag = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)view.transform.parent, pointerEventData.position, pointerEventData.pressEventCamera, out startPos);
            mouseOffset = (Vector2)(view.transform.localPosition) - startPos;
        }
        
        private void OnDrag(PointerEventData pointerEventData)
        {
            if (isDrag)
            {
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) view.transform.parent,
                    pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localCursor))
                    return;
                view.transform.localPosition = localCursor + mouseOffset;
            }
        }
        
        private void OnEndDrag(PointerEventData pointerEventData)
        {
            if(!isDrag) return;

            isDrag = false;
             
            Vector2 contentSize = view.GetWidgetSize();

            var safeRect = GetScreenSafeRect(contentSize);
       
            var localPosition = view.transform.localPosition;
            view.transform.localPosition = GetSafePosition(safeRect, localPosition);
            for (int i = 0; i < listTaskViews.Count; i++)
            {
                listTaskViews[i].viewController.UpdateDirection(isLeft);
            }
        }

        private void UpdateWidgetPosition()
        {
            Vector2 contentSize = view.GetWidgetSize();
            var safeRect = GetScreenSafeRect(contentSize);
            var localPosition = view.transform.localPosition;
            
            view.transform.localPosition = GetSafePosition(safeRect, localPosition);
        }
        
        public void ModifyWidgetCanvas(bool toTop)
        {
            var canvas = view.rootCanvas;
            if (canvas)
            {
                if (toTop)
                {
                    canvas.gameObject.SetActive(true);
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = 20;
                    canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
                    return;
                }
        
                canvas.overrideSorting = false;
                canvas.sortingOrder = 20;
                canvas.sortingLayerID = SortingLayer.NameToID("UI");   
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
                isLeft = true;
                targetX = safeRect.xMin;
            }
            else if (localPosition.x > 0)
            {
                isLeft = false;
                targetX = safeRect.xMax;
            }
            
            return new Vector2(targetX, targetY);
        }
    }
}