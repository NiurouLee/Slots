// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/25/20:18
// Ver : 1.0.0
// Description : AudioConfig.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public interface IMachineAudioConfig
    {
        string GetBaseBackgroundMusicName();
        string GetFreeBackgroundMusicName();
        string GetLinkBackgroundMusicName();
        string GetBonusBackgroundMusicName();

        string GetSymbolWinAudioName();
        string GetSymbolWinStopAudioName();
    }
}