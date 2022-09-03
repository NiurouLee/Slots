using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class FlowerElementContainer11025:ElementContainer
    {
        private Transform wheelTransform;
        public WheelBase11025 baseWheel;
        private int sortingLayerID;
        private Animator flowerAnimator;
        private TextMesh flowerText;
        public bool IsBlink = false;
        public bool IsAnti = false;
        public FlowerElementContainer11025(Transform transform, string sortingLayer)
            :base(transform,sortingLayer)
        {
            EnableSortingGroup(false);
            containerGroup = null;
            sortingLayerID = SortingLayer.NameToID(sortingLayer);
            wheelTransform = transform.parent.parent.parent;
        }
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            baseWheel = context.view.Get<WheelBase11025>();
            var flowerObject = context.assetProvider.InstantiateGameObject("Active_FlowerNum",true);
            flowerObject.transform.SetParent(transform,false);
            flowerObject.SetActive(false);
            flowerAnimator = flowerObject.GetComponent<Animator>();
            flowerText = flowerAnimator.transform.Find("NumText").GetComponent<TextMesh>();
        }

        public async void ShowCollectFlower(int flowerNum,Action callBack)
        {
            flowerText.text = flowerNum.ToString();
            flowerAnimator.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(flowerAnimator,"Blink");
            callBack.Invoke();
        }

        public Vector3 GetMidOffsetPos(Vector3 startPos,Vector3 endPos,float offsetScale)
        {
            Vector3 midLocalPos = (startPos + endPos) * 0.5f;
            var distanceX = endPos.x - startPos.x;
            var distanceY = endPos.y - startPos.y;
            midLocalPos.x -= distanceY * offsetScale;
            midLocalPos.y += distanceX * offsetScale;
            return midLocalPos;
        }
        
        public void FlyCollectFlower()
        {
            flowerAnimator.gameObject.SetActive(false);
            GameObject flyFlower= context.assetProvider.InstantiateGameObject("Active_FlowerNum",true);
            flyFlower.transform.Find("NumText").GetComponent<TextMesh>().text = flowerText.text;
            flyFlower.transform.SetParent(wheelTransform,false);
            flyFlower.transform.position = flowerAnimator.transform.position;
            
            Vector3 fromLocalPos = flyFlower.transform.localPosition;
            Vector3 targetWorldPos = baseWheel.storeNode.Find("Img").position;
            Vector3 targetLocalPos = flyFlower.transform.parent.InverseTransformPoint(targetWorldPos);
            float distanceScale = 0.1f;
            Vector3 midLocalPos = GetMidOffsetPos(fromLocalPos ,targetLocalPos,distanceScale);
            var wayPointList = new List<Vector3>(){fromLocalPos,targetLocalPos};
            //圆弧
            // int midPointCount = 0;
            // for (var i = 0; i < midPointCount; i++)
            // {
            //     var tempNewList = new List<Vector3>();
            //     tempNewList.Add(fromLocalPos);
            //     for (var j=0;j<wayPointList.Count-1;j++)
            //     {
            //         var midPoint = GetMidOffsetPos(wayPointList[j], wayPointList[j + 1], distanceScale);
            //         tempNewList.Add(midPoint);
            //     }
            //     tempNewList.Add(targetLocalPos);
            //     wayPointList = tempNewList;
            // }
            //拐点
            int midPointCount = 2;
            var distanceVector = targetLocalPos - fromLocalPos;
            var partDistance = distanceVector / midPointCount;
            
            for (var i = 0; i < midPointCount; i++)
            {
                var basePoint = fromLocalPos + partDistance * i;
                var nextPoint = fromLocalPos + partDistance * (i + 1);
                var midPoint = GetMidOffsetPos(basePoint, nextPoint, distanceScale * (i%2 == 0?1:-1));
                wayPointList.Insert(i+1,midPoint);
            }
            Vector3[] wayPoints = wayPointList.ToArray();
            flyFlower.SetActive(true);
            XUtility.PlayAnimation(flyFlower.GetComponent<Animator>(),"Fly");
            // var distanceVector = targetLocalPos - fromLocalPos;
            // var tempQuaternion = flyFlower.transform.rotation;
            // var tanValue = Math.Atan2(distanceVector.y,distanceVector.x);
            // var tanTangle = (float)(tanValue * 180 / Math.PI);
            // var totalTangle = tanTangle - 90f;
            // tempQuaternion.eulerAngles = new Vector3(0, 0, totalTangle);
            // flyFlower.transform.Find("Fly").rotation = tempQuaternion;
            flyFlower.transform.DOLocalPath(wayPoints, Constant11025.CollectFlowerTime, PathType.Linear, PathMode.Full3D, 10).SetEase(Ease.Linear).OnComplete(
                () =>
                {
                    flyFlower.transform.Find("NumText").gameObject.SetActive(false);
                    flyFlower.GetComponent<Animator>().enabled = false;
                    flyFlower.GetComponent<SpriteRenderer>().enabled = false;
                    context.WaitSeconds(0.5f, () =>
                    {
                        flyFlower.transform.Find("NumText").gameObject.SetActive(true);
                        flyFlower.GetComponent<Animator>().enabled = true;
                        flyFlower.GetComponent<SpriteRenderer>().enabled = true;
                        // tempQuaternion.eulerAngles = Vector3.zero;
                        // flyFlower.transform.rotation = tempQuaternion;
                        context.assetProvider.RecycleGameObject("Active_FlowerNum",flyFlower);
                    });
                })
            //     .OnUpdate(() =>
            // {
            //     GameObject pathPoint= context.assetProvider.InstantiateGameObject("Static_FlowerNum",true);
            //     pathPoint.transform.position = flyFlower.transform.position;
            //     pathPoint.transform.localScale = new Vector3(0.1f,0.1f,1);
            //     pathPoint.SetActive(true);
            // })
                ;
        }
        public override void UpdateElement(SequenceElement seqElement, bool active = false)
        {
            sequenceElement = seqElement;
            
            if (currentAttachElement != null)
            {
                currentAttachElement.DoRecycle();
                currentAttachElement = null;
            }
            
#if !PRODUCTION_PACKAGE
            if (sequenceElement != null && sequenceElement.config == null)
            {
                XDebug.Log("Invalid SequenceElement");
                return;
            }
#endif
            if (seqElement != null)
            {
                if (seqElement.config.position == 0 || (seqElement.config.createBigElementParts && seqElement.config.position > 0))
                {
                    currentAttachElement =
                        active ? seqElement.config.GetActiveElement() : seqElement.config.GetStaticElement();
                    currentAttachElement.UpdateOnAttachToContainer(transform, seqElement);
                    containerGroup = currentAttachElement.transform.GetComponent<SortingGroup>();
                    containerGroup.sortingLayerID = sortingLayerID;
                    ShiftSortOrder(active);
                    // UpdateBaseSortingOrder(baseSortOrder);
                    // if (!active)
                    // {
                    //     UpdateBaseSortingOrder(baseSortOrder);
                    // }
                }
            }
        }
        public override void UpdateBaseSortingOrder(int sortingOrder)
        {
            baseSortOrder = sortingOrder;
            if (containerGroup)
            {
                containerGroup.sortingOrder = extraSortOrder + baseSortOrder +(ReferenceEquals(sequenceElement, null) ? 0 :sequenceElement.config.zOffset);   
            }
        }

        public void PlayScatterAntiIdle()
        {
            PlayElementAnimation("Idle");
        }
    }
}