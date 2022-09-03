// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/25/19:27
// Ver : 1.0.0
// Description : AudioUtil.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Threading.Tasks;

namespace GameModule
{
    public class AudioUtil
    {
        private MachineContext _context;
        private MachineAssetProvider _provider;
        private GameObject _parent;

        private List<AudioSource> _audioSourcePool;
        private Dictionary<string, IList<AudioSource>> _playedFxDict;

        public AudioSource musicSource;
        public AudioSource oneShotSource;

        public Dictionary<string, AudioClip> musicAudioClipDict;

        public IEnumerator fadeAction;
        public IEnumerator audioFadeAction;
        private static AudioUtil _instance;

        public static AudioUtil Instance => _instance;

        private bool _musicEnabled = true;
        private bool _soundEnabled = true;

        private float _musicMaxVolume = 1.0f;
        private float _musicSavedVolume = 1.0f;
        private class SavePoint
        {
            public string musicName;
            public bool loop;
            public float time;
        }

        private List<SavePoint> _savePoints;

        public static void BindingContext(MachineContext inContext)
        {
            if (_instance == null)
            {
                _instance = new AudioUtil(inContext);
            }
            else
            {
                _instance.UpdateContext(inContext);
            }
        }

        private void UpdateContext(MachineContext inContext)
        {
            _context = inContext;
            _provider = inContext.assetProvider;

            musicAudioClipDict = new Dictionary<string, AudioClip>();
            _audioSourcePool = new List<AudioSource>();

            _musicMaxVolume = _musicEnabled ? 1 : 0;

            _parent = _context.transform.gameObject;

            _savePoints = new List<SavePoint>();
        }

        private AudioUtil(MachineContext inContext)
        {
            _instance = this;
            UpdateContext(inContext);
        }

        public void OnDestroy()
        {
            if (fadeAction != null)
            {
                _context.StopCoroutine(fadeAction);
            }

            StopAllAudioFx();
            _audioSourcePool?.Clear();

            musicSource = null;
            musicAudioClipDict?.Clear();
            _provider = null;

            _savePoints = null;
        }

        public AudioClip GetMusicAudioClip(string musicName)
        {
            if (musicAudioClipDict.ContainsKey(musicName))
            {
                return musicAudioClipDict[musicName];
            }

            var audioClip = _provider.GetAsset<AudioClip>(musicName);

            if (audioClip)
            {
                musicAudioClipDict.Add(musicName, audioClip);

                return audioClip;
            }

            return null;
        }

        public void MakeASavePoint()
        {
            var sp = new SavePoint();

            if (musicSource != null)
            {
                sp.musicName = musicSource.clip.name;
                sp.loop = musicSource.loop;
                sp.time = musicSource.time;
                _savePoints.Add(sp);
            }
        }

        public void RecoverLastSavePointMusicPlay()
        {
            if (_savePoints != null && _savePoints.Count > 0)
            {
                var sp = _savePoints[_savePoints.Count - 1];
                PlayMusic(sp.musicName, sp.loop, sp.time, 0);
                FadeInMusic(1, 0);
            }
        }

        public void PlayMusic(string name, bool loop = true, float time = 0, float volume = 1)
        {
            if (musicSource == null)
            {
                var clip = GetMusicAudioClip(name);
                if (clip != null)
                {
                    musicSource = CreateAudioSource(clip, loop, _parent, false);
                    musicSource.volume = Mathf.Min(volume, _musicMaxVolume);
                    musicSource.time = time;
                    musicSource.Play();
                }
            }
            else
            {
                if (musicSource.clip != null && musicSource.clip.name == name)
                {
                    musicSource.volume = _musicMaxVolume;

                    if (fadeAction != null)
                    {
                        _context.StopCoroutine(fadeAction);
                        fadeAction = null;
                    }

                    if (!musicSource.isPlaying)
                    {
                        musicSource.Play();
                    }
                }
                else
                {
                    if (musicSource.isPlaying)
                    {
                        musicSource.Stop();
                    }

                    musicSource.clip = GetMusicAudioClip(name);

                    if (musicSource.clip != null)
                    {
                        musicSource.loop = loop;
                        musicSource.time = time;
                        musicSource.volume = Mathf.Min(volume, _musicMaxVolume); ;
                        musicSource.Play();
                    }
                }
            }
        }



