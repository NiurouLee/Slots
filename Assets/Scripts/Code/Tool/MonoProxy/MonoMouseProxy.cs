/**********************************************
Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-11-24 21:59:03
Ver : 1.0.0
Description : 
ChangeLog :  添加基础功能的MonoBehaviour方便以后做功能扩展
**********************************************/

using System;
using UnityEngine;

public class MonoMouseProxy : MonoBehaviour
{
    private Action mouseDownAction;
    private Action mouseUpAction;
    private Action mouseDragAction;
    private Action mouseEnterAction;
    private Action mouseOverAction;
    private Action mouseExitAction;

    public void BindingMouseDownAction(Action inMouseDownAction)
    {
        mouseDownAction = inMouseDownAction;
    }

    public void BindingMouseUpAction(Action inMouseUpAction)
    {
        mouseUpAction = inMouseUpAction;
    }

    public void BindingMouseDragAction(Action inMouseDragAction)
    {
        mouseDragAction = inMouseDragAction;
    }

    public void BindingMouseEnterAction(Action inMouseEnterAction)
    {
        mouseEnterAction = inMouseEnterAction;
    }

    public void BindingMouseOverAction(Action inMouseOverAction)
    {
        mouseOverAction = inMouseOverAction;
    }

    public void BindingMouseExitAction(Action inMouseExitAction)
    {
        mouseEnterAction = inMouseExitAction;
    }

    void OnMouseDown()
    {
        mouseDownAction?.Invoke();
    }

    void OnMouseEnter()
    {
        mouseEnterAction?.Invoke();
    }

    void OnMouseOver()
    {
        mouseOverAction?.Invoke();
    }


    void OnMouseExit()
    {
        mouseEnterAction?.Invoke();
    }

    void OnMouseUp()
    {
        mouseUpAction?.Invoke();
    }

    void OnMouseDrag()
    {
        mouseDragAction?.Invoke();
    }
}