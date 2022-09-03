using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    
    [AssetAddress("UITreasureRaidBoosterAd")]
    public class TreasureRaidBoosterAdPopup : TreasureRaidBoosterPopup
    {
        public TreasureRaidBoosterAdPopup(string address) : base(address)
        {
            contentDesignSize = new Vector2(1465, 768);
        }
    }
}