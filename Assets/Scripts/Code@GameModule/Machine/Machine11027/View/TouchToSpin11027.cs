using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class TouchToSpin11027: TransformHolder
    {
        
        private Animator animator;
        private bool isResponesd = false;
        public TouchToSpin11027(Transform inTransform) : base(inTransform)
        {
           ComponentBinder.BindingComponent(this, transform);
           var pointerEventNightHandler = transform.gameObject.AddComponent<PointerEventCustomHandler>();
           pointerEventNightHandler.BindingPointerClick((e)=>ShowWhelIntro());
           animator = transform.GetComponent<Animator>();
           animator.keepAnimatorControllerStateOnDisable = true;
        }

        public async Task ShowTouchSpin()
        {
            transform.gameObject.SetActive(true);
            animator.Play("Intro");
            await context.WaitSeconds(0.5f);
            context.view.Get<WheelRollingView11027>().PLayWheelMusic();
            isResponesd = true;
            AudioUtil.Instance.PlayAudioFx("FreeGameStart_Open");
            await context.WaitSeconds(0.5f);
        }

        // public async void HideTouchSpin()
        // {
        //     if(!isResponesd)
        //         return;
        //     isResponesd = false;
        //     StopRollIndexAnticipationAnimation();
        //     animator.Play("Outro");
        //     await context.WaitSeconds(0.5f);
        //     await ShowWhelIntro();
        // }
        
        public async void ShowWhelIntro()
        {
            if(!isResponesd)
                return;
            isResponesd = false;
            // StopRollIndexAnticipationAnimation();
            animator.Play("WheelIntro");
            await context.WaitSeconds(0.677f);
            AudioUtil.Instance.PlayAudioFx("Wheel_PandaFly");
            // AudioUtil.Instance.PlayAudioFx("Wheel_Panda");
            await context.WaitSeconds(2.27f - 0.67f);
            await StartRolling();
        }

        public void HideTips()
        {
            transform.gameObject.SetActive(false);
        }
        
        private async Task StartRolling()
        {
            context.view.Get<TouchToSpin11027>().HideTips();
            context.view.Get<WheelRollingView11027>().PLayIntro();
            await context.view.Get<WheelFeature11027>().PlayToWheel();
            await context.WaitSeconds(2.0f);
            context.state.Get<JackpotInfoState>().LockJackpot = true;
            context.transform.Find("WheelFeature/Wheels/WheelBaseGame").gameObject.SetActive(false);
            context.transform.Find("WheelFeature/Wheels/CollectionGroup").gameObject.SetActive(false);
            context.transform.Find("WheelFeature/Wheels/WheelPickGame").gameObject.SetActive(false);
            context.transform.Find("WheelFeature/WheelBonusGame02").gameObject.SetActive(true);
            context.transform.Find("WheelFeature/JackpotPanel").position = new Vector3(0, 4.7f, 0);
            context.transform.Find("WheelFeature/WheelBonusGame02").position = new Vector3(0, 0.5f, 0);
            context.view.Get<WheelRollingView11027>().Show();
            context.view.Get<WheelRollingView11027>().InitializeWheelView(false,true);
            context.view.Get<WheelRollingView11027>().InitializeBonusWheelView();
        }

    }
}