// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/09/15:15
// Ver : 1.0.0
// Description : LevelRushFailPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule 
{
    [AssetAddress("UILevelRushFail", "UILevelRushFailV")]
    public class LevelRushFailPopup : Popup
    {
        public LevelRushFailPopup(string address)
            : base(address)
        {
            
        }

        public override void OnOpen()
        {
            base.OnOpen();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLevelrushFail);
        }
    }
}