//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-21 11:51
//  Ver : 1.0.0
//  Description : CollectCoinView11011.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace GameModule
{
    public class CollectCoinView11011: TransformHolder
    {
        private bool isFull;
        public bool IsFull => isFull;
        [ComponentBinder("BGGroup/Bowl")] 
        private Animator _animator;
        public CollectCoinView11011(Transform inTransform):base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public Vector3 GetCollectEndPosition()
        {
            return _animator.transform.position;
        }
        public void PlayTrigger()
        {
            AudioUtil.Instance.PlayAudioFx("Jackpot_Trigger");
            XUtility.PlayAnimation(_animator, "Trigger");
        }
        public async void PlayCollect(bool full)
        {
            isFull = full;
            await XUtility.PlayAnimationAsync(_animator, full ?  "Full":"Half");
        }

        public async void PlayToFull()
        {
            isFull = true;
            AudioUtil.Instance.PlayAudioFx("JuBaoPen_GoUp");
            XUtility.PlayAnimation(_animator, "HalfToFull");
        }

        public void UpdateState(bool full)
        {
            isFull = full;
            XUtility.PlayAnimation(_animator, isFull ? "IdleFull" : "IdleHalf");
        }
    }
}