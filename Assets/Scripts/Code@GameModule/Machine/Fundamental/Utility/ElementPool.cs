// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-06 2:11 PM
// Ver : 1.0.0
// Description : ElementPool.cs
// ChangeLog :
// **********************************************
using UnityEngine;
using System.Collections.Generic;
using System;
namespace GameModule
{
    public class ElementPool
    {
        private uint _elementId;
        private GameObject _activeTemplate;
        private GameObject _staticTemplate;
        private Queue<Element> _activeElementPool;
        private Queue<Element> _staticElementPool;
        private Transform _holder;
        private Type _elementClassType;
        private ElementConfig _elementConfig;
        public ElementPool(ElementConfig inElementConfig,
            MachineAssetProvider provider)
        {
            _elementId = inElementConfig.id;
            _elementConfig = inElementConfig;

            _holder = provider.GetPoolAttachTransform();
            
            _activeTemplate = provider.GetAsset<GameObject>(inElementConfig.activeAssetName);
            _staticTemplate = provider.GetAsset<GameObject>(inElementConfig.staticAssetName);

            var initializePoolSize = inElementConfig.initializePoolSize;
              
            _activeElementPool = new Queue<Element>();
            _staticElementPool = new Queue<Element>();

            _elementClassType = _elementConfig.elementClassType;
            
            
            if (_activeTemplate == null)
            {
                _activeTemplate = _staticTemplate;

               
            }
            
            if (_staticTemplate == null)
            {
                Debug.LogError($"Asset[{inElementConfig.staticAssetName}] Not Exist");
                return;
            }
             
            for (var i = 0; i < initializePoolSize; i++)
            {
                var constructor = _elementClassType.GetConstructor(new[] {typeof(Transform), typeof(bool)});
               
                if (constructor != null)
                {
                    Element staticElement = constructor.Invoke(new object[]
                        {GameObject.Instantiate(_staticTemplate, _holder).transform, true}) as Element;
                    _staticElementPool.Enqueue(staticElement);

                    Element activeElement = constructor.Invoke(new object[]
                        {GameObject.Instantiate(_activeTemplate, _holder).transform, false}) as Element;
                    _activeElementPool.Enqueue(activeElement);
                }
            }
        }

        public Element GetActiveElement() 
        {
            if (_activeElementPool.Count > 0)
            {
                return _activeElementPool.Dequeue();
            }
            
            var constructor = _elementClassType.GetConstructor(new[] {typeof(Transform), typeof(bool)});

            if (constructor != null)
            {
                return constructor.Invoke(new object[] {GameObject.Instantiate(_activeTemplate, _holder).transform, false}) as Element;
            }
            
            return null;
        }
        
        public Element GetStaticElement() 
        {
            if (_staticElementPool.Count > 0)
            {
                return _staticElementPool.Dequeue();
            }
            
            var constructor = _elementClassType.GetConstructor(new[] {typeof(Transform), typeof(bool)});

            if (constructor != null)
            {
                return constructor.Invoke(new object[] {GameObject.Instantiate(_staticTemplate, _holder).transform, true}) as Element;
            }
            return null;
        }

        public void Recycle(Element element)
        {
            if (element.isStaticElement)
            {
                _staticElementPool.Enqueue(element);
            }
            else
            {
                _activeElementPool.Enqueue(element);
            }
            
            element.transform.SetParent(_holder, false);
        }

        public void ClearPool()
        {
            _activeElementPool.Clear();
            _staticElementPool.Clear();
        }
    }
}