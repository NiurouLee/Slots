using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class WheelState11017: WheelState
    {
        public List<uint> changeList;
        public int panelCount;
        public SSpin serverSpinResult;
        public WheelState11017(MachineState state) : base(state)
        {
        }
        
        protected override void UpdateAppearAnimationInfo(Panel panel)
        {
            uint level = machineState.Get<ExtraState11017>().GetLevel();
            for (var i = 0; i < rollCount; i++)
            {
                if (level == 5)
                { 
                    if (i == 2) 
                    { 
                        panel.Columns[i].AppearingRows.Remove(2); 
                    }
                }
                blinkRows[i] = panel.Columns[i].AppearingRows;
            }
        }
        
        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
            serverSpinResult = new SSpin();
            base.UpdateStateOnRoomSetUp(gameEnterInfo);
        }
        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            haveFiveOfKinWinLine = false;
            resultIndex = 0;
            if (!wheelIsActive)
                return;

            var wheelSpinResult = spinResult.GameResult.Panels[resultIndex];
            panelCount = spinResult.GameResult.Panels.Count;
            serverSpinResult = spinResult;
            UpdateWheelStateInfo(wheelSpinResult);
        }
        
        public override void UpdateWheelStateInfo(Panel panel)
        {
            base.UpdateWheelStateInfo(panel);
            if (rollCount == 5)
            { 
                uint level = machineState.Get<ExtraState11017>().GetLevel(); 
                if (level == 5) 
                { 
                    var appearCount = 0;
                    for (var i = 0; i < 5; i++) 
                    { 
                        if (appearCount >=2) 
                        { 
                            anticipationInColumn[i] = true; 
                        }
                        else 
                        { 
                            anticipationInColumn[i] = false; 
                        }
                        if (IsSpinResultContainElementAtRollIndex(Constant11017.ScatterElementId, i)) 
                        { 
                            appearCount++;
                            if (i == 2) 
                            { 
                                var sequenceElements = spinResultElementDef[2].sequenceElements; 
                                if (sequenceElements[3].config.id == Constant11017.ScatterElementId) 
                                { 
                                    appearCount--; 
                                } 
                            } 
                        } 
                    }
                }
            }
        }
        
        public virtual void UpdateStateOnReceiveSpinResultByResult()
        {
            ++resultIndex;
            haveFiveOfKinWinLine = false;
            changeList = new List<uint>();
            if (!wheelIsActive)
                return;
            List<uint> lowLeveElementList = new List<uint>() { 5,6,7,8,9,10};
            if (resultIndex<serverSpinResult.GameResult.Panels.count)
            {
                var oldWheelSpinResult = serverSpinResult.GameResult.Panels[resultIndex-1];
                var wheelSpinResult = serverSpinResult.GameResult.Panels[resultIndex];
                 
                 //上一个panel中的元素Id
                 for (int i = 0; i < 5; i++)
                 { 
                     for (int j = 0; j < 1; j++)
                     {
                         //如果当前图标是低级图标,y == 0   记录需要消除下落的位置
                         if (lowLeveElementList.Contains(oldWheelSpinResult.Columns[i].Symbols[j]))
                         {
                             //如果当前图标下面的图标是低级图标,y==1
                             if (lowLeveElementList.Contains(oldWheelSpinResult.Columns[i].Symbols[j + 1]))
                             {
                                 //如果当前图标下面的图标是低级图标,y==2
                                 if (lowLeveElementList.Contains(oldWheelSpinResult.Columns[i].Symbols[j + 2]))
                                 {
                                     //一整列都会被移除,则新的一列每个都要移动三个格子
                                     changeList.Add(3);
                                     changeList.Add(3);
                                     changeList.Add(3);
                                 }
                                 //如果当前图标下面的图标不是低级图标,y==2
                                 else
                                 {
                                     //只移除0和1， 2是不需要补位的，则新的一列前两个都需要移动两个格子
                                     changeList.Add(2);
                                     changeList.Add(2);
                                     changeList.Add(0);
                                 }
                             }
                             //如果当前图标下面的图标不是低级图标，y==1
                             else
                             {
                                 //如果当前图标下面的图标是低级图标,y==2
                                 if (lowLeveElementList.Contains(oldWheelSpinResult.Columns[i].Symbols[j + 2]))
                                 {
                                     //只移除0，2，1要补到2的位置
                                     changeList.Add(2);
                                     changeList.Add(2);
                                     changeList.Add(1);
                                 }
                                 //如果当前图标下面的图标不是低级图标,y==2
                                 //0，1，2都不需要移除
                                 else
                                 {
                                     changeList.Add(1);
                                     changeList.Add(0);
                                     changeList.Add(0);
                                 }
                             }
                         }
                         //如果当前图标不是低级图标
                         else
                         {
                             //如果当前图标下面的图标是低级图标,y==1
                             if (lowLeveElementList.Contains(oldWheelSpinResult.Columns[i].Symbols[j + 1]))
                             {
                                 //如果当前图标下面的图标是低级图标,y==2
                                 if (lowLeveElementList.Contains(oldWheelSpinResult.Columns[i].Symbols[j + 2]))
                                 {
                                     //则第一个需要向下移动两个格子
                                     changeList.Add(2);
                                     changeList.Add(2);
                                     changeList.Add(2);
                                 }
                                 //如果当前图标下面的图标不是低级图标,y==2
                                 else
                                 {
                                     //则第二个格子需要向下移动1
                                     changeList.Add(1);
                                     changeList.Add(1);
                                     changeList.Add(0);
                                 }
                             }
                             //如果当前图标下面的图标不是低级图标，y==1
                             else
                             {
                                 //如果当前图标下面的图标是低级图标,y==2
                                 if (lowLeveElementList.Contains(oldWheelSpinResult.Columns[i].Symbols[j + 2]))
                                 {
                                     //只移除2
                                     changeList.Add(1);
                                     changeList.Add(1);
                                     changeList.Add(1);
                                 }
                                 //如果当前图标下面的图标不是低级图标,y==2
                                 //0，1，2都不需要移除
                                 else
                                 {
                                     changeList.Add(0);
                                     changeList.Add(0);
                                     changeList.Add(0);
                                 }
                             }
                         }
                     }
                 }
                 UpdateWheelStateInfo(wheelSpinResult);
            }
        }
    }
}