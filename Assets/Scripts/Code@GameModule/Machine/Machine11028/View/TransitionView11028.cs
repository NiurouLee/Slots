//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-17 17:45
//  Ver : 1.0.0
//  Description : CharacterView11028.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TransitionView11028: TransformHolder
    {
        private Animator animator;
        public TransitionView11028(Transform inTransform) : base(inTransform)
        {
            animator = transform.GetComponent<Animator>();
        }

        public async Task PlayCharacterAnimation()
        {
            transform.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("Expect");
            await XUtility.PlayAnimationAsync(animator,"Anticipation");
            transform.gameObject.SetActive(false);
        }
        
        public void PlayFreeTransitionAnimation()
        {
            transform.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("FreeSpin_Video1");
            XUtility.PlayAnimation(animator,"TransitionAnimation");
        }
    }
}