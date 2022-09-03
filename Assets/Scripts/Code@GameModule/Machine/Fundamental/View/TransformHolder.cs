// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-24 5:50 PM
// Ver : 1.0.0
// Description : TransformHolder.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace GameModule
{
    public class TransformHolder
    {
        public Transform transform;
        protected MachineContext context;
        public MonoUpdateProxy updateProxy;
        public TransformHolder(Transform inTransform)
        {
            transform = inTransform;
        }

        public virtual void Initialize(MachineContext inContext)
        {
            context = inContext;
        }

        public virtual bool MatchFilter(string filter)
        {
            return false;
        }
        
        public void EnableUpdate()
        {
            if (updateProxy == null)
            {
                updateProxy = transform.gameObject.AddComponent<MonoUpdateProxy>();
                updateProxy.BindingAction(Update);
            }
        }

        public MachineAssetProvider GetAssetProvider()
        {
            return context.assetProvider;
        }
        public MachineContext GetContext()
        {
            return context;
		}
 
        public virtual void OnOpen()
        {
        }


        public async virtual Task OnClose()
        {
            
        }


        public virtual void Update()
        {
        }

        public virtual void OnDestroy()
        {
            
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);     
        }
        
        public void SetTransformActive(Transform transform, bool visible)
        {
            if (transform)
            {
                transform.gameObject.SetActive(visible);
            }
        }
    }
}
