using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

namespace GameModule
{
    public class LightView11031 : TransformHolder
    {
        private Animator animator;

        [ComponentBinder("EFX")] protected Transform effect;

        public LightView11031(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        public async Task ShowLightView()
        {
            transform.gameObject.SetActive(true);
            transform.position = new Vector3(-1.85f, -0.55f, 0);

            XUtility.PlayAnimation(animator, "ActiveRespin");

            int index = 0;
            var chilliCount = (int) context.state.Get<ExtraState11031>().GetLinkPepperCount();
            if (chilliCount >= 6 && chilliCount < 40)
            {
                index = Constant11031.GetFinalPepperIndex((int) context.state.Get<ExtraState11031>()
                    .GetLinkPepperCount());
            }

            var endPos = context.view.Get<WinGroupView11031>()
                .GetIntegralPos(index).y - context.view.Get<WinGroupView11031>()
                .GetLowIntegralPos().y;
            if (endPos != 0)
            {
                transform.transform.DOMoveY(endPos, 0.6f).OnComplete(() => { });
            }

            await context.WaitSeconds(0.8f);
        }

        public void ShowLight(bool enable)
        {
            transform.position = new Vector3(-1.85f, -0.55f, 0);
            transform.gameObject.SetActive(enable);
        }
    }
}