/**********************************************

Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-09-30 21:07:03
Ver : 1.0.0
Description : 
ChangeLog :  
**********************************************/


using System;
using System.Threading.Tasks;
using GameModule.UI;
using UnityEngine;

namespace GameModule
{
    //该类用于抽象封装场景切换，进入和退出特定场景需要做的事情
    public class SceneSwitchAction
    {
        //该Action负责的场景的类型
        private SceneType sceneType;
        
        //离开场景需要等待的事件
        protected int readyLeaveMask;
        
        //进入场景需要等待的事件
        protected int readyEnterMask;

        //当前处于离开场景之前
        protected bool isBeforeLeave;
        
        //当前处于进入场景之前
        protected bool isBeforeEnter;

        //离开场景之后的回调
        protected Action onReadyLeaveScene;

        protected EventSwitchScene eventSwitchScene;
        
        public int SwitchActionId { get; set;}
        
        public SceneSwitchAction(SceneType inSceneType)
        {
            sceneType = inSceneType;
        }

        /// <summary>
        /// 当特定事件（服务器协议回来了，资源下载完成了）
        /// 触发后检查是否可以离开该场景，或者进入该场景
        /// </summary>
        /// <param name="mask"></param>
        /// <returns>是否离开旧场景, 或进入新场景</returns>
        public virtual bool OnSceneSwitchMask(SwitchMask mask)
        {
            if (isBeforeLeave)
            {
                readyLeaveMask &= ~(int) (mask);
                return CheckReadyLeaveMask();
            }
            
            if (isBeforeEnter)
            {
                readyEnterMask &= ~(int) (mask);
                return CheckReadyEnterMask();
            }

            return false;
        }
 
        //进入场景要做的准备工作
        public virtual void EnterScene()
        {
            InitializeEnterMask();

            isBeforeEnter = true;
            
            EventBus.Dispatch(new EventBeforeEnterScene(sceneType));

            DoEnterSceneAction();
             
            CheckReadyEnterMask();
        }

        public virtual void CleanUpAfterLeaveScene()
        {
            
        }

        //离开场景要做的清理工作
        public virtual void LeaveScene(Action inOnReadyLeaveScene)
        {
            onReadyLeaveScene = inOnReadyLeaveScene;
            InitializeLeaveMask();
            isBeforeLeave = true;

            DoLeaveSceneAction();
            CheckReadyLeaveMask();
        }

        public virtual void InitializeEnterMask()
        {
            readyEnterMask = 0;
        }

        public virtual void InitializeLeaveMask()
        {
            readyLeaveMask = 0;
        }

        public virtual void DoEnterSceneAction()
        {
        }

        public virtual void DoLeaveSceneAction()
        {
           // ViewManager.Instance.RemoveSceneView();
        }

        /// <summary>
        /// 判断并执行LeaveMask
        /// </summary>
        /// <returns>是否执行</returns>
        public virtual bool CheckReadyLeaveMask()
        {
            if (readyLeaveMask == 0 && onReadyLeaveScene != null)
            {
                isBeforeLeave = false;
                onReadyLeaveScene();
                onReadyLeaveScene = null;
                CleanUpAfterLeaveScene();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断并执行EnterMask
        /// </summary>
        /// <returns>是否执行</returns>
        public virtual bool CheckReadyEnterMask()
        {
            if (readyEnterMask == 0)
            {
                isBeforeEnter = false;

                if (ViewManager.Instance.HasHighPriorityView<SceneSwitchView>() || ViewManager.Instance.HasHighPriorityView<SceneSwitchView2>())
                {
                    var view = ViewManager.Instance.GetHighPriorityView<SceneSwitchView>();
                    
                    if (view == null)
                    {
                        view = ViewManager.Instance.GetHighPriorityView<SceneSwitchView2>();
                    }
                    
                    var controller = view.viewController;

                    if (controller.SwitchActionId == SwitchActionId)
                    {
                        controller.DoCompleteAction(() =>
                        {
                            try
                            {
                                //已经完成加载不能在点击返回了
                                controller.HideBackButton();

                                DoFinalSwitchAction();
                            }
                            catch (Exception e)
                            {
                               Debug.LogException(e);
                            }
                            
                        });
                    }
                    else
                    {
                        DoFinalSwitchAction();
                    }
                }
                else
                {
                    DoFinalSwitchAction();
                }
                
                return true;
            }
            
            return false;
        }

        public virtual async void DoFinalSwitchAction()
        {
            await CreateAndRunTargetScene();
            
            ViewManager.Instance.OnSwitchSceneEnd(false, eventSwitchScene.isBackToLastScene);
            GC.Collect();
          
        }

        public virtual void SetUpSwitchEventData(EventSwitchScene inEventSwitchScene)
        {
            eventSwitchScene = inEventSwitchScene;
        }

        public bool AllowBackButton()
        {
            return !eventSwitchScene.isBackToLastScene && eventSwitchScene.allowBackButton;
        }
         
        public virtual void OnAbortSwitchScene()
        {
            ViewManager.Instance.OnSwitchSceneEnd(true,false);
        }
 
        public async virtual Task CreateAndRunTargetScene()
        {
           
        }

        //是否是竖屏场景
        public virtual bool IsPortraitScene()
        {
            return false;
        }

        public virtual float GetSwitchProgress()
        {
            return 0;
        }

        public virtual bool ShowLoadingScreenView(SceneType inSceneType)
        {
            return true;
        }

        //获取进入当前场景需要的过场UIView
        public virtual string GetTargetSceneSwitchViewAddress(SceneType fromSceneType)
        {
            return null;
        }
    }
}