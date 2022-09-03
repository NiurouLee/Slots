using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class AnimationPlaySync: IUpdateable, IOnContextDestroy
    {
        protected string animName;
        protected float animTime;

        protected MachineContext machineContext;
        protected List<Animator> listAnimator = new List<Animator>();
        public AnimationPlaySync(MachineContext context,string name, float time)
        {
            machineContext = context;

            animName = name;
            animTime = time;
            
            
            machineContext.SubscribeDestroyEvent(this);
            UpdateScheduler.HookUpdate(this);
        }


        protected bool isStartUpdate = false;


        protected float tempTime = 0;
        public void Update()
        {
            if (isStartUpdate)
            {
                if (Time.time - tempTime > animTime)
                {
                    tempTime = Time.time;
                    for (int i = 0; i < listAnimator.Count; i++)
                    {
                        listAnimator[i].Play(animName,0,0);
                    }
                }
            }
        }


        public void StartPlay()
        {
            
            for (int i = 0; i < listAnimator.Count; i++)
            {
                listAnimator[i].speed = 1;
            }

            isStartUpdate = true;
        }


        public void PausePlay()
        {
            isStartUpdate = false;
            for (int i = 0; i < listAnimator.Count; i++)
            {
                listAnimator[i].speed = 0;
            }
        }


        public void StopPlay(string stopStateName)
        {
            isStartUpdate = false;
            if (!string.IsNullOrEmpty(stopStateName))
            {
                for (int i = 0; i < listAnimator.Count; i++)
                {
                    listAnimator[i].Play(stopStateName);
                }
            }
        }


        public bool IsPlaying()
        {
            return isStartUpdate;
        }


        public void Clear()
        {
            isStartUpdate = false;
            listAnimator.Clear();
        }


        public void RegisterAnimator(Animator animator)
        {
            if (!listAnimator.Contains(animator))
            {
                listAnimator.Add(animator);
            }
        }

        public bool UnregisterAnimator(Animator animator)
        {
            return listAnimator.Remove(animator);
        }




        public void OnContextDestroy()
        {
            UpdateScheduler.UnhookUpdate(this);
            Clear();
        }
    }
}