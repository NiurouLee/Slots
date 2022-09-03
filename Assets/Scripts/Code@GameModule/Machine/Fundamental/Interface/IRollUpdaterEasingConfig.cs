// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-22 8:43 PM
// Ver : 1.0.0
// Description : IRollUpdaterEasingConfig.cs
// ChangeLog :
// **********************************************
using System;
namespace GameModule
{
    public interface IRollUpdaterEasingConfig
    {
        Type GetUpdaterType();

        string GetReelStopSoundName();
    }
}