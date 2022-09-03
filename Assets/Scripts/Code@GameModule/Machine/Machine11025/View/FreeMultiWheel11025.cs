using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using SRF;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class SingleColumnWheel11025 : SingleColumnWheel
    {
        public SingleColumnWheel11025(Transform inTransform, float wheelHeight, int inRowCount,
            IElementProvider inElementProvider, int inStartIndex, bool inIsHorizontal = false,
            Action<GameObject> inRecycleFunc = null)
            : base(inTransform,wheelHeight,inRowCount,inElementProvider,inStartIndex,inIsHorizontal,inRecycleFunc)
        {
        }
        private List<float> paramList = new List<float>() {1,-2,-7,-7,-3};
        protected override float GetSlowStateByProcess(float process)
        {
            var x = process;
            float y = 0;
            for (int i = 0; i < paramList.Count; i++)
            {
                y += paramList[i] * (float) Math.Pow(x, i);
            }
            return y;
        }
    }
    public class FreeMultiWheel11025:TransformHolder,IElementProvider
    {
        private Animator animator;

        private bool enableClick = false;

        private SingleColumnWheel11025 _wheel;

        private Transform[] _elementArray;

        private ExtraState11025 extraState;

        private bool IsMulti;
        private bool IsPerformMulti;
        private List<int> wheelItems= new List<int>();
        private int spinResult;
        private List<StickyElementContainer11025> multiElementList;
        private SimpleRollUpdaterEasingConfig slowLoopConfig;
        private SimpleRollUpdaterEasingConfig multiPerformConfig;
        private SimpleRollUpdaterEasingConfig spinConfig;
        private float particleFlyTime = 0.5f;

        public FreeMultiWheel11025(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            // animator.keepAnimatorControllerStateOnDisable = true;
        }
        public void InitAfterBindingContext()
        {
            slowLoopConfig = (SimpleRollUpdaterEasingConfig)context.machineConfig.GetEasingConfig("MultiSlow");
            multiPerformConfig =(SimpleRollUpdaterEasingConfig)context.machineConfig.GetEasingConfig("MultiFast");
            spinConfig = (SimpleRollUpdaterEasingConfig)context.machineConfig.GetEasingConfig("MultiSpin");
            CreateWheelItems();
            _wheel = new SingleColumnWheel11025(transform.Find("Rolls"), 1.46f * 3, 3,
                this, 0,true,RecycleElement);
        }

        public void CreateWheelItems()
        {
            wheelItems.Clear();
            var staticRandomList = new List<int>() {2, 3, 4, 5, 8, 10};
            var groupList = new List<List<int>>() {new List<int>(){2,3},new List<int>(){4,5},new List<int>(){8,10}};
            var lastValue = 0;
            for (var n = 0; n < 5; n++)
            {
                var randomList = new List<int>(staticRandomList);
                for (var i = 0; i < 6; i++)
                {
                    var maxLength = 0;
                    var tempGroupList = new List<List<int>>(){new List<int>(),new List<int>(),new List<int>()};
                    var tempRandomList = new List<int>(randomList);
                    for (var j = 0; j < tempRandomList.Count; j++)
                    {
                        var tempRandomValue = tempRandomList[j];
                        for (var j1 = 0; j1 < groupList.Count; j1++)
                        {
                            if (groupList[j1].Contains(tempRandomValue))
                            {
                                tempGroupList[j1].Add(tempRandomValue);
                                if (tempGroupList[j1].Count > maxLength)
                                {
                                    maxLength = tempGroupList[j1].Count;
                                }
                            }
                        }
                    }
                    for (var j = 0; j < groupList.Count; j++)
                    {
                        if (groupList[j].Contains(lastValue))
                        {
                            tempGroupList.RemoveAt(j);
                            break;
                        }
                    }

                    for (var j = 0; j < tempGroupList.Count; j++)
                    {
                        if (tempGroupList[j].Count < maxLength)
                        {
                            tempGroupList.RemoveAt(j);
                            j--;
                        }
                    }

                    var tempGroup = tempGroupList[Random.Range(0, tempGroupList.Count)];
                    var randomNum = tempGroup[Random.Range(0, tempGroup.Count)];
                    randomList.Remove(randomNum);
                    wheelItems.Add(randomNum);
                    lastValue = randomNum;
                }
            }
        }

        private bool inPerform = false;
        public void InitState()
        {
            IsMulti = false;
            IsPerformMulti = false;
            CreateWheelItems();
            _wheel.ForeUpdateElementContainer(_wheel.GetCurrentIndex());
            XUtility.PlayAnimation(animator,"Idle");
            SlowSpin();
            
            // var btn = new GameObject("PressBtn");
            // btn.AddComponent<Transform>();
            // btn.AddComponent<BoxCollider2D>();
            // btn.GetComponent<BoxCollider2D>().size = new Vector2(4.38f,1.43f);
            // btn.transform.SetParent(transform.Find("TurntableMask"),false);
            // var pointerEventCustomHandler = btn.AddComponent<PointerEventCustomHandler>();
            // pointerEventCustomHandler.BindingPointerClick(async (pointerData) =>
            // {
            //     if (inPerform)
            //         return;
            //     IsMulti = false;
            //     IsPerformMulti = false;
            //     CreateWheelItems();
            //     _wheel.ForeUpdateElementContainer(_wheel.GetCurrentIndex());
            //     XUtility.PlayAnimation(animator,"Idle");
            //     SlowSpin();
            //     
            //     inPerform = true;
            //     var wheelTrans = context.assetProvider.InstantiateGameObject("TransitionWheel");
            //     var sortingGroup = wheelTrans.AddComponent<SortingGroup>();
            //     sortingGroup.sortingLayerID = SortingLayer.NameToID("UI");
            //     sortingGroup.sortingOrder = 5000;
            //     wheelTrans.transform.SetParent(context.transform.Find("Wheels"),false);
            //     wheelTrans.transform.localPosition = new Vector3(0, 4.81f, 0);
            //     wheelTrans.SetActive(true);
            //     XUtility.PlayAnimation(wheelTrans.GetComponent<Animator>(), "Transition", () =>
            //     {
            //         GameObject.Destroy(wheelTrans);
            //     });
            //     AudioUtil.Instance.PlayAudioFx("Store_SuperFree_Wheel_Video");
            //     await context.WaitSeconds(1.2f);
            //     await context.WaitSeconds(0.8f);
            //     AudioUtil.Instance.PlayAudioFx("Store_SuperFree_Wheel_Boom");
            //     await PerformMulti();
            //     inPerform = false;
            // });
        }
        public int GetReelMaxLength()
        {
            return wheelItems.Count;
        }

        public void RecycleElement(GameObject recycleObject)
        {
            var assetName = recycleObject.name.Replace("(Clone)", "");
            context.assetProvider.RecycleGameObject(assetName, recycleObject);
        }
        public GameObject GetElement(int index)
        {
            var elementType = IsPerformMulti?"Fast":IsMulti ? "Double" : "Normal";
            var element = context.assetProvider.InstantiateGameObject("MultiNode"+elementType+"_"+wheelItems[index],true);
            element.transform.localPosition = Vector3.zero;
            return element;
        }

        public int GetElementMaxHeight()
        {
            return 1;
        }

        public int ComputeReelStopIndex(int currentIndex, int slowDownStepCount)
        {
            var stopIndex = (currentIndex - slowDownStepCount + wheelItems.Count) % wheelItems.Count;
            int totalStopIndex = -1;
            if (wheelItems[stopIndex] != spinResult)
            {
                for (var i = stopIndex-1; i >= 0; i--)
                {
                    if (wheelItems[i] == spinResult)
                    {
                        totalStopIndex = i;
                        break;
                    }
                }

                if (totalStopIndex < 0)
                {
                    for (var i = wheelItems.Count - 1; i > stopIndex; i--)
                    {
                        if (wheelItems[i] == spinResult)
                        {
                            totalStopIndex = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                totalStopIndex = stopIndex;
            }

            if (totalStopIndex < 0)
            {
                throw new Exception("找不到合适的stopIndex");
            }

            totalStopIndex = (totalStopIndex - 2 + wheelItems.Count) % wheelItems.Count;
            return totalStopIndex;
        }

        public int OnReelStopAtIndex(int currentIndex)
        {
            return currentIndex;
        }

        public void SlowSpin()
        {
            _wheel.StartSpinning(slowLoopConfig, () => { },_wheel.GetCurrentIndex());
            _wheel.ForceStateToLoop();
        }

        public async Task PerformMulti()
        {
            if (IsMulti)
            {
                throw new Exception("already multi");
            }
            XUtility.PlayAnimation(animator,"Open");
            IsPerformMulti = true;
            _wheel.ForeUpdateElementContainer(_wheel.GetCurrentIndex());
            _wheel.StartSpinning(multiPerformConfig, () => { },_wheel.GetCurrentIndex());
            _wheel.ForceStateToLoop();
            var audioSource = AudioUtil.Instance.PlayAudioFx("Store_SuperFree_Wheel_Roll");
            await context.WaitSeconds(3f);
            if (audioSource != null)
                audioSource.Stop();
            AudioUtil.Instance.PlayAudioFx("Store_SuperFree_Wheel_Refresh");
            XUtility.PlayAnimation(animator,"Hit");
            await context.WaitSeconds(0.3f);
            await context.WaitSeconds(0.05f);
            IsPerformMulti = false;
            ChangeToMulti();
        }
        public void ChangeToMulti()
        {
            if (IsMulti)
            {
                throw new Exception("already multi");
            }
            IsMulti = true;
            for (var i = 0; i < wheelItems.Count; i++)
            {
                wheelItems[i] *= 2;
            }
            _wheel.ForeUpdateElementContainer(_wheel.GetCurrentIndex());
            SlowSpin();
        }
        public override void Update()
        {
            if (_wheel != null && transform.gameObject.activeInHierarchy)
            {
                _wheel.Update();
            }
        }

        public async Task StartSpin(List<StickyElementContainer11025> inMultiElementList,int inSpinResult)
        {
            spinResult = inSpinResult;
            multiElementList = inMultiElementList;
            AudioUtil.Instance.PlayAudioFx("Free_Wheel_Refresh");
            XUtility.PlayAnimation(animator,"Start");
            await context.WaitSeconds(1.23f);
            var spinEndTask = new TaskCompletionSource<bool>();
            context.AddWaitTask(spinEndTask, null);
            var audioSource = AudioUtil.Instance.PlayAudioFx("Free_Wheel_Roll");
            // AudioSource bounceBackAudioSource = null;
            _wheel.StartSpinning(spinConfig, async () =>
            {
                // audioSource.Stop();
                // if (bounceBackAudioSource != null)
                // {
                //     bounceBackAudioSource.Stop();   
                // }
                await PerformSpinEnd();
                spinEndTask.SetResult(true);
            },_wheel.GetCurrentIndex(), () =>
            {
                // audioSource.Stop();
                // bounceBackAudioSource = AudioUtil.Instance.PlayAudioFx("Free_Wheel_Roll_Back");
            },null,true);
            await context.WaitSeconds(3f);
            _wheel.OnSpinResultReceived();
            await spinEndTask.Task;
        }

        public async Task PerformSpinEnd()
        {
            AudioUtil.Instance.PlayAudioFx("Free_Wheel_Checked");
            await XUtility.PlayAnimationAsync(animator,"End");
            await context.WaitSeconds(1f);
            for (var i = 0; i < multiElementList.Count; i++)
            {
                var container = multiElementList[i];
                
                var particle = context.assetProvider.InstantiateGameObject("ParticleMulti", true);
                particle.SetActive(false);
                particle.SetActive(true);
                particle.transform.SetParent(transform.parent,false);
                Vector3 fromWorldPos = transform.Find("Rolls").position;
                Vector3 fromLocalPos = particle.transform.parent.InverseTransformPoint(fromWorldPos);
                particle.transform.localPosition = fromLocalPos;
                Vector3 targetWorldPos = container.transform.position;
                Vector3 targetLocalPos = particle.transform.parent.InverseTransformPoint(targetWorldPos);
                Vector3 midLocalPos = (fromLocalPos + targetLocalPos) * 0.5f;
                var distanceX = targetLocalPos.x - fromLocalPos.x;
                var distanceY = targetLocalPos.y - fromLocalPos.y;
                midLocalPos.y += distanceY * 0.25f;
                midLocalPos.x -= distanceX * 0.25f;
                Vector3[] wayPoints = new[] {fromLocalPos, midLocalPos, targetLocalPos};
                AudioUtil.Instance.PlayAudioFx("Free_Wheel_Fly");
                particle.transform.DOLocalPath(wayPoints, particleFlyTime, PathType.CatmullRom, PathMode.Full3D, 10)
                    .SetEase(Ease.InQuad).OnComplete(() =>
                    {
                        AudioUtil.Instance.PlayAudioFx("Free_Wheel_Fly_Stop");
                        container.ShowMultiAnimation();
                        context.WaitSeconds(0.3f, () =>
                        {
                            container.UpdateToDataState(true); 
                        });
                        context.WaitSeconds(0.5f, () =>
                        {
                            context.assetProvider.RecycleGameObject("ParticleMulti",particle);
                        });
                    });
                await context.WaitSeconds(0.5f);
            }
            await context.WaitSeconds(particleFlyTime);
            multiElementList = null;
            spinResult = 0;
            XUtility.PlayAnimation(animator,"Idle");
            SlowSpin();
        }

        public void SetLower(bool lowerFlag)
        {
            SortingGroup sortingGroup;
            if (lowerFlag)
            {
                if (!transform.gameObject.TryGetComponent<SortingGroup>(out sortingGroup))
                {
                    sortingGroup = transform.gameObject.AddComponent<SortingGroup>();
                }
                sortingGroup.enabled = true;
                sortingGroup.sortingLayerName = "Element";
                sortingGroup.sortingOrder = -2;
            }
            else
            {
                if (transform.gameObject.TryGetComponent<SortingGroup>(out sortingGroup))
                {
                    sortingGroup.enabled = false;
                }
            }
        }
    }
}