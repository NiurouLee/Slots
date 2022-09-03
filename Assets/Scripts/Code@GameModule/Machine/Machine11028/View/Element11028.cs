//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-27 15:06
//  Ver : 1.0.0
//  Description : Element11028.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;

namespace GameModule
{
    public class Element11028:Element
    {
        public Element11028(Transform transform, bool inIsStatic)
            : base(transform, inIsStatic)
        {
        }

        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            if (element.config.id == Constant11028.B01)
            {
                PlayAnimation("Tail",true);
            }
        }
    }
}