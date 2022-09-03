//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-26 21:24
//  Ver : 1.0.0
//  Description : FeatureCutView11011.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class FeatureCutView11011:TransformHolder
    {
        private Animator _animator;
        public FeatureCutView11011(Transform inTransform):base(inTransform)
        {
            _animator = transform.GetComponent<Animator>();
        }

        public void ToggleVisible(bool visible)
        {
            transform.gameObject.SetActive(visible);
        }

        public void PlayCutScreen()
        {
            ToggleVisible(true);
            XUtility.PlayAnimation(_animator, "Transition");
        }
    }
}