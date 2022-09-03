using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class CollectGroupView11031: TransformHolder
    {
        [ComponentBinder("Root/Img")] protected Transform tranCollectSymbol;
        
        [ComponentBinder("Root/ProgressBar")] protected Transform picProgressFill;
        
        [ComponentBinder("Button")] protected Transform btnTip;

        // [ComponentBinder("Bowl")] protected Transform bowlIcon;

        protected Animator _animator;
        
        private ExtraState11031 _extraState11031;
        
        private bool _buttonResponseEnabled = false;

        public Transform tranTip;
        
        public CollectGroupView11031(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            _animator = transform.GetComponent<Animator>(); 
            btnTip.gameObject.AddComponent<PointerEventCustomHandler>().BindingPointerClick(OnTipClick);
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            tranTip = context.transform.Find("Wheels").Find("WheelBaseTips");
        }

        public void EnableButtonResponse(bool enable)
        {
            _buttonResponseEnabled = enable;
        }

        private bool enableNextTrigger = true; 
        public void ShowTipView()
        {
            if (!enableNextTrigger)
                return;
           
            
            if (!tranTip.gameObject.activeSelf)
            {
                AudioUtil.Instance.PlayAudioFxOneShot("Tips_UnClick");
                XUtility.ShowTipAndAutoHide(tranTip, 5, 0.2f, true, context);
                
                enableNextTrigger = false;
               
                context.WaitSeconds(1, () =>
                {
                    enableNextTrigger = true;
                });
            }
            else
            {
                AudioUtil.Instance.PlayAudioFxOneShot("Tips_Click");
            }
        }

        public void OnTipClick(PointerEventData pointerEventData)
        {
            if (!_buttonResponseEnabled)
                return;
            ShowTipView();
        }

        // private void iconClick(PointerEventData pointerEventData)
        // {
        //     context.view.Get<LuckyPot11031>().ShowLuckyPot();
        // }

        public Vector3 GetIntegralPos()
        {
            return tranCollectSymbol.transform.position;
        }

        public async Task TriggerCollectHorse()
        {
            XUtility.PlayAnimation(_animator, "Blink");
            await context.WaitSeconds(0.5f);
            _extraState11031.DestroyFlyChilliList();
            _extraState11031.ClearFlyChlliList();
            await context.WaitSeconds(0.73f - 0.5f);

        }

        public async Task OpenCollectHorse()
        {
            AudioUtil.Instance.PlayAudioFx("MiniGame_ProgressiveBar_Full");
            XUtility.PlayAnimation(_animator, "Finish");
            await context.WaitSeconds(0.5f);
            context.view.Get<BowlView11031>().PlayActive();
            await context.WaitSeconds(2.0f - 0.5f);
            context.view.Get<BowlView11031>().PlayIdle();
        }

        public bool IsBowlFly()
        {
            var wheel = context.state.Get<WheelsActiveState11031>().GetRunningWheel()[0];
            var bonusWinLines = wheel.wheelState.GetBonusWinLine();
            if (bonusWinLines.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task BowlFly()
        {
            //碗开始飞
            await context.view.Get<BowlView11031>().Fly();
        }

        protected float lenghtMin = -7.24f;
        protected float lenghtMax = 0;

        public void SetProgress(float num)
        {
            num = Mathf.Clamp(num, 0, 1);
            float newProgress = lenghtMin;
            newProgress = lenghtMin + (lenghtMax - lenghtMin) * num;
            var pos = picProgressFill.transform.localPosition;
            pos.x = newProgress;
            picProgressFill.transform.localPosition = pos;
        }

        public async Task ChangeFill(bool playAnimation = true, bool audio = true,bool isFrom = false)
        {
            _extraState11031 = context.state.Get<ExtraState11031>();
            bool isLevelUp = _extraState11031.IsMapLevelUp();
            var currentPoint = _extraState11031.GetMapPoint();
            var maxPoint = _extraState11031.GetMapMaxPoint();
            float newProgress = currentPoint / (float)maxPoint;
            if (currentPoint < maxPoint)
            {
                if (isLevelUp && isFrom == false)
                {
                    SetProgress(1);
                    await TriggerCollectHorse();
                    if (playAnimation)
                    {
                        // await OpenCollectHorse();
                    }
                }
                else
                {
                    SetProgress(newProgress);
                    if (audio)
                    {
                        AudioUtil.Instance.PlayAudioFx("Map_FlySelect");
                    }

                    if (playAnimation)
                    {
                        await TriggerCollectHorse();
                    }
                }
            }
            else
            {
                SetProgress(1);
                await context.view.Get<CollectGroupView11031>().TriggerCollectHorse();
                if (playAnimation)
                {
                    // await OpenCollectHorse();
                }
            }
        }
    }
}