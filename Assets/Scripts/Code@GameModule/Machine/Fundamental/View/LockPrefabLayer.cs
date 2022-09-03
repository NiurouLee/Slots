using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class LockPrefabLayer
    {
        private Wheel wheel;

        protected Transform tranLockElementLayer;

        protected Dictionary<(int, int), GameObject> dictElement;

        protected MachineContext machineContext;
        
        public LockPrefabLayer(Wheel mWheel,MachineContext context)
        {
            machineContext = context;
            wheel = mWheel;
            Init();
        }

        protected void Init()
        {
            dictElement = new Dictionary<(int, int), GameObject>();

            tranLockElementLayer = new GameObject("LockElementLayer").transform;
            tranLockElementLayer.parent = wheel.transform;
            tranLockElementLayer.localPosition = Vector3.zero;
        }

        public void RefreshWheel(Wheel mWheel)
        {
            wheel = mWheel;
            tranLockElementLayer.parent = wheel.transform;
            tranLockElementLayer.localPosition = Vector3.zero;
        }


        public GameObject AttachToElement(string objName, int row, int column)
        {
            var key = (row, column);
            GameObject obj = null;
            if (!dictElement.TryGetValue(key,out obj))
            {

                var objElement = machineContext.assetProvider.InstantiateGameObject(objName, true);
                objElement.name = objName;

                var containerPos = wheel.GetRoll(column).GetVisibleContainerPosition(row);
                objElement.transform.parent = tranLockElementLayer;
                objElement.transform.position = containerPos;
                objElement.transform.localScale = Vector3.one;

                dictElement[key] = objElement;
                obj = objElement;
            }

            return obj;
        }

        public GameObject ReplaceOrAttachToElement(string objName, int row, int column)
        {
            var key = (row, column);

            if (dictElement.ContainsKey(key))
            {
                Remove(row, column);
            }

            return AttachToElement(objName, row, column);
        }

        public bool Contains(int row, int column)
        {
            var key = (row, column);
            return dictElement.ContainsKey(key);
        }

        public string GetContainsName(int row, int column)
        {
            var key = (row, column);
            if (dictElement.TryGetValue(key, out GameObject element))
            {
                return element.name;
            }


            return string.Empty;
        }
        
        
        public GameObject GetElement(int row, int column)
        {
            var key = (row, column);
            if (dictElement.TryGetValue(key, out GameObject element))
            {
                return element;
            }


            return null;
        }


        public bool Remove(int row, int column)
        {
            if (dictElement.TryGetValue((row, column), out GameObject element))
            {
                //element.DoRecycle();
                machineContext.assetProvider.RecycleGameObject(element.name,element);
                dictElement.Remove((row, column));
                return true;
            }

            return false;
        }


        public void Clear()
        {
            foreach (var elementPair in dictElement)
            {
                //elementPair.Value.DoRecycle();
                var obj = elementPair.Value;
                machineContext.assetProvider.RecycleGameObject(obj.name,obj);
            }

            dictElement.Clear();
        }
        
        /// <summary>
        /// 封装更改当前锁定图标的layer参数
        /// </summary>
        /// <param name="isShow"> 是否显示</param>
        /// <param name="sortingLayerName"> sortingLayerName</param>
        /// <param name="sortingLayerValue"> sortLayerValue</param>
        public void ChangeLockElementLayer(bool isShow = true,string sortingLayerName = "Element",int sortingLayerValue = 100){
            var sortingGroup = tranLockElementLayer.GetComponent<SortingGroup>();
            if(sortingGroup==null)
                sortingGroup = tranLockElementLayer.gameObject.AddComponent<SortingGroup>();
            if(!isShow)
                sortingGroup.enabled = false;
            else
                sortingGroup.enabled = true;

            sortingGroup.sortingLayerName = sortingLayerName;
            sortingGroup.sortingOrder = sortingLayerValue;
        }
    }
}