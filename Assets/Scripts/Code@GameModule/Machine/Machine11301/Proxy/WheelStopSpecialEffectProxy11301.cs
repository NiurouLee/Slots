using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;
using UnityEngine.Rendering;

namespace GameModule
{
	public class WheelStopSpecialEffectProxy11301 : WheelStopSpecialEffectProxy
	{
		public WheelStopSpecialEffectProxy11301(MachineContext machineContext)
		:base(machineContext)
		{

	
		}


		protected override async void HandleCustomLogic()
		{
			machineContext.view.Get<DoorView11301>().CloseLinkAnticipation();
			await machineContext.view.Get<DoorView11301>().OpenDoor();
			await machineContext.view.Get<LinkLockView11301>().RefreshReSpinCount(false);
			
			if(machineContext.state.Get<FreeSpinState>().IsOver && !machineContext.state.Get<ReSpinState11301>().IsInRespin && Constant11301.AllGoldList.Count != 0){
                List<GameObject> tempGoldArr = new List<GameObject>();
                foreach (var item in Constant11301.AllGoldList)
                {
                    tempGoldArr.Add(item);
                }
                Constant11301.AllGoldList = new List<GameObject>();
                GoldFlyWheel(tempGoldArr);
                await XUtility.WaitSeconds(0.1f);
            }
                
			base.HandleCustomLogic();
		}


		/// <summary>
        /// 金币飞向轮盘
        /// </summary>
        /// <returns></returns>
        private void GoldFlyWheel(List<GameObject> tempGoldArr)
        {
            //先获取收集数据，后面再刷新UI，避免，快速SPIN，导致，下一次的SPIN刷再当前SPIN上
	        var collectItems = machineContext.state.Get<ExtraState11301>().GetCollectItems();
            
	        var shopEntranceView = machineContext.view.Get<ShopEntranceView11301>();
            if (tempGoldArr.Count != 0)
            {
                AudioUtil.Instance.PlayAudioFx("Currency_Fly");
                for (int i = 0; i < tempGoldArr.Count; i++)
                {
                    var index = i;
                    var curGold = tempGoldArr[index];
                    curGold.transform.SetParent(shopEntranceView.transform);
                    curGold.GetComponent<SortingGroup>().sortingLayerName ="LocalFx";
                    curGold.transform.Find("Token").GetComponent<Animator>().Play("Token");
                    XUtility.FlyLocal(curGold.transform, curGold.transform.localPosition,
                        shopEntranceView.transform.Find("Box").transform.localPosition, 0,0.6f,-1,()=>{
                            GameObject.Destroy(curGold.gameObject);
                        });
                }
            }
            shopEntranceView.goldIsFlying = true;
            machineContext.WaitSeconds(0.6f,()=>{
                shopEntranceView.transform.GetComponent<Animator>().Play("Open");
                shopEntranceView.goldIsFlying = false;
                // 飞到目的地，更新宝箱
                machineContext.view.Get<ShopEntranceView11301>().RefreshTokenLimitValue(collectItems);
                machineContext.view.Get<ShopEntranceView11301>().SetBoxTokensNum(collectItems);
            });
			
            
        }



	}
}