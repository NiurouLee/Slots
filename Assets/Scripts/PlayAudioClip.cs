//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-07 12:12
//  Ver : 1.0.0
//  Description : PlayAudioClip.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;


#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class PlayAudioClip: MonoBehaviour
{
    [SerializeField,ReadOnly] private int audioIndex=-1;
    [SerializeField,ReadOnly] private bool isAudioLoop;
    [SerializeField] private AudioClip[] audioClips;
    private AudioSource _audioSource;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.enabled = false;
        _audioSource.playOnAwake = false;   
        
        if (Camera.main)
        {
            var audioCentralizeController = Camera.main.GetComponent<AudioCentralizeController>();
            if (audioCentralizeController != null)
                audioCentralizeController.RegisterAudio(this);
        }
    }

    private void OnDisable()
    {
        StopPlay();
    }

    private void StopPlay()
    {
        audioIndex = -1;
        isAudioLoop = false;
        _audioSource.enabled = false;
        _audioSource.Stop();    
    }

    private void OnDestroy()
    {
        if (Camera.main)
        {
            var audioCentralizeController = Camera.main.GetComponent<AudioCentralizeController>();
            if (audioCentralizeController != null)
                audioCentralizeController.UnRegisterAudio(this);
        }
    }

    public void SafeStopPlay()
    {
        if (_audioSource && _audioSource.isPlaying)
        {
            StopPlay();
        }
    }

    // private void OnDidApplyAnimationProperties()
    // {
    //     if (!_audioSource.isPlaying)
    //     {
    //         PlaySound();
    //     }
    // }

    private void PlaySound()
    {
        var audioCentralizeController = Camera.main.GetComponent<AudioCentralizeController>();
        if (audioCentralizeController == null || !audioCentralizeController.IsSoundEnabled)
        {
            return;
        }
        
        _audioSource.loop = isAudioLoop;
        if (audioClips!=null && audioClips.Length>0)
        {
            _audioSource.clip = audioClips[Mathf.Clamp(audioIndex, 0,audioClips.Length-1)];   
        }
        if (_audioSource.clip)
        {
            _audioSource.enabled = true;
            _audioSource.Play();
            if (!isAudioLoop)
            {
                StartCoroutine(WaitPlayEnd());   
            }
        }
    }

    private IEnumerator WaitPlayEnd()
    {
        yield return new WaitForSeconds(_audioSource.clip.length);
        StopPlay();
    }
}