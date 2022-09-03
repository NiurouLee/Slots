// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/25/15:27
// Ver : 1.0.0
// Description : SuperSpinXGetCard.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class Activity_SuperSpinWild:ActivityBase
    {
        public Activity_SuperSpinWild() 
            : base(ActivityType.SuperSpinGiveCard)
        {
            
        }

        public override void OnEnterLobby()
        {
            if (!activityPosterPopped)
            {
                EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(typeof(SuperSpinXAWildPosterPopup))));
            }
            
            base.OnEnterLobby();
        }

        /// <summary>
        /// 获取是否过期
        /// </summary>
        protected override bool IsExpired()
        {
            return false;
        }

        /// <summary>
        /// 获取是否解锁状态
        /// </summary>
        public override bool IsUnlockState()
        {
            return true;
        }
        
        public override void OnBannerJump(JumpInfo jumpInfo)
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), "SuperSpinXWildActivity")));
        }
    }
}