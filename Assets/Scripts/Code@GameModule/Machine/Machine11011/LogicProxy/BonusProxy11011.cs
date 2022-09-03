//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-19 15:52
//  Ver : 1.0.0
//  Description : BonusProxy11011.cs
//  ChangeLog :
//  **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using LitJson;
using UnityEngine;

namespace GameModule
{
    
    public class BonusProxy11011: BonusProxy
    {
        public BonusProxy11011(MachineContext context)
            : base(context)
        {
            
        }

        protected override async void HandleCustomLogic()
        {
            if (!IsFromMachineSetup())
            {
                AudioUtil.Instance.PauseMusic();
                var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                var triggerElementContainers = wheel.GetElementMatchFilter((container) =>
                {
                    if (Constant11011.IsLinkElement(container.sequenceElement.config.id))
                    {
                        return true;
                    }
                    return false;
                });
                for (int i = 0; i < triggerElementContainers.Count; i++)
                {
                    triggerElementContainers[i].PlayElementAnimation("Trigger");
                }
                AudioUtil.Instance.PlayAudioFx("B01_Trigger");
                await machineContext.WaitSeconds(3);

                var view = machineContext.view.Get<CollectBonusView11011>();
                view.ToggleVisible(true);
                
                var activeState = machineContext.state.Get<WheelsActiveState>();
                var runningWheel = activeState.GetRunningWheel()[0];
                
                long totalWin = 0;
                for (int i = 0; i < runningWheel.rollCount; i++)
                {
                    for (int j = 0; j < runningWheel.GetRoll(i).rowCount; j++)
                    {
                        var elementContainer = runningWheel.GetRoll(i).GetVisibleContainer(j);
                        if (Constant11011.IsLinkElement(elementContainer.sequenceElement.config.id))
                        {
                            elementContainer.PlayElementAnimation("Idle");
                            var element = elementContainer.GetElement() as Element11011;
                            var startPos = element.GetStartWorldPos();
                            totalWin += element.TotalWin;
                            var flyContainer = machineContext.assetProvider.InstantiateGameObject("FlyTxt",true);
                            flyContainer.GetComponent<TextMesh>().text = element.TotalWin.GetAbbreviationFormat();
                            flyContainer.transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
                            flyContainer.transform.SetParent(machineContext.transform,false);
                            var endPos = view.GetEndWorldPos();
                            await XUtility.FlyAsync(flyContainer.transform, startPos, endPos, 0, 0.5f);
                            machineContext.assetProvider.RecycleGameObject("FlyTxt",flyContainer);
                            view.UpdateCollectChips(totalWin);
                            AudioUtil.Instance.PlayAudioFx("B01_Collect");
                        }
                    }
                }
                await machineContext.WaitSeconds(0.5f);
                view.ToggleVisible(false);
            }

            AudioUtil.Instance.PlayAudioFx("SelectionFeature_Start");
            var popup = PopUpManager.Instance.ShowPopUp<UIFortuneChoose>();
            popup.Initialize(machineContext);
            popup.SetChooseCallback(ChooseCallback);
        }

        private async void ChooseCallback(int choice)
        {
            CBonusProcess cBonusProcess = new CBonusProcess();
            cBonusProcess.Json = "Link";
            await machineContext.state.Get<ExtraState11011>().SendBonusProcess(choice == 0 ? cBonusProcess : null);
            Proceed();
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            var extraState = machineContext.state.Get<ExtraState>();
            if (extraState != null && extraState.HasBonusGame())
            {
                return true;
            }
            return false;
        }
    }
}