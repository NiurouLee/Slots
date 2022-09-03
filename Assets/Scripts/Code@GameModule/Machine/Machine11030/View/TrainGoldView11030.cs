using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using ILRuntime.Runtime;
using SRF;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class TrainGoldView11030:TransformHolder
    {
        [ComponentBinder("TrackGroup/TrackBodyGold/RewardGroup/IntegralGroup/IntegralText")]
        protected TextMesh collectNum;
        [ComponentBinder("TrackGroup/TrackBodyGold/RewardGroup/RewardEFX")]
        protected Animator collectNumAnimationController;
        [ComponentBinder("TrackGroup/TrackBodyGold/RewardGroup/RepeatGroup/IntegralText")]
        protected TextMesh eachCabinNum;
        [ComponentBinder("TrackGroup/TrackBodyGold")]
        protected Animator goldTrackAnimator;

        public List<TrainCabin11030> performTrain;
        public ExtraState11030 extraState;
        public static float distanceScale = 100;
        public static float trainYOffset = 100/distanceScale;
        public static float trainXOffset = 465/distanceScale;
        public static float trainHeadXOffset = 900/distanceScale;
        public float trainSpeed = 300/distanceScale;
        public static float speedParamN = 3;
        public static float speedParamK = 10;
        public static float speedParamX = 0.6f;
        // public static float speedParamN = 2f;
        // public static float speedParamK = 0;
        // public static float speedParamX = 1f;
        public static float beginXPos = -(2000 - 600)/distanceScale;
        public static float endXPos = (2000 - 600)/distanceScale;
        public static float waitTime = 0f;
        public static float numScale = 1f;
        public static float numDropTime = 0.4f;
        public List<TrainCabin11030> recycleTrainPool;
        public long collectValue;
        public Vector3 defaultTrainPosition;
        public TrainGoldView11030(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
            recycleTrainPool = new List<TrainCabin11030>();
        }
        public void InitAfterBindingContext()
        {
            extraState = context.state.Get<ExtraState11030>();
            defaultTrainPosition = transform.Find("TrainGroup").position;
            eachCabinNum.text = "";
            collectNum.text = "";
        }
        public void RefreshViewState()
        {
            collectNum.text = "";
            var baseWinRate = extraState.GetNowTrainBaseWinRate();
            var chips = context.state.Get<BetState>().GetPayWinChips(baseWinRate);
            eachCabinNum.text = Tools.GetLeastDigits(chips,6);
        }

        public async void UpdateCollectNum(long showCollectValue)
        {
            ShowRewardEFX();
            if (!collectNum.gameObject.activeSelf)
            {
                collectNum.gameObject.SetActive(true);
            }
            collectNum.text = Tools.GetLeastDigits(showCollectValue,12);
        }

        public async Task ShowRewardEFX()
        {
            collectNumAnimationController.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(collectNumAnimationController,"RewardEFX");
            // collectNumAnimationController.gameObject.SetActive(false);
        }
        public async Task ShowCollectEFX()
        {
            collectNumAnimationController.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(collectNumAnimationController,"Collect");
            // collectNumAnimationController.gameObject.SetActive(false);
        }
        public async Task StartPerform()
        {
            XUtility.PlayAnimation(goldTrackAnimator,"GoldTrackIntro");
            performTrain = new List<TrainCabin11030>();
            collectValue = 0;
            var trainData = extraState.GetLastGoldTrain();
            {
                var totalTrainLength = trainData.Results.Count * trainXOffset +  trainHeadXOffset - beginXPos + endXPos;
                trainSpeed = totalTrainLength / (Math.Pow(trainData.Results.Count, speedParamX).ToFloat() * speedParamN + speedParamK);
                trainSpeed = 400/distanceScale;
            }
            var trainType = trainData.Id;
            var trainContainer = new GameObject("train");
            trainContainer.SetActive(true);
            trainContainer.transform.SetParent(transform, false);
            float nowWaitTime = waitTime + (-beginXPos / trainSpeed);
            var trainCrossCompleteTask = new TaskCompletionSource<bool>();
            context.AddWaitTask(trainCrossCompleteTask,null);
            float nowXOffset = 0;
            {
                nowXOffset -= trainHeadXOffset / 2;
                nowWaitTime += trainHeadXOffset / 2 / trainSpeed;
                var localXOffset = nowXOffset;
                var localWaitTime = nowWaitTime;
                var localCreateTime = localWaitTime - (-beginXPos / trainSpeed);
                var localDestroyTime = localWaitTime + endXPos / trainSpeed;
                nowXOffset -= trainHeadXOffset / 2;
                nowWaitTime += trainHeadXOffset / 2 / trainSpeed;
                {
                    context.WaitSeconds(localCreateTime, async () =>
                    {
                        var trainHeadContainer = new GameObject("trainHead");
                        trainHeadContainer.SetActive(true);
                        trainHeadContainer.transform.SetParent(trainContainer.transform, false);
                         var tempLocalPosition = trainHeadContainer.transform.localPosition;
                        tempLocalPosition.x = localXOffset;
                        trainHeadContainer.transform.localPosition = tempLocalPosition;
                        TrainCabin11030 singleCabin;
                        if (recycleTrainPool.Count > 0)
                        {
                            singleCabin = recycleTrainPool.Pop();
                            singleCabin.transform = trainHeadContainer.transform;
                        }
                        else
                        {
                            singleCabin = new TrainCabin11030(trainHeadContainer.transform);   
                            singleCabin.Initialize(context);
                        }
                        var tempTrainHeadData = new GoldRushTrainGameResultExtraInfo.Types.Train.Types.Result();
                        tempTrainHeadData.JackpotId = 0;
                        tempTrainHeadData.JackpotPay = 0;
                        tempTrainHeadData.WinRate = 0;
                        singleCabin.SetCabinData(tempTrainHeadData, trainType);
                        performTrain.Add(singleCabin);
                        context.WaitSeconds(localDestroyTime - localCreateTime, async () =>
                        {
                            performTrain.Remove(singleCabin);
                            recycleTrainPool.Add(singleCabin);
                            singleCabin.ReCycleCabin();
                            GameObject.Destroy(trainHeadContainer);
                        });
                    });
                }
            }
            for (var i = 0; i < trainData.Results.count; i++)
            {
                nowXOffset -= trainXOffset / 2;
                nowWaitTime += trainXOffset / 2 / trainSpeed;
                var localXOffset = nowXOffset;
                var localWaitTime = nowWaitTime;
                var localCreateTime = localWaitTime - (-beginXPos / trainSpeed);
                var localDestroyTime = localWaitTime + endXPos / trainSpeed;
                nowXOffset -= trainXOffset / 2;
                nowWaitTime += trainXOffset / 2 / trainSpeed;
                var localI = i;
                {
                    context.WaitSeconds(localCreateTime, async () =>
                    {
                        var cabinContainer = new GameObject("cabin");
                        cabinContainer.SetActive(true);
                        cabinContainer.transform.SetParent(trainContainer.transform, false);
                        var tempLocalPosition = cabinContainer.transform.localPosition;
                        tempLocalPosition.x = localXOffset;
                        cabinContainer.transform.localPosition = tempLocalPosition;

                        TrainCabin11030 singleCabin;
                        if (recycleTrainPool.Count > 0)
                        {
                            singleCabin = recycleTrainPool.Pop();
                            singleCabin.transform = cabinContainer.transform;
                        }
                        else
                        {
                            singleCabin = new TrainCabin11030(cabinContainer.transform);   
                            singleCabin.Initialize(context);
                        }
                        singleCabin.SetCabinData(trainData.Results[localI], trainType);
                        performTrain.Add(singleCabin);
                        var dropValue = singleCabin.GetContainerValue();
                        var dropObject = singleCabin.GetContainer();

                        context.WaitSeconds(localWaitTime-localCreateTime, async () =>
                        {
                            singleCabin.PlayBump();
                            var tempDropObject = GameObject.Instantiate(dropObject, transform, true);
                            dropObject.SetActive(false);
                            tempDropObject.SetActive(true);
                            // tempDropObject.transform.SetParent(transform,false);
                            // tempDropObject.transform.position = dropObject.transform.position;
                            var tempSortingGroup = tempDropObject.AddComponent<SortingGroup>();
                            tempSortingGroup.sortingLayerName = "LocalFx";
                            tempSortingGroup.sortingOrder = 0;
                            AudioUtil.Instance.PlayAudioFx("Fly");
                            var dropSequence = DOTween.Sequence();
                            var stayScaleTime = 0.2f;
                            var initScale = tempDropObject.transform.localScale;
                            var stayScale = initScale * 1.2f;
                            dropSequence.Append(tempDropObject.transform.DOScale(stayScale,stayScaleTime));
                            dropSequence.Append(tempDropObject.transform.DOMove(collectNum.transform.position, numDropTime).SetEase(Ease.Linear));
                            dropSequence.Insert(stayScaleTime,tempDropObject.transform.DOScale(initScale,numDropTime));
                            dropSequence.AppendCallback(() =>
                            {
                                AudioUtil.Instance.PlayAudioFx("FlyStop");
                                GameObject.Destroy(tempDropObject);
                                collectValue += dropValue;
                                UpdateCollectNum(collectValue);
                            });
                            dropSequence.target = context.transform;
                        });
                        context.WaitSeconds(localDestroyTime - localCreateTime, async () =>
                        {
                            performTrain.Remove(singleCabin);
                            recycleTrainPool.Add(singleCabin);
                            singleCabin.ReCycleCabin();
                            GameObject.Destroy(cabinContainer);
                        });
                    });
                }
            }
            
            nowWaitTime += endXPos / trainSpeed;
            context.WaitSeconds(nowWaitTime, async () =>
            {
                context.RemoveTask(trainCrossCompleteTask);
                trainCrossCompleteTask.SetResult(true);
            });
            trainContainer.transform.position = defaultTrainPosition;
            var tempPos = trainContainer.transform.localPosition;
            tempPos.x = beginXPos;
            trainContainer.transform.localPosition = tempPos;
            var moveDistanceX = -nowXOffset + endXPos - beginXPos + tempPos.x;
            var moveDuration = nowWaitTime - waitTime;
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(waitTime);
            sequence.Append(trainContainer.transform.DOLocalMoveX(moveDistanceX, moveDuration).SetEase(Ease.Linear));
            sequence.target = context.transform;
            AudioUtil.Instance.PlayAudioFx("Video2",true);
            await trainCrossCompleteTask.Task;
            AudioUtil.Instance.StopAudioFx("Video2");
            GameObject.Destroy(trainContainer.gameObject);
            performTrain.Clear();
        }
    }
}