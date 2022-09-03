// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/07/19:47
// Ver : 1.0.0
// Description : XUtility.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Tool;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModule
{
    public static class XUtility
    {
        public static long SecondsOfOneDay = 24 * 60 * 60;
        public static float GetLeftTime(ulong timeStamp)
        {
            return ((long)timeStamp - (long)APIManager.Instance.GetServerTime()) * 0.001f;
        }

        public static bool ServerTimeIsInRange(ulong start, ulong end)
        {
            return start < APIManager.Instance.GetServerTime() && end > APIManager.Instance.GetServerTime();
        }
        public static string GetTimeText(float leftTime, bool onlyHms = false, bool useUpperCase = false)
        {
            var duration = TimeSpan.FromSeconds(Math.Max(0, leftTime));

            if (duration.TotalDays > 1 && !onlyHms)
            {
                if (duration.TotalDays >= 2)
                    return duration.ToString("%d") + (useUpperCase ? " DAYS" : " days");

                return duration.ToString("%d") + (useUpperCase ? " DAY" : " day");
            }
            var timeSpan = TimeSpan.FromSeconds(Math.Max(0, leftTime));

            return string.Format("{0:00}:{1:mm}:{1:ss}", (int)timeSpan.TotalHours, timeSpan);

            // return TimeSpan.FromSeconds(Math.Max(0,leftTime)).ToString(@"hh\:mm\:ss");
        }

        public static async Task PlayAnimationAsync(Animator animator, string stateName, IPauseAndCancelableContext context = null, bool forceCallback = false)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            PlayAnimation(animator, stateName, () =>
            {
                context?.RemoveTask(taskCompletionSource);
                taskCompletionSource.SetResult(true);
            }, context, forceCallback);

            if (!taskCompletionSource.Task.IsCompleted)
            {
                context?.AddWaitTask(taskCompletionSource, null);
            }

            await taskCompletionSource.Task;
        }

        public static async void PlayAnimation(Animator animator, string stateName, Action finishCallback = null, IPauseAndCancelableContext context = null, bool forceCallback = false)
        {
            if (animator == null || !animator.HasState(stateName))
            {
                finishCallback?.Invoke();
                return;
            }

            if (finishCallback == null)
            {
                if (animator.HasState(stateName))
                {
                    animator.speed = 1;
                    animator.Play(stateName, -1, 0);
                }
                return;
            }

            if (!animator.gameObject.activeInHierarchy)
            {
                XDebug.Log("GameObject Is Inactive, Animation Playing Not Possible");

                finishCallback?.Invoke();
                return;
            }

            if (context == null)
            {
                context = DefaultPauseAndCancelContext.Instance;
            }

            var customDataProxy = animator.GetComponent<MonoCustomDataProxy>();
            if (!customDataProxy)
            {
                customDataProxy = animator.gameObject.AddComponent<MonoCustomDataProxy>();
            }

            customDataProxy.SetCustomData("LastPlayAnimationStateName", stateName);
            animator.speed = 1;
            animator.Play(stateName, -1, 0);

            await WaitNFrame(1, context);

            if (animator)
            {
                var clipInfo = animator.GetCurrentAnimatorClipInfo(0);

                if (clipInfo.Length > 0)
                    await WaitSeconds(clipInfo[0].clip.length, context);
                else
                {
                    finishCallback?.Invoke();
                    return;
                }

                var lastStateName = customDataProxy.GetCustomData<string>("LastPlayAnimationStateName");
                if (lastStateName == stateName || forceCallback)
                    finishCallback?.Invoke();
                else
                {
                    XDebug.LogError($"StateNameNotMatch[{stateName}/{lastStateName}]");
                }
            }
        }

        public static bool IsAnimationExist(Animator animator, string stateName)
        {
            //===================added by james====================
            //美术可能先做弹窗，后做动画，此时框架中打开窗口时会自动调佣此函数判断是否是否包含Open动画，但是弹窗节点
            //没有挂animator，此时会崩溃，加入这个判断可防止崩溃。
            if (animator == null || animator.runtimeAnimatorController == null) return false;
            //===================added by james====================

            var animationClips = animator.runtimeAnimatorController.animationClips;

            foreach (AnimationClip animationClip in animationClips)
            {
                if (animationClip.name.ToLower().Contains(stateName.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 方便异步调用的等几秒
        /// </summary>
        /// <param name="timeToDelay"></param>
        /// <param name="context"></param>
        public static async Task WaitSeconds(float timeToDelay, IPauseAndCancelableContext context = null)
        {
            if (context == null)
            {
                context = DefaultPauseAndCancelContext.Instance;
            }

            if (timeToDelay <= 0)
                return;

            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();

            Coroutine coroutine = IEnumeratorTool.instance.StartCoroutine(WaitSeconds(timeToDelay, waitTask, context));

            context?.AddWaitTask(waitTask, coroutine);

            await waitTask.Task;
        }

        public static void WaitSeconds(float timeToDelay,
            CancelableCallback cancelableCallback, IPauseAndCancelableContext context = null, bool realTime = false)
        {
            if (context == null)
            {
                context = DefaultPauseAndCancelContext.Instance;
            }

            if (timeToDelay > 0)
            {
                var coroutine =
                    IEnumeratorTool.instance.StartCoroutine(WaitSecondCoroutine(timeToDelay, realTime, cancelableCallback,
                        context));

                if (cancelableCallback != null)
                    cancelableCallback.BindingContextAndCoroutine(coroutine, context);
            }
            else if (cancelableCallback != null && !cancelableCallback.callBackCanceled)
            {
                cancelableCallback.callback.Invoke();
            }
        }
        
       
        public static IEnumerator WaitSecondCoroutine(float timeToDelay, bool realTime,
            CancelableCallback callback, IPauseAndCancelableContext context)
        {
            context.AddCancelableCallback(callback);
           
            if(realTime)
            {
                yield return new WaitForSecondsRealtime(timeToDelay);
            }
            else
            {
                yield return new WaitForSeconds(timeToDelay);
            }

            if (!callback.callBackCanceled && callback.callback != null)
            {
                callback.callback.Invoke();
            }

            context.RemoveCancelableCallback(callback);
        }

        private static IEnumerator WaitSeconds(float seconds, TaskCompletionSource<bool> waitTask, IPauseAndCancelableContext context)
        {
            float elapsedTime = 0;

            while (elapsedTime < seconds)
            {
                while (context != null && context.IsPaused)
                {
                    yield return null;
                }

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            context?.RemoveTask(waitTask);
            waitTask.SetResult(true);
        }

        /// <summary>
        /// 延迟N帧
        /// </summary>
        /// <param name="frameCount"></param>
        /// <param name="context"></param>
        public static async Task WaitNFrame(int frameCount, IPauseAndCancelableContext context = null)
        {
            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();

            Coroutine coroutine = IEnumeratorTool.instance.StartCoroutine(WaitNFrame(frameCount, waitTask, context));

            context?.AddWaitTask(waitTask, coroutine);

            await waitTask.Task;
        }

        /// <summary>
        /// 延迟N帧
        /// </summary>
        /// <param name="frameCount"></param>
        /// <param name="callback"></param>
        /// <param name="context"></param>
        public static void WaitNFrame(int frameCount, Action callback, IPauseAndCancelableContext context = null)
        {
            if (frameCount > 0)
            {
                if (context == null)
                {
                    context = DefaultPauseAndCancelContext.Instance;
                }

                Coroutine coroutine = null;

                coroutine = IEnumeratorTool.instance.StartCoroutine(WaitNFrame(frameCount, () =>
                {
                    context.RemoveCoroutine(coroutine);
                    callback?.Invoke();
                }));

                context.AddCoroutine(coroutine);
            }
            else
            {
                callback.Invoke();
            }


        }

        private static IEnumerator WaitNFrame(int frameCount, Action callback)
        {
            yield return null;

            for (var i = 0; i < frameCount; i++)
            {
                yield return new WaitForEndOfFrame();
            }

            callback.Invoke();
        }

        private static IEnumerator WaitNFrame(int frameCount, TaskCompletionSource<bool> waitTask, IPauseAndCancelableContext context)
        {
            yield return null;

            for (var i = 0; i < frameCount; i++)
            {
                yield return new WaitForEndOfFrame();
            }

            context?.RemoveTask(waitTask);
            waitTask.SetResult(true);
        }

        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            float Func(float x) => 4 * (-height * x * x + height * x);

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, Func(t) + Mathf.Lerp(start.y, end.y, t));
        }

        public static Tweener Fly(Transform tran, Vector3 startPos, Vector3 endPos, float height, float duration,
            Action actionEnd, Ease ease = Ease.Linear, IPauseAndCancelableContext context = null)
        {

            if (context == null)
            {
                context = DefaultPauseAndCancelContext.Instance;
            }






            var tweener = DOTween.To(setter: value => { tran.position = Parabola(startPos, endPos, height, value); },
                    startValue: 0, endValue: 1, duration: duration)
                .SetEase(ease);
            tweener.onComplete += () =>
            {
                context.RemoveTweener(tweener);
                actionEnd?.Invoke();

            };
            context.AddTweener(tweener);
            return tweener;
        }

        public static void FlyLocal(Transform tran, Vector3 startPos, Vector3 endPos, float height, float duration, float targetScale = -1.0f,
            Action actionEnd = null, Ease ease = Ease.Linear, IPauseAndCancelableContext context = null)
        {
            if (context == null)
            {
                context = DefaultPauseAndCancelContext.Instance;
            }

            var tweener = DOTween.To(setter: value => { tran.localPosition = Parabola(startPos, endPos, height, value); },
                    startValue: 0, endValue: 1, duration: duration)
                .SetEase(ease);

            if (targetScale > 0)
            {
                tran.DOScale(new Vector3(targetScale, targetScale, targetScale), duration - 0.01f);
            }

            tweener.onComplete += () =>
            {
                context.RemoveTweener(tweener);
                actionEnd?.Invoke();
            };
            context.AddTweener(tweener);
        }

        public static async Task FlyAsync(Transform tran, Vector3 startPos, Vector3 endPos, float height,
            float duration,
            Ease ease = Ease.Linear, IPauseAndCancelableContext context = null)
        {
            if (context == null)
                context = DefaultPauseAndCancelContext.Instance;


            while (context.IsPaused)
            {
                await WaitNFrame(1, context);
            }


            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            context.AddWaitTask(taskCompletionSource, null);

            Fly(tran, startPos, endPos, height, duration, () =>
            {
                context.RemoveTask(taskCompletionSource);
                taskCompletionSource.SetResult(true);
            }, ease, context);


            await taskCompletionSource.Task;
        }

        public static async Task FlyLocalAsync(Transform tran, Vector3 startPos, Vector3 endPos, float height,
            float duration, float targetScale = -1.0f,
            Ease ease = Ease.Linear, IPauseAndCancelableContext context = null)
        {
            if (context == null)
                context = DefaultPauseAndCancelContext.Instance;

            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            context.AddWaitTask(taskCompletionSource, null);

            FlyLocal(tran, startPos, endPos, height, duration, targetScale, () =>
            {
                context.RemoveTask(taskCompletionSource);
                taskCompletionSource.SetResult(true);
            }, ease, context);


            await taskCompletionSource.Task;
        }

        public static void ShowBlockUserOperationMask(bool flag)
        {
            Transform container = GameObject.Find("HighPriorityUIContainerCanvas/FlyUtils").transform;
            Transform mask = container.Find("Mask");

            if (mask != null && flag)
            {
                mask.gameObject.SetActive(true);
            }
            else if (mask != null)
            {
                mask.gameObject.SetActive(false);
            }
        }

        public static string GetLimitLengthAbbreviationFormat(long num, int limitLength = 5)
        {
            var text = num.GetAbbreviationFormat(0);

            //减1是减去小数点占位
            if (text.Length < limitLength - 1)
            {
                text = num.GetAbbreviationFormat(limitLength - text.Length - 1);
            }

            return text;
        }

        public static async Task FlyCoins(Transform source, EventBalanceUpdate eventBalanceUpdate, CurrencyCoinView currencyCoinView = null, bool blockOperation = true,
            bool playSound = true, bool showParticle = false, int coinCount = 15)
        {
            if (currencyCoinView == null)
            {
                EventBus.Dispatch(new EventCoinCollectFx(true));
            }
            else
            {
                currencyCoinView.viewController.ShowCollectFx();
            }

            if (playSound)
            {
                SoundController.PlaySfx("CashCrazy_Coins_Fly");
            }
            Transform container = GameObject.Find("HighPriorityUIContainerCanvas/FlyUtils").transform;

            if (blockOperation)
                ShowBlockUserOperationMask(true);
            // if(playSound)
            //     SoundManager.Instance.PlaySfx("Slot_CoinFly03");

            Vector3 sourceWorldPos = source.parent.TransformPoint(source.localPosition);
            Vector3 fromLocalPos = container.InverseTransformPoint(sourceWorldPos);
            Transform target = TopPanel.GetCoinIcon();

            if (currencyCoinView != null)
            {
                target = currencyCoinView.GetCoinIcon();
            }

            Vector3 targetWorldPos = target.parent.TransformPoint(target.localPosition);
            Vector3 targetLocalPos = container.InverseTransformPoint(targetWorldPos);

            Vector3 midLocalPos = (fromLocalPos + targetLocalPos) * 0.5f;
            midLocalPos.x -= 50;
            midLocalPos.y -= 50;
            midLocalPos.z = -100;

            GameObject particle = null;

            if (showParticle)
            {
                particle = AssetHelper.InstantiateResidentAsset<GameObject>("CoinFlyParticles");
                Vector3 localScale = particle.transform.localScale;
                particle.transform.SetParent(container);
                particle.transform.localScale = localScale;
                particle.transform.localPosition = fromLocalPos;
            }

            Vector3[] wayPoints = new[] { fromLocalPos, midLocalPos, targetLocalPos };

            // int coinCount = 15;

            for (int i = 0; i < coinCount; i++)
            {
                var index = i;
                var coinCollectFx = AssetHelper.InstantiateResidentAsset<GameObject>("CoinCollectFx").transform;
                coinCollectFx.SetParent(container, false);
                coinCollectFx.SetSiblingIndex(1);
                coinCollectFx.localScale = Vector3.one * 0.5f;

                coinCollectFx.localPosition = fromLocalPos + new Vector3(UnityEngine.Random.Range(-50, 50),
                    UnityEngine.Random.Range(-50, 50), 0);

                coinCollectFx.Find("Image").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                coinCollectFx.Find("Image").GetComponent<Image>().DOFade(1, 0.3f);
                coinCollectFx.DOScale(1, 0.4f);
                coinCollectFx.DOScale(0.67f, 0.6f).SetDelay(0.4f);

                wayPoints[0] = coinCollectFx.localPosition;

                Tweener t = coinCollectFx.DOLocalPath(wayPoints, 1f, PathType.CatmullRom, PathMode.Full3D, 10)
                    .SetDelay(0.1f).OnComplete(() =>
                    {
                        GameObject.Destroy(coinCollectFx.gameObject);

                        if (index == 0)
                        {
                            EventBus.Dispatch(eventBalanceUpdate);
                        }

                    }).SetEase(Ease.InQuad);

                await WaitSeconds(0.1f);
            }

            if (blockOperation)
                ShowBlockUserOperationMask(false);

            await WaitSeconds(2f);

            if (currencyCoinView == null)
                EventBus.Dispatch(new EventCoinCollectFx(false));
        }

        public static int RandomSelect(List<int> weights, int totalWeight)
        {
            var randomHit = UnityEngine.Random.Range(0, totalWeight);

            var currentWeight = 0;

            for (var i = 0; i < weights.Count; i++)
            {
                currentWeight += weights[i];

                if (currentWeight >= randomHit)
                {
                    return i;
                }
            }
            return 0;
        }

        public static void ShowTipAndAutoHide(Transform transform, float secondsToHide = 3.0f, float scaleTime = 0.2f,
            bool clickAnyWhereToHide = true, IPauseAndCancelableContext context = null)
        {
            transform.localScale = Vector3.zero;
            transform.gameObject.SetActive(true);
            bool isHidingTip = false;

            transform.DOKill();
            transform.DOScale(new Vector3(1, 1, 1), scaleTime).OnComplete(async () =>
            {
                WaitSeconds(secondsToHide, new CancelableCallback(() =>
                {
                    if (transform != null && transform.gameObject.activeSelf && !isHidingTip)
                    {
                        isHidingTip = true;
                        transform.DOKill();
                        transform.DOScale(Vector3.zero, scaleTime * 0.5f).OnComplete(() =>
                        {
                            transform.gameObject.SetActive(false);
                        }).SetEase(Ease.InQuad);
                    }
                }), context);

            }).SetEase(Ease.OutBack);

            if (clickAnyWhereToHide)
            {
                EventSystem.current.SetSelectedGameObject(transform.gameObject);

                var selectEventCustomHandler = transform.gameObject.GetComponent<SelectEventCustomHandler>();

                if (selectEventCustomHandler == null)
                {
                    selectEventCustomHandler = transform.gameObject.AddComponent<SelectEventCustomHandler>();
                }

                selectEventCustomHandler.BindingDeselectedAction(async (BaseEventData baseEventData) =>
                {
                    XDebug.Log("XUtility.DeselectedAction");

                    if (!isHidingTip)
                    {
                        isHidingTip = true;

                        await WaitNFrame(1, context);

                        transform.DOKill();

                        transform.DOScale(Vector3.zero, scaleTime * 0.5f).OnComplete(() =>
                        {
                            XDebug.Log("XUtility.DeselectedAction:gameObject.SetActive(false)");
                            transform.gameObject.SetActive(false);
                        });
                    }
                });
            }
        }

        public static void SendHelpEmail()
        {
            string email = "support@casualjoygames.com";

            var userId = StorageManager.Instance.GetStorage<StorageCommon>().PlayerId;
            var deviceId = DeviceHelper.GetDeviceId();

            string subject = $"HelpMail_From_{userId}";

            string body = $"userId:{userId},deviceId:{deviceId}";

            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }
    }
}
