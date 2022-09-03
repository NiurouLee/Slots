using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine.Rendering;

namespace GameModule{
    public class WheelAnimationController11312 : WheelAnimationController
    {
        private ExtraState11312 extraInfo;
        private FeatureView11312 featureView;
        private FreeFeatureView11312 freeFeatureView;
        private FreeWheelRowFx11312 freeWheelRowFx;
        private FreeSpinState freeSpin;
        private ReSpinState11312 respin;
        private WheelsActiveState11312 wheelsActiveState;
        // 当前行播放音效次数统计
        private int curColSoundTimes = -1;
        private bool IsLastRespin;
        public override void OnWheelStartSpinning()
        {
            base.OnWheelStartSpinning();
            if(Constant11312.UIRespinFeature!=null){
                Constant11312.UIRespinFeature.Close();
            }
            curColSoundTimes = -1;
            // base free 三种Feature
            if (featureView == null)
                featureView = wheel.GetContext().view.Get<FeatureView11312>();
            if (freeFeatureView == null)
                freeFeatureView = wheel.GetContext().view.Get<FreeFeatureView11312>();
            if (freeWheelRowFx == null)
                freeWheelRowFx = wheel.GetContext().view.Get<FreeWheelRowFx11312>();
            if (extraInfo == null)
                extraInfo = wheel.GetContext().state.Get<ExtraState11312>();
            if(freeSpin==null)
                freeSpin = wheel.GetContext().state.Get<FreeSpinState>();
            if(respin==null)
                respin = wheel.GetContext().state.Get<ReSpinState11312>();
            if(wheelsActiveState==null)
                wheelsActiveState = wheel.GetContext().state.Get<WheelsActiveState11312>();

            IsLastRespin = false;
            if(respin.IsInRespin && respin.ReSpinLimit!=0 && respin.ReSpinCount == respin.ReSpinLimit)
                IsLastRespin = true;
                
            if(!freeSpin.IsOver && extraInfo.AllLastLockedSymbols.Count != 0){
                //根据上次数据，创建新的图标，保证每次都是最新数据和symbols
                // 先创建展示图标
                FreeLockedSymbolsShow();
                //向下位移图标
                freeFeatureView.LockedSymbolMove();
                //更新Row背景特效状态
                freeWheelRowFx.LockedRowShow(freeFeatureView.RolArrs);
            }
        }
        private bool IsCoinInFrames(){
            return extraInfo.CoinInFrame!=null && extraInfo.CoinInFrame.Count!=0;
        }
        public override void OnRollSpinningStopped(int rollIndex, Action rollLogicEnd)
        {
            if(IsLastRespin){
                var animationInfo = wheel.wheelState.GetBlinkAnimationInfo(rollIndex);
                ShowBlinkAnimation(animationInfo, rollIndex);
            }
            rollLogicEnd.Invoke();
        }

