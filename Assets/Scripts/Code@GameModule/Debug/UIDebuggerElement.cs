#if UNITY_EDITOR || !PRODUCTION_PACKAGE
using System;
using GameModule;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDebuggerElement:Controller
{
    public Transform debuggerTransform;
    public Transform transform;
    
    [ComponentBinder("GroupDisplay/ElementShowGroup/btnDefault")]
    private Button btnDefault;
    // [ComponentBinder("GroupDisplay")] 
    // protected CanvasGroup GroupDisplay;
    protected Button btnElement;
    protected string buttonText;
    protected Action buttonCallback;

    public int priority;
    public UIDebuggerElement(string inButtonText = "defaultBtnText", int inPriority = 10, Action inButtonCallback = null)
    {
        buttonText = inButtonText;
        buttonCallback = inButtonCallback;
        priority = inPriority;
    }
    
    public virtual void InitContext(Transform inDebuggerTransform)
    {
        debuggerTransform = inDebuggerTransform;
        ComponentBinder.BindingComponent(this, debuggerTransform);
        btnElement = GetElementBtn().GetComponent<Button>();
        btnElement.gameObject.SetActive(true);
        btnElement.transform.Find("Text").GetComponent<Text>().text = buttonText;
        btnElement.onClick.AddListener(OnButtonClick);
    }

    public virtual void OnButtonClick()
    {
        if (buttonCallback != null)
        {
            buttonCallback.Invoke();   
        }
        else
        {
            XDebug.LogError("btn callback is null");
        }
    }

    public virtual GameObject GetElementBtn()
    {
        var btn = GameObject.Instantiate(btnDefault.gameObject,btnDefault.transform.parent,false);
        return btn;
    }

    public virtual bool IsArrayInList()
    {
        return true;
    }
    public virtual void SetElementPosition(Vector3 position)
    {
        if (btnElement != null)
        {
            btnElement.transform.localPosition = position;   
        }
    }

    public virtual void ShowElementList(bool activeFlag)
    {
            
    }

    public virtual Vector2 GetContainerPivot()
    {
        if (btnElement != null)
        {
            var btnPosition = btnElement.transform.position;
            var tempPivot = new Vector2();
            if (btnPosition.x >= 0)
            {
                tempPivot.x = 1;
            }
            else
            {
                tempPivot.x = 0;
            }

            if (btnPosition.y >= 0)
            {
                tempPivot.y = 1;
            }
            else
            {
                tempPivot.y = 0;
            }

            return tempPivot;
        }
        return Vector2.zero;
    }

    protected Transform dragObject;
    protected Transform dragButton;
    public virtual void SetDragPointAndObject(Transform inDragObject,Transform inDragButton)
    {
        dragObject = inDragObject;
        dragButton = inDragButton;
        var dropEventCustomHandler = dragButton.gameObject.AddComponent<DragDropEventCustomHandler>();
        dropEventCustomHandler.BindingDragAction(OnDrag);
        dropEventCustomHandler.BindingBeginDragAction(OnBeginDrag);
        dropEventCustomHandler.BindingEndDragAction(OnEndDrag);
    }
    protected bool isDrag = false;
    protected Vector2 mouseOffset;
    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        isDrag = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) dragObject.parent,
            pointerEventData.position, pointerEventData.pressEventCamera, out var startPos);
        mouseOffset = (Vector2) (dragObject.localPosition) - startPos;
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        if (isDrag)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) dragObject.parent,
                pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localCursor))
                return;
            dragObject.localPosition = localCursor + mouseOffset;
        }
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        if (!isDrag) return;

        isDrag = false;
        
        SetPositionInSafeRect(((RectTransform) dragObject).sizeDelta,dragObject);
    }
    public void SetPositionInSafeRect(Vector2 contentSize,Transform targetObject)
    {
        var safeRect = GetScreenSafeRect(contentSize);
        var localPosition = targetObject.localPosition;
        var safePosition = GetSafePosition(safeRect, localPosition);
        targetObject.localPosition = safePosition;
        // dragObject.localPosition = localPosition;
    }
    private Rect GetScreenSafeRect(Vector2 contentSize)
    {
        bool isPortrait = ViewManager.Instance.IsPortrait;

        var referenceResolution = ViewResolution.referenceResolutionLandscape;

        if (isPortrait)
        {
            referenceResolution = ViewResolution.referenceResolutionPortrait;
        }

        var controlPanelHeight = MachineConstant.controlPanelHeight;

        var machineUIHeight = MachineConstant.topPanelHeight + MachineConstant.controlPanelHeight;

        if (isPortrait)
        {
            controlPanelHeight = MachineConstant.controlPanelVHeight;
            machineUIHeight = MachineConstant.topPanelVHeight + MachineConstant.controlPanelVHeight;
        }

        var safeRect = new Rect(-referenceResolution.x * 0.5f + contentSize.x * 0.5f,
            -referenceResolution.y * 0.5f + controlPanelHeight + contentSize.y * 0.5f,
            referenceResolution.x - contentSize.x, referenceResolution.y - machineUIHeight - contentSize.y);

        return safeRect;
    }

    private Vector2 GetSafePosition(Rect safeRect, Vector2 localPosition)
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

        if (localPosition.x < safeRect.xMin)
        {
            targetX = safeRect.xMin;
        }
        else if (localPosition.x > safeRect.xMax)
        {
            targetX = safeRect.xMax;
        }
        
        return new Vector2(targetX, targetY);
    }

}
#endif