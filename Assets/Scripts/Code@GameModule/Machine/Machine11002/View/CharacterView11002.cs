using System.Threading.Tasks;
using Spine.Unity;
using UnityEngine;

namespace GameModule
{
    public class CharacterView11002 : TransformHolder
    {
        private SkeletonAnimation _skeletonAnimation;

        private Animator animator;

        private MeshRenderer _meshRenderer;

        private GameObject objFx;

        public CharacterView11002(Transform inTransform) : base(inTransform)
        {
            // _skeletonAnimation = transform.Find("AnimSpine")
            //     .GetComponent<SkeletonAnimation>();

            // _meshRenderer = transform.Find("AnimSpine")
            //     .GetComponent<MeshRenderer>();

            // objFx = transform.parent.Find("Fx_light").gameObject;
            // objFx.SetActive(false);

            animator = transform.GetComponent<Animator>();
        }

        public void AnimIdle()
        {
            animator.Play("Idle");
        }

        public void AnimAppear()
        {
            animator.Play("Appear");
        }

        // public async void AnimWin1()
        // {
        //     await XUtility.PlayAnimationAsync(animator, "Win1", context);
        //     AnimIdle();
        // }


        // public async Task AnimWin2()
        // {
        //     AnimFx();
        //     await XUtility.PlayAnimationAsync(animator, "Win2", context);

        // }


        // public async Task AnimFx()
        // {
        //     objFx.SetActive(true);
        //     await XUtility.WaitSeconds(6);
        //     objFx.SetActive(false);
        // }

        // public async Task AnimJumpInFree()
        // {
        //     await XUtility.PlayAnimationAsync(animator, "Jump2", context);

        //     await AnimJumpInBase();
        // }

        // public async Task AnimJumpInBase()
        // {
        //     _meshRenderer.sortingLayerID = SortingLayer.NameToID("LocalFx");

        //     await XUtility.PlayAnimationAsync(animator, "Jump3", context);
        //     await AnimWin2();
        //     _meshRenderer.sortingLayerID = SortingLayer.NameToID("SceneBackground");
        //     AnimIdle();
        // }
    }
}