        public override void OnRollStartBounceBack(int rollIndex)
        {
             var animationInfo = wheel.wheelState.GetBlinkAnimationInfo(rollIndex);
             bool hasBlinkAppeared = false;
            // 如果为最后一次respin 特殊处理blink动画和音效
            if(IsLastRespin){
                if (animationInfo != null && animationInfo.Count > 0)
                    hasBlinkAppeared = true;
                if (!hasBlinkAppeared)
                {
                    PlayReelStop(rollIndex);
                }
            }else{
                hasBlinkAppeared = ShowBlinkAnimation(animationInfo, rollIndex);
                ReelStopSoundState.SoundState item = GetReelStopSoundState(rollIndex);
                if (item != null)
                {
                    if (item.RollStopCount ==
                        wheel.GetContext().state.Get<WheelsActiveState>().GetRunningWheel().Count)
                    {
                        if (string.IsNullOrEmpty(item.SoundName))
                        {
                            PlayReelStop(rollIndex);
                        }
                        else
                        {
                            AudioUtil.Instance.PlayAudioFx(item.SoundName);
                        }
                    }
                }
                else
                {
                    if (!hasBlinkAppeared)
                    {
                        PlayReelStop(rollIndex);
                    }
                }
                
            }
            if(!respin.IsInRespin){
                if (featureView.S01Symbols.Count != 0)
                    featureView.SymbolsStartBound(rollIndex, featureView.S01Symbols);
                if (featureView.WildSymbols.Count != 0)
                    featureView.SymbolsStartBound(rollIndex, featureView.WildSymbols);
                if (featureView.ScOrCoinSymbols.Count != 0)
                    featureView.SymbolsStartBound(rollIndex, featureView.ScOrCoinSymbols);

                if (!freeSpin.IsOver && freeFeatureView.LastLockedSymbols.Count != 0)
                    freeFeatureView.SymbolsStartBound(rollIndex);
            }
        }
        public override bool ShowBlinkAnimation(RepeatedField<uint> blinkInfo, int rollIndex)
        {
            bool blinkAnimationPlayed = false;

            ReelStopSoundState.SoundState item = GetReelStopSoundState(rollIndex);
            if (blinkInfo != null && blinkInfo.Count > 0)
            {
                int blinkIndex = GetNeedPlayBlinkSoundRowIndex(blinkInfo, rollIndex);

                for (var i = 0; i < blinkInfo.Count; i++)
                {
                    bool isHasRandomSc = false;
                    if (extraInfo.RandomScatter != null && extraInfo.RandomScatter.Count != 0)
                    {
                        foreach (var data in extraInfo.RandomScatter)
                        {
                            var posArr = Constant11312.ConversionDataToPos((int)data.Key, 4);
                            if (rollIndex == posArr[0] && posArr[1] == (int)blinkInfo[i])
                                isHasRandomSc = true;
                        }
                    }

                    var container = wheel.GetRoll(rollIndex).GetVisibleContainer((int)blinkInfo[i]);

                    XDebug.Log($"ShowAppearAnimation:{rollIndex}:{blinkInfo[i]}:{container.sequenceElement.config.id}");

                    container.ShiftSortOrder(true);

                    if (container.sequenceElement.config.blinkType != BlinkAnimationPlayStyleType.Default)
                    {
                        containerPlayBlinkAnimation.Add(container);
                    }

                    var appearKey = wheel.wheelName + rollIndex + blinkInfo[i];
                    listWheelAppears.Add(appearKey);

                    if (isHasRandomSc)
                    {
                        DealContainerLogic(container, appearKey);
                    }
                    else
                    {
                        if (!blinkAnimationPlayed && blinkInfo[i] == blinkIndex)
                        {
                            if (item != null)
                            {
                                if (item.blinkSoundOrderId < container.sequenceElement.config.blinkSoundOrderId)
                                {
                                    item.blinkSoundOrderId = container.sequenceElement.config.blinkSoundOrderId;
                                    item.SoundName = GetBlinkSoundName(container, rollIndex, (int)blinkInfo[i]);
                                }
                                item.RollStopCount++;
                            }
                            else
                            {
                                PlayBlinkSound(container, rollIndex, (int)blinkInfo[i]);
                            }
                        }
                        container.PlayElementAnimation("Blink", false, () =>
                        {
                            DealContainerLogic(container, appearKey);
                        });
                    }

                    if (container.sequenceElement.config.id == Constant11312.ScSymbolId)
                    {
                        container.ShiftSortOrder(false);
                    }
                    if (wheel.GetContext().elementExtraInfoProvider.CanShowElementAnticipation(container.sequenceElement.config.id))
                    {
                        ShowElementAnticipationAnimation(rollIndex, container.sequenceElement.config.id);
                    }

                    blinkAnimationPlayed = true;
                }
            }
            else
            {
                if (item != null)
                {
                    item.RollStopCount++;
                }
            }

            return blinkAnimationPlayed;
        }
        public override void PlayReelStop(int rollIndex = -1)
        {
            if(respin.IsInRespin){
                // 设置一行最多只播一次音效
                var pos = Constant11312.ConversionDataToPos(rollIndex, 7);
            //    var panelHeight = extraInfo.PanelHeight;
                if (curColSoundTimes >= pos[0]) return;
                if (curColSoundTimes < pos[0])
                    curColSoundTimes = pos[0];
            }
            
            // XDebug.Log("curColSoundTimes:"+curColSoundTimes);
            if (!wheel.wheelState.playerQuickStopped || rollIndex < 0)
            {
                var reelStopSoundName = wheel.wheelState.GetEasingConfig().GetReelStopSoundName();
                if (!string.Equals(reelStopSoundName, GetReelStopSoundName(rollIndex)))
                {
                    reelStopSoundName = GetReelStopSoundName(rollIndex);
                }
                AudioUtil.Instance.PlayAudioFxOneShot(reelStopSoundName);
            }
        }


