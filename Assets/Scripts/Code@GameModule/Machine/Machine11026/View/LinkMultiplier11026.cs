using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class LinkMultiplier11026: TransformHolder
    {
      
        protected Transform multiplier;
        private Animator _animator;

        [ComponentBinder("Num/2")]
        protected Transform multiplierTwo;
        
        [ComponentBinder("Num/3")]
        protected Transform multiplierThree;
        
        [ComponentBinder("Num/4")]
        protected Transform multiplierFour;
        
        [ComponentBinder("MoreRows")]
        protected Transform multiplierTip;
        
        [ComponentBinder("animationNOde")]
        protected Transform animationNode;
        
        [ComponentBinder("allwins")] 
        protected Transform multiplierAllWins;

        public LinkMultiplier11026(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
             multiplier = inTransform;
            _animator = multiplier.GetComponent<Animator>();
            if(_animator)
                _animator.keepAnimatorControllerStateOnDisable = true;
        }

        public void ShowAllWinsImage(long multiplierNumber = 0)
        {
            if (multiplierNumber == 0)
            {
                multiplierNumber = context.state.Get<ExtraState11026>().GetAllWinMultiplier();
            }
            if (multiplierNumber == 2)
            {
                multiplierTwo.gameObject.SetActive(true);
            }
            else if (multiplierNumber == 3)
            {
                multiplierThree.gameObject.SetActive(true);
            }
            else if (multiplierNumber == 4)
            {
                multiplierFour.gameObject.SetActive(true);
            }
            else
            {
                multiplierTip.gameObject.SetActive(true);
            }
        }
        
        public void ShowAllWinsFinishImage(long multiplierNum = 0)
        {
            if (multiplierNum == 0)
            {
                multiplierNum = context.state.Get<ExtraState11026>().GetAllWinMultiplier();
            }
            if (multiplierNum == 2) 
            {
                multiplierTip.gameObject.SetActive(true);
            }else if (multiplierNum == 3)
            {   
                multiplierTwo.gameObject.SetActive(false);
            }
            else if (multiplierNum == 4)
            {
                multiplierThree.gameObject.SetActive(false);
            }
        }

        public void PLayAllWins(long multiplierNum = 0)
        {
            if (multiplierNum == 0)
            {
                multiplierNum = context.state.Get<ExtraState11026>().GetAllWinMultiplier();
            }

            if (multiplierNum == 2)
            {
                _animator.Play("X2Blink");
                // await XUtility.PlayAnimationAsync(_animator, "X2Blink");
            }
            else if (multiplierNum == 3)
            {
                _animator.Play("X3Blink");
                // await XUtility.PlayAnimationAsync(_animator, "X3Blink");
            }
            else if (multiplierNum == 4)
            {
                _animator.Play("X4Blink");
                // await XUtility.PlayAnimationAsync(_animator, "X4Blink");
            }
            else
            {
                _animator.Play("Idle");
            }
        }

        //结算时将做左边隐藏
        public void HideAllWins()
        {
            multiplier.gameObject.SetActive(false);
        }
        
        public void ShowAllWins()
        {
            multiplier.gameObject.SetActive(true);
            multiplierTip.gameObject.SetActive(true);
            multiplierAllWins.gameObject.SetActive(false);
            multiplierTwo.gameObject.SetActive(false); 
            multiplierThree.gameObject.SetActive(false); 
            multiplierFour.gameObject.SetActive(false);
        }
        
        //Recover时播放Idle动画
        public void PLayAllWinsIdle()
        {
            uint multipplierNumber = context.state.Get<ExtraState11026>().GetAllWinMultiplier();
            if (multipplierNumber >= 2)
            {
                _animator.Play($"X{multipplierNumber}Idle");
            }
            else
            {
                _animator.Play("Idle");
            }
        }
        
        //进入link时的Idle动画
        public void PlayBeginIdle()
        {
             _animator.Play("Idle");
        }
        
        public Vector3 GetIntegralPos()
        {
            return animationNode.transform.position;
        }
    }
}