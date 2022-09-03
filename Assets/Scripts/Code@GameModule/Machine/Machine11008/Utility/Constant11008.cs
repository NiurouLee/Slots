using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class Constant11008
    {
        public static readonly List<string> WheelName = new List<string>(){"WheelBaseGame","WheelFreeGame"};
        public static readonly uint WildSymbolId = 5;

        public static readonly uint wolfId = 4;

        public static ElementContainer GetTargetPos(MachineContext machineContext,uint curId){
            List<int> posArr = ConversionDataToPos((int)curId-1);
            var col = posArr[0];
            var rol = posArr[1];
            var curWhell = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            Debug.Log("Id:"+curId+"col:"+col+"rol"+rol);
            var targetSymbol = curWhell.GetRoll(col).GetVisibleContainer(rol);
            return targetSymbol;
        }
        /// <summary>
        /// 转化获取的金币索引到两个轮盘上的位置0-49，转成两个轮盘上的pos
        /// </summary>
        private static List<int> ConversionDataToPos(int data){
            //每个轮盘以5个图标数量分割，先能获取哪一列
            List<int> arr=new List<int>();
            var col = Math.Floor(((decimal)data / 3));
            //获取当前列的第几行
            var row = data - 3 * col;
            
            arr.Add((int)col);
            arr.Add((int)row);
            return arr;
        }
    }

}
