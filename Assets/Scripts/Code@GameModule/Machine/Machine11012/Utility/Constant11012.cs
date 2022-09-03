using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class Constant11012
    {
        
        public static readonly uint NormalLinkElementId = 5;
        
        public static readonly uint ScatterElementId = 10;
        
        public static readonly Dictionary<uint,string> DicBigElementId2Name = new Dictionary<uint,string>()
        {
            {1,"BigSymbol_S01"},
            {2,"BigSymbol_S02"},
            {3,"BigSymbol_S03"},
            {4,"BigSymbol_S04"},
            {14,"BigSymbol_W01"},
        };
        
        
        public static readonly Dictionary<uint,string> DicBigElementId2AudioName = new Dictionary<uint,string>()
        {
	        {1,"FreeSpin_MoneyTree"},
	        {2,"FreeSpin_Caishen"},
	        {3,"FreeSpin_Shou"},
	        {4,"FreeSpin_Heshang"},
	        {14,"FreeSpin_Wild"},
        };

        public static readonly List<uint> ListWildElementId = new List<uint>()
        {
            14,35
        };

        public static readonly List<uint> ListNormalCoins = new List<uint>()
        {
            11,15,16,17,18,19,20,21
        };
        
        public static readonly List<uint> ListNormalJackpotCoins = new List<uint>()
        {
            22,23,24,25
        };
        
        
        public static readonly List<uint> ListDoorCoins = new List<uint>()
        {
            36,37,38,39,40,41,42,43
        };
        
        public static readonly List<uint> ListDoorJackpotCoins = new List<uint>()
        {
            44,45,46,47
        };
        
        
        public static readonly List<uint> ListDoorElementIds = new List<uint>()
        {
            26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47
        };
        
        
        public static readonly Dictionary<uint,uint> ListDoorToNoElementIds = new Dictionary<uint,uint>()
        {
            {26,1},
            {27,2},
            {28,3},
            {29,4},
            {30,5},
            {31,6},
            {32,7},
            {33,8},
            {34,9},
            {35,14},
            {36,11},
            {37,15},
            {38,16},
            {39,17},
            {40,18},
            {41,19},
            {42,20},
            {43,21},
            {44,22},
            {45,23},
            {46,24},
            {47,25},
        };




        public static async Task PlayDoorAnim(GameObject container,MachineContext context,string stateName)
        {
            Transform tranDoorGroup = container.transform;
            if (tranDoorGroup != null)
            {
                Animator animatorDoor = tranDoorGroup.GetComponent<Animator>();
                await XUtility.PlayAnimationAsync(animatorDoor, stateName, context);
            }
        }
        
        
        public static async Task PlayLockAnim(GameObject container,MachineContext context,string stateName)
        {
            Transform tranDoorGroup = container.transform;
            if (tranDoorGroup != null)
            {
                Animator animatorLock = tranDoorGroup.Find("LockScale/Lock").GetComponent<Animator>();
                animatorLock.gameObject.SetActive(true);
                await XUtility.PlayAnimationAsync(animatorLock, stateName, context);
            }
        }
        
        
         
        
        
        
        
        public static async Task OpenDoor(GameObject container,MachineContext context)
        {
            await PlayDoorAnim(container, context, "Open");
        }
        
        public static async Task OpenBaseDoor(GameObject container,MachineContext context)
        {
	        
	        await PlayDoorAnim(container, context, "Open");
        }
        
        public static async Task CloseDoor(GameObject container,MachineContext context)
        {
            await PlayDoorAnim(container, context, "Close");
        }
        
        
        
        public static async Task OpenLockDoor(GameObject container,MachineContext context)
        {
            OpenLock(container, context);
            //await OpenDoor(container, context);
            await PlayDoorAnim(container, context, "FreeOpen");
        }
        
        
        public static async Task CloseLockDoor(GameObject container,MachineContext context)
        {
            CloseLock(container, context);
            await CloseDoor(container, context);
        }
        
        
        public static async Task OpenLock(GameObject container,MachineContext context)
        {
            await PlayLockAnim(container, context, "LockOpen");
        }


        public static async Task CloseLock(GameObject container,MachineContext context)
        {
            await PlayLockAnim(container, context,"LockClose");
        }

        public static async Task DoorIdle(GameObject container,MachineContext context)
        {
            await PlayDoorAnim(container, context, "Idle");
        }
        
        
        public static async Task LockDoorIdle(GameObject container,MachineContext context)
        {
            PlayLockAnim(container, context, "IdleClose");
            await PlayDoorAnim(container, context, "Idle");
        }


        public static async Task PlayOpenDoorAudio()
        {
	        await AudioUtil.Instance.PlayAudioFxAsync("FreeSpin_DoorOpen");
        }
        
        
        public static async Task PlayCloseDoorAudio()
        {
	        await AudioUtil.Instance.PlayAudioFxAsync("FreeSpin_DoorClose");
        }
        
      
        



        protected static readonly string[] _jackpotsAddress =
	        {"UIJackpotMini11012","UIJackpotMinor11012", "UIJackpotMajor11012","UIJackpotMega11012","UIJackpotGrand11012"};

        
        public static async Task ShowJackpot(MachineContext machineContext, LogicStepProxy logicStepProxy,
	        uint jackpotId,ulong jackpotPay,bool isAddBelence)
        {
	        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
	        machineContext.AddWaitTask(taskCompletionSource,null);

	        var assetAddress = _jackpotsAddress[jackpotId - 1];

	        AudioUtil.Instance.PlayAudioFx("Link_Jackpot");
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
        
        
        
        public static async Task BounsFly(MachineContext machineContext, LogicStepProxy logicStepProxy,BonusWinView11012 bonusWinView)
        {
	        
	        var wheel = machineContext.state.Get<WheelsActiveState11012>().GetRunningWheel()[0];
	   


	        var extraState = machineContext.state.Get<ExtraState11012>();
	        var activeState = machineContext.state.Get<WheelsActiveState11012>();
	        var endPos = bonusWinView.GetIntegralPos();
	        
	        int XCount = wheel.rollCount;
	        int YCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;

	        
	        for (int x = 0; x < XCount; x++)
	        {
		        for (int y = 0; y < YCount; y++)
		        {
			        var container = wheel.GetRoll(x).GetVisibleContainer(y);
			        uint elementId = container.sequenceElement.config.id;
			        if (Constant11012.ListNormalCoins.Contains(elementId) ||
			            Constant11012.ListNormalJackpotCoins.Contains(elementId) ||
			            ListDoorCoins.Contains(elementId) ||
			            Constant11012.ListDoorJackpotCoins.Contains(elementId))
			        {
				       

				        
				        ulong chips = 0;

				        if (Constant11012.ListNormalJackpotCoins.Contains(container.sequenceElement.config.id) ||
				            Constant11012.ListDoorJackpotCoins.Contains(container.sequenceElement.config.id))
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
				        
				        
				        AudioUtil.Instance.PlayAudioFx("Link_J01_Fly");
				        container.PlayElementAnimation("Collect");

				        var objFly = machineContext.assetProvider.InstantiateGameObject("BonusFly", true);
				        objFly.transform.parent = machineContext.transform;
				        var startPos = container.transform.position;
				        objFly.transform.position = startPos;
				        await XUtility.FlyAsync(objFly.transform, startPos, endPos, 0, 0.333f, Ease.Linear, machineContext);
				        machineContext.assetProvider.RecycleGameObject("BonusFly", objFly);


		        
				        await bonusWinView.AddWin((long) chips);
			        }
		        }
		        
	        }
        }


        public static void ClearDoor(MachineContext context)
        {
	        var wheel = context.state.Get<WheelsActiveState11012>().GetRunningWheel()[0];
	        var elementConfigSet = context.state.machineConfig.elementConfigSet;
                
	        List<Task> listTask = new List<Task>();
	        int XCount = wheel.rollCount;
	        int YCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;
	        int indexDoor = 0;
	        for (int x = 0; x < XCount; x++)
	        {
		        for (int y = 0; y < YCount; y++)
		        {
			        var container = wheel.GetRoll(x).GetVisibleContainer(y);
			        if (Constant11012.ListDoorElementIds.Contains(container.sequenceElement.config.id))
			        {
				        uint newId = Constant11012.ListDoorToNoElementIds[container.sequenceElement.config.id];
				        var elementConfig = elementConfigSet.GetElementConfig(newId);
				        container.UpdateElement(new SequenceElement(elementConfig, context));
			        }
		        }
	        }
        }

    }
}