// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/25/20:56
// Ver : 1.0.0
// Description : AudioCentralizeController.cs
// ChangeLog :
// **********************************************


using System.Collections.Generic;
using UnityEngine;

public class AudioCentralizeController:MonoBehaviour
{
    private List<PlayAudioClip> _currentPlayers;


    public bool IsSoundEnabled = true;
    
    public void RegisterAudio(PlayAudioClip audioClipPlayer)
    {
        if (_currentPlayers == null)
        {
            _currentPlayers = new List<PlayAudioClip>();
        }
        
        _currentPlayers.Add(audioClipPlayer);
    }
    
    public void UnRegisterAudio(PlayAudioClip audioClipPlayer)
    {
        if (_currentPlayers != null && _currentPlayers.Contains(audioClipPlayer))
        {
            _currentPlayers.Remove(audioClipPlayer);
        }
    }

    public void OnSoundEnabled()
    {
        IsSoundEnabled = true;
    }

    public void OnSoundDisabled()
    {
        IsSoundEnabled = false;
        StopAllRegisteredPlayingAudioClip();
    }

    public void StopAllRegisteredPlayingAudioClip()
    {
        if (_currentPlayers != null)
        {
            for (var i = 0; i < _currentPlayers.Count; i++)
            {
                _currentPlayers[i].SafeStopPlay();
            }
        }
    }
}
 