
using System.Collections.Generic;
using System.Threading.Tasks;
using GameModule.Utility;
using UnityEngine;

namespace GameModule
{
    public class BoxesView11009: TransformHolder
    {
        private GameObject goCountNotice5x4;
        private GameObject goCountNotice5x6;
        public BoxesView11009(Transform inTransform) : base(inTransform)
        {
            tranBoxJackpot = transform.parent.Find("BoxesGroup_Jackpot");
            goCountNotice5x4 = transform.parent.Find("Wheels/WheelFreeGame5X4/CountNotice").gameObject;
            goCountNotice5x6 = transform.parent.Find("Wheels/WheelFreeGame5X6/CountNotice").gameObject;

        }


        protected List<BoxesViewItem11009> listBox = new List<BoxesViewItem11009>();


        protected Transform tranBoxJackpot;
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            for (int i = 0; i < 3; i++)
            {
                Transform tranBox = transform.Find($"Box{i + 1}");
                BoxesViewItem11009 item = new BoxesViewItem11009(tranBox);
                item.Initialize(context);
                item.SetCollectElementId(Constant11009.ListCollectElementId[i],i);
                listBox.Add(item);
            }
            
            
        }


        public void SetCountNoticeState()
        {
            var extraState = context.state.Get<ExtraState11009>();
            if (extraState.GetFreePurpleState())
            {
                goCountNotice5x4.SetActive(true);
                goCountNotice5x6.SetActive(true);
            }
            else
            {
                goCountNotice5x4.SetActive(false);
                goCountNotice5x6.SetActive(false);
            }
        }


        public async Task CollectElement(ElementContainer elementContainer)
        {

            if (elementContainer.sequenceElement.config.id == Constant11009.ElementIdGreen)
            {
                //绿
                await listBox[Constant11009.IndexGreen].CollectElements(elementContainer);
            }
            else if (elementContainer.sequenceElement.config.id == Constant11009.ElementIdRed)
            {
                //红
                await listBox[Constant11009.IndexRed].CollectElements(elementContainer);
            }
            else if (elementContainer.sequenceElement.config.id == Constant11009.ElementIdPurple ||
                     Constant11009.ListElementIdPurpleVaraint.Contains(elementContainer.sequenceElement.config.id))
            {
                var extraState = context.state.Get<ExtraState11009>();
                var freeSpinState = context.state.Get<FreeSpinState11009>();

                if (!extraState.GetFreeRedState() || freeSpinState.IsTriggerFreeSpin) //没有红色或者这局是teiger才收集
                {
                    //紫
                    await listBox[Constant11009.IndexPurple].CollectElements(elementContainer);
                }
            }


        }

        public void RefreshBoxStateNoAnim()
        {
            for (int i = 0; i < listBox.Count; i++)
            {
                listBox[i].RefreshBoxStateNoAnim();
            }
        }

        


        public void AttachPoint(Wheel wheel)
        {
           var attachPoint =  wheel.transform.Find("AttachBoxPoint");
           this.transform.position = attachPoint.transform.position;
           tranBoxJackpot.position = attachPoint.transform.position;
           
           var extraState = context.state.Get<ExtraState11009>();

           if (extraState.GetFreeRedState()) //有红色就不显示宝箱
           {
               this.transform.gameObject.SetActive(false);
               tranBoxJackpot.gameObject.SetActive(true);
           }
           else
           {
               this.transform.gameObject.SetActive(true);
               tranBoxJackpot.gameObject.SetActive(false);
           }
           //Test
           //this.transform.gameObject.SetActive(false);
        }


        public async Task RefreshElement(LogicStepProxy stepProxy)
        {
            
            //jackpot是一个一个翻面，当前翻完当前就飞jackpot面板
            //其他element只是翻面
            
            var wheelActiveState = context.state.Get<WheelsActiveState11009>();
            var listWheel = wheelActiveState.GetRunningWheel();
            
            var listElement = listWheel[0].GetElementMatchFilter((element) =>
            {
            
                if (Constant11009.ListElementIdGoldenVaraint.Contains(element.sequenceElement.config.id) ||
                    Constant11009.ListElementIdPurpleVaraint.Contains(element.sequenceElement.config.id))
                {
                    
                    return true;
                }
            
                return false;
            });

            for (int i = 0; i < listElement.Count; i++)
            {
                //jackpot 符合条件就收集，没有就跳过
                if (Constant11009.ListElementIdAllJackpotVaraint.Contains(listElement[i].sequenceElement.config.id))
                {
                    await context.view.Get<JackpotView11009>().CollectJackpot(listElement[i],stepProxy);

                }
                else
                {
                    //其他普通翻面
                    await Constant11009.FlipElement(context, listElement[i],stepProxy);
                }
            }
        }


    }
}