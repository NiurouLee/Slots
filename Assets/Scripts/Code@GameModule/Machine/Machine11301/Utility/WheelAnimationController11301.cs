using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelAnimationController11301: WheelAnimationController
    {

        protected DoorView11301 doorView;

        public MachineContext machineContext;
        public ShopEntranceView11301 shopEntranceView;
        public override void OnWheelStartSpinning()
        {
            base.OnWheelStartSpinning();
            Constant11301.AllGoldList = new List<GameObject>();
     
        }
        public override void OnRollStartBounceBack(int rollIndex)
        {
            XDebug.Log("创建金币-rollIndex:"+rollIndex);
            if(wheel.GetContext().state.Get<FreeSpinState>().IsOver)
                CreateGoldForWheel(rollIndex);
            base.OnRollStartBounceBack(rollIndex);
            
        }
        public override async void OnRollSpinningStopped(int rollIndex, Action rollLogicEnd)
        {
            
            machineContext = ViewManager.Instance.GetSceneView<MachineScene>().viewController.machineContext;
            if (doorView == null)
            {
                doorView = machineContext.view.Get<DoorView11301>();
            }
            
            shopEntranceView = machineContext.view.Get<ShopEntranceView11301>();
            var freeSpinState = machineContext.state.Get<FreeSpinState>();
            if (freeSpinState.IsInFreeSpin)
            {
                List<Task> listTask = new List<Task>();
                var roll = wheel.GetRoll(rollIndex);
                int count = roll.rowCount;
                for (int i = 0; i < count; i++)
                {
                    var container =  roll.GetVisibleContainer(i);
                    uint id = container.sequenceElement.config.id;
                    listTask.Add(doorView.RollingStopLockFreeDoor(rollIndex,i,id));
                }

                if (listTask.Count > 0)
                {
                    AudioUtil.Instance.PlayAudioFxOneShot("FreeSpin_DoorBlink");
                    await Task.WhenAll(listTask);
                }
            }

            
            
            base.OnRollSpinningStopped(rollIndex, rollLogicEnd);
        }

        public override void OnAllRollSpinningStopped(Action callback)
        {
            base.OnAllRollSpinningStopped(callback);
            var curExtraState = machineContext.state.Get<ExtraState11301>();
            var mapData = curExtraState.GetAttachItems();
            foreach (var item in mapData)
            {
                XDebug.Log("创建金币-AttachItems, Key:" + item.Key+" /value:"+item.Value);
            }
        }

        /// <summary>
        /// 创建金币
        /// </summary>
        /// <param name="rollIndex"></param>
        public void CreateGoldForWheel(int rollIndex)
        {
            if(shopEntranceView==null)
                shopEntranceView = wheel.GetContext().view.Get<ShopEntranceView11301>();
            
            var curExtraState = wheel.GetContext().state.Get<ExtraState11301>();
            var mapData = curExtraState.GetAttachItems();
            if (mapData != null || mapData.Count!=0)
            {
                foreach (var item in mapData)
                {
                    var idPos = (int)item.Key;
                    var goldNum = item.Value;
                    var posArr = Constant11301.ConversionAttachFlagsDataToPos(idPos,3);
                    if (posArr[0] == rollIndex)
                    {
                        var curSymbol = wheel.GetRoll(posArr[0]).GetVisibleContainer(posArr[1]);
                        XDebug.Log("创建金币==col"+posArr[0]+" /rol:"+posArr[1]+" /goldNum:"+goldNum+" /rollIndex:"+rollIndex);
                        var coin = wheel.GetContext().assetProvider.InstantiateGameObject("Tokens");
                        if(Constant11301.ListDoorElementIds.Contains(curSymbol.sequenceElement.config.id)){
                            var DoorGroupStatic = curSymbol.GetElement().transform.Find("DoorGroupStatic");
                            if (DoorGroupStatic != null)
                            {
                                coin.transform.SetParent(DoorGroupStatic.transform);
                            }
                        }else{
                            coin.transform.SetParent(curSymbol.transform);
                        }
                        coin.transform.localScale = Vector3.one;
                        coin.transform.localPosition = Constant11301.GoldLocalPos;
                        var sortGroup = coin.gameObject.AddComponent<SortingGroup>();
                        sortGroup.sortingLayerName = "Default";
                        sortGroup.sortingOrder = 5;
                        coin.transform.Find("Token/Text").GetComponent<MeshRenderer>().sortingLayerName ="Default";
                        coin.transform.Find("Token/Text").GetComponent<TextMesh>().text = ""+goldNum;
                        Constant11301.AllGoldList.Add(coin);
                    }
                }
            }
        }
        
    }
}