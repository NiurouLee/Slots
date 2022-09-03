using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class CollectBarView11024:TransformHolder
    {
        private static float barLength = 6f;
        private static float barStartX = -barLength;
        private static float barMoveTime = 1;
        private bool isLocked;

        public bool IsLocked()
        {
            return isLocked;
        }
        private float percent;
        private int nowLevel;
        private Transform bar;
        private Tween barMoveTween;
        private Animator animator;
        private ExtraState11024 _extraState;
        private Animator lockAnimator;
        private List<Transform> iconList = new List<Transform>();
        public ExtraState11024 extraState
        {
            get
            {
                if (_extraState == null)
                {
                    _extraState =  context.state.Get<ExtraState11024>();
                }
                return _extraState;
            }
        }
        public CollectBarView11024(Transform inTransform):base(inTransform)
        {
            var iconGroup = transform.Find("PigIconBg");
            iconList.Clear();
            for (var i = 1; i <= 5; i++)
            {
                iconList.Add(iconGroup.Find("NPCIcon"+i));
            }
            bar = transform.Find("BGsg");
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
            transform.gameObject.AddComponent<PointerEventCustomHandler>().BindingPointerClick(ClickBar);
            lockAnimator = transform.Find("UnOpened").GetComponent<Animator>();
            lockAnimator.keepAnimatorControllerStateOnDisable = true;
        }

        public void RefreshIcon()
        {
            var iconIndex = nowLevel / 5;
            if (iconIndex > 4)
            {
                iconIndex = 4;
            }

            for (var i = 0; i < iconList.Count; i++)
            {
                if (i == iconIndex)
                {
                    iconList[i].gameObject.SetActive(true);
                }
                else
                {
                    iconList[i].gameObject.SetActive(false);
                }
            }
        }
        public void InitAfterBindingContext()
        {
            
        }
        public void RefreshShopLockState()
        {
            if (context.state.Get<BetState>().IsFeatureUnlocked(0) && isLocked)
            {
                isLocked = false; 
                AudioUtil.Instance.PlayAudioFx("Map_Unlock");
                XUtility.PlayAnimation(lockAnimator,"Unlock");
            }
            else if (!context.state.Get<BetState>().IsFeatureUnlocked(0) && !isLocked)
            {
                isLocked = true;
                AudioUtil.Instance.PlayAudioFx("Map_lock");
                XUtility.PlayAnimation(lockAnimator,"Lock");
            }
        }
        public void InitLockState()
        {
            XUtility.PlayAnimation(animator,"Idle");
            if (context.state.Get<BetState>().IsFeatureUnlocked(0))
            {
                isLocked = false;
                XUtility.PlayAnimation(lockAnimator, "UnlockIdle");
            }
            else
            {
                isLocked = true;  
                XUtility.PlayAnimation(lockAnimator, "LockIdle");
            }
        }
        public async void ClickBar(PointerEventData clickPoint)
        {
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CUSTOM_CLICK,BtnType11024.OpenMap);
        }
        public void InitState()
        {
            nowLevel = (int)extraState.GetMapLevel();
            percent = extraState.GetMapCollectProgressPercent();
            SetPositionToPercent(percent);
            InitLockState();
            RefreshIcon();
        }

        public void SetPositionToPercent(float targetPercent)
        {
            bar.DOKill();
            var targetX = barStartX + targetPercent * barLength;
            bar.localPosition = new Vector3(targetX, 0, 0);
        }

        public Vector3 GetCollectPosition()
        {
            return transform.Find("NPCIconBg").transform.position;
        }
        public async Task PerformBarAdd()
        {
            var targetLevel = (int)extraState.GetMapLevel();
            var targetPercent = extraState.GetMapCollectProgressPercent();
            if (targetLevel > nowLevel)
            {
                throw new Exception("地图越级");
            }
            if (targetPercent >= 1)
            {
                await PerformCollect();
                await MoveBar(targetPercent, barMoveTime);
                await PerformFull();
            }
            else if (targetLevel == nowLevel)
            {
                async void NonBlockingCollect()
                {
                    await PerformCollect();
                    await MoveBar(targetPercent, barMoveTime);
                }
                NonBlockingCollect();
            }
            else
            {
                throw new Exception("进度条等级倒退");
            }
        }

        public async Task PerformCollect()
        {
            XUtility.PlayAnimation(animator,"Trigger");
            await context.WaitSeconds(0.2f);
        }
        public async Task MoveBar(float targetPercent,float duration = 0.1f)
        {
            bar.DOKill();
            var targetX = barStartX + targetPercent * barLength;
            var task = new TaskCompletionSource<bool>();
            bar.DOLocalMoveX(targetX, duration).OnComplete(() =>
            {
                task.SetResult(true);
            });
            await task.Task;
        }

        public async Task PerformFull()
        {
            AudioUtil.Instance.PlayAudioFx("Map_Trigger");
            XUtility.PlayAnimation(animator,"Open");
            await context.WaitSeconds(0.5f);
        }
    }
}