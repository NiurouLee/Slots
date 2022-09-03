using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class MoneyBag11029 : TransformHolder
    {
        private Animator animator;
        
        private ExtraState11029 _extraState11029;
        
        private uint oldLevel = 0;
        
        [ComponentBinder("Stage1")] protected Transform animationNode0;

        [ComponentBinder("Stage2")] protected Transform animationNode1;

        [ComponentBinder("Stage3")] protected Transform animationNode2;

        [ComponentBinder("animationNode")] protected Transform animationNode;

        [ComponentBinder("Fx_Cionstrigger")] protected Transform fxTrail;

        protected Transform[] collectionGroupsNodes;
        
        public MoneyBag11029(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            collectionGroupsNodes = new[] {animationNode0, animationNode1, animationNode2};
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            _extraState11029 = inContext.state.Get<ExtraState11029>();
        }

        public Vector3 GetIntegralPos()
        {
            return animationNode.transform.position;
        }
        
        public async Task ShowCollectionGroup(bool active = false)
        {
            uint level = _extraState11029.GetBagLevel();
            var index = (int) level;
            if (active)
            {
                switch (index)
                {
                    case 0:
                        animator.Play("Idle");
                        break;
                    case 1:
                        oldLevel = level;
                        AudioUtil.Instance.PlayAudioFx("Wallet_Toggle");
                        await XUtility.PlayAnimationAsync(animator, "Stage2");
                        break;
                    case 2:
                        oldLevel = level;
                        AudioUtil.Instance.PlayAudioFx("Wallet_Toggle");
                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stage3_Idle"))
                        {
                            await XUtility.PlayAnimationAsync(animator, "Stage3_1");
                        }
                        else
                        {
                            await XUtility.PlayAnimationAsync(animator, "Stage3");
                        }
                      
                        break;
                }
            }
            else
            {
                switch (index)
                {
                    case 0:
                        animator.Play("Idle");
                        break;
                    case 1:
                        animator.Play("Stage2_Idle");
                        break;
                    case 2:
                        animator.Play("Stage3_Idle");
                        break;
                }
            }
        }

        public async Task ShowCollectionFiveGroup(bool active = false)
        {
            uint level = _extraState11029.GetBagLevel();
            var index = (int) level;
            if (active)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Stage3_Idle"))
                {
                    await XUtility.PlayAnimationAsync(animator, "Stage3");
                }
                else
                {
                    await XUtility.PlayAnimationAsync(animator, "Stage3_1");
                }
            }
            else
            {
                animator.Play("Stage3_Idle");
            }
        }
        
        public async Task ShowCollectionGroupZhen(long chips)
        {
            AudioUtil.Instance.StopMusic();
            AudioUtil.Instance.PlayAudioFx("Wallet_Trigger");
            fxTrail.transform.gameObject.SetActive(true);
            await context.WaitSeconds(2.5f);
            fxTrail.transform.gameObject.SetActive(false);
            AudioUtil.Instance.PlayAudioFx("Wallet_CoinFall");
            await XUtility.PlayAnimationAsync(animator, "Stage3_Zhenping");
            await context.view.Get<BonusClollectionPop11029>().ShowCollectionPop(chips);
            await context.WaitSeconds(1f);
            animator.Play("Idle1");
        }
    }
}
