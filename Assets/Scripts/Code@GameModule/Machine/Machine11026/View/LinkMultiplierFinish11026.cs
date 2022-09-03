using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class LinkMultiplierFinish11026: TransformHolder
    {
      
        protected Transform multiplier;
        private Animator _animator;
        [ComponentBinder("Allwins/Allwins2x")]
        protected Transform multiplierTwo;
        
        [ComponentBinder("Allwins/Allwins3x")]
        protected Transform multiplierThree;
        
        [ComponentBinder("Allwins/Allwins4x")]
        protected Transform multiplierFour;
        

        public LinkMultiplierFinish11026(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
            multiplier = inTransform;
            _animator = multiplier.GetComponent<Animator>();
            if(_animator) 
                _animator.keepAnimatorControllerStateOnDisable = true;
        }
        
        public void setMultiplierImage()
        {
            uint multipplierNumber = context.state.Get<ExtraState11026>().GetAllWinMultiplier();
            if (multipplierNumber == 2)
            { 
                multiplierTwo.gameObject.SetActive(true);
                multiplierThree.gameObject.SetActive(false);
                multiplierFour.gameObject.SetActive(false);

            }else if (multipplierNumber == 3)
            {
                multiplierTwo.gameObject.SetActive(false);
                multiplierThree.gameObject.SetActive(true);
                multiplierFour.gameObject.SetActive(false);
            }else if(multipplierNumber == 4)
            {
                multiplierTwo.gameObject.SetActive(false);
                multiplierThree.gameObject.SetActive(false);
                multiplierFour.gameObject.SetActive(true);
            }
            else
            {
                multiplierTwo.gameObject.SetActive(false);
                multiplierThree.gameObject.SetActive(false);
                multiplierFour.gameObject.SetActive(false);
            }
        }
        
        public async Task PLayAllWinsFisnish()
        {
            AudioUtil.Instance.PlayAudioFx("J01Multiplier_increase");
            await XUtility.PlayAnimationAsync(_animator, "AllwinsFinish");
        }

        public void HideMultiplierImage()
        {
             multiplierTwo.gameObject.SetActive(false); 
             multiplierThree.gameObject.SetActive(false);
             multiplierFour.gameObject.SetActive(false);
        }
    }
}