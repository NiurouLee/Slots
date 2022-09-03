using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
namespace GameModule{
    public class FreeFeatureView11312 : TransformHolder
    {
        public List<GameObject> LastLockedSymbols;
        public int[] RolArrs;
        
        public FreeFeatureView11312(Transform inTransform) : base(inTransform)
        {
            LastLockedSymbols = new List<GameObject>();
            RolArrs = new int[5];
        }

        /// <summary>
        /// Free中锁定图标位移 每次向下位移一个格子。需要判断是否移出轮盘外
        /// </summary>
        public void LockedSymbolMove(){
            if(LastLockedSymbols.Count==0) return;
            RolArrs = new int[5];
            List<int> tempArrs =new List<int>();
            // 获取图标之间的距离
            var stepSize = context.view.Get<Wheel>().GetRoll(0).stepSize;
            foreach (var item in LastLockedSymbols)
            {
                XDebug.Log("LockedSymbols:"+item.name);
                var col = int.Parse(item.name.Split('_')[1]);
                var rol = int.Parse(item.name.Split('_')[2]);

                var targetSize = item.transform.localPosition.y - stepSize;
                item.transform.DOLocalMoveY(targetSize,1).OnComplete(()=>{
                    if(item.transform.localPosition.y<=-2.5f){
                        item.gameObject.SetActive(false);
                    }
                });
                // 记录当前col有几个图标
                if(rol<4)
                    RolArrs[col]++;
                // 如果当前col的为最后一行时，也有数据，需要记录
                if(rol==3)
                    tempArrs.Add(col);   
            }
            // 筛选当前列是否为最后一行，并且当前金币数只有一个
            if(tempArrs.Count!=0){
                foreach (var col in tempArrs)
                {
                    if(RolArrs[col]==1){
                        context.view.Get<FreeWheelRowFx11312>().SetRowShow(col);
                    }
                    // 剔除重合当前列的第四行
                    if(RolArrs[col]>0){
                        RolArrs[col]--;
                    }
                }
            }
        
        }
        public void ClearAllLastLockedSymbols(){
            if(LastLockedSymbols.Count==0) return;
            foreach (var item in LastLockedSymbols){
                GameObject.Destroy(item.gameObject);
            }
            LastLockedSymbols = new List<GameObject>();
        }

        public void SymbolsStartBound(int rollIndex){
            if(LastLockedSymbols.Count==0) return;
            foreach (var item in LastLockedSymbols){
                var posArr = item.name.Split('_');
                var nextPosRol = int.Parse(posArr[2])+1;
                if(int.Parse(posArr[1]) == rollIndex && item.gameObject.activeSelf){
                    var curWheel = context.view.Get<Wheel>(1);
                    var curVisibleSymbol = curWheel.GetRoll(int.Parse(posArr[1])).GetVisibleContainer(nextPosRol);
                    var elementConfigSet = context.state.machineConfig.GetElementConfigSet();
                    var seqElement = new SequenceElement(elementConfigSet.GetElementConfig(uint.Parse(posArr[3])), context);
                    curVisibleSymbol.UpdateElement(seqElement);
                    
                    curVisibleSymbol.PlayElementAnimation("Loop");
                    curVisibleSymbol.ShiftSortOrder(false);
                    curVisibleSymbol.GetElement().UpdateMaskInteraction(SpriteMaskInteraction.VisibleInsideMask);
                    // 如果是蓝币
                    if(Constant11312.ListCoinElementIds.Contains(uint.Parse(posArr[3]))){
                        var IntegralText = curVisibleSymbol.GetElement().transform.Find("AnimRoot/IntegralGroup/IntegralText");
                        IntegralText.GetComponent<TextMesh>().text 
                            = ""+item.transform.Find("AnimRoot/IntegralGroup/IntegralText").GetComponent<TextMesh>().text;
                        IntegralText.GetComponent<MeshRenderer>().material.SetFloat("_StencilComp", 2);
                    }

                    item.gameObject.SetActive(false);
                    
                }
            }
            
        }
    }
}
