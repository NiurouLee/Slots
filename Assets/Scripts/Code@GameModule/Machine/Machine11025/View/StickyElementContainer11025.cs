using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class StickyElementContainer11025:ElementContainer
    {
        private ElementConfigSet tempElementConfigSet;

        public ChameleonGameResultExtraInfo.Types.StickyItem nowStickyElementData;
        public StickyElementContainer11025(Transform transform, string sortingLayer):base(transform,sortingLayer)
        {
        }

        public void SetSortingOrder(int rollIndex, int rowIndex)
        {
            containerGroup.sortingOrder = 500 + 1 + rollIndex * 10 + rowIndex;
        }
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            tempElementConfigSet = context.machineConfig.GetElementConfigSet();
        }
        public bool HasContainer()
        {
            return !ReferenceEquals(currentAttachElement, null);
        }

        public void SetStickyData(ChameleonGameResultExtraInfo.Types.StickyItem stickyElementData,bool force = true)
        {
            SetStickyDataWithoutRefresh(stickyElementData);
            if (currentAttachElement == null)
            {
                var element = new SequenceElement(tempElementConfigSet.GetElementConfig(nowStickyElementData.Id), context);
                UpdateElement(element,true);   
                currentAttachElement.UpdateMaskInteraction(SpriteMaskInteraction.None);
            }
            UpdateToDataState(force);
        }

        public void SetStickyDataWithoutRefresh(ChameleonGameResultExtraInfo.Types.StickyItem stickyElementData)
        {
            nowStickyElementData = stickyElementData;
        }
        public void UpdateToDataState(bool afterMulti)
        {
            if (Constant11025.ValueList.Contains(nowStickyElementData.Id))
            {
                if (afterMulti)
                {
                    ((ElementValue11025) GetElement()).SetWinRate((long) nowStickyElementData.WinRate);
                }
                else
                {
                    ((ElementValue11025) GetElement()).SetWinRate((long) nowStickyElementData.WinRate/nowStickyElementData.Multiplier);   
                }
            }
        }

        public void ShowMultiAnimation()
        {
            XUtility.PlayAnimation(GetElement().animator,"Hit");
        }

        public Sequence GetCollectJumpTween(Vector3 targetPosition,int count)
        {
            XUtility.PlayAnimation(currentAttachElement.animator,"Win");
            Vector3 fromLocalPos = transform.localPosition;
            Vector3 targetWorldPos = targetPosition;
            Vector3 targetLocalPos = transform.parent.InverseTransformPoint(targetWorldPos);
            Vector3 midLocalPos = fromLocalPos;
            midLocalPos.y += 0.5f;
            Vector3[] wayPoints = new[] {fromLocalPos, midLocalPos, targetLocalPos};
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                AudioUtil.Instance.PlayAudioFx("J01_Fall");
            });
            sequence.Append(transform.DOLocalMove(midLocalPos, 0.3f).SetEase(Ease.OutQuad));
            sequence.Append(transform.DOLocalMove(targetLocalPos, 0.3f).SetEase(Ease.InQuad));
            sequence.AppendCallback(() =>
            {
                AudioUtil.Instance.PlayAudioFx("J01_Fall_Stop0"+count);
            });
            sequence.target = context.transform;
            return sequence;
        }
        public Sequence GetCollectJackpotJumpTween(Vector3 targetPosition,Transform tongueMask,int count)
        {
            Vector3 fromLocalPos = transform.localPosition;
            Vector3 targetWorldPos = targetPosition;
            Vector3 targetLocalPos = transform.parent.InverseTransformPoint(targetWorldPos);
            Vector3 midLocalPos = fromLocalPos;
            midLocalPos.y += 0.5f;
            Vector3 distanceToMask = tongueMask.localPosition - fromLocalPos;
            
            // var distanceToMid = midLocalPos - fromLocalPos;
            // var distanceToTarget = targetLocalPos - midLocalPos;
            // var fromMaskLocalPos = tongueMask.localPosition;
            // var midMaskLocalPos = fromMaskLocalPos + distanceToMid;
            // var targetMaskLocalPos = midMaskLocalPos + distanceToTarget;
            
            Vector3[] wayPoints = new[] {fromLocalPos, midLocalPos, targetLocalPos};
            var sequence = DOTween.Sequence();
            // sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() =>
            {
                AudioUtil.Instance.PlayAudioFx("Jackpot_Fall_Ago");
                XUtility.PlayAnimation(currentAttachElement.animator,"Win");
            });
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() =>
            {
                AudioUtil.Instance.PlayAudioFx("J01_Fall");
                distanceToMask = tongueMask.localPosition - fromLocalPos;
            });
            sequence.Append(transform.DOLocalMove(midLocalPos, 0.3f).SetEase(Ease.OutQuad).OnUpdate(() =>
            {
                tongueMask.localPosition = transform.localPosition + distanceToMask;
            }));
            sequence.Append(transform.DOLocalMove(targetLocalPos, 0.3f).SetEase(Ease.InQuad).OnUpdate(() =>
            {
                tongueMask.localPosition = transform.localPosition + distanceToMask;
            }));
            sequence.AppendCallback(() =>
            {
                AudioUtil.Instance.PlayAudioFx("J01_Fall_Stop0"+count);
                tongueMask.parent.gameObject.SetActive(false);
            });
            sequence.target = context.transform;
            return sequence;
        }
        public void RemoveStickyElement()
        {
            nowStickyElementData = null;
            if (!ReferenceEquals(currentAttachElement, null))
            {
                var tempRotation = currentAttachElement.transform.rotation;
                tempRotation.eulerAngles = Vector3.zero;
                currentAttachElement.transform.rotation = tempRotation;
            }
            RemoveElement();
        }

        public void PerformReadyToCollect()
        {
            AudioUtil.Instance.PlayAudioFx("J01_Up");
            XUtility.PlayAnimation(currentAttachElement.animator,"CollectBlink");
        }
    }
}