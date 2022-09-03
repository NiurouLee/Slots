using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DragonU3DSDK.Audio;
using DragonU3DSDK.Network.API.ILProtocol;
using ILRuntime.Runtime;
using SRF;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class TrainView11030 : TransformHolder
    {
        [ComponentBinder("TrackGroup/TrackBodyNormal/RewardGroup/IntegralGroup/IntegralText")]
        protected TextMesh collectNum;

        [ComponentBinder("TrackGroup/TrackBodyNormal/RewardGroup/RewardEFX")]
        protected Animator collectNumAnimationController;

        [ComponentBinder("TrackGroup/TrackBodyNormal/RewardGroup/JPGroup/JPGrandSprite")]
        protected Transform redTip;

        [ComponentBinder("TrackGroup/TrackBodyNormal/RewardGroup/JPGroup/JPMajorSprite")]
        protected Transform greenTip;

        [ComponentBinder("TrackGroup/TrackBodyNormal/RewardGroup/JPGroup/JPMinorSprite")]
        protected Transform blueTip;

        [ComponentBinder("TrackGroup/TrackBodyNormal/RewardGroup/JPGroup/JPMiniSprite")]
        protected Transform purpleTip;

        [ComponentBinder("TrackGroup/TrackBodyNormal/RewardGroup/JPGroup/AvailableSprite")]
        protected Transform availableTip;

        [ComponentBinder("TrackGroup/TrackBodyNormal/SymbolGroup/SymbolJ04Group/IntegralGroup/IntegralText")]
        protected TextMesh purpleNum;

        [ComponentBinder("TrackGroup/TrackBodyNormal/SymbolGroup/SymbolJ04Group/DisableState")]
        protected Transform purpleBoard;

        [ComponentBinder("TrackGroup/TrackBodyNormal/SymbolGroup/SymbolJ04Group")]
        protected Animator purpleGroup;

        [ComponentBinder("TrackGroup/TrackBodyNormal/SymbolGroup/SymbolJ03Group/IntegralGroup/IntegralText")]
        protected TextMesh blueNum;

        [ComponentBinder("TrackGroup/TrackBodyNormal/SymbolGroup/SymbolJ03Group/DisableState")]
        protected Transform blueBoard;

        [ComponentBinder("TrackGroup/TrackBodyNormal/SymbolGroup/SymbolJ03Group")]
        protected Animator blueGroup;

        [ComponentBinder("TrackGroup/TrackBodyNormal/SymbolGroup/SymbolJ02Group/IntegralGroup/IntegralText")]
        protected TextMesh greenNum;

        [ComponentBinder("TrackGroup/TrackBodyNormal/SymbolGroup/SymbolJ02Group/DisableState")]
        protected Transform greenBoard;

        [ComponentBinder("TrackGroup/TrackBodyNormal/SymbolGroup/SymbolJ02Group")]
        protected Animator greenGroup;

        [ComponentBinder("TrackGroup/TrackBodyNormal/SymbolGroup/SymbolJ01Group/IntegralGroup/IntegralText")]
        protected TextMesh redNum;

        [ComponentBinder("TrackGroup/TrackBodyNormal/SymbolGroup/SymbolJ01Group/DisableState")]
        protected Transform redBoard;

        [ComponentBinder("TrackGroup/TrackBodyNormal/SymbolGroup/SymbolJ01Group")]
        protected Animator redGroup;

        public Dictionary<uint, TextMesh> JpTypeDictionary;
        public Dictionary<uint, Transform> JpBoardDictionary;
        public Dictionary<uint, Transform> JpTipDictionary;
        public Dictionary<uint, Animator> JpGroupDictionary;

        public Dictionary<uint, long> CollectValueDictionary;
        public List<TrainCabin11030> performTrain;
        public List<TrainCabin11030> recycleTrainPool;
        public ExtraState11030 extraState;
        public static float distanceScale = 100;
        public static float trainYOffset = 100 / distanceScale;
        public static float trainXOffset = 465 / distanceScale;
        public static float trainHeadXOffset = 900 / distanceScale;
        public float trainSpeed = 300 / distanceScale;
        public static float speedParamN = 3;
        public static float speedParamK = 10;

        public static float speedParamX = 0.6f;

        // public static float speedParamN = 2f;
        // public static float speedParamK = 0;
        // public static float speedParamX = 1f;
        public static float beginXPos = -(2000 - 600) / distanceScale;
        public static float endXPos = (2000 - 600) / distanceScale;
        public static float waitTime = 0f;
        public static float numScale = 1f;
        public static float numDropTime = 0.4f;
        public static float beforeMoveCollectNumTime = 0f;
        public static float moveCollectNumTime = 0.66f;
        public static float afterMoveCollectNumTime = 1f;
        public static float moveCollectNumScale = 0.5f;
        public long collectValue;
        public float nextDropX;
        public Vector3 defaultTrainPosition;

        public TrainView11030(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            recycleTrainPool = new List<TrainCabin11030>();
            JpTypeDictionary = new Dictionary<uint, TextMesh>()
            {
                {18, purpleNum},
                {17, blueNum},
                {16, greenNum},
                {15, redNum},
            };
            JpBoardDictionary = new Dictionary<uint, Transform>()
            {
                {18, purpleBoard},
                {17, blueBoard},
                {16, greenBoard},
                {15, redBoard},
            };
            JpTipDictionary = new Dictionary<uint, Transform>()
            {
                {18, purpleTip},
                {17, blueTip},
                {16, greenTip},
                {15, redTip},
            };
            JpGroupDictionary = new Dictionary<uint, Animator>()
            {
                {18, purpleGroup},
                {17, blueGroup},
                {16, greenGroup},
                {15, redGroup},
            };
        }

        public void InitAfterBindingContext()
        {
            extraState = context.state.Get<ExtraState11030>();
            defaultTrainPosition = transform.Find("TrainGroup").position;
        }

        public void RefreshViewState()
        {
            CollectValueDictionary = new Dictionary<uint, long>()
            {
                {18, 0},
                {17, 0},
                {16, 0},
                {15, 0},
            };
            for (uint i = 15; i <= 18; i++)
            {
                var num = GetNumByID(i);
                num.gameObject.SetActive(true);
                num.text = "";
                var numBoardCover = GetNumBoardCoverByID(i);
                numBoardCover.gameObject.SetActive(true);
                var numTip = GetNumTipCoverByID(i);
                numTip.gameObject.SetActive(false);
                var numAnimator = GetNumGroupCoverByID(i);
                numAnimator.gameObject.SetActive(true);
                // XUtility.PlayAnimation(numAnimator, "disIdle");
            }

            availableTip.gameObject.SetActive(false);
            collectNum.text = "";
        }

        public TextMesh GetNumByID(uint jpType)
        {
            return JpTypeDictionary[jpType];
        }

        public Transform GetNumBoardCoverByID(uint jpType)
        {
            return JpBoardDictionary[jpType];
        }

        public Animator GetNumGroupCoverByID(uint jpType)
        {
            return JpGroupDictionary[jpType];
        }

        public Transform GetNumTipCoverByID(uint jpType)
        {
            return JpTipDictionary[jpType];
        }

        public async void UpdateCollectNum(long showCollectValue)
        {
            collectNumAnimationController.gameObject.SetActive(true);
            XUtility.PlayAnimation(collectNumAnimationController, "RewardEFX",
                () =>
                {
                    // collectNumAnimationController.gameObject.SetActive(false);
                });
            if (!collectNum.gameObject.activeSelf)
            {
                collectNum.gameObject.SetActive(true);
            }

            collectNum.text = Tools.GetLeastDigits(showCollectValue, 12);
        }

        public async Task StartPerform()
        {
            performTrain = new List<TrainCabin11030>();
            collectValue = 0;
            nextDropX = trainHeadXOffset + trainXOffset / 2;
            var trainData = extraState.GetNextRunningTrain();
            {
                var totalTrainLength = trainData.Results.Count * trainXOffset + trainHeadXOffset - beginXPos + endXPos;
                trainSpeed = totalTrainLength /
                             (Math.Pow(trainData.Results.Count, speedParamX).ToFloat() * speedParamN + speedParamK);
                trainSpeed = 400 / distanceScale;
            }
            var trainType = trainData.Id;
            var numTip = GetNumTipCoverByID(trainType);
            numTip.gameObject.SetActive(true);
            availableTip.gameObject.SetActive(true);
            var trainContainer = new GameObject("train");
            trainContainer.SetActive(true);
            trainContainer.transform.SetParent(transform, false);
            float nowWaitTime = waitTime + (-beginXPos / trainSpeed);
            var collectCompleteTask = new TaskCompletionSource<bool>();
            var trainCrossCompleteTask = new TaskCompletionSource<bool>();
            context.AddWaitTask(collectCompleteTask, null);
            context.AddWaitTask(trainCrossCompleteTask, null);
            float nowXOffset = 0;
            for (var i = 0; i < 8; i++)
            {
                NewContainer(trainContainer);
            }
            {
                nowXOffset -= trainHeadXOffset / 2;
                nowWaitTime += trainHeadXOffset / 2 / trainSpeed;
                var localXOffset = nowXOffset;
                var localWaitTime = nowWaitTime;
                var localCreateTime = localWaitTime - (-beginXPos / trainSpeed);
                var localDestroyTime = localWaitTime + endXPos / trainSpeed;
                nowXOffset -= trainHeadXOffset / 2;
                nowWaitTime += trainHeadXOffset / 2 / trainSpeed;
                TrainCabin11030 singleCabin;
                var tempTrainHeadData = new GoldRushTrainGameResultExtraInfo.Types.Train.Types.Result();
                tempTrainHeadData.JackpotId = 0;
                tempTrainHeadData.JackpotPay = 0;
                tempTrainHeadData.WinRate = 0;
                context.WaitSeconds(localCreateTime, () =>
                {
                    if (recycleTrainPool.Count == 0)
                    {
                        NewContainer(trainContainer);
                        XDebug.LogError("车厢池子不够用了");
                    }
                    singleCabin = CreateCabin(localXOffset,tempTrainHeadData, trainType);
                    context.WaitSeconds(localDestroyTime - localCreateTime, () =>
                    {
                        DestroyCabin(singleCabin);
                    });
                });
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
                TrainCabin11030 singleCabin;
                var cabinData = trainData.Results[localI];
                var jackpotType = cabinData.JackpotId;
                if (localI == trainData.Results.count - 1 && (jackpotType == 0 || jackpotType == 1))
                {
                    context.RemoveTask(collectCompleteTask);
                    collectCompleteTask.SetResult(false);
                }
                context.WaitSeconds(localCreateTime, () =>
                {
                    if (recycleTrainPool.Count == 0)
                    {
                        NewContainer(trainContainer);
                        XDebug.LogError("车厢池子不够用了");
                    }
                    singleCabin = CreateCabin(localXOffset,cabinData, trainType);
                    context.WaitSeconds(localWaitTime - localCreateTime, () =>
                    {
                        PerformDrop(singleCabin, collectCompleteTask);
                        context.WaitSeconds(localDestroyTime - localWaitTime, () =>
                        {
                            DestroyCabin(singleCabin);
                        });
                    });
                });
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
            AudioUtil.Instance.PlayAudioFx("Video2", true);
            await trainCrossCompleteTask.Task;
            AudioUtil.Instance.StopAudioFx("Video2");
            await collectCompleteTask.Task;
            GameObject.Destroy(trainContainer.gameObject);
            performTrain.Clear();
            await PerformCollectToSmallBoard(trainType);
        }

        public void NewContainer(GameObject trainContainer)
        {
            var cabinContainer = new GameObject("cabin");
            cabinContainer.transform.SetParent(trainContainer.transform, false);
            var singleCabin = new TrainCabin11030(cabinContainer.transform);   
            singleCabin.Initialize(context);
            cabinContainer.SetActive(false);
            recycleTrainPool.Add(singleCabin);
        }
        public void DestroyCabin(TrainCabin11030 singleCabin)
        {
            performTrain.Remove(singleCabin);
            recycleTrainPool.Add(singleCabin);
            singleCabin.ReCycleCabin();
            singleCabin.transform.gameObject.SetActive(false);
            // GameObject.Destroy(cabinContainer);
        }
        public void PerformDrop(TrainCabin11030 singleCabin,TaskCompletionSource<bool> collectCompleteTask)
        {
            var dropValue = singleCabin.GetContainerValue();
            var jackpotType = singleCabin.GetJackpotType();
            var dropObject = singleCabin.GetContainer();
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
            dropSequence.AppendCallback(async () =>
            {
                AudioUtil.Instance.PlayAudioFx("FlyStop");
                GameObject.Destroy(tempDropObject);
                if (jackpotType > 0 && jackpotType != 1)
                {
                    await ShowJackpotPopup<UIJackpotBase11030>((int) jackpotType, dropValue);
                    context.RemoveTask(collectCompleteTask);
                    collectCompleteTask.SetResult(true);
                }
                collectValue += dropValue;
                UpdateCollectNum(collectValue);
            });
            dropSequence.target = context.transform;
        }
        public TrainCabin11030 CreateCabin(float localXOffset,GoldRushTrainGameResultExtraInfo.Types.Train.Types.Result cabinData,uint trainType)
        {
            TrainCabin11030 singleCabin;
            GameObject cabinContainer;
            singleCabin = recycleTrainPool.Pop();
            cabinContainer = singleCabin.transform.gameObject;
            cabinContainer.transform.localPosition = new Vector3(localXOffset,0,0);
            cabinContainer.SetActive(true);
            singleCabin.SetCabinData(cabinData, trainType);
            performTrain.Add(singleCabin);
            return singleCabin;
        }
        public async Task PerformCollectToSmallBoard(uint trainType)
        {
            var trainCollectNumCompleteTask = new TaskCompletionSource<bool>();
            context.AddWaitTask(trainCollectNumCompleteTask, null);
            var numTip = GetNumTipCoverByID(trainType);
            var numObject = GetNumByID(trainType);
            var numBoardCover = GetNumBoardCoverByID(trainType);
            var numGroup = GetNumGroupCoverByID(trainType);
            bool firstLight = !(CollectValueDictionary[trainType] > 0);
            CollectValueDictionary[trainType] += collectValue;
            var nowCollectValue = CollectValueDictionary[trainType];

            // var collectNumMoveObject = context.assetProvider.InstantiateGameObject("FlyToSmall");
            var collectNumMoveObject = context.assetProvider.InstantiateGameObject("FlyNew");
            collectNumMoveObject.transform.parent = transform;
            collectNumMoveObject.transform.position = collectNum.transform.position;
            var collectNumMoveAnimator = collectNumMoveObject.GetComponent<Animator>();
            // XUtility.PlayAnimation(collectNumMoveAnimator, "FlyToSmall");
            XUtility.PlayAnimation(collectNumMoveAnimator, "Fly");
            // var collectNumMoveSortingGroup = collectNumMoveObject.AddComponent<SortingGroup>();
            // collectNumMoveSortingGroup.sortingLayerName = "LocalFx";
            // collectNumMoveSortingGroup.sortingOrder = 1;
            collectNumAnimationController.gameObject.SetActive(true);
            XUtility.PlayAnimation(collectNumAnimationController, "RewardEFX");
            numTip.gameObject.SetActive(false);
            availableTip.gameObject.SetActive(false);
            // var numContainer = collectNumMoveObject.transform.Find("scaleObject");
            // var showNum = GameObject.Instantiate(collectNum.gameObject,numContainer, true);
            collectNum.gameObject.SetActive(false);
            var flySequence = DOTween.Sequence();
            flySequence.AppendInterval(beforeMoveCollectNumTime);
            Vector3 fromLocalPos = collectNumMoveObject.transform.localPosition;
            Vector3 targetWorldPos = numObject.transform.position;
            Vector3 targetLocalPos = collectNumMoveObject.transform.parent.InverseTransformPoint(targetWorldPos);
            Vector3 midLocalPos = (fromLocalPos + targetLocalPos) * 0.5f;
            midLocalPos.y += 0.5f;
            Vector3[] wayPoints = new[] {fromLocalPos, midLocalPos, targetLocalPos};
            flySequence.AppendCallback(() =>
            {
                AudioUtil.Instance.PlayAudioFx("Disappear");
            });
            flySequence.Append(collectNumMoveObject.transform.DOLocalPath(wayPoints, moveCollectNumTime, PathType.CatmullRom, PathMode.Full3D, 10).SetEase(Ease.InQuad));
            // flySequence.Append(collectNumMoveObject.transform.DOMove(numObject.transform.position, moveCollectNumTime).SetEase(Ease.InSine));
            flySequence.AppendCallback(() =>
            {
                XUtility.PlayAnimation(collectNumMoveAnimator, "FX",() => { GameObject.Destroy(collectNumMoveObject); });
                // showNum.SetActive(false);
                // context.WaitSeconds(afterMoveCollectNumTime, () => { GameObject.Destroy(collectNumMoveObject); });
                numObject.text = nowCollectValue.GetAbbreviationFormat(1);
                AudioUtil.Instance.PlayAudioFx("LightUp");
                if (firstLight)
                {
                    XUtility.PlayAnimation(numGroup, "intro");
                }
                else
                {
                    XUtility.PlayAnimation(numGroup, "Add");
                }

                // if (numBoardCover.gameObject.activeSelf)
                // {
                //     numBoardCover.gameObject.SetActive(false);
                // }
                context.RemoveTask(trainCollectNumCompleteTask);
                trainCollectNumCompleteTask.SetResult(true);
            });
            flySequence.target = context.transform;
            await trainCollectNumCompleteTask.Task;
        }

        public async Task PerformCollectToCenterBoard()
        {
            int smallNumCount = 0;
            for (uint i = 15; i <= 18; i++)
            {
                if (CollectValueDictionary[i] > 0)
                {
                    smallNumCount++;
                }
            }

            if (smallNumCount < 2)
            {
                return;
            }

            var tempTaskList = new List<Task>();
            long totalAddValue = 0;
            for (uint i = 15; i <= 18; i++)
            {
                if (CollectValueDictionary[i] > 0)
                {
                    var trainType = i;
                    totalAddValue += CollectValueDictionary[trainType];
                    var trainCollectNumCompleteTask = new TaskCompletionSource<bool>();
                    tempTaskList.Add(trainCollectNumCompleteTask.Task);
                    context.AddWaitTask(trainCollectNumCompleteTask, null);
                    var numTip = GetNumTipCoverByID(trainType);
                    var numObject = GetNumByID(trainType);
                    var numBoardCover = GetNumBoardCoverByID(trainType);
                    var numGroup = GetNumGroupCoverByID(trainType);
                    CollectValueDictionary[trainType] += collectValue;

                    // var collectNumMoveObject = context.assetProvider.InstantiateGameObject("FlyToCenter");
                    var collectNumMoveObject = context.assetProvider.InstantiateGameObject("FlyNew");
                    collectNumMoveObject.transform.parent = transform;
                    collectNumMoveObject.transform.position = numObject.transform.position;
                    var collectNumMoveAnimator = collectNumMoveObject.GetComponent<Animator>();
                    XUtility.PlayAnimation(collectNumMoveAnimator, "Fly");
                    // XUtility.PlayAnimation(collectNumMoveAnimator, "FlyToCenter");
                    // var collectNumMoveSortingGroup = collectNumMoveObject.AddComponent<SortingGroup>();
                    // collectNumMoveSortingGroup.sortingLayerName = "LocalFx";
                    // collectNumMoveSortingGroup.sortingOrder = 1;

                    numTip.gameObject.SetActive(false);
                    availableTip.gameObject.SetActive(false);
                    // var showNum = GameObject.Instantiate(numObject.gameObject,
                    //     collectNumMoveObject.transform.Find("scaleObject"), true);
                    numObject.gameObject.SetActive(false);
                    XUtility.PlayAnimation(numGroup, "Dis");
                    var flySequence = DOTween.Sequence();
                    flySequence.AppendInterval(beforeMoveCollectNumTime);
                    Vector3 fromLocalPos = collectNumMoveObject.transform.localPosition;
                    Vector3 targetWorldPos = collectNum.transform.position;
                    Vector3 targetLocalPos = collectNumMoveObject.transform.parent.InverseTransformPoint(targetWorldPos);
                    Vector3 midLocalPos = (fromLocalPos + targetLocalPos) * 0.5f;
                    midLocalPos.y += 1f;
                    Vector3[] wayPoints = new[] {fromLocalPos, midLocalPos, targetLocalPos};
                    flySequence.Append(collectNumMoveObject.transform.DOLocalPath(wayPoints, moveCollectNumTime, PathType.CatmullRom, PathMode.Full3D, 10).SetEase(Ease.InQuad));
                    // flySequence.Append(collectNumMoveObject.transform.DOMove(collectNum.transform.position, moveCollectNumTime));
                    flySequence.AppendCallback(() =>
                    {
                        // showNum.SetActive(false);
                        XUtility.PlayAnimation(collectNumMoveAnimator, "FX",() => { GameObject.Destroy(collectNumMoveObject); });
                        // context.WaitSeconds(afterMoveCollectNumTime, () => { GameObject.Destroy(collectNumMoveObject); });
                        context.RemoveTask(trainCollectNumCompleteTask);
                        trainCollectNumCompleteTask.SetResult(true);
                    });
                    flySequence.target = context.transform;
                }
            }
            
            AudioUtil.Instance.PlayAudioFx("4TrainFly");   
            await Task.WhenAll(tempTaskList);
            AudioUtil.Instance.PlayAudioFx("4TrainFlyStop");
            collectNum.gameObject.SetActive(true);
            collectNum.text = Tools.GetLeastDigits(totalAddValue, 12);
            collectNumAnimationController.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(collectNumAnimationController, "RewardEFX");
            // collectNumAnimationController.gameObject.SetActive(false);
        }

        public void ReconnectCollectValue(GoldRushTrainGameResultExtraInfo.Types.Train train)
        {
            var trainType = train.Id;
            var winRate = (long) train.TotalWinRate;
            var chips = context.state.Get<BetState>().GetPayWinChips(winRate);
            CollectValueDictionary[trainType] += chips;
        }

        public void ReconnectCollectValueView()
        {
            var keyList = new List<uint>(CollectValueDictionary.Keys);
            for (int i = 0; i < keyList.Count; i++)
            {
                var tempCollectValue = CollectValueDictionary[keyList[i]];
                if (tempCollectValue > 0)
                {
                    var numObject = GetNumByID(keyList[i]);
                    numObject.text = tempCollectValue.GetAbbreviationFormat(1);
                    var numBoardCover = GetNumBoardCoverByID(keyList[i]);
                    numBoardCover.gameObject.SetActive(false);
                    var numGroup = GetNumGroupCoverByID(keyList[i]);
                    XUtility.PlayAnimation(numGroup,"Idle");
                }
            }
        }

        protected virtual async Task ShowJackpotPopup<T>(int jpType, long chips) where T : UIJackpotBase11030
        {
            string address = "UIJackpot" + Constant11030.JackpotName[jpType] + context.assetProvider.AssetsId;

            if (context.assetProvider.GetAsset<GameObject>(address) == null)
            {
                XDebug.LogError($"ShowJackmpotPopUp:{address} is Not Exist");
                return;
            }

            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
            context.AddWaitTask(waitTask, null);
            AudioUtil.Instance.PlayAudioFx("Jackpot");
            var startPopUp = PopUpManager.Instance.ShowPopUp<T>(address);
            startPopUp.SetJackpotWinNum((ulong) chips);
            startPopUp.SetPopUpCloseAction(() =>
            {
                context.RemoveTask(waitTask);
                waitTask.SetResult(true);
            });

            await waitTask.Task;
        }
    }
}