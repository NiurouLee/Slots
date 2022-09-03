using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class BgView11008 : TransformHolder
    {
        public BgView11008(Transform inTransform) : base(inTransform)
        {
        }

        public void PlayBgViewAnim(string animName){
            transform.GetComponent<Animator>().Play(animName);
        }
        public bool IsFree(){
            var anim = transform.GetComponent<Animator>();
            var name = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            return name == "BackgroundFree";
        }
    }
}