        public void StopMusic()
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }

        public void PauseMusic()
        {
            _musicMaxVolume = 0;
            //Debug.LogError($"=======PauseMusic");

            if (musicSource != null && musicSource.isPlaying)
            {
                _musicSavedVolume = musicSource.volume;
                musicSource.volume = _musicMaxVolume;
            }
        }

        public void UnPauseMusic()
        {
            //Debug.LogError($"=======UnPauseMusic _musicSavedVolume:{_musicSavedVolume}");
            _musicMaxVolume = _musicEnabled ? 1 : 0;

            if (musicSource != null)
            {
                if (musicSource.volume > 0)
                {
                    return;
                }

                musicSource.volume = Math.Min(_musicMaxVolume, _musicSavedVolume);

            }
        }

        public IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
        {
            float currentTime = 0;
            float start = audioSource.volume;

            while (currentTime < duration)
            {
                if (!_context.IsPaused)
                    currentTime += Time.deltaTime;

                audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);

                if (audioSource == musicSource)
                    audioSource.volume = Mathf.Min(audioSource.volume, _musicMaxVolume);
                yield return null;
            }
        }

        public void FadeOutMusic(float duration = 1, float startVolume = -1)
        {
            FadeMusicTo(0, duration, startVolume);
        }

        public void FadeInMusic(float duration = 1, float startVolume = -1)
        {
            FadeMusicTo(1, duration, startVolume);
        }

        public void FadeMusicTo(float musicVolume, float duration, float startVolume = -1)
        {
            if (!_musicEnabled)
                return;

            if (fadeAction != null)
            {
                _context.StopCoroutine(fadeAction);
                fadeAction = null;
            }

            _musicSavedVolume = musicVolume;

            if (musicSource != null && musicSource.isPlaying)
            {
                if (startVolume >= 0)
                {
                    musicSource.volume = startVolume;
                }


                fadeAction = StartFade(musicSource, duration, musicVolume);
                _context.StartCoroutine(fadeAction);
            }
        }

        public AudioSource PlayAudioFxIfNotPlaying(string fxName, bool loop = false, GameObject gameObject = null)
        {
            var audioSource = GetCurAudioFx(fxName);
            if (audioSource == null || audioSource.isPlaying == false)
            {

                return PlayAudioFx(fxName, loop, gameObject);
            }
            return audioSource;
        }

        public AudioSource PlayAudioFx(string fxName, bool loop = false, GameObject gameObject = null)
        {
            if (!_soundEnabled || _context.IsPaused)
                return null;

            var audioClip = _provider.GetAsset<AudioClip>(fxName);
            var audioSource = GetAudioSource(audioClip, loop, gameObject == null ? _parent : gameObject);
            audioSource.Play();

            //Debug.LogError($"==========name:{audioSource.clip.name}  md5:{audioSource.clip.GetHashCode()}");
            return audioSource;
        }

        public async Task<AudioSource> PlayAudioFxAsync(string fxName, bool loop = false, GameObject gameObject = null)
        {
            var audioSource = PlayAudioFx(fxName, loop, gameObject);
            if (audioSource != null)
            {
                var clip = audioSource.clip;
                if (clip != null)
                {
                    float time = clip.length;
                    await _context.WaitSeconds(time);
                }


            }

            return audioSource;
        }

        public void PlayAudioFxOneShot(string fxName, bool loop = false, GameObject gameObject = null)
        {
            if (!_soundEnabled || _context.IsPaused)
                return;

            if (oneShotSource == null)
            {
                oneShotSource = CreateAudioSource(null, false, _parent);
            }

            var audioClip = _provider.GetAsset<AudioClip>(fxName);

            if (audioClip != null)
                oneShotSource.PlayOneShot(audioClip);
        }

        public void StopAudioFx(string fxName)
        {
            for (var i = 0; i < _audioSourcePool.Count; i++)
            {
                if (_audioSourcePool[i].clip != null && _audioSourcePool[i].clip.name == fxName)
                {
                    _audioSourcePool[i].Stop();
                }
            }
        }

        public void StopAllAudioFx()
        {
            if (_audioSourcePool != null)
            {
                for (var i = 0; i < _audioSourcePool.Count; i++)
                {
                    _audioSourcePool[i].Stop();
                }
            }
        }

        public void FadeOutAudio(string fxName, float duration = 1, float startVolume = -1)
        {
            FadeAudioTo(fxName, 0, duration, startVolume);
        }

        public void FadeAudioTo(string fxName, float audioVolume, float duration, float startVolume = -1)
        {
            if (!_soundEnabled)
                return;

            if (audioFadeAction != null)
            {
                _context.StopCoroutine(audioFadeAction);
                audioFadeAction = null;
            }

            AudioSource audios = GetCurAudioFx(fxName);
            if (audios != null && audios.isPlaying)
            {
                if (startVolume >= 0)
                {
                    audios.volume = startVolume;
                }

                audioFadeAction = StartFade(audios, duration, audioVolume);
                _context.StartCoroutine(audioFadeAction);
            }
        }

        public AudioSource GetCurAudioFx(string fxName)
        {
            for (var i = 0; i < _audioSourcePool.Count; i++)
            {
                if (_audioSourcePool[i].clip != null && _audioSourcePool[i].clip.name == fxName)
                {
                    return _audioSourcePool[i];
                }
            }

            return null;
        }

        protected AudioSource GetAudioSource(AudioClip clip, bool loop, GameObject gameObject)
        {
            AudioSource audioSource = null;
            for (var i = 0; i < _audioSourcePool.Count; i++)
            {
                //Debug.LogError($"======_audioSourcePool {_audioSourcePool[i].GetHashCode()}:{_audioSourcePool[i].isPlaying}");

                if (!_audioSourcePool[i].isPlaying && _audioSourcePool[i].gameObject == gameObject)
                {
                    audioSource = _audioSourcePool[i];
                    audioSource.clip = clip;
                    audioSource.volume = 1;
                    audioSource.playOnAwake = false;
                    audioSource.loop = loop;
                    audioSource.time = 0;
                    break;
                }
            }

            if (audioSource == null)
            {
                audioSource = CreateAudioSource(clip, loop, gameObject);
            }

            return audioSource;
        }

        public AudioSource CreateAudioSource(AudioClip clip, bool loop, GameObject gameObject, bool usePool = true)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = 1;
            source.loop = loop;
            source.playOnAwake = false;
            if (usePool)
            {
                _audioSourcePool.Add(source);
            }
            return source;
        }

        public void OnSoundEnabled(bool enabled)
        {
            _soundEnabled = enabled;
            if (!enabled)
            {
                StopAllAudioFx();
            }
        }
        public void OnMusicEnabled(bool enabled)
        {
            if (_musicEnabled != enabled)
            {
                if (!enabled)
                {
                    if (fadeAction != null)
                    {
                        _context.StopCoroutine(fadeAction);
                        fadeAction = null;
                    }

                    _musicMaxVolume = 0.00001f;

                    if (musicSource)
                    {
                        musicSource.volume = _musicMaxVolume;
                    }
                }
                else
                {
                    _musicMaxVolume = 1.0f;

                    if (musicSource)
                    {
                        musicSource.volume = _musicMaxVolume;
                    }
                }

                _musicEnabled = enabled;
            }
        }
    }
}