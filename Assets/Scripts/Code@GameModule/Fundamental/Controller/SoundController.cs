using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.NetworkInformation;
using DG.Tweening;
using DragonU3DSDK.Storage;

namespace GameModule
{
    public class SoundController:LogicController
    {
       protected AudioSource bgAudioSource;
       protected static GameObject soundContainer;
       protected float musicMaxVolume = 1.0f;
       protected Dictionary<string, AssetReference> soundAssetsDict;
       protected List<AudioSource> sfxAudioSourceList = new List<AudioSource>();

       private static SoundController _instance;

       public class SaveSpot
       {
           public string musicName;
           public float progress;
       }
     
       protected List<SaveSpot> musicSaveSpotList;
      

       public SoundController(Client client)
           : base(client)
       {
           _instance = this;
       }

        protected override void Initialization()
        {
            soundContainer = GameObject.Find("SoundContainer");
         
            if (soundContainer == null)
            {
                soundContainer = new GameObject("SoundContainer");
            }
            
            soundAssetsDict = new Dictionary<string, AssetReference>();
            
            musicSaveSpotList = new StorageList<SaveSpot>();
            
            base.Initialization();
        }

        public override void CleanUp()
        {
            base.CleanUp();
            Clear();
        }

        public override void Start()
        {
            bool musicEnabled = PreferenceController.IsMusicEnabled();

            if (!musicEnabled)
            {
                musicMaxVolume = 0.00001f;
            }
            else
            {
                musicMaxVolume = 1.0f;
            }
        }
        
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventMusicEnabled>(OnMusicEnabled);
        }

        public static void PlayBgMusic(string address, Action successCallback = null)
        {
            _instance.CreateBgSaveSpot();
            _instance.PlayBgMusicInternal(address, successCallback);
        }
        
        public static void RecoverLastMusic()
        {
            _instance.RecoverLastPlayingMusic();
        }
 
        protected void PlayBgMusicInternal(string address, Action successCallback = null)
        {
            if (soundAssetsDict.ContainsKey(address))
            {
                PlayBgMusic(soundAssetsDict[address].GetAsset<AudioClip>());
                successCallback?.Invoke();
            }
            else
            {
                var sceneType = ViewManager.ActiveSceneType;

                if (ViewManager.Instance.IsInSwitching())
                {
                    sceneType = ViewManager.SwitchingSceneType;
                }
                
                AssetHelper.PrepareAsset<AudioClip>(address, (task) =>
                {
                    //避免由于音效还没加载完，场景切换后在错误的地方播放音效
                    if (ViewManager.ActiveSceneType != sceneType
                        || ViewManager.Instance.IsInSwitching()
                        || task == null)
                    {
                        return;
                    }
                    
                    PlayBgMusic(task.GetAsset<AudioClip>());
                    
                    successCallback?.Invoke();
                    
                    if (soundAssetsDict.ContainsKey(address))
                    {
                        task.ReleaseOperation();
                    }
                    else
                    {
                        soundAssetsDict.Add(address, task);
                    }
                });
            }
        }

        public void RecoverLastPlayingMusic()
        {
            if (musicSaveSpotList.Count > 0)
            {
                var sp = musicSaveSpotList[musicSaveSpotList.Count - 1];
                musicSaveSpotList.RemoveAt(musicSaveSpotList.Count - 1);
                 
                var clip = soundAssetsDict[sp.musicName].GetAsset<AudioClip>();
                 
                if (bgAudioSource.isPlaying)
                {
                    if (bgAudioSource.clip.name == clip.name)
                    {
                        return;
                    }
                    
                    bgAudioSource.Stop();
                }
                 
                bgAudioSource.clip = clip;
                bgAudioSource.loop = true;
                bgAudioSource.time = sp.progress;
                bgAudioSource.Play();
                bgAudioSource.DOKill();
                bgAudioSource.DOFade(0, 0.6f).From();
            
                bgAudioSource.DOFade(musicMaxVolume, 0.6f);
            }
            else
            {
                if (bgAudioSource != null)
                {
                    if (bgAudioSource.isPlaying)
                        bgAudioSource.Stop();

                    bgAudioSource.time = 0;
                }
            }
        }

        protected void CreateBgSaveSpot()
        {
            if (bgAudioSource)
            {
                if (bgAudioSource.isPlaying)
                {
                    SaveSpot sp = new SaveSpot();
                    sp.musicName = bgAudioSource.clip.name;
                    sp.progress = bgAudioSource.time;
                    musicSaveSpotList.Add(sp);
                }
            }
        }
        
        protected void PlayBgMusic(AudioClip clip)
        {
            if (bgAudioSource == null)
            {
                bgAudioSource = soundContainer.AddComponent<AudioSource>();
            }

            if (bgAudioSource.isPlaying)
            {
                bgAudioSource.Stop();
            }
            
            bgAudioSource.clip = clip;
            bgAudioSource.loop = true;
            bgAudioSource.Play();
            bgAudioSource.DOKill();
            bgAudioSource.DOFade(0, 0.6f).From();
            
            bgAudioSource.DOFade(musicMaxVolume, 0.6f);
        }

        public static string GetPlayingBgMusicName()
        {
            if (_instance.bgAudioSource == null)
                return string.Empty;
            if (_instance.bgAudioSource.clip == null)
                return string.Empty;

            return _instance.bgAudioSource.clip.name;
        }

