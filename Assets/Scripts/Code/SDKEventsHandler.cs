using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.SDKEvents;
using UnityEngine;

public class SDKEventsHandler : MonoBehaviour,IEventHandler<ProfileConflictEvent>
{
    private void Awake()
    {
        EventManager.Instance.Subscribe<ProfileConflictEvent>(this);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe<ProfileConflictEvent>(this);
    }

    public void OnNotify(ProfileConflictEvent message)
    {
        DragonU3DSDK.Storage.StorageManager.Instance.ResolveProfileConfict(message.ServerProfile, false);

    }
}
