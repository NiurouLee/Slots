// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/06/14:56
// Ver : 1.0.0
// Description : SystemWidgetView.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public interface ISystemWidget
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Need Hide SystemWidget</returns>
        ///
        void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController);
      
        void SetSystemWidgetContainerViewController(SystemWidgetContainerViewController widgetContainerViewController);

        void OnContainerEnabled();
        
        void DestroyWidget();

        void SetWidgetContainer(Transform widgetContainer);

        void HideWidget();

        void ShowWidget();
        
        bool IsWidgetActive();

        Transform GetWidgetTransform();

        int GetWidgetPriority();
    }

    public interface ISystemSecondLevelWidget
    {

        Vector2 GetContentSize();
        
        void DestroyWidget();
        
        void ShowWidget();

        void HideWidget();

        void AttachDragDropEvent(SystemWidgetContainerViewController widgetContainerViewController);

        void SetParent(Transform inParentTransform);

        bool IsWidgetActive();
        
        Transform GetActivityCollectTargetTransform();
    }
    
    public class SystemWidgetView<T>: View<T>, ISystemWidget where T:ViewController
    {
        public SystemWidgetContainerViewController systemWidgetContainerViewController;
        protected Transform widgetContainer;
        public SystemWidgetView(string address)
            : base(address)
        {
            
        }
        
        public virtual void SetSystemWidgetContainerViewController(SystemWidgetContainerViewController widgetContainerViewController)
        {
            systemWidgetContainerViewController = widgetContainerViewController;
        }
        
        public virtual void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
        }

        public virtual void OnContainerEnabled()
        {
        
        }

        public virtual void DestroyWidget()
        {
            base.Destroy();
        }

       

        public void SetWidgetContainer(Transform inWidgetContainer)
        {
            widgetContainer = inWidgetContainer;
            transform.SetParent(widgetContainer,false);
        }

        public void HideWidget()
        {
            widgetContainer.gameObject.SetActive(false);
            systemWidgetContainerViewController.UpdateWidgetContentSize();
        }
        
        public void ShowWidget()
        {
            widgetContainer.gameObject.SetActive(true);
            systemWidgetContainerViewController.UpdateWidgetContentSize();
        }

        public bool IsWidgetActive()
        {
            return IsActive();
        }

        public Transform GetWidgetTransform()
        {
            return transform;
        }

        public virtual int GetWidgetPriority()
        {
            return 0;
        }
    }

    public class SystemSecondLevelWidgetView<T> : View<T>, ISystemSecondLevelWidget where T : ViewController
    {
        public SystemSecondLevelWidgetView(string address)
            : base(address)
        {
            
        }
        
        public SystemWidgetContainerViewController widgetContainerViewController;
        
        public virtual Vector2 GetContentSize()
        {
            return rectTransform.sizeDelta;
        }

        public void DestroyWidget()
        {
            Destroy();
        }

        public virtual void ShowWidget()
        {
            Show();
        }

        public virtual void SetParent(Transform inParentTransform)
        {
            transform.SetParent(inParentTransform,false);
        }
        
        public void HideWidget()
        {
            Hide();
        }

        public virtual void AttachDragDropEvent(SystemWidgetContainerViewController inWidgetContainerViewController)
        {
            widgetContainerViewController = inWidgetContainerViewController;
        }
        
        public virtual bool IsWidgetActive()
        {
            return IsActive();
        }

        public virtual Transform GetActivityCollectTargetTransform()
        {
            return transform;
        }
    }
}