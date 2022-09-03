using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TransitionsView11031 : TransformHolder
    {
        [ComponentBinder("TruckTransition")] protected Animator animatorTruck;


        public TransitionsView11031(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public void PlayTruckTransition()
        {
            AudioUtil.Instance.PlayAudioFxOneShot("Video1");
            animatorTruck.gameObject.SetActive(true);
            var wheel = context.state.Get<WheelsActiveState11031>().GetRunningWheel()[0];
            var animatorZhenping = wheel.GetContext().transform.GetComponent<Animator>();
            XUtility.PlayAnimation(animatorZhenping, "Win");
            XUtility.PlayAnimation(animatorTruck, "Transition", () =>
            {
                if(animatorTruck != null)
                    animatorTruck.gameObject.SetActive(false);
            }, context);
        }

        public void HideTruckTransition()
        {
            animatorTruck.gameObject.SetActive(false);
        }
    }
}