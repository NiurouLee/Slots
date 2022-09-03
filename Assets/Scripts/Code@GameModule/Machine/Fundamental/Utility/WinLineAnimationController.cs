// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/08/11:37
// Ver : 1.0.0
// Description : WinLineAnimationController.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class WinLineAnimationController: IWinLineAnimationController
    {
        protected Wheel wheel;
        protected int repeatOperationId = 0;
        protected int blinkAllWinLineOperationId = 0;
        protected WinFrameLayer winFrameLayer;
        protected PayLineLayer payLineLayer;
        protected MachineAssetProvider assetProvider;
 
        public bool IsWinCyclePlaying { get; private set; }

        public void BindingWheel(Wheel inWheel)
        {
            wheel = inWheel;
            assetProvider = wheel.GetAssetProvider();
            
            InitializeWheelWinFrameLayer();
            InitializePayLineLayer();
        }
         
        public void InitializeWheelWinFrameLayer()
        {
            var wheelConfig = wheel.wheelState.GetWheelConfig();
            if (!string.IsNullOrEmpty(wheelConfig.winFrameAssetName) && winFrameLayer == null)
            {
                var winFrameAnimationObj =
                    assetProvider.InstantiateGameObject(wheelConfig.winFrameAssetName);
                if (winFrameAnimationObj != null)
                {
                    GameObject winFrameRoot = new GameObject("WinFrameRoot");
                    winFrameRoot.transform.SetParent(wheel.transform,false);
                    winFrameLayer = new WinFrameLayer(winFrameRoot.transform,
                        winFrameAnimationObj, "Element");
                }
            }
        }

        protected virtual void InitializePayLineLayer()
        {
            var winFrameAnimationObj =
                assetProvider.InstantiateGameObject("PayLine");
            if (winFrameAnimationObj != null)
            {
                payLineLayer = new PayLineLayer(wheel,
                    winFrameAnimationObj, "Element");
            }
        }
         
        public void BlinkAllWinLine(Action finishCallback)
        {
            var winLines = wheel.wheelState.GetNormalWinLine();

            for (var i = 0; i < winLines.Count; i++)
            { 
                BlinkWinLine(winLines[i]);
            }

            blinkAllWinLineOperationId++;           
 
            var operationId = blinkAllWinLineOperationId;
            
            wheel.GetContext().WaitSeconds(wheel.wheelState.GetWheelConfig().winLineBlinkDuration, ()=>
            {
                if (operationId == blinkAllWinLineOperationId)
                {
                    finishCallback?.Invoke();
                }
            });
        }

        public async void BlinkWinLineOneByOne()
        {
            try
            {
                var winLines = wheel.wheelState.GetNormalWinLine();

                if (winLines.Count == 0)
                {
                    return;
                }

                var blinkActionId = repeatOperationId;
            
                int currentIndex = 0;
              
                while (blinkActionId == repeatOperationId)
                {
                    IsWinCyclePlaying = true;
                    
                    await BlinkWinLineAsync(winLines[currentIndex]);
                
                    currentIndex++;
                    if (currentIndex >= winLines.Count)
                    {
                        currentIndex = 0;
                    }
                }

                IsWinCyclePlaying = false;
            }
            catch (Exception e)
            {
                IsWinCyclePlaying = false;
                XDebug.Log(e);
            }
        }
 
        public virtual async void BlinkWinLine(WinLine winLine)
        {
            try
            {
                await BlinkWinLineAsync(winLine);
            }
            catch (Exception e)
            {
                XDebug.Log("Exp");
            }
        }
        
        public virtual async Task BlinkWinLineAsync(WinLine winLine)
        {
            if (winLine.Positions == null || winLine.Positions.Count == 0)
                return;

            var wheelConfig = wheel.wheelState.GetWheelConfig();
            var count = winLine.Positions.Count;
            for (var i = 0; i < count; i++)
            {
                var pos = winLine.Positions[i];
                var rollIndex = (int)pos.X;
                if (wheelConfig.isIndependentWheel)
                {
                    rollIndex = (int)(pos.X / wheelConfig.rollRowCount);
                }
                if (((winLine.Mask >> rollIndex) & 1) > 0 || NeedPlayWinAnimation(winLine))
                {
                    PlayElementWinAnimation(pos.X, pos.Y, NeedShowWinFrame(winLine));   
                }   
            }
            payLineLayer?.ShowPayLine(winLine);

            wheel.ResetDoneTag();

            int actionId = repeatOperationId;

            await wheel.GetContext().WaitSeconds(wheel.wheelState.GetWheelConfig().winLineBlinkDuration);

            if (actionId != repeatOperationId)
                return;
  
            //如果只有一条时间线循环播放，那么不使用stop方法，stop方法内部处理精灵遮罩会造成层级跳跃
            //TODO:如果所有图标都在一条线上，但并不是只有一条，这里会发生跳帧，考虑一下优化
            for (var i = 0; i < count; i++)
            {
                var pos = winLine.Positions[i];
                var rollIndex = (int)pos.X;
                if (wheelConfig.isIndependentWheel)
                {
                    rollIndex = (int)(pos.X / wheelConfig.rollRowCount);
                }

                if (((winLine.Mask >> rollIndex) & 1) > 0 || NeedPlayWinAnimation(winLine))
                {
                    StopElementWinAnimation(pos.X, pos.Y);   
                }
            }
            
            winFrameLayer?.HideAllWinFrame();
            payLineLayer?.HideAllPayLines();
            
            wheel.ResetDoneTag();
        }

        /// <summary>
        /// Mask为0也需要播放图标赢钱动画
        /// </summary>
        /// <param name="winLine"></param>
        /// <returns></returns>
        protected virtual bool NeedPlayWinAnimation(WinLine winLine)
        {
            return false;
        }
        
        protected virtual bool NeedShowWinFrame(WinLine winLine)
        {
            return true;
        }

        public virtual void PlayElementWinAnimation(uint rollIndex, uint rowIndex, bool showWinFrame=true)
        {
            var elementContainer = wheel.GetWinLineElementContainer((int)rollIndex, (int)rowIndex);
            var winFrameElementContainer = wheel.GetWinFrameElementContainer((int)rollIndex, (int)rowIndex);
            if (showWinFrame)
            {
                winFrameLayer?.ShowWinFrame(winFrameElementContainer);   
            }

            var element = elementContainer.GetElement();
            if (element!=null && !element.HasAnimState("Win"))
            {
                return;
            }
            
            if (!elementContainer.doneTag)
            {
                elementContainer.ShiftSortOrder(true);
                elementContainer.PlayElementAnimation("Win");

                elementContainer.doneTag = true;
            }
        }

        public virtual void StopElementWinAnimation(uint rollIndex, uint rowIndex)
        {
            var container = wheel.GetWinLineElementContainer((int)rollIndex, (int)rowIndex);

            var element = container.GetElement();
            if (element!=null && !element.HasAnimState("Win"))
            {
                return;
            }
            
            if (!container.doneTag)
            {
                //  symbolContainerView.ToggleAnimationShareInstance(false);
                container.UpdateAnimationToStatic();
                container.ShiftSortOrder(false);
                //   panelView.ReelViews[col].UpdateContainerWinSortingOrder(symbolContainerView, false);
                container.doneTag = true;
            }
        }

        public virtual async Task BlinkBonusLine()
        {
           var bonusWinLines =  wheel.wheelState.GetBonusWinLine();
           List<string> listTriggerAudio = new List<string>();
           for (int i = 0; i < bonusWinLines.Count; i++)
           {
               for (var index = 0; index < bonusWinLines[i].Positions.Count; index++)
               {
                   var pos = bonusWinLines[i].Positions[index];
                   var container =  wheel.GetWinLineElementContainer((int) pos.X,(int) pos.Y);
                   //AudioUtil.Instance.PlayAudioFx(container.sequenceElement.config.name + "_Trigger");
                   container.PlayElementAnimation("Trigger");
                   container.ShiftSortOrder(true);
                   if (!listTriggerAudio.Contains(container.sequenceElement.config.name))
                   {
                       listTriggerAudio.Add(container.sequenceElement.config.name);
                   }
               }
           }

           if (bonusWinLines.Count>0)
           {
               foreach (var triggerAudio in listTriggerAudio)
               {
                   AudioUtil.Instance.PlayAudioFx(triggerAudio + "_Trigger");
               }
               await wheel.GetContext().WaitSeconds(wheel.wheelState.GetWheelConfig().bonusLineBlinkDuration);   
           }
        }
        
        
        public virtual async Task BlinkReSpinLine()
        {
            var reSpinWinLine =  wheel.wheelState.GetReSpinWinLine();
            List<string> listTriggerAudio = new List<string>();
            for (int i = 0; i < reSpinWinLine.Count; i++)
            {
                
                for (var index = 0; index < reSpinWinLine[i].Positions.Count; index++)
                {
                    var pos = reSpinWinLine[i].Positions[index];
                    var container =  wheel.GetWinLineElementContainer((int) pos.X,(int) pos.Y);
                    //AudioUtil.Instance.PlayAudioFx(container.sequenceElement.config.name + "_Trigger");
                    container.PlayElementAnimation("Trigger");
                    container.ShiftSortOrder(true);
                    if (!listTriggerAudio.Contains(container.sequenceElement.config.name))
                    {
                        listTriggerAudio.Add(container.sequenceElement.config.name);
                    }
                }
            }

            if (reSpinWinLine.Count > 0)
            {
                foreach (var triggerAudio in listTriggerAudio)
                {
                    AudioUtil.Instance.PlayAudioFx(triggerAudio + "_Trigger");
                }
                await wheel.GetContext().WaitSeconds(wheel.wheelState.GetWheelConfig().bonusLineBlinkDuration);
            }
        }
        
        public virtual async Task BlinkJackpotLine()
        {
            var winLines =  wheel.wheelState.GetJackpotWinLines();
            List<string> listTriggerAudio = new List<string>();
            for (int i = 0; i < winLines.Count; i++)
            {
                for (var index = 0; index < winLines[i].Positions.Count; index++)
                {
                    var pos = winLines[i].Positions[index];
                    var container =  wheel.GetWinLineElementContainer((int) pos.X,(int) pos.Y);
                    //AudioUtil.Instance.PlayAudioFx(container.sequenceElement.config.name + "_Trigger");
                    container.PlayElementAnimation("Trigger");
                    container.ShiftSortOrder(true);
                    if (!listTriggerAudio.Contains(container.sequenceElement.config.name))
                    {
                        listTriggerAudio.Add(container.sequenceElement.config.name);
                    }
                }
            }

            if (winLines.Count>0)
            {
                foreach (var triggerAudio in listTriggerAudio)
                {
                    AudioUtil.Instance.PlayAudioFx(triggerAudio + "_Trigger");
                }
                await wheel.GetContext().WaitSeconds(wheel.wheelState.GetWheelConfig().bonusLineBlinkDuration);   
            }
        }
        
        
        public virtual async Task BlinkFreeSpinTriggerLine()
        {
            var freeSpinTriggerLines =  wheel.wheelState.GetFreeSpinTriggerLine();

            List<string> listTriggerAudio = new List<string>();
            
            for (int i = 0; i < freeSpinTriggerLines.Count; i++)
            {
                var line = freeSpinTriggerLines[i];
                
                for (var index = 0; index < line.Positions.Count; index++)
                {
                    var pos = freeSpinTriggerLines[i].Positions[index];
                    var container =  wheel.GetWinLineElementContainer((int) pos.X,(int) pos.Y);
                    container.PlayElementAnimation("Trigger");
                    container.ShiftSortOrder(true);
                    if (!listTriggerAudio.Contains(container.sequenceElement.config.name))
                    {
                        listTriggerAudio.Add(container.sequenceElement.config.name);
                    }
                }
            }

            if (freeSpinTriggerLines.Count>0)
            {
                foreach (var triggerAudio in listTriggerAudio)
                {
                    AudioUtil.Instance.PlayAudioFx(triggerAudio + "_Trigger");
                }

                await wheel.GetContext().WaitSeconds(wheel.wheelState.GetWheelConfig().bonusLineBlinkDuration);   
            }
        }

        public virtual void StopAllElementAnimation(bool force = false)
        {
            repeatOperationId++;
            blinkAllWinLineOperationId++;
            
            blinkAllWinLineOperationId %= 100;
            
            repeatOperationId %= 10;

            for (var rollIndex = 0; rollIndex < wheel.rollCount; rollIndex++)
            {
                var roll = wheel.GetRoll(rollIndex);
                var rollRowCount = roll.containerCount;

                for (var row = 0; row < rollRowCount; row++)
                {
                    var container = roll.GetContainer(row);

                    if (!container.sequenceElement.config.keepStateWhenStopAllAnimation || force)
                    {
                        container.UpdateAnimationToStatic(false);
                    }

                    container.ShiftSortOrder(false);
                }
            }
            
          
            
            winFrameLayer?.HideAllWinFrame();
            payLineLayer?.HideAllPayLines();
        }
    }
}