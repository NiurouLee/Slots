// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/09/18:17
// Ver : 1.0.0
// Description : MachineAudioConfig.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class MachineAudioConfig: IMachineAudioConfig
    {
        public string postFix;

        public MachineAudioConfig(string inPostFix)
        {
            postFix = inPostFix;
        }
        public virtual string GetBaseBackgroundMusicName()
        {
            return "Bg_BaseGame_" + postFix;
        }

        public virtual string GetFreeBackgroundMusicName()
        {
            return "Bg_FreeGame_" + postFix;
        }

        public virtual string GetLinkBackgroundMusicName()
        {
            return "Bg_LinkGame_" + postFix;
        }

        public string GetBonusBackgroundMusicName()
        {
            return "Bg_BonusGame_" + postFix;
        }

        public string GetSymbolWinAudioName()
        {
            return "Symbol_Win_" + postFix;
        }
        
        public string GetSymbolWinStopAudioName()
        {
            return "SymbolWin_Stop";
        }
        
        public virtual string GetSymbolWinStopAudioName(ulong winChips)
        {
            return "SymbolWin_Stop";
        }
    }
}