        public static void PlaySfx(string address, bool loop = false, CancelableCallback finishCallback = null)
        {
            if(!string.IsNullOrEmpty(address))
                _instance.PlaySfxInternal(address, loop, finishCallback);
        }

        public static void PreloadSoundAssets(List<string> addresses, Action finishAction = null)
        {
            int finishCount = 0;
            var needCount = addresses.Count;
            for (var i = 0; i < needCount; i++)
            {
                _instance.PreloadSoundAsset(addresses[i], () =>
                {
                    finishCount++;
                    if (finishCount == needCount)
                    {
                        finishAction?.Invoke();
                    }
                });
            }
        }
    
        public void PreloadSoundAsset(string address, Action finishAction)
        {
            AssetHelper.PrepareAsset<AudioClip>(address, (assetReference) =>
            {
                finishAction?.Invoke();
            });
        }
        
        public static float GetAudioLeftTimeToFinish(string address)
        {
            return _instance.GetAudioLeftTimeToFinishInternal(address);
        }

        public float GetAudioLeftTimeToFinishInternal(string address)
        {
            for (int i = 0; i < sfxAudioSourceList.Count; i++)
            {
                if (!sfxAudioSourceList[i].isPlaying && sfxAudioSourceList[i].clip.name == address)
                {
                    return sfxAudioSourceList[i].clip.length - sfxAudioSourceList[i].time;
                }
            }
            return 0;
        }
        
        protected void PlaySfxInternal(string address, bool loop = false, CancelableCallback finishCallback = null)
        {
           if (!PreferenceController.IsSoundEnabled()) 
               return;
 
            if (soundAssetsDict.ContainsKey(address))
            {
                AudioSource source = GetAudioSource();
                source.clip = soundAssetsDict[address].GetAsset<AudioClip>();
                source.loop = loop;
                source.Play();
                
                 
                if(finishCallback != null)
                    XUtility.WaitSeconds(source.clip.length, finishCallback);
            }
            else
            {
                AssetHelper.PrepareAsset<AudioClip>(address, (assetReference) =>
                {
                    AudioSource source = GetAudioSource();
                    source.loop = loop;
                    source.clip = assetReference.GetAsset<AudioClip>();
                    source.Play();
                     
                    if(finishCallback != null)
                        XUtility.WaitSeconds(source.clip.length, finishCallback);
                    
                    if (soundAssetsDict.ContainsKey(address))
                    {
                        assetReference.ReleaseOperation();
                    }
                    else
                    {
                        soundAssetsDict.Add(address, assetReference);
                    }
                });
            }
        }
        public static void StopSfx(string address)
        {
            _instance.StopSfxInternal(address);
        }
 
        public static void PlayButtonClick()
        {
            _instance.PlaySfxInternal("CashCrazy_Button_Click");
        }  
        
        public static void PlaySwitchFx()
        {
            _instance.PlaySfxInternal("General_SwitchWindow");
        }
        
        public static void PlayButtonClose()
        {
            _instance.PlaySfxInternal("CashCrazy_Button_Close");
        }
        
        public static void PlayPopUpClose()
        {
            _instance.PlaySfxInternal("CashCrazy_Dialog_Close");
        }

        public void StopSfxInternal(string address)
        {
           var audioSource = GetAudioSourceBySfx(address);
         
           if(audioSource != null)
                audioSource.Stop();
        }

        public AudioSource GetAudioSourceBySfx(string address)
        {
            for (int i = 0; i < sfxAudioSourceList.Count; i++)
            {
                //一般不会有名字包含的情况。
                if (sfxAudioSourceList[i].clip && sfxAudioSourceList[i].clip.name.Contains(address))
                {
                    return sfxAudioSourceList[i];
                }
            }
            
            return null;
        }
 
        private AudioSource GetAudioSource()
        {
            for (int i = 0; i < sfxAudioSourceList.Count; i++)
            {
                if (!sfxAudioSourceList[i].isPlaying)
                {
                    return sfxAudioSourceList[i];
                }
            }
            
            var source = soundContainer.AddComponent<AudioSource>();
            sfxAudioSourceList.Add(source);
            return source;
        }
         
        public void Clear()
        {
            if (bgAudioSource != null && bgAudioSource.isPlaying)
            {
                bgAudioSource.Stop();
            }
            
            musicSaveSpotList.Clear();

            for (int i = sfxAudioSourceList.Count - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(sfxAudioSourceList[i]);
                sfxAudioSourceList.RemoveAt(i);
            }
            sfxAudioSourceList.Clear();

            foreach (var item in soundAssetsDict)
            {
                item.Value.ReleaseOperation();
            }
            
            soundAssetsDict.Clear();
        }
 
        protected void OnMusicEnabled(EventMusicEnabled evt)
        {
            if (!evt.enabled)
            {
                musicMaxVolume = 0.00001f;
            
                if (bgAudioSource)
                {
                    bgAudioSource.volume = musicMaxVolume;
                }
            }
            else
            {
                musicMaxVolume = 1.0f;
            
                if (bgAudioSource)
                {
                    bgAudioSource.volume = musicMaxVolume;
                }
            }
        }
    }
}