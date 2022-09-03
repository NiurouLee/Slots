using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class FreeCutAnimationView11016:TransformHolder
    {
        private Animator _animator;
        public FreeCutAnimationView11016(Transform inTransform) : base(inTransform)
        {
            _animator = transform.GetComponent<Animator>();
        }

        public async void ShowCutFree(MachineContext context)
        {
            _animator.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(_animator, "Free");
            _animator.gameObject.SetActive(false);
        }
    }
}