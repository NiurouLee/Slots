// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2020-12-01 9:04 PM
// Ver : 1.0.0
// Description : FilterId.cs
// ChangeLog :
// **********************************************


namespace GameModule
{
    public class FilterId
    {
        //玩家是新用户
        public const int FILTER_NEW_USER = 1000;
        
        //玩家是付费用户
        public const int FILTER_IAP_USER = 2000;
        //玩家还没购买首充
        public const int FILTER_HAVE_ONE_TIMER_OFFER = 3000;
        //玩家当前 有Deal Offer可以购买
        public const int FILTER_HAVE_DEAL_OFFER = 3001;
        //玩家关闭主商城没有购买商品
        public const int FILTER_NOT_PAID_IN_MAIN_STORE = 4001;

        //有cashback活动
        public const int FILTER_HAVE_CASH_BACK = 5000;
        //玩家当前有RewardSpin未完成
        public const int FILTER_HAVE_UNFINISHED_REWARD_SPIN = 5001;

        //VegasPass有可以claim的奖励
        public const int FILTER_VEGASPASS_HAVE_CLAIM = 5002;
        //VegasPass购买可以立即获得的奖励
        public const int FILTER_VEGASPASS_BUY_INSTANT_REWARD = 5003;
        //首充处于StageOne
        public const int FILTER_FIRST_SHOP_IN_STAGE_ONE = 5004;
      
        //首充没有处于StageOne
        public const int FILTER_FIRST_SHOP_NOT_IN_STAGE_ONE = 5005;
        
        //距离首充登录弹出是否过来两小时冷却时间
        public const int FILTER_FIRST_SHOP_LOGIN_COLLING_END = 5006;

        public const int FILTER_FIRST_SHOP_ENABLED = 5007;
        public const int FILTER_FIRST_SHOP_NOT_ENABLED = 5008;

        public const int FILTER_SHOP_GIFT_PACK = 5009;
    }
}
