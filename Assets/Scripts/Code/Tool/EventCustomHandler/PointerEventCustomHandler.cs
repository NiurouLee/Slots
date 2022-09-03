using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerEventCustomHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IScrollHandler
{
    private Action<PointerEventData> onPointerClickAction;
    private Action<PointerEventData> onPointerDownAction;
    private Action<PointerEventData> onPointerEnterAction;
    private Action<PointerEventData> onPointerExitAction;
    private Action<PointerEventData> onPointerUpAction;
    private Action<PointerEventData> onScrollAction;

    public void BindingPointerClick(Action<PointerEventData> pointerClickAction)
    {
        onPointerClickAction = pointerClickAction;
    }
    
    public void BindingPointerDown(Action<PointerEventData> pointerDownAction)
    {
        onPointerDownAction = pointerDownAction;
    }
    
    public void BindingPointerEnter(Action<PointerEventData> pointerEnterAction)
    {
        onPointerEnterAction = pointerEnterAction;
    }
    
    public void BindingPointerExit(Action<PointerEventData> pointerExitAction)
    {
        onPointerExitAction = pointerExitAction;
    }
    
    public void BindingPointerUp(Action<PointerEventData> pointerUpAction)
    {
        onPointerUpAction = pointerUpAction;
    }
    
    public void BindingPointerScroll(Action<PointerEventData> pointerScroll)
    {
        onScrollAction = pointerScroll;
    }
    
    public void ResetBindingAction()
    {
        onPointerClickAction = null;
        onPointerDownAction = null;
        onPointerEnterAction = null;
        onPointerExitAction = null;
        onPointerUpAction = null;
        onScrollAction = null;
    }
    
    public void OnPointerClick(PointerEventData eventData) 
    {
        onPointerClickAction?.Invoke(eventData);    
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDownAction?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       onPointerEnterAction?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExitAction?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUpAction?.Invoke(eventData);
    }
    
    public void OnScroll(PointerEventData eventData)
    {
        onScrollAction?.Invoke(eventData);
    }
}