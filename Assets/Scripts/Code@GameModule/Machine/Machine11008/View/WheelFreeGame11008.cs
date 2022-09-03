using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class WheelFreeGame11008 : TransformHolder
    {
        public WheelFreeGame11008(Transform inTransform) : base(inTransform)
        {
        }
          public void PlayBgViewAnim(string animName){
            transform.GetComponent<Animator>().Play(animName);
        }
    }
}
