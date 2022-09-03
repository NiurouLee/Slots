 using System;
 using UnityEngine;
 using UnityEngine.EventSystems;
 
    public class DragDropEventCustomHandler:MonoBehaviour, IBeginDragHandler, IDragHandler , IEndDragHandler, IDropHandler , IInitializePotentialDragHandler
    {

        private Action<PointerEventData> onInitializePotentialDragAction;
        private Action<PointerEventData> onDragAction;
        private Action<PointerEventData> onBeginDragAction;
        private Action<PointerEventData> onEndDragAction;
        private Action<PointerEventData> onDropAction;

        public void BindingOnInitializePotentialDragAction(Action<PointerEventData> initializePotentialDragAction)
        {
            onInitializePotentialDragAction = initializePotentialDragAction;
        }
        
        public void BindingBeginDragAction(Action<PointerEventData> beginDragAction)
        {
            onBeginDragAction = beginDragAction;
        }
        
        public void BindingDragAction(Action<PointerEventData> dragAction)
        {
            onDragAction = dragAction;
        }

        public void BindingEndDragAction(Action<PointerEventData> endDragAction)
        {
            onEndDragAction = endDragAction;
        }
        
        public void BindingDropAction(Action<PointerEventData> dropAction)
        {
            onDragAction = dropAction;
        }
        
        public void ResetBindingAction()
        {
            onInitializePotentialDragAction = null;
            onBeginDragAction = null;
            onDragAction = null;
            onEndDragAction = null;
            onDragAction = null;
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            onInitializePotentialDragAction?.Invoke(eventData);
        } 
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDragAction?.Invoke(eventData);
        } 
        
        public void OnDrag(PointerEventData eventData)
        {
            onDragAction?.Invoke(eventData);
        } 
        
        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDragAction?.Invoke(eventData);
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            onDragAction?.Invoke(eventData);
        }
    }
 