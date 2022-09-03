using System.Collections.Generic;
using System.Threading.Tasks;
using GameModule.Utility;
using UnityEngine;

namespace GameModule
{
    public class JackpotView11009: JackPotPanel
    {

        protected AnimationPlaySync animationPlaySync;
        public JackpotView11009(Transform inTransform) : base(inTransform)
        {
        }


        protected List<JackpotItem11009> listItem = new List<JackpotItem11009>();
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            animationPlaySync = new AnimationPlaySync(context, "Idle", 0.9f);
            animationPlaySync.StartPlay();
            for (int i = 0; i < 4; i++)
            {
                Transform tranItem = transform.Find($"Level{i+1}Group");
                JackpotItem11009 item = new JackpotItem11009(tranItem);
                item.Initialize(context);
                item.SetData(i+1,animationPlaySync);
                listItem.Add(item);
            }
        }


        public async Task CollectJackpot(ElementContainer elementContainer,LogicStepProxy stepProxy)
        {
            var extraState = context.state.Get<ExtraState11009>();
            if (extraState.GetFreeRedState()) //有红色才收集
            {

                for (int i = 0; i < listItem.Count; i++)
                {
                    await listItem[i].CollectJackpot(elementContainer,stepProxy);
                }

            }
        }
        
        
        public void RefreshJackpotNoAnim()
        {
            for (int i = 0; i < listItem.Count; i++)
            {
                listItem[i].RefreshJackpotNoAnim();
            }
        }


        public void RefreshJackpotState()
        {
            for (int i = 0; i < listItem.Count; i++)
            {
                listItem[i].RefreshJackpotState();
            }
        }

       
        
        
        
    }
}