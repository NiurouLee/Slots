// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/06/10:27
// Ver : 1.0.0
// Description : ISequenceElementConstructor.cs
// ChangeLog :
// **********************************************
using System.Collections.Generic;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
namespace GameModule
{
    public interface ISequenceElementConstructor
    {
        void ConstructReelSequenceFromServerData(ElementConfigSet elementConfigSet, MapField<string, Reels> reelsMap);

        List<ReelSequence> GetReelSequences(string reelName);

        List<ReelSequence> ConstructSpinResultReelSequence(WheelState wheelState,
            DragonU3DSDK.Network.API.ILProtocol.Panel panel);

        Dictionary<int, Dictionary<int, SequenceElement>> ConstructReelSubstituteInfo(
            RepeatedField<Panel.Types.ReelSubstitute> substitutes);
    }
}