using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class BaseGameInfomationView11006: TransformHolder
    {
        
       
        
        public BaseGameInfomationView11006(Transform transform) : base(transform)
        {
            
          
        }


        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            
        }

        

        public void Open()
        {
            this.transform.gameObject.SetActive(true);
            
        }

        public void Close()
        {
            this.transform.gameObject.SetActive(false);
            
            
        }

    }
}