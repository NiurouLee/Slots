using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace GameModule
{
    public class LockElementLayer: TransformHolder
    {
        protected Wheel wheel;

        protected Transform tranLockElementLayer;

        protected Dictionary<(int, int), Element> dictElement;

        public LockElementLayer(Transform transform)
            : base(transform)
        {
            Init();
        }

        public void BindingWheel(Wheel inWheel)
        {
            wheel = inWheel;
        }

        protected void Init()
        {
            dictElement = new Dictionary<(int, int), Element>();

            tranLockElementLayer = new GameObject("LockElementLayer").transform;
            tranLockElementLayer.SetParent(transform,false);
        }
        public void SetSortingGroup(string layerName, int sortOrder)
        {
            var sortingGroup = tranLockElementLayer.GetComponent<SortingGroup>();
           
            if (sortingGroup == null)
                sortingGroup = tranLockElementLayer.gameObject.AddComponent<SortingGroup>();
            
            sortingGroup.sortingLayerID = SortingLayer.NameToID(layerName);
            sortingGroup.sortingOrder = sortOrder;
        }
        public bool LockElement(SequenceElement sequenceElement, int row, int column, bool isActive = false)
        {
            var key = (row, column);

            if (!dictElement.ContainsKey(key))
            {
                Element element = null;
                if (isActive)
                {
                    element = sequenceElement.config.GetActiveElement();
                }
                else
                {
                    element = sequenceElement.config.GetStaticElement();
                }

                var containerPos = wheel.GetRoll(column).GetVisibleContainerPosition(row);
                element.transform.SetParent(tranLockElementLayer,false);
                element.transform.position = containerPos;
                element.sequenceElement = sequenceElement;
                dictElement[key] = element;
                return true;
            }

            return false;
        }

        public Element GetElement(int row, int column)
        {
             var key = (row, column);

             if (dictElement.ContainsKey(key))
             {
                 return dictElement[key];
             }

             return null;
        }

        public bool ClearElement(int row, int column)
        {
            if (dictElement.TryGetValue((row, column), out Element element))
            {
                element.transform.localPosition = Vector3.zero;
                element.transform.localScale = Vector3.one;
                element.DoRecycle();
                dictElement.Remove((row, column));
                return true;
            }

            return false;
        }
        
        public void ClearAllElement()
        {
            foreach (var elementPair in dictElement)
            {
                elementPair.Value.transform.localScale = Vector3.one;
                elementPair.Value.transform.localPosition = Vector3.zero;
                elementPair.Value.DoRecycle();
            }

            dictElement.Clear();
        }

    }
}