using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class FreeWheelRowFx11312 : TransformHolder
    {
        public FreeWheelRowFx11312(Transform inTransform) : base(inTransform)
        {

        }
        /// <summary>
        /// 刷新Row特效
        /// </summary>
        /// <param name="RolArrs"></param>
        public void LockedRowShow(int[] RolArrs){
            for (int col = 0; col < 5; col++)
            {
                var curRow = transform.Find("Row"+col);
                var anim = curRow.GetComponent<Animator>();
                var clips = anim.GetCurrentAnimatorClipInfo(0);
                // 如果新数据包含这一列，并且不是在idle状态
                if(RolArrs[col]>0){
                    if(clips.Length > 0 && clips[0].clip.name != "Row_B")
                        anim.Play("Open");
                }else{
                    if(clips.Length > 0 && (clips[0].clip.name == "Row_B" || clips[0].clip.name == "Row_A"))
                        anim.Play("Close");
                }
            }
        }
        public void SetRowShow(int rollIndex){
            var anim = transform.Find("Row"+rollIndex).GetComponent<Animator>();
            var clips = anim.GetCurrentAnimatorClipInfo(0);
            if(clips.Length>0){
                if(clips[0].clip.name == "Row_B")
                    anim.Play("Close");
            }
                
        }

        public void CloseAllRowShow(){
            for (int i = 0; i < 5; i++)
            {
                transform.Find("Row"+i).GetComponent<Animator>().Play("Normal");
            }
            
        }
    }
}
