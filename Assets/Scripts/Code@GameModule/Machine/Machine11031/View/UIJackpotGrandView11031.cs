using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace GameModule
{
    public class UIJackpotGrandView11031 : TransformHolder
    {
        private Animator animator;

        [ComponentBinder("Root/IntegralText")]
        protected TextMesh _textMeshProUGUIJackpotWinNum;

        public UIJackpotGrandView11031(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }
        
        public async Task ShowJackpot(ulong jackpotWin)
        {
            _textMeshProUGUIJackpotWinNum.SetText(jackpotWin.GetCommaFormat());
            AudioUtil.Instance.PlayAudioFx("Grand_Open");
            transform.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(animator, "UIJackpot_Open");
            // AudioUtil.Instance.PlayAudioFx("Close");
            await XUtility.PlayAnimationAsync(animator, "UIJackpot_Close");
        }
    }
}