        private void DealContainerLogic(ElementContainer container,string appearKey){
            if (container.sequenceElement.config.id == Constant11312.ScSymbolId)
            {
                container.PlayElementAnimation("Idle");
                container.ShiftSortOrder(false);
            }
            if ((respin.ReSpinTriggered || !respin.NextIsReSpin) && Constant11312.AllListCoinElementId.Contains(container.sequenceElement.config.id))
            {
                container.PlayElementAnimation("Loop");
                container.ShiftSortOrder(false);
                container.GetElement().UpdateMaskInteraction(SpriteMaskInteraction.VisibleInsideMask);
                if (Constant11312.ListCoinElementIds.Contains(container.sequenceElement.config.id))
                {
                    var IntegralText = container.GetElement().transform.Find("AnimRoot/IntegralGroup/IntegralText");

                    IntegralText.GetComponent<MeshRenderer>().material.SetFloat("_StencilComp", 2);
                }
            }

            listWheelAppears.Remove(appearKey);
            if (container.sequenceElement.config.blinkType == BlinkAnimationPlayStyleType.Default)
            {
                container.UpdateAnimationToStatic();
                container.ShiftSortOrder(false);
            }
            else
            {
                containerPlayBlinkAnimation.Remove(container);
            }

            if (containerPlayBlinkAnimation.Count == 0 && canCheckBlinkFinished)
            {
                CheckAndStopBlinkAnimation();
            }

            CheckAllBlinkFinished();
        }
        public override void OnAllRollSpinningStopped(Action callback)
        {
            base.OnAllRollSpinningStopped(callback);
            // S01feature
            if(featureView.S01Symbols.Count!=0){
                featureView.ClearAllBombFxs();
                featureView.ReplaceS01Symbols();
            }
            if(featureView.WildSymbols.Count!=0){
                featureView.ReplaceWildSymbols();
            }
            if(featureView.ScOrCoinSymbols.Count!=0){
                featureView.ClearAllFishFxs();
                featureView.ReplaceScOrCoinSymbols();
            }
            
            if(!freeSpin.IsOver)
                freeFeatureView.ClearAllLastLockedSymbols();
        }

        public override string GetBlinkSoundName(ElementContainer container, int rollIndex, int rowIndex)
        {
            var config = container.sequenceElement.config;

            var blinkSoundName = config.blinkSoundName + "0" + (GetBlinkSoundIndex(config, rollIndex) + 1);

            if (assetProvider.GetAsset<AudioClip>(blinkSoundName))
            {
                var freeState = container.sequenceElement.machineContext.state.Get<FreeSpinState>();
                if (freeState.IsInFreeSpin && assetProvider.GetAsset<AudioClip>(blinkSoundName + "_Free"))
                {
                    blinkSoundName = blinkSoundName + "_Free";
                }
            }
            else
            {
                blinkSoundName = config.blinkSoundName + "01";
                var freeState = container.sequenceElement.machineContext.state.Get<FreeSpinState>();
                if (freeState.IsInFreeSpin && assetProvider.GetAsset<AudioClip>(blinkSoundName+ "_Free"))
                {
                    blinkSoundName = blinkSoundName + "_Free";
                }
            }
            if(Constant11312.AllListCoinElementId.Contains(config.id))
                blinkSoundName = "J01_Blink01";
            return blinkSoundName;
        }
 
        /// <summary>
        /// free下锁定图标显示
        /// </summary>
        public void FreeLockedSymbolsShow(){
            foreach (var item in extraInfo.AllLastLockedSymbols)
            {
                XDebug.Log(item.Key);
                var posId = (int)item.Key;
                var symbolId = item.Value;
                var posArr = Constant11312.ConversionDataToPos(posId,4);
                var targetPos = wheel.GetRoll(posArr[0]).GetVisibleContainerPosition(posArr[1]);
                var elementConfigSet = wheel.GetContext().state.machineConfig.GetElementConfigSet();
                var seqElement = new SequenceElement(elementConfigSet.GetElementConfig(symbolId), wheel.GetContext());
                var lockedSymbol = wheel.GetContext().assetProvider.InstantiateGameObject(seqElement.config.staticAssetName);
                lockedSymbol.transform.SetParent(freeFeatureView.transform);
                lockedSymbol.transform.position = targetPos;
                lockedSymbol.transform.localScale = Vector3.one;
                var sort = lockedSymbol.AddComponent<SortingGroup>();
                sort.sortingLayerName = "SoloElement";
                sort.sortingOrder = 10*posArr[0]+2*posArr[1];
                lockedSymbol.name = "locked_"+posArr[0]+"_"+posArr[1]+"_"+symbolId;
                lockedSymbol.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                if(Constant11312.ListCoinElementIdsJackot.Contains(symbolId))
                    lockedSymbol.transform.Find("AnimRoot/JPGroup/JPSprite1").GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                if(Constant11312.ListCoinElementIds.Contains(seqElement.config.id)){
                    var curIndex = Constant11312.ListCoinElementIds.IndexOf(seqElement.config.id);
                    var winRate = Constant11312.ListCoinElementWinRates[curIndex];
                    var winChips = wheel.GetContext().state.Get<BetState>().GetPayWinChips(winRate);
                    var IntegralText = lockedSymbol.transform.Find("AnimRoot/IntegralGroup/IntegralText");
                    if(IntegralText!=null){
                        // MeshRenderer meshRenderer = IntegralText.GetComponent<MeshRenderer>();
                        // meshRenderer.material.SetFloat("_StencilComp",8);
                        IntegralText.GetComponent<TextMesh>().text = ""+winChips.GetAbbreviationFormat(1);
                    }       
                }
                freeFeatureView.LastLockedSymbols.Add(lockedSymbol);
            }
        }
        
        
    }
}

