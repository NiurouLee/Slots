using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;

namespace GameModule
{
    public class Constant11019
    {
	    
	    
	    public static int wheelDesignHeight = 844 + 30;
        
	    public static int jackpotPanelHeight = 86 * 3;
	    
	    public static readonly uint NormalLinkElementId = 5;

	    public static readonly uint WildElementId = 1;
	    
        public static readonly List<uint> ListBonusElementIds = new List<uint>()
        {
            13,14,15,16,17,18,19,20,21,22,23,24
        };
        
        public static readonly List<uint> ListBonusJackpotElementIds = new List<uint>()
        {
	        25,26,27,28
        };
        
        public static readonly List<uint> ListBonusAllElementIds = new List<uint>()
        {
	        13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28
        };
        
        
        public static readonly List<uint> ListLowLevelElementIds = new List<uint>()
        {
	        1,2,3,4,5,6,7,8,9,10
        };

        public static readonly uint ScatterElementId = 12;

        public static readonly string FreeBonusWinViewName = "FreeTotalBonusWinNotice";
        
        public static readonly string LinkBonusWinViewName = "LinkTotalBonusWinNotice";


        public static readonly List<int> listLinkUnlockPaperNeedCount = new List<int>()
        {
	        8,12,16,20
        };
        
        
        protected static readonly string[] _jackpotsAddress =
	        {"UIJackpotMini11019","UIJackpotMinor11019", "UIJackpotMajor11019","UIJackpotMega11019"};

        
        public static async Task ShowJackpot(MachineContext machineContext, LogicStepProxy logicStepProxy,
	        uint jackpotId,ulong jackpotPay,bool isAddBelence)
        {
	        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
	        machineContext.AddWaitTask(taskCompletionSource,null);

	        var assetAddress = _jackpotsAddress[jackpotId - 1];

	        AudioUtil.Instance.PlayAudioFx("Jackpot");
	        var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase>(assetAddress);
	        var jackpotWin = machineContext.state.Get<BetState>().GetPayWinChips(jackpotPay);
            
	        view.SetJackpotWinNum(jackpotWin);

	        view.SetPopUpCloseAction( async ()=>
	        {
		        machineContext.RemoveTask(taskCompletionSource);
		        if (isAddBelence)
		        {
			        logicStepProxy.AddWinChipsToControlPanel(jackpotWin);
			        float time = machineContext.view.Get<ControlPanel>().GetNumAnimationEndTime();
			        await machineContext.WaitSeconds(time);
		        }

		        taskCompletionSource.SetResult(true);
	        });

	        await taskCompletionSource.Task;
        }
        
        
        
        public static async Task BounsFly(MachineContext machineContext, LogicStepProxy logicStepProxy,BonusWinView11019 bonusWinView)
        {
	        
	        var wheel = machineContext.state.Get<WheelsActiveState11019>().GetRunningWheel()[0];
	   


	        var extraState = machineContext.state.Get<ExtraState11019>();
	        var activeState = machineContext.state.Get<WheelsActiveState11019>();
	        var endPos = bonusWinView.GetIntegralPos();
	        
	        int XCount = wheel.rollCount;
	        int YCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;

	        
	        for (int x = 0; x < XCount; x++)
	        {
		        for (int y = 0; y < YCount; y++)
		        {
			        var container = wheel.GetRoll(x).GetVisibleContainer(y);
			        
			        if (Constant11019.ListBonusElementIds.Contains(container.sequenceElement.config.id) ||
			            ListBonusJackpotElementIds.Contains(container.sequenceElement.config.id))
			        {
				        if (activeState.IsInLink)
				        {
					        int posId = x * YCount + y;
					        if (!extraState.LinkIsUnLock((uint)x))
					        {
						        //link中没解锁的不收集
						       continue; 
					        }
				        }

				        

				        

				        ulong chips = 0;

				        if (ListBonusJackpotElementIds.Contains(container.sequenceElement.config.id))
				        {
					        var item = extraState.GetLinkJackpotPay(x);
					        chips = machineContext.state.Get<BetState>().GetPayWinChips(item.JackpotPay);

					        await ShowJackpot(machineContext, logicStepProxy, item.JackpotId, item.JackpotPay, false);
				        }
				        else
				        {
					        ulong winRate = container.sequenceElement.config.GetExtra<ulong>("winRate");
					        chips = machineContext.state.Get<BetState>().GetPayWinChips(winRate);
				        }
				        
				        AudioUtil.Instance.PlayAudioFx("J01_Breathe");
				        container.PlayElementAnimation("Collect");
				        var objFly = machineContext.assetProvider.InstantiateGameObject("Active_Fly", true);
				        objFly.transform.parent = machineContext.transform;
				        var startPos = container.transform.position;
				        objFly.transform.position = startPos;
				        await XUtility.FlyAsync(objFly.transform, startPos, endPos, 0, 0.5f, Ease.Linear, machineContext);
				        machineContext.assetProvider.RecycleGameObject("Active_Fly", objFly);

		        
				        await bonusWinView.AddWin((long) chips);
			        }
		        }
		        
	        }
        }

    }
}