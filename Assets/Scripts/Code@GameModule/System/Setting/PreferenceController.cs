// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/25/20:29
// Ver : 1.0.0
// Description : PreferenceController.cs
// ChangeLog :
// **********************************************

using System;
using UnityEngine;

namespace GameModule
{
    public class PreferenceController : LogicController
    {
        protected static LocalStorableInt musicEnableStatus = new LocalStorableInt("Preference_MusicEnableStatus", 1);
        protected static LocalStorableInt soundEnableStatus = new LocalStorableInt("Preference_SoundEnableStatus", 1);

        protected static LocalStorableInt jackpotNotificationEnableStatus =
            new LocalStorableInt("Preference_JackpotNotificationEnableStatus", 1);

        public PreferenceController(Client client)
            : base(client)
        {
        }

        public static bool IsMusicEnabled()
        {
            return musicEnableStatus.Value > 0;
        }


        protected override void Initialization()
        {
            base.Initialization();
            
            if (IsSoundEnabled())
            {
                Camera.main.gameObject.SendMessage("OnSoundEnabled");
            }
            else
            {
                Camera.main.gameObject.SendMessage("OnSoundDisabled");
            }
        }

        public static bool IsSoundEnabled()
        {
            return soundEnableStatus.Value > 0;
        }

        public static bool IsJackpotNotificationEnabled()
        {
            return jackpotNotificationEnableStatus.Value > 0;
        }

        public static void SetMusicEnabled(bool enabled)
        {
            musicEnableStatus.Value = enabled ? 1 : 0;
            
            EventBus.Dispatch(new EventMusicEnabled(enabled));
        }
 
        public static void SetSoundEnabled(bool enabled)
        {
            soundEnableStatus.Value = enabled ? 1 : 0;

            //Trick here
            //由于Code代码和Code@GameModule的代码都是打成DLL的方式的，没办法直接访问到Assembly-CSharp的代码，
            //这里通过GameObjectSendMessage的方式停止通过PlayAudioClip（策划直接挂在Prefab上的音效声音的播放
            if (enabled)
            {
                Camera.main.gameObject.SendMessage("OnSoundEnabled");
            }
            else
            {
                Camera.main.gameObject.SendMessage("OnSoundDisabled");
            }
             
            EventBus.Dispatch(new EventSoundEnabled(enabled));
        }

        public static void SetJackpotNotificationEnabled(bool enabled)
        {
            jackpotNotificationEnableStatus.Value = enabled ? 1 : 0;
            
            EventBus.Dispatch(new EventJackpotNotificationEnabled(enabled));
        }
    }
}