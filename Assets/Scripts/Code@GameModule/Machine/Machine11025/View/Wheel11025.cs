using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Dlugin;
using DragonU3DSDK.Network.API.ILProtocol;
using Facebook.Unity.Editor;
using Google.ilruntime.Protobuf.Collections;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace GameModule
{
    public enum RollMaskOpacityLevel11025
    {
        None,
        Level1,
        Level2,
        Level3,
        Full,
    }
    public enum FrameColor11025
    {
        None,
        Blue,
        Green,
        Yellow,
    }
    public class FrameData11025
    {
        public static MachineContext machineContext;
        public uint length;
        public Animator frame;
        public FrameColor11025 nowColor = FrameColor11025.None;

        public static void SetContext(MachineContext context)
        {
            machineContext = context;
        }

        public static void PutFrameData(FrameData11025 classObject)
        {
            machineContext.classProvider.RecycleClassObject(classObject);
        }
        public static FrameData11025 GetFrameData()
        {
            var frameData = machineContext.classProvider.InstantiateClassObject<FrameData11025>();
            return frameData;
        }

        public static string GetFrameAssetName(FrameColor11025 colorType)
        {
            string frameAssetName;
            switch (colorType)
            {
                case FrameColor11025.Blue:
                    frameAssetName = "BlueFrame";
                    break;
                case FrameColor11025.Green:
                    frameAssetName = "GreenFrame";
                    break;
                case FrameColor11025.Yellow:
                    frameAssetName = "YellowFrame";
                    break;
                default:
                    frameAssetName = "";
                    break;
            }
            return frameAssetName;
        }
        public FrameData11025()
        {
            frame = null;
            length = 0;
            nowColor = FrameColor11025.None;
        }
        public void SetFrameParent(Transform parent,bool worldPositionStays = false)
        {
            var frameTransform = frame.transform;
            frameTransform.SetParent(parent,worldPositionStays);
        }

        public void SetFrameLocalPosition(Vector3 localPosition)
        {
            var frameTransform = frame.transform;
            frameTransform.localPosition = localPosition;
        }
        public GameObject GetFrame(FrameColor11025 colorType)
        {
            string frameAssetName = GetFrameAssetName(colorType);
            var newFrame = machineContext.assetProvider.InstantiateGameObject(frameAssetName,true);
            return newFrame;
        }
        public void PutFrame(GameObject frameObject,FrameColor11025 colorType)
        {
            string frameAssetName = GetFrameAssetName(colorType);
            machineContext.assetProvider.RecycleGameObject(frameAssetName,frameObject);
        }
        public void InitState(FrameColor11025 colorType)
        {
            nowColor = colorType;
            frame = GetFrame(nowColor).GetComponent<Animator>();
            frame.keepAnimatorControllerStateOnDisable = true;
        }
        public void SetLength(uint inLength,bool force = false)
        {
            length = inLength;
            if (frame != null)
            {
                var animatorName = "Frame" + length;
                if (force)
                {
                    animatorName += "_Idle";
                }
                XUtility.PlayAnimation(frame,animatorName);
            }
        }

        public void RemoveSelf()
        {
            PutFrame(frame.gameObject, nowColor);
            frame = null;
            length = 0;
            nowColor = FrameColor11025.None;
            PutFrameData(this);
        }
    }

    public class ChameleonNode : TransformHolder
    {
        public Animator tongueAnimator;
        public Animator animator;
        public TextMesh valueText;
        public long collectValue;
        public static List<float> tongueLickTime = new List<float>()
        {
            9f/30,
            9f/30,
            9f/30,
            9f/30,
            12f/30,
            10f/30,
            10f/30,
            9f/30,
        };
        public ChameleonNode(Transform inTransform, MachineContext context) : base(inTransform)
        {
            Initialize(context);
            animator = transform.GetComponent<Animator>();
            collectValue = 0;
            valueText = transform.Find("Root/NumericalValue/NumText").GetComponent<TextMesh>();
            tongueAnimator = transform.Find("Root/shetou").GetComponent<Animator>();
            tongueAnimator.enabled = false;
            tongueAnimator.gameObject.SetActive(false);
        }

        public void RandomPlayChameleonIdle()
        {
            float idleLength = 6f;
            int partNum = 12;
            var randomNum = Random.Range(0, partNum);
            float randomNormalizedNum = (float)randomNum/partNum;
            animator.Play("NumericalValueIdle", -1, randomNormalizedNum);
            var skeletonAnimation = transform.Find("Root/New_SkeletonAnimation").GetComponent<SkeletonAnimation>();
            var animationState = skeletonAnimation.state;
            if (animationState != null)
            {
                var animation = animationState.SetAnimation(0, "Idle", true);
                animation.TrackTime = idleLength * randomNormalizedNum;   
            }
            else
            {
                XDebug.Log("animationState is null");
            }
        }
        
        public void SetValue(long inValue)
        {
            collectValue = inValue;
            valueText.text = Tools.GetLeastDigits(inValue, 3);
        }

        public void AddValue(long inValue)
        {
            SetValue(collectValue + inValue);
        }
        public long GetValue()
        {
            return collectValue;
        }
        public async Task OpenMouth()
        {
            SetValue(0);
            await XUtility.PlayAnimationAsync(animator,"Openmouth");
        }

        public void EatOpen()
        {
            XUtility.PlayAnimation(animator,"BigEatOpen");
        }
        public async void EatCoin()
        {
            // XUtility.PlayAnimation(animator,"Eat");
            transform.gameObject.SetActive(false);
            transform.gameObject.SetActive(true);
            XUtility.PlayAnimation(animator,"SmallEatopen");
            await context.WaitSeconds(0.2f);
            XUtility.PlayAnimation(animator,"BigEatClose");
            var particle = context.assetProvider.InstantiateGameObject("FX_HitRainbow",true);
            particle.transform.SetParent(transform,false);
            particle.SetActive(false);
            particle.SetActive(true);
            context.WaitSeconds(1f, () =>
            {
                context.assetProvider.RecycleGameObject("FX_HitRainbow",particle);
            });
        }
        public void EatJackpot()
        {
            XUtility.PlayAnimation(animator,"BigEatClose");
            var particle = context.assetProvider.InstantiateGameObject("FX_HitRainbow",true);
            particle.transform.SetParent(transform,false);
            particle.SetActive(false);
            particle.SetActive(true);
            context.WaitSeconds(1f, () =>
            {
                context.assetProvider.RecycleGameObject("FX_HitRainbow",particle);
            });
        }
        public async Task LickJackpot(int length)
        {
            XUtility.PlayAnimation(animator,"BigEatOpen");
            await context.WaitSeconds(0.7f);
            tongueAnimator.gameObject.SetActive(true);
            tongueAnimator.enabled = true;
            XUtility.PlayAnimation(tongueAnimator,length.ToString(), () =>
            {
                tongueAnimator.enabled = false;
            });
            await context.WaitSeconds(0.18f);
            // await context.WaitSeconds(tongueLickTime[length-1]);
        }
        // public void CloseMouth()
        // {
        //     // XUtility.PlayAnimation(animator,"Toshutup");
        // }

        public async void CollectValue()
        {
            await XUtility.PlayAnimationAsync(animator,"Close");
            SetValue(0);
        }
    }
    public class RollPointBar : TransformHolder
    {
        public Animator animator;
        public int pointCount;
        public List<Transform> pointList;
        public bool showState;
        public RollPointBar(Transform inTransform,MachineContext context):base(inTransform)
        {
            pointList = new List<Transform>();
            Initialize(context);
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
            for (var i = 0; i < 3; i++)
            {
                pointList.Add(transform.Find("Spot"+(i+1)).Find("Gold"));
            }
            pointCount = 0;
            RefreshPointView();
            showState = false;
            // Hide();
        }

        public void ShowBar(bool activeFlag)
        {
            if (activeFlag != showState)
            {
                showState = activeFlag;
                if (showState)
                {
                    // Show();
                    XUtility.PlayAnimation(animator,"SetPoint_Open");
                }
                else
                {
                    XUtility.PlayAnimation(animator,"SetPoint_Close");
                }
            }
        }

        public void ReducePoint()
        {
            if (pointCount > 0)
            {
                SetPoint(pointCount-1);
            }
        }
        public bool UpdatePoint(int targetPoint,bool force,out bool playRefresh,out bool playSlide)
        {
            bool waitFlag = false;
            playRefresh = false;
            playSlide = false;
            if (force)
            {
                if (targetPoint > 0)
                {
                    ShowBar(true);
                }
                else
                {
                    ShowBar(false);
                }
            }
            else
            {
                if (showState)
                {
                    if (targetPoint == 3)
                    {
                        playRefresh = true;
                        // AudioUtil.Instance.PlayAudioFx("Respin_Refresh");
                        XUtility.PlayAnimation(animator,"SetPoint_Win");
                        waitFlag = true;
                    }
                    else if (targetPoint == 0)
                    {
                        ShowBar(false);
                        waitFlag = true;
                    }   
                }
                else
                {
                    if (targetPoint > 0)
                    {
                        playSlide = true;
                        // AudioUtil.Instance.PlayAudioFx("Respin_Slide");
                        ShowBar(true);
                        waitFlag = true;
                    }
                }   
            }
            SetPoint(targetPoint);
            return waitFlag;
        }
        public void SetPoint(int targetPoint)
        {
            pointCount = targetPoint;
            RefreshPointView();
        }
        public void RefreshPointView()
        {
            for (var i = 0; i < pointList.Count; i++)
            {
                pointList[i].gameObject.SetActive(pointCount>i);
            }
        }
        
    }
    public class Wheel11025:IrregularWheel
    {
        public GameObject StickyNode;
        public List<GameObject> StickyRoll;
        public List<Dictionary<uint, StickyElementContainer11025>> StickyMap;
        public List<Dictionary<uint,FrameData11025>> FrameDataMap;
        public ExtraState11025 extraState;
        [ComponentBinder("MaskGrop")] 
        public Transform rollMaskGroup;
        [ComponentBinder("ChameleonGrop")] 
        public Transform chameleonGrop;

        public List<SpriteRenderer> rollMaskList;
        public Dictionary<int,Tweener> rollMaskTweenersList;
        public List<RollPointBar> pointBarList;
        public List<ChameleonNode> chameleonList;
        public Wheel11025(Transform transform) : base(transform)
        {
            ComponentBinder.BindingComponent(this,transform);
            rollMaskList = new List<SpriteRenderer>();
            rollMaskTweenersList = new Dictionary<int,Tweener>();
            pointBarList = new List<RollPointBar>();
            chameleonList = new List<ChameleonNode>();
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            extraState = context.state.Get<ExtraState11025>();
        }

        public override void BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(WheelState inWheelState,
            string inWheelElementSortingLayerName = "Element")
        {
            base.BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(inWheelState,inWheelElementSortingLayerName);
            for (var i = 0; i < rollCount; i++)
            {
                rollMaskList.Add(rollMaskGroup.Find("WheelMask"+(5-i)).GetComponent<SpriteRenderer>());
                SetRollMaskColor(i,RollMaskOpacityLevel11025.None);
                
                pointBarList.Add(new RollPointBar(chameleonGrop.Find("SetPointContainer"+(i+1)+"/SetPoint"+(i+1)),context));
                chameleonList.Add(new ChameleonNode(chameleonGrop.Find("Chameleon"+(i+1)),context));
            }
        }

        public void RandomPlayChameleonIdle()
        {
            for (var i = 0; i < chameleonList.Count; i++)
            {
                chameleonList[i].RandomPlayChameleonIdle();
            }
        }

        public override void SetActive(bool active,bool keepObjectState = false)
        {
            base.SetActive(active,keepObjectState);
            RandomPlayChameleonIdle();
        }
        
        public bool UpdateChameleonState(RepeatedField<uint> columnState,bool force = false)
        {
            bool waitFlag = false;
            bool playRefresh = false;
            bool playSlide = false;
            for (var i = 0; i < rollCount; i++)
            {
                bool tempPlayRefresh;
                bool tempPlaySlide;
                if (pointBarList[i].UpdatePoint((int) columnState[i], force,out tempPlayRefresh,out tempPlaySlide))
                {
                    waitFlag = true;
                }

                if (tempPlayRefresh)
                {
                    playRefresh = true;
                }
                if (tempPlaySlide)
                {
                    playSlide = true;
                }
            }

            if (playRefresh)
            {
                AudioUtil.Instance.PlayAudioFx("Respin_Refresh");
            }
            if (playSlide)
            {
                AudioUtil.Instance.PlayAudioFx("Respin_Slide");
            }
            return waitFlag;
        }

        public void ReducePointOnSpinning()
        {
            for (var i = 0; i < pointBarList.Count; i++)
            {
                pointBarList[i].ReducePoint();
            }
        }

        public void SetRollMaskColor(int rollIndex, RollMaskOpacityLevel11025 opacityLevel, float time = 0)
        {
            float opacity = 0;
            switch (opacityLevel)
            {
                case RollMaskOpacityLevel11025.None:
                    opacity = 0;
                    break;
                case RollMaskOpacityLevel11025.Level1:
                    opacity = 0.25f;
                    break;
                case RollMaskOpacityLevel11025.Level2:
                    opacity = 0.45f;
                    break;
                case RollMaskOpacityLevel11025.Level3:
                    opacity = 0.65f;
                    break;
                case RollMaskOpacityLevel11025.Full:
                    opacity = 1f;
                    break;
            }
            SetRollMaskColor(rollIndex, opacity, time);
        }
        public void SetRollMaskColor(int rollIndex, float opacity, float time = 0)
        {
            var rollMask = rollMaskList[rollIndex];
            if (rollMaskTweenersList.ContainsKey(rollIndex))
            {
                rollMaskTweenersList[rollIndex].Kill();
                rollMaskTweenersList.Remove(rollIndex);
            }
            if (time > 0)
            {
                var tweener = DOTween.To(setter: value =>
                        {
                            var tempColor = rollMask.color;
                            tempColor.a = value;
                            rollMask.color = tempColor;
                        },
                        startValue: rollMask.color.a, endValue: opacity, duration: time)
                    .SetEase(Ease.Linear);
                rollMaskTweenersList[rollIndex] = tweener;
            }
            else
            {
                var tempColor = rollMask.color;
                tempColor.a = opacity;
                rollMask.color = tempColor;
            }
        }

        public void CleanWheel()
        {
            for (var i = 0; i < rollCount; i++)
            {
                CleanRoll(i);
            }
        }
        public void CleanRoll(int rollIndex)
        {
            CleanFrameRoll(rollIndex);
            CleanStickyRoll(rollIndex);
        }
        public void CleanFrameRoll(int rollIndex)
        {
            var frameDataMap = FrameDataMap[rollIndex];
            var frameKeyList = frameDataMap.Keys.ToList();
            for (var i = 0; i < frameKeyList.Count; i++)
            {
                frameDataMap[frameKeyList[i]].RemoveSelf();
            }
            frameDataMap.Clear();
        }
        public void CleanStickyRoll(int rollIndex)
        {
            var stickyRollMap = StickyMap[rollIndex];
            var stickyKeyList = stickyRollMap.Keys.ToList();
            for (var i = 0; i < stickyKeyList.Count; i++)
            {
                stickyRollMap[stickyKeyList[i]].RemoveStickyElement();
            }
        }
        public void SetUpStickyNode()
        {
            FrameDataMap = new List<Dictionary<uint,FrameData11025>>();
            StickyNode = new GameObject("StickyNode");
            StickyNode.transform.SetParent(transform,false);
            StickyNode.transform.localPosition = wheelMask.transform.localPosition;
            StickyMap = new List<Dictionary<uint, StickyElementContainer11025>>();
            StickyRoll = new List<GameObject>();
            var rollHeightList = Constant11025.RollHeightList;
            for (var i = 0; i < rollHeightList.Count; i++)
            {
                var roll = GetRoll(i);
                var stickyRoll = new GameObject("StickyRoll"+i);
                stickyRoll.transform.SetParent(StickyNode.transform);
                stickyRoll.transform.localPosition = GetRollPosition(i);
                StickyRoll.Add(stickyRoll);
                var stickyElementContainerList = new Dictionary<uint, StickyElementContainer11025>();
                for (var j = 0; j < rollHeightList[i]; j++)
                {
                    var stickyElementContainer = new GameObject("StickyElementContainer");
                    stickyElementContainer.transform.SetParent(stickyRoll.transform);
                    stickyElementContainer.transform.localPosition = roll.GetVisibleContainerLocalPosition(j);
                    var stickyElement = new StickyElementContainer11025(stickyElementContainer.transform,"Element");
                    stickyElement.SetSortingOrder(i,j);
                    stickyElement.Initialize(context);
                    stickyElementContainerList[(uint)j] = stickyElement;
                }
                StickyMap.Add(stickyElementContainerList);
                var rollFrameDataList = new Dictionary<uint,FrameData11025>();
                FrameDataMap.Add(rollFrameDataList);
            }
        }

        public async Task UpdateNormalDataPanel(
            RepeatedField<ChameleonGameResultExtraInfo.Types.StickyItem> stickyItems,
            RepeatedField<uint> fullRollIndexList = null,
            RepeatedField<uint> cleanRollIndexList = null,
            bool force = false,bool ignoreFullFrame = false)
        {
            var newStickyList = new List<StickyElementContainer11025>();
        	var newStickyRollList = new List<int>(){0,0,0,0,0};
            var nowStickyRollList = new List<int>(){0,0,0,0,0};
        	for (var i = 0; i < stickyItems.Count; i++)
        	{
                var stickyItem = stickyItems[i];
                if (!(fullRollIndexList != null && fullRollIndexList.Contains(stickyItem.X)))
                {
                    var stickyElement = StickyMap[(int) stickyItem.X][stickyItem.Y];
                    nowStickyRollList[(int) stickyItem.X]++;
                    if (!stickyElement.HasContainer())
                    {
                        stickyElement.SetStickyData(stickyItem,force);
                        newStickyList.Add(stickyElement);
                        newStickyRollList[(int) stickyItem.X]++;
                    }
                    else
                    {
                        stickyElement.SetStickyDataWithoutRefresh(stickyItem);
                    }
                }
            }

        	for (var i = 0; i < newStickyRollList.Count; i++)
        	{
                if (cleanRollIndexList != null && cleanRollIndexList.Contains((uint)i))
                {
                    CleanRoll(i);
                }
                else if (newStickyRollList[i] > 0)
        		{
                    FrameColor11025 frameColor = FrameColor11025.Green;
                    var rollHeight = Constant11025.RollHeightList[i];
                    if (nowStickyRollList[i] == rollHeight-1)
                    {
                        frameColor = FrameColor11025.Blue;
                    }
                    if (nowStickyRollList[i] == rollHeight)
                    {
                        frameColor = FrameColor11025.Yellow;
                    }

                    if (ignoreFullFrame && frameColor == FrameColor11025.Yellow)
                    {
                        continue;
                    }

                    if (!force)
                    {
                        if (frameColor == FrameColor11025.Green)
                        {
                            AudioUtil.Instance.PlayAudioFx("Green");
                        }
                        else if (frameColor == FrameColor11025.Blue)
                        {
                            AudioUtil.Instance.PlayAudioFx("Blue");
                        }
                        else if (frameColor == FrameColor11025.Yellow)
                        {
                            AudioUtil.Instance.PlayAudioFx("Free_Orange_Double");
                        }
                    }
                    await CheckRollFrameUpdate(i,frameColor,force);  
                }
            }
        }
        public async Task CheckRollFrameUpdate(int rollIndex,FrameColor11025 frameColor,bool force = false)
		{
			var roll = GetRoll(rollIndex);
			var stickyRollObject = StickyRoll[rollIndex];
            var rollFrame = FrameDataMap[rollIndex];
            var stickyRoll = StickyMap[rollIndex];
            bool continueFlag = false;
            int headIndex = 0;
            int length = 0;
            bool waitFlag = false;
            for (var j = 0; j < stickyRoll.Count; j++)
            {
                var tempStickyElement = stickyRoll[(uint) j];
                if (tempStickyElement.HasContainer())
                {
                	if (continueFlag)
                	{
                		length++;
                	}
                	else
                	{
                		headIndex = j;
                		length = 1;
                		continueFlag = true;
                	}
                }

                if (continueFlag && (!tempStickyElement.HasContainer() || j == stickyRoll.Count - 1))
                {
                    if (rollFrame.ContainsKey((uint)headIndex) && rollFrame[(uint)headIndex].length == length && rollFrame[(uint)headIndex].nowColor == frameColor)
                	{
                	}
                	else
                	{
                		for (var tempJ = headIndex; tempJ < headIndex + length; tempJ++)
                		{
                			if (rollFrame.ContainsKey((uint) tempJ))
                			{
                				rollFrame[(uint) tempJ].RemoveSelf();
                                rollFrame.Remove((uint) tempJ);
                			}
                		}

                		// var frameData = FrameData11025.GetFrameData(frameColor);

                        var frameData = FrameData11025.GetFrameData();
                        frameData.InitState(frameColor);
                        
                        var centerPos = (roll.GetVisibleContainerLocalPosition(headIndex) +
                                        roll.GetVisibleContainerLocalPosition(headIndex + length - 1))/2;
                        frameData.SetFrameParent(stickyRollObject.transform);
                        frameData.SetFrameLocalPosition(centerPos);
                        frameData.SetLength((uint)length,force);
                        rollFrame[(uint) headIndex] = frameData;
                        waitFlag = true;
                    }
                	headIndex = 0;
                	length = 0;
                	continueFlag = false;
                }
            }

            if (waitFlag && !force)
            {
                await context.WaitSeconds(0.5f);
            }
		}
        
        
        
        public long GetCollectValue(int rollIndex)
        {
            return chameleonList[rollIndex].GetValue();
        }
        public async Task CollectToFloor(int rollIndex)
        {
            var chameleon = chameleonList[rollIndex];
            chameleon.CollectValue();
            var collectParticle = context.assetProvider.InstantiateGameObject("ParticleCollect");
            collectParticle.SetActive(false);
            collectParticle.SetActive(true);
            collectParticle.transform.SetParent(transform,false);
            collectParticle.transform.position = chameleon.transform.position;
            Vector3 fromLocalPos = collectParticle.transform.localPosition;
            Vector3 targetWorldPos = context.view.Get<ControlPanel>().GetWinTextRefWorldPosition(Vector3.zero);
            Vector3 targetLocalPos = collectParticle.transform.parent.InverseTransformPoint(targetWorldPos);
            Vector3 midLocalPos = (fromLocalPos + targetLocalPos) * 0.5f;
            midLocalPos.y += 0.5f;
            Vector3[] wayPoints = new[] {fromLocalPos, midLocalPos, targetLocalPos};
            var tempTask = new TaskCompletionSource<bool>();
            context.AddWaitTask(tempTask,null);
            AudioUtil.Instance.PlayAudioFx("Butterfly_Appear");
            AudioUtil.Instance.PlayAudioFx("Butterfly_Fly");
            collectParticle.transform.DOLocalPath(wayPoints, 0.7f, PathType.CatmullRom, PathMode.Full3D, 10).SetEase(Ease.InQuad).OnComplete(() =>
            {
                AudioUtil.Instance.PlayAudioFx("Butterfly_Fly_Stop");
                context.WaitSeconds(0.5f,()=>
                {
                    GameObject.Destroy(collectParticle);
                });
                var floorParticle = context.assetProvider.InstantiateGameObject("TotaLWinEffetcs");
                floorParticle.transform.SetParent(context.view.Get<ControlPanel>().transform, false);
                floorParticle.SetActive(true);
                floorParticle.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 45, 0);
                context.WaitSeconds(1f,()=>
                {
                    GameObject.Destroy(floorParticle);
                });
                tempTask.SetResult(true);
            });
            await tempTask.Task;
        }

        public void ChameleonPrepareCollectCoin(int rollIndex)
        {
            var chameleon = chameleonList[rollIndex];
            context.WaitSeconds(0, () =>
            {
                chameleon.EatOpen();
            });
        }
        public async void ChameleonCollectCoin(int rollIndex,int rowIndex,long value,int count)
        {
            var chameleon = chameleonList[rollIndex];
            var stickyElementList = StickyMap[rollIndex];
            var elementContainer = stickyElementList[(uint) rowIndex];
            var tempPosition = elementContainer.transform.localPosition;
            context.WaitSeconds(0.3f + 0.2f - 0.2f, () =>
            {
                chameleon.EatCoin();
            });
            elementContainer.GetCollectJumpTween(chameleon.transform.position,count).AppendCallback(() =>
            {
                chameleon.AddValue(value);
                elementContainer.RemoveStickyElement();
                elementContainer.transform.localPosition = tempPosition;
            });
        }
        public async Task ChameleonCollectJackpot(int rollIndex,int rowIndex,long value,int count)
        {
            var chameleon = chameleonList[rollIndex];
            var lickLength = Constant11025.RollHeightList[rollIndex] - rowIndex;
            var stickyElementList = StickyMap[rollIndex];
            var elementContainer = stickyElementList[(uint) rowIndex];
            var tempPosition = elementContainer.transform.localPosition;
            var sortingGroup = elementContainer.transform.gameObject.GetComponent<SortingGroup>();
            var tempSortingLayerID = sortingGroup.sortingLayerID;
            var tempSortingOrder = sortingGroup.sortingOrder;
            sortingGroup.sortingLayerName = "LocalFx";
            sortingGroup.sortingOrder = 6;
            var tempTask = new TaskCompletionSource<bool>();
            context.AddWaitTask(tempTask, null);
            await chameleon.LickJackpot(lickLength);
            context.WaitSeconds(0.5f + 0.3f + 0.2f, () =>
            {
                chameleon.EatJackpot();
            });
            elementContainer.GetCollectJackpotJumpTween(chameleon.transform.position,chameleon.tongueAnimator.transform.Find("Mask"),count).AppendCallback(() =>
            {
                chameleon.AddValue(value);
                elementContainer.RemoveStickyElement();
                elementContainer.transform.localPosition = tempPosition;
                sortingGroup.sortingLayerID = tempSortingLayerID;
                sortingGroup.sortingOrder = tempSortingOrder;
                tempTask.SetResult(true);
            });
            await tempTask.Task;
        }
    }
}