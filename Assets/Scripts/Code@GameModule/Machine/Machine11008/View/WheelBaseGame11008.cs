using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class WheelBaseGame11008 : TransformHolder
    {
        public WheelBaseGame11008(Transform inTransform) : base(inTransform)
        {
        }
        public void PlayBgViewAnim(string animName){
            transform.GetComponent<Animator>().Play(animName);
        }
    }
}