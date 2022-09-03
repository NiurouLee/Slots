using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class Background11312 : TransformHolder
    {
        private Transform BgGround;
        private Transform bgMask;
        public Background11312(Transform inTransform) : base(inTransform)
        {
            BgGround = transform.Find("Background");
            bgMask = transform.Find("bgMask");
        }
        public void PlayBgAnim(int index){
            var anim = BgGround.GetComponent<Animator>();
            if(index == 0)
                anim.Play("BaseScene");
            else if(index == 1)
                anim.Play("FreeScene");
            else if(index == 2)
                anim.Play("LinkScene");
        }
        public void UpdateMaskShow(bool isShow){
            bgMask.gameObject.SetActive(isShow);
        }
    }

}
