//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-16 16:24
//  Ver : 1.0.0
//  Description : LinkTitle11007.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GameModule
{
    public class LinkBonusTitle11007: TransformHolder
    {
        [ComponentBinder("Top/Justice")] private Transform transRandomWild;
        [ComponentBinder("Top/Foxiness")] private Transform transSpinSameWin;
        [ComponentBinder("Top/Stunner")] private Transform transSpinUntilWin;
        [ComponentBinder("Top/Hound")] private Transform transMultiWin;
        [ComponentBinder("Top/Hound/Object/Bet10")] private Transform transBet10;
        [ComponentBinder("Top/Hound/Object/Bet15")] private Transform transBet15;
        [ComponentBinder("Top/Hound/Object/Bet20")] private Transform transBet20;
        [ComponentBinder("Top/Hound/Object/Bet25")] private Transform transBet25;
        [ComponentBinder("Top/Hound/Object/Bet30")] private Transform transBet30;
        private List<Transform> lstTypeLink;
        private List<Transform> lstTransMulti;
        private Animator _animator;
        private bool isPlayAgain;
        private bool hasPlaySwitch;

        public LinkBonusTitle11007(Transform inTransform)
        : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            transform = inTransform;
            
            // {29, transSpinSameWin},      //Foxiness
            // {30, transRandomWild},    //Justice
            // {31, transSpinUntilWin},     //Stunner
            // {32, transMultiWin}        //Hound
            lstTypeLink = new List<Transform> {transSpinSameWin,transRandomWild,transSpinUntilWin,transMultiWin};
            lstTransMulti = new List<Transform>{transBet10, transBet15, transBet20, transBet25, transBet30};
            _animator = transSpinSameWin.GetComponent<Animator>();
            _animator.keepAnimatorControllerStateOnDisable = true;
        }
        

        public void ShowSpinInLinkTitle(uint symbolId)
        {
            for (int i = 0; i < lstTypeLink.Count; i++)
            {
                lstTypeLink[i].gameObject.SetActive(false);
            }
            int index = (int)symbolId - 29;
            if (!ReferenceEquals(lstTypeLink[index],null))
            {
                lstTypeLink[index].gameObject.SetActive(true);
            }
            isPlayAgain = false;
            UpdateBonusMultiply(0);
            XUtility.PlayAnimation(_animator, "Spin");
        }

        public void UpdateBonusMultiply(uint multiply)
        {
            for (int i = 0; i < lstTransMulti.Count; i++)
            {
                lstTransMulti[i].gameObject.SetActive(false);
            }
            if (multiply > 0)
            {
                lstTransMulti[(int)multiply/5-2].gameObject.SetActive(true); 
            }
        }

        public void ShowBonusTitleAnimation()
        {
            if (!isPlayAgain)
            {
                isPlayAgain = true;
                hasPlaySwitch = false;
            }
        }
        
        public void ShowBonusSameSpinStartAnim()
        {
            if (isPlayAgain)
            {
                AudioUtil.Instance.PlayAudioFx("Respin_PlayAgain");
                XUtility.PlayAnimation(_animator,  hasPlaySwitch?"Spin2":"Switch");
                hasPlaySwitch = true;
            }
        }

        public void ToggleFoxinessTitle(bool visible)
        {
            transSpinSameWin.gameObject.SetActive(visible);
        }
    }
}