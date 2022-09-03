
/**********************************************
Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-11-24 21:59:03
Ver : 1.0.0
Description : 
ChangeLog :  添加基础功能的MonoBehaviour方便以后做功能扩展
**********************************************/


using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class MonoAnimationEventProxy : MonoBehaviour
{
    class EventEntry
    {
        public AnimationEvent animationEvent;
        public string stateName;
        public string extraArgs;
        public Action<string> eventAction;
        public bool oneOffEvent;
        public int clipIndex;
    }

    private Animator animator;
    private Action<string> onAnimationEnd;

    private Dictionary<string, EventEntry> eventCallbackDict;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        // animationClips = animator.runtimeAnimatorController.animationClips;
    }

    public void SetEndEventCallback(Action<string> inAnimationEnd)
    {
        onAnimationEnd = inAnimationEnd;
    }

    public float GetStateDuration(string stateName)
    {
        if (!animator)
        {
            return -1;
        }

        var animationClips = animator.runtimeAnimatorController.animationClips;

        for (var i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i].name.Contains(stateName))
            {
                return animationClips[i].length;
            }
        }

        return -1;
    }

    public bool AddAnimationEvent(string stateName, Action<string> eventCallback, float time = 1000f,
        string extraArgs = "", bool oneOffEvent = true)
    {
        if (!animator)
        {
            return false;
        }

        var animationClips = animator.runtimeAnimatorController.animationClips;

        for (var i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i].name.Contains(stateName))
            {
                AnimationEvent animationEvent = new AnimationEvent();
                animationEvent.stringParameter = animationClips[i].name + "_" + time;
                animationEvent.functionName = "OnAnimationEvent";
                animationEvent.time = Math.Max(0, Math.Min(time, animationClips[i].length));
                animationClips[i].AddEvent(animationEvent);

                EventEntry entry = new EventEntry();
                entry.stateName = stateName;
                entry.eventAction = eventCallback;
                entry.animationEvent = animationEvent;
                entry.oneOffEvent = oneOffEvent;
                entry.extraArgs = extraArgs;
                entry.clipIndex = i;

                if (eventCallbackDict == null)
                {
                    eventCallbackDict = new Dictionary<string, EventEntry>();
                }

                eventCallbackDict.Add(animationEvent.stringParameter, entry);

                return true;
            }
        }

        return true;
    }

    public bool AddEndEventToAllAnimation()
    {
        if (!animator)
        {
            return false;
        }

        var animationClips = animator.runtimeAnimatorController.animationClips;

        for (var i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i].events.Length == 0)
            {
                AnimationEvent startEvent = new AnimationEvent();
                startEvent.stringParameter = animationClips[i].name;
                //     XDebug.Log("AnimationName:" + animationClips[i].name);
                startEvent.functionName = "OnAnimationEnd";
                startEvent.time = animationClips[i].length;
                animationClips[i].AddEvent(startEvent);
            }
        }

        return true;
    }

    private void OnDestroy()
    {
        if (!animator)
        {
            return;
        }

        if (animator.runtimeAnimatorController != null)
        {
            var animationClips = animator.runtimeAnimatorController.animationClips;
            for (var i = 0; i < animationClips.Length; i++)
            {
                if (animationClips[i].events.Length > 0)
                {
                    animationClips[i].events = null;
                }
            }
        }

        if (eventCallbackDict != null)
            eventCallbackDict.Clear();
    }

    void OnAnimationEvent(string eventKey)
    {
        if (eventCallbackDict == null || !eventCallbackDict.ContainsKey(eventKey))
            return;

        var entry = eventCallbackDict[eventKey];
        if (entry.oneOffEvent)
        {
            var animationClips = animator.runtimeAnimatorController.animationClips;

            if (animationClips[entry.clipIndex].name.Contains(entry.stateName))
            {
                if (animationClips[entry.clipIndex].events.Length > 0)
                {
                    var ls = animationClips[entry.clipIndex].events.ToList();
                    ls.Remove(entry.animationEvent);
                    animationClips[entry.clipIndex].events = ls.ToArray();
                    eventCallbackDict.Remove(eventKey);
                }
            }
        }

        var action = entry.eventAction;
        if (entry.extraArgs != "")
        {
            action.Invoke(entry.extraArgs);
        }
        else
        {
            action?.Invoke(entry.stateName);
        }
    }

    void OnAnimationEnd(string animationName)
    {
        if (onAnimationEnd != null)
        {
            onAnimationEnd.Invoke(animationName);
        }
    }
}