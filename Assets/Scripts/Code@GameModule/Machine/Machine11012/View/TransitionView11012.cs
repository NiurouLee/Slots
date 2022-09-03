using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TransitionView11012: TransformHolder
    {
        
        [ComponentBinder("BigDoor")]
        protected Animator animatorBigDoor;

        [ComponentBinder("FeatureStartCutPopup")]
        protected Animator animatorFreeCut;
        
        
        private Dictionary<uint,Animator> dicAnimatorBigElement = new Dictionary<uint, Animator>();
        
        public TransitionView11012(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);


            foreach (var item in Constant11012.DicBigElementId2Name)
            {
                Animator animatorBig = transform.Find(item.Value).GetComponent<Animator>();
                dicAnimatorBigElement[item.Key] = animatorBig;
            }
            
            
            
        }


        public async Task OpenBigDoor()
        {
            AudioUtil.Instance.FadeMusicTo(0,0.1f);
            if (!animatorBigDoor.gameObject.activeInHierarchy)
            {
                animatorBigDoor.gameObject.SetActive(true);
            }

            AudioUtil.Instance.PlayAudioFx("FreeSpin_BigDoorOpen");
            await XUtility.PlayAnimationAsync(animatorBigDoor, "Open", context);
            animatorBigDoor.gameObject.SetActive(false);

            await OpenBigElement();
            AudioUtil.Instance.FadeMusicTo(1,0.1f);
        }


        public async Task CloseBigDoor()
        {

            if (!animatorBigDoor.gameObject.activeInHierarchy)
            {
                animatorBigDoor.gameObject.SetActive(true);
            }
            
            AudioUtil.Instance.PlayAudioFx("FreeSpin_BigDoorClose");
            await XUtility.PlayAnimationAsync(animatorBigDoor, "Close", context);
            XUtility.PlayAnimationAsync(animatorBigDoor, "Keep", context);
        }


        public async Task OpenBigElement()
        {
            var extraState = context.state.Get<ExtraState11012>();
            var listDoorId = extraState.GetFreeLockDoorIds();
            if (listDoorId.Count > 0)
            {
                var activeState = context.state.Get<WheelsActiveState11012>();
                var wheel = activeState.GetRunningWheel()[0];
                uint elementId = wheel.GetRoll(0).GetVisibleContainer(0).sequenceElement.config.id;
                if (Constant11012.DicBigElementId2Name.ContainsKey(elementId))
                {
                    Animator animatorBig = dicAnimatorBigElement[elementId];
                    animatorBig.gameObject.SetActive(true);
                    AudioUtil.Instance.PlayAudioFx(Constant11012.DicBigElementId2AudioName[elementId]);
                    await XUtility.PlayAnimationAsync(animatorBig, "Trigger", context);
                    animatorBig.gameObject.SetActive(false);
                }
            }
        }



        public async Task OpenFreeCut()
        {
            AudioUtil.Instance.PlayAudioFx("FreeSpin_Video1");
            animatorFreeCut.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(animatorFreeCut, "StartCut", context);

            animatorFreeCut.gameObject.SetActive(false);
        }
        
        
        
    }
}