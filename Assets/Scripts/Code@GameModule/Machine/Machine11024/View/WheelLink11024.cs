using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class WheelLink11024:IndependentWheel
    {
        private ExtraState11024 _extraState;

        public ExtraState11024 extraState
        {
            get
            {
                if (_extraState == null)
                {
                    _extraState = context.state.Get<ExtraState11024>();
                }
                return _extraState;
            }
        }
        public int linkWheelIndex = 0;
        private Transform countBoard;
        private TextMesh countText;
        private Transform featureEndBoard;
        private Transform winValueBoard;
        private TextMesh winValueText;
        public long collectValue;
        public List<List<StickyElementContainer11024>> stickyElementList = new List<List<StickyElementContainer11024>>();
        public WheelLink11024(Transform transform) : base(transform)
        {
            countBoard = transform.Find("CountBoard");
            countText = countBoard.Find("Num").GetComponent<TextMesh>();
            featureEndBoard = transform.Find("FeatureEndBoard");
            winValueBoard = transform.Find("WinValueBoard");
            winValueText = winValueBoard.Find("Num").GetComponent<TextMesh>();
        }

        public Vector3 GetCountTextPosition()
        {
            return countText.transform.position;
        }
        public Vector3 GetCollectBoardPosition()
        {
            return winValueBoard.position;
        }
        public override void BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(WheelState inWheelState,
            string inWheelElementSortingLayerName = "Element")
        {
            base.BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(inWheelState, inWheelElementSortingLayerName);
            stickyElementList.Clear();
            for (var i = 0; i < 5; i++)
            {
                var tempList = new List<StickyElementContainer11024>();
                for (var i1 = 0; i1 < 3; i1++)
                {
                    var roll = GetRoll(i * 3 + i1);
                    var stickyElementContainer = new GameObject("StickyElementContainer");
                    stickyElementContainer.transform.SetParent(transform);
                    stickyElementContainer.transform.position = roll.transform.position;
                    var stickyElement = new StickyElementContainer11024(stickyElementContainer.transform,"Element");
                    stickyElement.SetElementPosition(i,i1);
                    stickyElement.SetSortingOrder(i,i1);
                    stickyElement.Initialize(context);
                    stickyElement.Hide();
                    tempList.Add(stickyElement);
                }
                stickyElementList.Add(tempList);
            }
        }

        public override void SetActive(bool active, bool keepObjectState = false)
        {
            base.SetActive(active, keepObjectState);
            if (active)
            {
                if (extraState.HasReSpinType(1))
                {
                    transform.Find("CountBoard2")?.gameObject.SetActive(true);
                }
                countBoard.gameObject.SetActive(true);
                featureEndBoard?.gameObject.SetActive(false);
                winValueBoard.gameObject.SetActive(false);
                var nowSpinTimes = GetLeftSpinTimes();
                SetLeftSpinTimes(nowSpinTimes);
                for (var i = 0; i < 5; i++)
                {
                    for (var i1 = 0; i1 < 3; i1++)
                    {
                        stickyElementList[i][i1].Hide();
                    }
                }
            }
        }

        public void ShowFeatureEndBoard()
        {
            countBoard.gameObject.SetActive(false);
            featureEndBoard?.gameObject.SetActive(true);
            winValueBoard.gameObject.SetActive(false);
        }
        
        public void ShowWinValueBoard(bool activeState)
        {
            if (extraState.HasReSpinType(1))
            {
                transform.Find("CountBoard2")?.gameObject.SetActive(false);
            }
            countBoard.gameObject.SetActive(false);
            featureEndBoard?.gameObject.SetActive(false);
            winValueBoard.gameObject.SetActive(activeState);
            SetCollectValue(0);
        }

        public void AddCollectValue(long addValue)
        {
            var flyEndAnimator = context.assetProvider.InstantiateGameObject("Fly_linkEnd", true);
            var baseScale = flyEndAnimator.transform.localScale;
            if (extraState.HasReSpinType(1))
            {
                var tempScale = baseScale;
                tempScale.x *= 1.2f;
                tempScale.y *= 1.13f;
                flyEndAnimator.transform.localScale = tempScale;
            }
            else
            {
                flyEndAnimator.transform.localScale = baseScale*1.33f;
            }
            flyEndAnimator.SetActive(false);
            flyEndAnimator.SetActive(true);
            flyEndAnimator.transform.SetParent(transform,false);
            flyEndAnimator.transform.position = winValueBoard.position;
            context.WaitSeconds(2f, () =>
            {
                flyEndAnimator.transform.localScale = baseScale;
                context.assetProvider.RecycleGameObject("Fly_linkEnd", flyEndAnimator);
            });
            SetCollectValue(collectValue + addValue);
        }

        public void SetCollectValue(long value)
        {
            collectValue = value;
            if (collectValue > 0)
            {
                winValueText.SetText(collectValue.GetCommaFormat());
            }
            else
            {
                winValueText.SetText("");
            }
        }
        public void SetLeftSpinTimes(int nowSpinTimes)
        {
            if (nowSpinTimes <= 1)
            {
                countBoard.Find("Spins").gameObject.SetActive(false);
                countBoard.Find("Spin").gameObject.SetActive(true);
            }
            else
            {
                countBoard.Find("Spins").gameObject.SetActive(true);
                countBoard.Find("Spin").gameObject.SetActive(false);
            }

            countText.text = nowSpinTimes.ToString();
        }
        public void ReduceLeftSpinTimes()
        {
            var nowSpinTimes = GetLeftSpinTimes();
            if (nowSpinTimes > 0)
            {
                SetLeftSpinTimes(nowSpinTimes - 1);   
            }
        }

        public void RefreshLeftSpinTimes()
        {
            int maxSpinTimes = 3;
            if (extraState.HasReSpinType(2))
            {
                maxSpinTimes = 4;
            }
            if (NeedRefreshSpinTimes())
            {
                AudioUtil.Instance.PlayAudioFx("LinkGame_RespinRefresh");
                LightSpinTimes();
                SetLeftSpinTimes(maxSpinTimes);
            }
            else
            {
                XDebug.Log("不需要刷新link次数");
            }
        }

        public bool NeedRefreshSpinTimes()
        {
            int maxSpinTimes = 3;
            if (extraState.HasReSpinType(2))
            {
                maxSpinTimes = 4;
            }
            var nowSpinTimes = countText.text.ToInt();
            if (nowSpinTimes != maxSpinTimes)
            {
                return true;
            }
            return false;
        }

        public virtual int GetLeftSpinTimes()
        {
            return extraState.GetReSpinLeftSpin(linkWheelIndex);
        }

        public void ShowWheelCover(bool active)
        {
            transform.Find("WheelCover").gameObject.SetActive(active);
        }
        public async void ShowBoostCover(bool active)
        {
            var boostCover = transform.Find("BoostCover");
            if (active)
            {
                boostCover.gameObject.SetActive(true);
                await XUtility.PlayAnimationAsync(boostCover.GetComponent<Animator>(),"Open");
                boostCover.gameObject.SetActive(false);
            }
            else
            {
                XUtility.PlayAnimation(boostCover.GetComponent<Animator>(),"Close", () =>
                {
                    boostCover.gameObject.SetActive(false);
                },context);
            }
        }
        
        public virtual ElementContainer UpdateRunningElement(uint elementId, int rollIndex, int rowIndex = 0, bool active=false)
        {
            var lockRoll = GetRoll(rollIndex);
            var elementConfigSet = context.state.machineConfig.GetElementConfigSet();
            var seqElement = new SequenceElement(elementConfigSet.GetElementConfig(elementId), context);
            ElementContainer elementContainer= lockRoll.GetVisibleContainer(rowIndex);
            if (elementContainer != null)
            {
                elementContainer.UpdateElement(seqElement, active);
                if (active)
                {
                    elementContainer.ShiftSortOrder(true);
                    elementContainer.UpdateElementMaskInteraction(true);
                }
            }
            return elementContainer;
        }

        public void ChangeToFeatureEnd()
        {
            ShowFeatureEndBoard();
            for (var i = 0; i < rollCount; i++)
            {
                if (!wheelState.IsRollLocked(i))
                {
                    wheelState.SetRollLockState(i, true);
                }
            }
        }

        public PigGameResultExtraInfo.Types.LinkData GetLinkData()
        {
            return extraState.GetLinkData(linkWheelIndex);
        }
        public PigGameResultExtraInfo.Types.LinkData.Types.Item GetItemData(int rollIndex)
        {
            var linkData = GetLinkData().Items;
            return linkData[rollIndex];
            // for (var i = 0; i < linkData.Count; i++)
            // {
            //     var tempItemData = linkData[i];
            //     if (tempItemData.PositionId == rollIndex)
            //     {
            //         return tempItemData;
            //     }
            // }
            // throw new Exception("没找到对应位置的itemData");
        }

        public StickyElementContainer11024 GetStickyElement(int rollIndex)
        {
            var posX = rollIndex / 3;
            var posY = rollIndex % 3;
            var stickyElement = stickyElementList[posX][posY];
            return stickyElement;
        }

        public void CleanWheel()
        {
            for (var i = 0; i < rollCount; i++)
            {
                UpdateRunningElement(Constant11024.GetRandomNormalElementId(),i,0,false);
                UpdateRunningElement(6,i,1,false);
                GetStickyElement(i).RemoveStickyElement();
                wheelState.SetRollLockState(i, false);
                GetRoll(i).transform.gameObject.SetActive(true);
            }
        }

        public ulong GetGrandValue()
        {
            return GetLinkData().FullWinRate;
        }

        public void LightSpinTimes()
        {
            var light = context.assetProvider.InstantiateGameObject("Fx_GlowLink", true);
            light.SetActive(false);
            light.SetActive(true);
            light.transform.SetParent(countText.transform.parent,false);
            light.transform.position = countText.transform.position;
            context.WaitSeconds(1, () =>
            {
                context.assetProvider.RecycleGameObject("Fx_GlowLink", light);
            });
        }
        
        public async Task RaiseSpinTimes()
        {
            var addInterval = 0.5f;
            for (var i = 1; i <= 3; i++)
            {
                AudioUtil.Instance.PlayAudioFx("LinkGame_RespinRaise");
                // LightSpinTimes();
                SetLeftSpinTimes(i);
                await context.WaitSeconds(addInterval);
            }
        }

        public void HideSpinTimes()
        {
            countText.text = "";
        }
        public override Vector3 GetAnticipationAnimationPosition(int rollIndex)
        {
            return GetRoll(rollIndex).initialPosition;
        }
    }
}