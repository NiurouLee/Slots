//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-20 17:17
//  Ver : 1.0.0
//  Description : WheelStopSpecialEffectProxy11011.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11011: WheelStopSpecialEffectProxy
    {
        public WheelStopSpecialEffectProxy11011(MachineContext machineContext) : base(machineContext)
        {
            
        }

        protected override async void HandleCustomLogic()
        {
                var isFreeSpin = machineContext.state.Get<FreeSpinState>().IsInFreeSpin;
                if (!isFreeSpin)
                {
                    Proceed();
                    return;
                }
                
                var runningWheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                var winLines = runningWheel.wheelState.GetJackpotWinLines();
                for (int i = 0; i < winLines.Count; i++)
                {
                    var winLine = winLines[i];
                    if (winLine.JackpotId >= 4)
                    {
                        TaskCompletionSource<bool> taskGrandWin = new TaskCompletionSource<bool>();
                        var grandWinChips = machineContext.state.Get<BetState>().GetPayWinChips(winLine.Pay);
                        var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase>(Constant11011.GetJackpotAddress(3));
                        view.SetJackpotWinNum((ulong)grandWinChips);
                        view.SetPopUpCloseAction(async () =>
                        {
                            taskGrandWin.SetResult(true);
                        });
                        await taskGrandWin.Task;
                        AddWinChipsToControlPanel((ulong)grandWinChips, 1f);
                        await machineContext.WaitSeconds(1.5f);
                        break;      
                    }
                }
                winLines = runningWheel.wheelState.GetBonusWinLine();
                if (winLines.Count>0 && winLines[winLines.Count-1].BonusGameId == 3)
                {
                    var lastWinRate = machineContext.state.Get<FreeSpinState11011>().LastTotalWinRate;
                    var deltaWinRate = machineContext.state.Get<ExtraState11011>().FreeGameWinRate;
                    var winLine = winLines[winLines.Count - 1];
                    for (int i = 0; i < winLine.Positions.count; i++)
                    {
                        var position = winLine.Positions[i];
                        var elementContainer = runningWheel.GetRoll((int) position.X).GetVisibleContainer((int)position.Y);

                        var endPos = machineContext.view.Get<FeatureView11011>(1).GetFlyEndPos();
                        
                        var flyName = "FreeGameB02Fly";
                        elementContainer.PlayElementAnimation("CollectFree");
                        var element = elementContainer.GetElement() as Element11011;
                        var flyContainer = GetFlyContainer(flyName, element);
                        XUtility.PlayAnimation(flyContainer.GetComponent<Animator>(), "Fly");
                        await machineContext.WaitSeconds(0.67f);
                        await XUtility.FlyLocalAsync(flyContainer.transform, element.GetStartWorldPos(), endPos, 0, 0.34f, 0.3f);
                        lastWinRate += deltaWinRate;
                        AudioUtil.Instance.PlayAudioFx("Free_Feedback");
                        machineContext.view.Get<FeatureView11011>(1).PlayCollect();
                        machineContext.view.Get<FeatureView11011>(1).UpdateNextWin(lastWinRate);
                        machineContext.assetProvider.RecycleGameObject(flyName,flyContainer.gameObject);  
                        await machineContext.WaitSeconds(0.2f);
                    }
                    await machineContext.WaitSeconds(0.4f);
                    var freeGameWinRateTotal = machineContext.state.Get<ExtraState11011>().FreeGameCoinTotalWinRate;
                    machineContext.state.Get<FreeSpinState11011>().LastTotalWinRate = freeGameWinRateTotal;
                }
                Proceed();
        }

        private Transform GetFlyContainer(string flyName, Element11011 element)
        {
            var startPos = element.GetStartWorldPos();
            var flyContainer = machineContext.assetProvider.InstantiateGameObject(flyName,true);
            flyContainer.transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
            flyContainer.transform.SetParent(machineContext.transform,false);
            if (flyContainer.GetComponent<Animator>())
            {
                XUtility.PlayAnimation(flyContainer.GetComponent<Animator>(), "Fly");
            }
            return flyContainer.transform;
        }
    }
}