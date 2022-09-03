using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIWheelBonus11005: MachinePopUp
    {

        [ComponentBinder("WheelMainGroup")]
        protected Animator animatorMainGroup;

        [ComponentBinder("MainGroup")]
        protected Transform tranMainGroup;

        [ComponentBinder("WheelBonusButton")]
        protected Button btnSpin;
        
        private float anglePerFan = 360f/6;
        
        public UIWheelBonus11005(Transform transform) : base(transform)
        {
            btnSpin.onClick.AddListener(OnBtnSpinClick);
        }

        private SBonusProcess sBonusProcess;
        private async void OnBtnSpinClick()
        {
            btnSpin.interactable = false;

            var extraState = context.state.Get<ExtraState11005>();
            sBonusProcess = await extraState.SendBonusProcess();

            StartWheelBonus();
        }


        public override bool IsCloseMustUnpauseMusic()
        {
            return true;
        }


        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
        }
        
        
        protected async void StartWheelBonus()
        {
            AudioUtil.Instance.PlayAudioFx("WheelSpin");
            AudioUtil.Instance.PlayAudioFx("Wheel_Rolling_Start");
            await XUtility.PlayAnimationAsync(animatorMainGroup, "Start",context);

            AudioUtil.Instance.PlayAudioFx("Wheel_Rolling", true);
            XUtility.PlayAnimationAsync(animatorMainGroup, "Loop",context);
            while (sBonusProcess == null)
            {
                await context.WaitSeconds(0.1f);
            }

            
            await StopWheelBonus();
        }


        public async Task StopWheelBonus()
        {
            btnSpin.interactable = false;
            var extraState = context.state.Get<ExtraState11005>();

            uint index = extraState.GetMapInfo().NumberRolled - 1;
            float targetAngle = anglePerFan * index;

            tranMainGroup.localEulerAngles = new Vector3(0, 0, targetAngle);
            AudioUtil.Instance.StopAudioFx("Wheel_Rolling");
            AudioUtil.Instance.PlayAudioFx("Wheel_Rolling_End");
            await XUtility.PlayAnimationAsync(animatorMainGroup, "Finish",context);
            
           
            AudioUtil.Instance.PlayAudioFx("Wheel_Set");
            await XUtility.PlayAnimationAsync(animatorMainGroup, "Win",context);

            //await context.WaitSeconds(2.0f, context);

            Close();
        }

    }
}