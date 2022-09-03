using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class LinkRowMore11026: TransformHolder
    {
        protected Transform moreRow;
        private Animator _animator;

        [ComponentBinder("Num/1")]
        protected Transform rowOne;
        
        [ComponentBinder("Num/2")]
        protected Transform rowTwo;
        
        [ComponentBinder("Num/3")]
        protected Transform rowThree;
        
        [ComponentBinder("MoreRows")]
        protected Transform moreRowsTip;
        
        [ComponentBinder("MoreRow")]
        protected Transform bottomTip;

        public LinkRowMore11026(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
            moreRow = inTransform;
            _animator = moreRow.GetComponent<Animator>();
            if(_animator)
                _animator.keepAnimatorControllerStateOnDisable = true;
        }
        
        public Vector3 GetIntegralPos()
        {
            return moreRow.transform.position;
        }
        
        public void PlayRowMore(long rowNumber = 0)
        { 
            if (rowNumber == 0)
            {
                rowNumber = context.state.Get<ExtraState11026>().GetRowsMore();
            }
            if (rowNumber == 1) 
            {
                _animator.Play("+1Blink");
                // await XUtility.PlayAnimationAsync(_animator, "+1Blink");
            }else if (rowNumber == 2) 
            { 
                _animator.Play("+2Blink");
                // await XUtility.PlayAnimationAsync(_animator, "+2Blink");
            }
            else if(rowNumber == 3)
            {
                _animator.Play("+3Blink");
                // await XUtility.PlayAnimationAsync(_animator, "+3Blink");
            }
            else
            {
                 _animator.Play("Idle");
            }
        }
        
        public void ShowAllWinsImage(long rowNumber = 0)
        {
            if (rowNumber == 0)
            {
                rowNumber = context.state.Get<ExtraState11026>().GetRowsMore();
            }
            if (rowNumber == 1) 
            {
                rowOne.gameObject.SetActive(true);
            }else if (rowNumber == 2)
            { 
                rowTwo.gameObject.SetActive(true);
            }
            else if (rowNumber == 3)
            {
                rowThree.gameObject.SetActive(true);
            }
            else
            {
                moreRowsTip.gameObject.SetActive(true);
            }
        }
         
        public void ShowRowMoreFinishImage(long rowNumber = 0)
        {
            if (rowNumber == 0)
            {
                rowNumber = context.state.Get<ExtraState11026>().GetRowsMore();
            }
            if (rowNumber == 1) 
            {
                moreRowsTip.gameObject.SetActive(false);
            }else if (rowNumber == 2)
            {   
                rowOne.gameObject.SetActive(false);
            }
            else if (rowNumber == 3)
            {
                rowTwo.gameObject.SetActive(false);
            }
            else
            {
                moreRowsTip.gameObject.SetActive(true);
            }
        }
        
        
        public void PLayRowMoreIdle()
        {
             uint rowMore = context.state.Get<ExtraState11026>().GetRowsMore();
             if (rowMore >= 1)
             {
                 _animator.Play($"+{rowMore}Idle");
             }
             else
             {
                  _animator.Play("Idle");
             }
        }
        
        public void PlayBeginIdle()
        {
             _animator.Play("Idle");
        }

        public void HideRowsMore()
        {
            moreRow.gameObject.SetActive(false);
        }
        public void ShowRowsMore()
        {
            moreRow.gameObject.SetActive(true);
            moreRowsTip.gameObject.SetActive(true);
            bottomTip.gameObject.SetActive(false);
            rowOne.gameObject.SetActive(false);
            rowTwo.gameObject.SetActive(false);
            rowThree.gameObject.SetActive(false);
        }
    }
}