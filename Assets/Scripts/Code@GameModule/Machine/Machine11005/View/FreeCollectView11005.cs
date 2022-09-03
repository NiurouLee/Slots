using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class FreeCollectView11005: TransformHolder
    {

        [ComponentBinder("Left")]
        protected Transform tranLeftCollectMask;
        
        
        [ComponentBinder("Right")]
        protected Transform tranRightCollectMask;

        [ComponentBinder("CollectCountText")]
        protected TextMesh txtCollectNum;

        [ComponentBinder("LeftTargetObject")]
        protected Transform tranLeftTarget;


        [ComponentBinder("TopGroup")]
        protected Transform tranTopGroup;

        [ComponentBinder("AllCollectedTopGroup")]
        protected Transform tranAllCollectedTopGroup;
        
        [ComponentBinder("TopGroup")]
        private Animator animatorTopGroup;



        [ComponentBinder("Left")]
        protected Animator animatorLeft;
        
        
        [ComponentBinder("Right")]
        protected Animator animatorRight;
        
        public FreeCollectView11005(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
        }


        protected ExtraState11005 extraState;

        protected List<Wheel> listFreeWheel = new List<Wheel>();

        protected Animator animatorZhenping;
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            extraState = context.state.Get<ExtraState11005>();

            for (int i = 0; i < 4; i++)
            {
                listFreeWheel.Add(context.view.Get<Wheel>($"WheelFreeGame{i+1}"));
            }

            animatorZhenping = context.transform.Find("ZhenpingAnim").GetComponent<Animator>();
        }


        public void RefreshUINoAnim()
        {
            uint nowNum = extraState.GetLuckCount();
            txtCollectNum.text = nowNum.ToString();
            SetMaskState(nowNum);
        }

        protected async Task SetMaskState(uint num)
        {
            //先刷新之前的状态
            txtCollectNum.text = num.ToString();
            if (num >=30)
            {
                tranLeftCollectMask.gameObject.SetActive(false);
                tranRightCollectMask.gameObject.SetActive(false);
                tranTopGroup.gameObject.SetActive(false);
                tranAllCollectedTopGroup.gameObject.SetActive(true);
            }
            else if (num >= 20)
            {
                tranLeftCollectMask.gameObject.SetActive(false);
                tranRightCollectMask.gameObject.SetActive(false);
                tranTopGroup.gameObject.SetActive(true);
                tranAllCollectedTopGroup.gameObject.SetActive(false);
            }
            else if (num >= 10)
            {
                tranLeftCollectMask.gameObject.SetActive(false);
                tranRightCollectMask.gameObject.SetActive(true);
                tranTopGroup.gameObject.SetActive(true);
                tranAllCollectedTopGroup.gameObject.SetActive(false);

                await XUtility.PlayAnimationAsync(animatorRight, "RightNormal",context);
                //ActiveAllChild(tranRightCollectMask,true);
                //animator.Play("RightNormal");
            }
            else
            {
                tranLeftCollectMask.gameObject.SetActive(true);
                tranRightCollectMask.gameObject.SetActive(true);
                tranTopGroup.gameObject.SetActive(true);
                tranAllCollectedTopGroup.gameObject.SetActive(false);
                
                await XUtility.PlayAnimationAsync(animatorLeft, "LeftNormal",context);
                await XUtility.PlayAnimationAsync(animatorRight, "RightNormal",context);

                // ActiveAllChild(tranLeftCollectMask,true);
                // ActiveAllChild(tranRightCollectMask,true);
                //animator.Play("LeftNormal");
                //animator.Play("RightNormal");
            }
        }

        protected void ActiveAllChild(Transform transform,bool active)
        {
            foreach (Transform tran in transform)
            {
                if (tran.gameObject.activeInHierarchy != active)
                {
                    tran.gameObject.SetActive(active);
                }
            }
        }


        public async Task RefresUI()
        {
            uint lastNum = extraState.GetLastLuckCount();
            uint nowNum = extraState.GetLuckCount();
            
            //Debug.LogError($"=====lastNum:{lastNum}  nowNum:{nowNum}");
            
            //先刷新之前的状态
            txtCollectNum.text = lastNum.ToString();
            await SetMaskState(lastNum);
            

            if (nowNum > lastNum)
            {
                List<ElementContainer> highElementContainers = new List<ElementContainer>();
                List<string> listWheelName = Constant11005.GetListFree(lastNum);
                for (int i = 0; i < listWheelName.Count; i++)
                {
                    var wheel = context.view.Get<Wheel>(listWheelName[i]);
                    var list = wheel.GetElementMatchFilter((container) =>
                    {
                        if (container.sequenceElement.config.id == Constant11005.ElementIdBoom)
                        {
                            return true;
                        }
                
                        return false;
                    });
                    
                    highElementContainers.AddRange(list);
                }
               
                
                
                
                uint nowIndex = lastNum + 1;
                uint count = nowNum - lastNum ;
                for (int i = 0; i < count; i++)
                {
                    if (nowIndex > 30)
                    {
                        continue;
                    }

                    if (i >= highElementContainers.Count)
                    {
                        Debug.LogError($"=======炸弹收集数量:{nowNum-lastNum}和实际element数量:{highElementContainers.Count}不对应 ");
                        break;
                    }

                    ElementContainer container = highElementContainers[i];

                    
                    container.PlayElementAnimationAsync("Fly");
                    
                    //飞炸弹
                    var effectFly = context.assetProvider.InstantiateGameObject("Active_FlyBomb",true);
                    effectFly.transform.position = container.transform.position;
                    effectFly.transform.parent = transform;
                    effectFly.transform.localScale = Vector3.one;
                    
                    AudioUtil.Instance.PlayAudioFx("B02_Fly");
                    var animatorFly = effectFly.GetComponent<Animator>();
                    animatorFly.Play("Fly");

                    await context.WaitSeconds(0.166f);
                    
                    
                    await XUtility.FlyAsync(effectFly.transform, container.transform.position,
                        tranLeftTarget.position, 0, 0.35f,Ease.Linear,context);
                    AudioUtil.Instance.PlayAudioFx("B02_Set");
                    
                    XUtility.PlayAnimationAsync(animatorTopGroup, "Blink",context);
                    await XUtility.PlayAnimationAsync(animatorFly, "Explode",context);
                    context.assetProvider.RecycleGameObject("Active_FlyBomb",effectFly);
                    
                    
                    txtCollectNum.text = nowIndex.ToString();
                    
                    
                    if (nowIndex == 10)
                    {
                        
                        //TODO:动画
                        AudioUtil.Instance.PlayAudioFx("B02_Full");
                        XUtility.PlayAnimationAsync(animatorZhenping, "ZhenpingCollect",context);
                        XUtility.PlayAnimation(animatorLeft, "LeftBomb", () =>
                        {
                            
                        });


                        await context.WaitSeconds(1.733f);
                        var wheel = context.view.Get<Wheel>("WheelFreeGame3");
                        var wheelsActiveState = context.state.Get<WheelsActiveState11005>();
                        wheelsActiveState.AddRunningWheel(wheel,-1,false);
                        RefreshDefultWheel(wheel,wheelsActiveState);
                        await context.WaitSeconds(4-1.733f);
                        tranLeftCollectMask.gameObject.SetActive(false);
                        //wheelsActiveState.UpdateRunningWheel(Constant11005.listFree20,false);
                    

                    }
                    else if (nowIndex == 20)
                    {
                        AudioUtil.Instance.PlayAudioFx("B02_Full");
                        XUtility.PlayAnimationAsync(animatorZhenping, "ZhenpingCollect",context);
                        XUtility.PlayAnimation(animatorRight, "RightBomb", () =>
                        {
                            
                        });
                        

                        await context.WaitSeconds(1.733f);
                        
                        var wheel = context.view.Get<Wheel>("WheelFreeGame4");
                        var wheelsActiveState = context.state.Get<WheelsActiveState11005>();
                        wheelsActiveState.AddRunningWheel(wheel,-1,false);
                        RefreshDefultWheel(wheel,wheelsActiveState);
                        await context.WaitSeconds(4-1.733f);
                        tranRightCollectMask.gameObject.SetActive(false);
                        //wheelsActiveState.UpdateRunningWheel(Constant11005.listFree30,false);
                     
                    }
                    else if (nowIndex == 30)
                    {
                        tranTopGroup.gameObject.SetActive(false);
                        tranAllCollectedTopGroup.gameObject.SetActive(true);
                        await context.WaitSeconds(0.5f);
                    }

                    

                    nowIndex++;
                   
                }
            }
        }

        public void RefreshDefultWheel(Wheel wheel,WheelsActiveState wheelsActiveState)
        {
            var wheelState = wheel.wheelState;
            List<int> listIndex = new List<int>();
            for (int i = 0; i < wheelState.rollCount; i++)
            {
                int index = wheelState.GetActiveSequenceElement(i).Count-1;
                listIndex.Add(index);
            }
            
            wheelState.UpdateCurrentActiveSequence(wheelsActiveState.GetReelNameForWheel(wheel),listIndex);
            wheel.ForceUpdateElementOnWheel(true,true);
        }



    }
}