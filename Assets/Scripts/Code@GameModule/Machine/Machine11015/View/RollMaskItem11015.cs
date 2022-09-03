using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class RollMaskItem11015: TransformHolder
    {

        [ComponentBinder("RollMask")]
        protected SpriteMask rollMask;

        [ComponentBinder("LeftShort")]
        protected Transform tranLeftShort;
        
        [ComponentBinder("RightShort")]
        protected Transform tranRightShort;

        [ComponentBinder("LeftLong")]
        protected Transform tranLeftLong;
        
        [ComponentBinder("RightLong")]
        protected Transform tranRightLong;

        [ComponentBinder("LeftGlow")]
        protected Transform tranLeftGlow;
        
        [ComponentBinder("RightGlow")]
        protected Transform tranRightGlow;

        protected Transform tranWheelBgLeft;
        
        protected Transform tranWheelBgRight;

        protected Animator animator;
        
        public RollMaskItem11015(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            
            
        }


        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            tranWheelBgLeft = context.transform.Find("Wheels/WheelBaseGame/BGGroup/Left");
            tranWheelBgRight = context.transform.Find("Wheels/WheelBaseGame/BGGroup/Right");
            
            PlayCloseIdle();

            
        }

        protected int rollIndex;
        public void SetData(int index)
        {
            rollIndex = index;
        }


        public async Task PlayOpenAnim(bool hasBefor, bool hasNext, bool isFirst, bool isLast)
        {

            if (isFirst)
            {
                tranWheelBgLeft.gameObject.SetActive(false);
            }

            if (isLast)
            {
                tranWheelBgRight.gameObject.SetActive(false);
            }

            
            tranLeftLong.gameObject.SetActive(isFirst);
            tranRightLong.gameObject.SetActive(isLast);
            
            tranLeftShort.gameObject.SetActive(!isFirst && !hasBefor);
            tranRightShort.gameObject.SetActive(!isLast && !hasNext);

            
            tranLeftGlow.gameObject.SetActive(!hasBefor);
            tranRightGlow.gameObject.SetActive(!hasNext);



            await XUtility.PlayAnimationAsync(animator, "Open", context);
        }

        public async Task PlayCloseAnim()
        {
            await XUtility.PlayAnimationAsync(animator, "Close", context);
            tranWheelBgLeft.gameObject.SetActive(true);
            tranWheelBgRight.gameObject.SetActive(true);
        }


        public void PlayOpenIdle()
        {
            animator.Play("OpenIdle");
        }
        
        public async void PlayCloseIdle()
        {
            while (!transform.gameObject.activeInHierarchy)
            {
                await context.WaitNFrame(1);
            }

            animator.Play("CloseIdle");
        }

    }
}