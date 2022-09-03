//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-18 13:37
//  Ver : 1.0.0
//  Description : LinkCounter11007.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace GameModule
{
    public class LinkCounter11007: TransformHolder
    {
        private readonly Animator _animator;
        [ComponentBinder("CountText")] private TextMesh txtLinkCounter;
        public LinkCounter11007(Transform inTransform)
        :base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            _animator = transform.GetComponent<Animator>();
            _animator.keepAnimatorControllerStateOnDisable = true;
        }

        public void UpdateLinkCount(uint linkCount)
        {
            if (txtLinkCounter)
            {
                txtLinkCounter.text = linkCount.ToString();
            }

            if (linkCount == 3)
            {
                AudioUtil.Instance.PlayAudioFx("Respin_Reset");
                XUtility.PlayAnimation(_animator, "Respins");
            }
        }
  
        public async void Hide(bool isEnterRoom)
        {
            if (!isEnterRoom)
            {
                await XUtility.PlayAnimationAsync(_animator, "Close");      
            }
            transform.gameObject.SetActive(false);
        }
    }
}