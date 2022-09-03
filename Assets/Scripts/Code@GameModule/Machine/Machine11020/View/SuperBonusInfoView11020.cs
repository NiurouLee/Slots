
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModule
{
    public class SuperBonusInfoView11020 : TransformHolder
    {

        private Animator animator;

        private int oldCount;
         
        public SuperBonusInfoView11020(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);

            animator = transform.GetComponent<Animator>();

            oldCount = -1;
        }

        public void StartWheel()
        {
            XUtility.PlayAnimation(animator, "Idle");
        }

        public void UpdateSuperBonusInfo(int count, bool animation)
        {
            if (oldCount == count)
            {
                return;
            }

            oldCount = count;

            for (int i = 1; i < 6; ++i)
            {
                transform.Find("Coin" + i).gameObject.SetActive(i <= count);
            }

            if (animation)
            {
                if (count > 0)
                {
                    XUtility.PlayAnimation(animator, count == 5 ? "Open" : $"Coin{count}_Open", 
                        ()=>{
                            XUtility.PlayAnimation(animator, "Idle");
                    });

                    return;
                }
            }
            XUtility.PlayAnimation(animator, "Idle");
        }
    }
}
