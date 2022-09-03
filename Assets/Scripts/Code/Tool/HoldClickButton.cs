using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldClickButton : Button
{
    private bool pointerDown;
    private float pointerDownTimer;
    
    private float requiredHoldTime = 1;
    public float RequiredHoldTime
    {
        get => requiredHoldTime;
        set => requiredHoldTime = value;
    }

    private bool holdClickTriggered = false;

    public UnityEvent onLongClick  = new ButtonClickedEvent();
    public UnityEvent onPointerExit  = new ButtonClickedEvent();
 
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (interactable)
        {
            pointerDown = true;
            pointerDownTimer = Time.realtimeSinceStartup;
         //   Debug.Log("OnPointerDown");
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        
        holdClickTriggered = false;
        pointerDown = false;

        // Debug.Log("OnPointerUp");

        // if (pointerDown)
        // {
        //     pointerDownTimer = Time.realtimeSinceStartup - pointerDownTimer;
        //
        //     if (pointerDownTimer >= requiredHoldTime && onLongClick != null)
        //     {
        //         onLongClick.Invoke();
        //     }
        //     else
        //     {
        //         base.OnPointerClick(eventData);
        //     }
        // }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
       // Debug.Log("OnPointerClick");
        if (!holdClickTriggered)
        {
            var deltaTime = Time.realtimeSinceStartup - pointerDownTimer;
            if (deltaTime < requiredHoldTime)
            {
                base.OnPointerClick(eventData);
            }
        }
        
        ResetState();
        holdClickTriggered = false;
    }

    private void Update()
    {
        if (interactable && pointerDown)
        {
            var deltaTime = Time.realtimeSinceStartup - pointerDownTimer;
            if (deltaTime >= requiredHoldTime && !holdClickTriggered)
            {
                Debug.Log("TriggerHoldClickTime" + deltaTime);
                onLongClick.Invoke();

                InstantClearState();
                
                holdClickTriggered = true;
                ResetState();
            }
        }
    }
    private  void ResetState()
    {
        pointerDown = false;
        pointerDownTimer = 0;
    }
}