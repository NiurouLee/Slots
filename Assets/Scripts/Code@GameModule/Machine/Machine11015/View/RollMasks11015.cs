using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class RollMasks11015: TransformHolder
    {

        protected List<RollMaskItem11015> listRollMask = new List<RollMaskItem11015>();

        
        protected List<Animator> listAnimatorEfx = new List<Animator>();

        protected Transform tranFreeGlow;
        
        public RollMasks11015(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            for (int i = 0; i < 5; i++)
            {
                var tranRollMask = this.transform.Find($"RollLong{i}");
                RollMaskItem11015 item = new RollMaskItem11015(tranRollMask);
                item.Initialize(inContext);
                item.SetData(i);
                listRollMask.Add(item);

                var tranFreeEfx = context.transform.Find($"Wheels/WheelFreeGame12X4/Rolls/GroupFreeEfx/RollLongEfx{i}")
                    .GetComponent<Animator>();
                listAnimatorEfx.Add(tranFreeEfx);
            }

            tranFreeGlow = context.transform.Find("Wheels/WheelFreeGame12X4/FeatureGlow");
        }
        
        
        
        protected List<RollMaskItem11015> listOutItems = new List<RollMaskItem11015>();
        
        public async Task PlayOpenAnim()
        {
            var wheel = context.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var listRoll = wheel.GetVisibleElementInfo();
            
            List<Task> listTask = new List<Task>();
            listOutItems.Clear();


            RollMaskItem11015[] listShowItems = new RollMaskItem11015[listRoll.Count];

            for (int x = 0; x < listRoll.Count; x++)
            {
                var listElement = listRoll[x];
                for (int y = 0; y < listElement.Count; y++)
                {
                    var element = listElement[y];
                    if (element.config.id == Constant11015.ShieldElementId)
                    {
                        
                        // listTask.Add(listRollMask[x].PlayOpenAnim());
                        //listOutItems.Add(listRollMask[x]);
                        listShowItems[x] = listRollMask[x];
                        break;
                    }
                }
            }

            if (listShowItems.Length > 0)
            {
                var transitionView = context.view.Get<TransitionView11015>();
                await transitionView.PlayZeusSuperior();
                transitionView.PlayZeusDown();
            }

            RollMaskItem11015 beforElement = null;
            for (int i = 0; i < listShowItems.Length; i++)
            {
                var element = listShowItems[i];
                if (element != null)
                {
                    bool hasBefor = beforElement != null;
                    bool hasNext = false;
                    if (i + 1 < listShowItems.Length)
                    {
                        hasNext = listShowItems[i + 1] != null;
                    }

                    bool isFirst = i == 0;
                    bool isLast = i == listShowItems.Length - 1;


                    listTask.Add(element.PlayOpenAnim(hasBefor,hasNext,isFirst,isLast));
                    listOutItems.Add(element);
                }

                beforElement = element;
            }
            

            //AudioUtil.Instance.PlayAudioFxOneShot("Reel_Extend");
            
            await Task.WhenAll(listTask);
        }

        public async Task PlayOpenFreeAnim()
        {
            var wheel = context.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var listRoll = wheel.GetVisibleElementInfo();

            List<Task> listTask = new List<Task>();
            
            for (int x = 0; x < listRoll.Count; x++)
            {
                var listElement = listRoll[x];
                for (int y = 0; y < listElement.Count; y++)
                {
                    var element = listElement[y];
                    if (element.config.id == Constant11015.ShieldElementId)
                    {
                        listTask.Add(XUtility.PlayAnimationAsync(listAnimatorEfx[x],"FeatureBg",context));
                        //listAnimatorEfx[x].Play("FeatureBg",0,0);
                        break;
                    }
                }
            }

            if (listTask.Count > 0)
            {
                AudioUtil.Instance.PlayAudioFxOneShot("Reel_Extend");
                OpenFreeGlow();
                await Task.WhenAll(listTask);
            }
            
        }

        public void OpenFreeGlow()
        {
            this.tranFreeGlow.gameObject.SetActive(true);
        }
        
        public void CloseFreeGlow()
        {
            this.tranFreeGlow.gameObject.SetActive(false);
        }


        public async Task PlayCloseAnim()
        {
            if (listOutItems.Count > 0)
            {
                
                context.view.Get<TransitionView11015>().OpenIcon();
                
                var transitionView = context.view.Get<TransitionView11015>();
                transitionView.PlayZeusAppear();
                
                List<Task> listTask = new List<Task>();
                for (int i = 0; i < listOutItems.Count; i++)
                {
                    listTask.Add(listOutItems[i].PlayCloseAnim());
                }
                listOutItems.Clear();
                await Task.WhenAll(listTask);
            }
        }


        public void PlayOpenIdle()
        {
          
        }
        
        public void PlayCloseIdle()
        {
            
        }
        
        
        
        
        
        
    }
}