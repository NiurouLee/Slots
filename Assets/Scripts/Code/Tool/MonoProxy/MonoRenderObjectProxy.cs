/**********************************************
Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-11-24 21:59:03
Ver : 1.0.0
Description : 
ChangeLog :  添加基础功能的MonoBehaviour方便以后做功能扩展
**********************************************/


using UnityEngine;
using System;

public class MonoRenderObjectProxy : MonoBehaviour
{
    private Action renderObjectAction;

    public void BindingAction(Action inRenderObjectAction)
    {
        renderObjectAction = inRenderObjectAction;
    }

    void OnRenderObject()
    {
        renderObjectAction?.Invoke();
    }
}