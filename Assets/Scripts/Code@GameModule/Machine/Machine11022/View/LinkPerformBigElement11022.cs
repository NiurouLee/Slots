using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class LinkPerformBigElement11022:TransformHolder
    {
        public ElementContainer containerView;
        public bool HasUpdateElement;
        public Vector3 position;
        public int width;
        public int height;
        public float tempComparison;
        public MeshRenderer num;
        public LinkPerformBigElement11022(Transform inTransform,Vector3 inPosition,SequenceElement element,int inWidth,int inHeight): base(inTransform)
        {
            width = inWidth;
            height = inHeight;
            HasUpdateElement = false;
            GameObject cv = new GameObject("ContainerView");
            cv.SetActive(true);
            containerView = new ElementContainer(cv.transform, "Element");
            containerView.Initialize(context);
            cv.transform.SetParent(transform, false);
            position = inPosition;
            cv.transform.localPosition = position;
            containerView.UpdateElement(element, true);
            containerView.UpdateExtraSortingOrder(1);
            var sortingGroup = containerView.transform.GetComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "SoloElement";
            containerView.GetElement().UpdateMaskInteraction(SpriteMaskInteraction.None,true);
        }

        public void UpdateElement(SequenceElement element,bool active)
        {
            HasUpdateElement = true;
            containerView.UpdateElement(element,active);
            var tempElement = containerView.GetElement();
            tempElement.UpdateMaskInteraction(SpriteMaskInteraction.None,true);
            if (Constant11022.ValueList.Contains(element.config.id))
            {
                if (new List<uint>() {14, 15, 16}.Contains(element.config.id))
                {
                    num = tempElement.transform.Find("Coin/NUM")?.GetComponent<MeshRenderer>();
                }
                else
                {
                    num = tempElement.transform.Find("NUM")?.GetComponent<MeshRenderer>();
                }

                if (num != null)
                {
                    tempComparison = num.material.GetFloat("_StencilComp");
                    num.material.SetFloat("_StencilComp",8f);   
                }
                // CompareFunction
            }
        }
        
        public void PerformOpenBox()
        {
            if (HasUpdateElement)
            {
                XDebug.LogError("PerformOpenBox while box has been opened");
            }
            GameObject cv = new GameObject("ContainerView");
            cv.SetActive(true);
            var openBoxContainer = new ElementContainer(cv.transform, "Element");
            openBoxContainer.Initialize(context);
            cv.transform.SetParent(transform, false);
            cv.transform.localPosition = position;
            openBoxContainer.UpdateElement(containerView.sequenceElement, true);
            openBoxContainer.UpdateExtraSortingOrder(1000);
            var sortingGroup = openBoxContainer.transform.GetComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "SoloElement";
            openBoxContainer.GetElement().UpdateMaskInteraction(SpriteMaskInteraction.None,true);
            openBoxContainer.PlayElementAnimation("Win",false, () =>
            {
                openBoxContainer.RemoveElement();
                GameObject.Destroy(openBoxContainer.transform.gameObject);
            });
            AudioUtil.Instance.PlayAudioFx("J01_Broken");
            // containerView.transform.gameObject.SetActive(false);
        }

        public uint GetContainerSymbolId()
        {
            return containerView.sequenceElement.config.id;
        }
        public bool IsPerformEnd()
        {
            return HasUpdateElement;
        }

        public void DestorySelf()
        {
            if (IsPerformEnd() && num!=null)
            {
                num.material.SetFloat("_StencilComp",tempComparison);
            }

            containerView.RemoveElement();
            GameObject.Destroy(containerView.transform.gameObject);
        }
    }
}