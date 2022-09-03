using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class UICrazeSmashGame_Silver : UICrazeSmashGame
    {
        protected override string GetEggPrefabPath()
        {
            return "PrefabRoot/CrazeSmashGameSilverEggCell";
        }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            if (eggs == null || eggs.Length == 0) { return; }
            for (int i = 0; i < eggs.Length; i++)
            {
                var index = i;
                var egg = eggs[i];
                egg.SetOnClick(async () =>
               {
                  
                   ViewManager.Instance.BlockingUserClick(true, "CrazeSmash_Silver");
                 
                   var smashEgg = await crazeSmashController.SendCSmashEgg(false, (uint)index);

                   if (smashEgg == null)
                   {
                       return;
                   }
 
                   var eggInfo = crazeSmashController.eggInfo;
                   var isOver = eggInfo.SilverOver;
 
                   var eggData = eggInfo.SilverEggs[index];
                  
                   if (eggData.Open)
                   {
                       string rewardType = "1";
                       await egg.Smash(eggData);
                      
                       if (eggData.Win)
                       {
                           rewardType = "3";
                           await PopupStack.ShowPopup<UICrazeSmashFinishRewardPopup>(argument:smashEgg.Rewards[0].Items);
                       }
                       else
                       {
                           if (smashEgg.Rewards.count > 0)
                           {
                               var rewardPopup = await PopupStack.ShowPopup<UICommonGetRewardPopup>();
                               rewardPopup.viewController.SetUpReward(smashEgg.Rewards,"SmashEgg");
                           }
                       }

                       var time = APIManager.Instance.GetServerTime();
                       var userController = Client.Get<UserController>();
                       var level = userController.GetUserLevel();
                       var remains = eggInfo.SilverHammer;
                   
                       BiManagerGameModule.Instance.SendGameEvent(
                           BiEventFortuneX.Types.GameEventType.GameEventCrazeSmashWin,
                           ("time", $"{time}"),
                           ("level", $"{level}"),
                           ("remains", $"{remains}"),
                           ("reward type", $"{rewardType}")
                       );
                   }

                   SetItems(true);
                  
                   await viewController.WaitForSeconds(1.0f);

                   if (isOver)
                   {
                       EventBus.Dispatch(new Event_CrazeSmash_GameFinish());
                   }
                   
                   ViewManager.Instance.BlockingUserClick(false, "CrazeSmash_Silver");
               });
            }
        }

        public void SetEggs()
        {
            if (crazeSmashController.eggInfo == null)
            {
                return;
            }
            
            var eggInfo = crazeSmashController.eggInfo;
            SetEggs(eggInfo.SilverEggs);
        }

        public void SetItems(bool animateCoin = false)
        {
            if (crazeSmashController.eggInfo == null)
            {
                return;
            }

            var eggInfo = crazeSmashController.eggInfo;
            
            SetItems(eggInfo.SilverTotalFinalReward?.Items, animateCoin);
        }

        public void SetNotice()
        {
            if (crazeSmashController.eggInfo == null)
            {
                return;
            }
            
            var eggInfo = crazeSmashController.eggInfo;
            SetNotice(eggInfo.SilverHammer);
        }

        public void Set()
        {
            SetEggs();
            SetNotice();
            SetItems();
        }

        public void Refresh()
        {
            SetNotice();
        }
    }
}