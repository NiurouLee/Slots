using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelBase11025:Wheel11025
    {
        [ComponentBinder("WheelBaseBg/quantity")] 
        public Transform storeNode;
        
        [ComponentBinder("WheelBaseBg/quantity/QuantityText")] 
        public TextMesh storeCoinText;
        
        [ComponentBinder("WheelBaseBg/quantity/Fx_Glowquantity")] 
        public Transform storeLight;

        [ComponentBinder("WheelBaseBg/quantity/TIPS")] 
        public Animator storeTipAnimator;

        private bool isShopLock;
        public ShopView11025 _shopView;
        public ShopView11025 shopView
        {
            get
            {
                if (_shopView != null)
                {
                    return _shopView;
                }

                if (context != null)
                {
                    _shopView = context.view.Get<ShopView11025>();
                    return _shopView;
                }
                return null;
            }
        }
        private List<FlowerElementContainer11025> flyContainerList;
        public WheelBase11025(Transform transform) : base(transform)
        {
            flyContainerList = new List<FlowerElementContainer11025>();
            ComponentBinder.BindingComponent(this,transform);
            storeTipAnimator.gameObject.SetActive(false);
            storeNode.gameObject.AddComponent<PointerEventCustomHandler>().BindingPointerClick(ClickShop);
        }
        public async void ClickShop(PointerEventData clickPoint)
        {
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CUSTOM_CLICK,BtnType11025.OpenShop);
        }

        public void InitShopLockState()
        {
            if (context.state.Get<BetState>().IsFeatureUnlocked(0))
            {
                isShopLock = false; 
                XUtility.PlayAnimation(storeNode.GetComponent<Animator>(),"Idle");
            }
            else
            {
                isShopLock = true;  
                XUtility.PlayAnimation(storeNode.GetComponent<Animator>(),"CloseIdle");
            }
        }

        public void SetStoreCoin(ulong coinNum)
        {
            storeCoinText.text = Tools.GetLeastDigits(coinNum, 12);
        }
        public void RefreshShopLockState()
        {
            if (context.state.Get<BetState>().IsFeatureUnlocked(0) && isShopLock)
            {
                isShopLock = false; 
                AudioUtil.Instance.PlayAudioFx("Store_UnLock");
                XUtility.PlayAnimation(storeNode.GetComponent<Animator>(),"Open");
            }
            else if (!context.state.Get<BetState>().IsFeatureUnlocked(0) && !isShopLock)
            {
                isShopLock = true;
                AudioUtil.Instance.PlayAudioFx("Store_Lock");
                XUtility.PlayAnimation(storeNode.GetComponent<Animator>(),"Close");
            }
        }
        public async Task ShowShopTip()
        {
            storeNode.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            storeNode.gameObject.GetComponent<SortingGroup>().enabled = true;
            storeTipAnimator.gameObject.SetActive(true);
            XUtility.PlayAnimation(storeTipAnimator,"Open");
            var btn = storeTipAnimator.transform.Find("ControlTIPS/CloseBtn");
            var tempTask = new TaskCompletionSource<bool>();
            context.AddWaitTask(tempTask, null);
            btn.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            Coroutine coroutine = null;
            void CloseFunc()
            {
                if (coroutine != null)
                {
                    context.StopCoroutine(coroutine);   
                }
                btn.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                XUtility.PlayAnimation(storeTipAnimator,"Close", () =>
                {
                    storeTipAnimator.gameObject.SetActive(false);
                    tempTask.SetResult(true);
                });
            }
            btn.gameObject.AddComponent<PointerEventCustomHandler>().BindingPointerClick((point) =>
            {
                CloseFunc();
            });
            coroutine = context.WaitSeconds(3f, CloseFunc);
            await tempTask.Task;
            storeNode.gameObject.GetComponent<SortingGroup>().enabled = false;
            storeNode.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }

        public bool HasFlowersNeedToCollect()
        {
            return flyContainerList.Count > 0;
        }
        
        public void AddFlowerContainer(FlowerElementContainer11025 inFlowerContainer)
        {
            flyContainerList.Add(inFlowerContainer);
        }

        public async Task FlyAllFlowerOnWheel()
        {
            if (flyContainerList.Count > 0)
            {
                AudioUtil.Instance.PlayAudioFx("J03_Fly");
                context.WaitSeconds(Constant11025.CollectFlowerTime, () =>
                {
                    AudioUtil.Instance.PlayAudioFx("J03_Fly_Stop");
                });
            }
            for (var i = 0; i < flyContainerList.Count; i++)
            {
                flyContainerList[i].FlyCollectFlower();
            }
            flyContainerList.Clear();
            var storeUpdateValue = extraState.GetNowShopCoins();
            if (shopView.nowCoins != storeUpdateValue)
            {
                var nowShopData = extraState.GetShopData();
                await context.WaitSeconds(Constant11025.CollectFlowerTime);
                storeLight.gameObject.SetActive(false);
                storeLight.gameObject.SetActive(true);
                SetStoreCoin(nowShopData.Credits);
                shopView.SetShopData(nowShopData);
                shopView.RefreshCoins();
            }
        }
        
        public async Task FinishCollectRoll(int rollIndex)
        {
            var rollHeight = Constant11025.RollHeightList[rollIndex];
            var collectLight = transform.Find("CollectLightSingleRoll" + rollHeight + "(Clone)").gameObject;
            context.assetProvider.RecycleGameObject("CollectLightSingleRoll" + rollHeight,collectLight);
            var chameleon = chameleonList[rollIndex];
            SetRollMaskColor(rollIndex,RollMaskOpacityLevel11025.None,0.2f);
            // chameleon.CloseMouth();
            CleanRoll(rollIndex);
            var roll = GetRoll(rollIndex);
            await context.WaitSeconds(0.3f);
            for (int i = 0; i < roll.rowCount; i++)
            {
                // var container = roll.GetVisibleContainer(i);
                // var id = container.sequenceElement.config.id;
                // if (Constant11025.ValueList.Contains(id) || Constant11025.JackpotList.Contains(id))
                // {
                //     roll.GetVisibleContainer(i).UpdateExtraSortingOrder(-100);   
                // }
                roll.GetVisibleContainer(i).UpdateExtraSortingOrder(0);
            }
        }
        public async Task PrepareCollectRoll(int rollIndex)
        {
            var chameleon = chameleonList[rollIndex];
            var openMouthTask = chameleon.OpenMouth();
            var rollHeight = Constant11025.RollHeightList[rollIndex];
            CleanFrameRoll(rollIndex);
            var roll = GetRoll(rollIndex);
            var collectLight = context.assetProvider.InstantiateGameObject("CollectLightSingleRoll" + rollHeight, true);
            collectLight.transform.SetParent(transform,false);
            collectLight.transform.position = roll.transform.position;
            collectLight.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("J01_Orange");
            SetRollMaskColor(rollIndex,RollMaskOpacityLevel11025.Level3);
            for (int i = 0; i < roll.rowCount; i++)
            {
                var container = roll.GetVisibleContainer(i);
                var id = container.sequenceElement.config.id;
                if (Constant11025.ValueList.Contains(id) || Constant11025.JackpotList.Contains(id))
                {
                    roll.GetVisibleContainer(i).UpdateExtraSortingOrder(-100);   
                }
            }
            await context.WaitSeconds(0.1f);
            var stickyElementList = StickyMap[rollIndex];
            for (var i = rollHeight - 1; i >= 0; i--)
            {
                var elementContainer = stickyElementList[(uint) i];
                elementContainer.PerformReadyToCollect();
                await context.WaitSeconds(0.1f);
            }
            await openMouthTask;
        }
        
    }
}