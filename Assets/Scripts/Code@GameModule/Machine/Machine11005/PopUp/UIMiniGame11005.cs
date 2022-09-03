using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIMiniGame11005: MachinePopUp
    {

        protected List<RectTransform> listPoint = new List<RectTransform>();


        [ComponentBinder("PlayFlag")]
        protected RectTransform tranPlayer;

        [ComponentBinder("IntegralGroup")]
        protected RectTransform tranIntegralGroup;

        [ComponentBinder("IntegralText")]
        protected Text txtIntegral;

        [ComponentBinder("BG")]
        protected RectTransform tranBG;

        [ComponentBinder("Root","PlayFlag")]
        protected Animator animatorPlayer;

        protected Animator animator;
        
        public UIMiniGame11005(Transform transform) : base(transform)
        {
            for (int i = 1; i <= 100; i++)
            {
                listPoint.Add(transform.Find($"Root/BG/PositionGroup/Point{i}") as RectTransform);
            }
            animator = transform.GetComponent<Animator>();
            //animator.enabled = false;
            //tranIntegralGroup.gameObject.SetActive(false);
        }

        protected ExtraState11005 extraState;
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            extraState = context.state.Get<ExtraState11005>();

            
        }

        public override void OnOpen()
        {
            base.OnOpen();
            PopUpManager.Instance.SetGrayMaskState(false);

        }


        private float offsetY = 160f;
        public float timeJump = 0.466f;

        private TaskCompletionSource<bool> _taskCompletionSource;
        public async Task RefreshUI()
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();
            StartRefreshUI(true);
            await _taskCompletionSource.Task;
        }

        protected Vector3 showPos = new Vector3(0, -1136, 0);
        protected async Task StartRefreshUI(bool isStart)
        {
            var mapInfo = extraState.GetMapInfo();
            txtIntegral.SetText(mapInfo.JackpotWin.GetCommaFormat());

            if (!mapInfo.Rolled)
            {
                if (isStart)
                {
                    var tranOld = listPoint[(int) mapInfo.Step];
                    // tranPlayer.position = tranOld.position;
                    // await XUtility.WaitNFrame(1);
                    //tranPlayer.position = tranOld.position;
                    int rowOld = (int) (mapInfo.Step / 10f);
                    
                    if (mapInfo.Step == 0)
                    {
                        tranBG.localPosition = showPos;
                    }
                    else
                    {
                        await BGMove(rowOld, rowOld,timeJump);
                    }
                    
                    

                    //等界面打开动画
                    XUtility.PlayAnimation(animator, "OpenView", null, context);
                    
                    await context.WaitNFrame(1);
                    tranPlayer.localPosition = tranOld.localPosition;
                    await context.WaitSeconds(0.5f);
                    
                    
                    if (mapInfo.Step == 0)
                    {
                       //await XUtility.PlayAnimationAsync(animator, "Normal");
                       
                       await context.WaitSeconds(1);
                       await XUtility.FlyLocalAsync(tranBG, showPos, Vector3.zero, 0, 3,-1,Ease.Linear,context);
                    }
                }

                await context.WaitSeconds(1);
                
                var popUp = PopUpManager.Instance.ShowPopUp<UIWheelBonus11005>("UIWheelBonus11005");
                popUp.SetPopUpCloseAction(() =>
                {
                    var mapInfoBonus = extraState.GetMapInfo();
                    RunPlayer(mapInfoBonus);
                });
            }
            else
            {
                //已经转过了，客户端恢复一次步数
                if (isStart)
                {
                    var popUp = PopUpManager.Instance.ShowPopUp<UIWheelBonus11005>("UIWheelBonus11005");
                    await popUp.StopWheelBonus();
                    
                    var tranOld = listPoint[(int)mapInfo.StepOld];
                    int rowOld = (int)(mapInfo.StepOld / 10f);
                    await BGMove(rowOld,rowOld,timeJump);
                    
                    XUtility.PlayAnimation(animator, "OpenView", null, context);
                    
                    await context.WaitNFrame(1);
                    tranPlayer.position = tranOld.position;
                    
                    await context.WaitSeconds(0.5f);
                    
                }

                await context.WaitSeconds(2);
                
                
                RunPlayer(mapInfo);
            }

            

        }

        protected async void RunPlayer(GoldMineGameResultExtraInfo.Types.Adventure mapInfo)
        {
            uint countStep = mapInfo.NumberRolled;
            uint moveStepId = mapInfo.StepOld + 1;
            
            for (int i = 0; i < countStep; i++)
            {
                int rowOld = (int)((moveStepId-1) / 10f);
                await PlayerMove(moveStepId-1,moveStepId, rowOld);

                moveStepId++;
                if (moveStepId > 99)
                {
                    break;
                }
            }

            
            //大跳
            if (moveStepId - 1 != mapInfo.Step)
            {
                int oldMove = (int)((mapInfo.StepOld + mapInfo.NumberRolled) / 10f);
                await PlayerMove(moveStepId-1,mapInfo.Step, oldMove);
            }

            await extraState.SendBonusProcess();
            var mapInfoProcess = extraState.GetMapInfo();

            if (mapInfoProcess.StepWin > 0)
            {

                AudioUtil.Instance.StopMusic();
                //AudioUtil.Instance.StopAudioFx(context.machineConfig.audioConfig.GetBonusBackgroundMusicName());
                
                if (mapInfo.Step == 99)
                {
                    AudioUtil.Instance.PlayAudioFx("BonusComplete");
                    await XUtility.PlayAnimationAsync(animatorPlayer, "Oversized",context);
                    XUtility.PlayAnimation(animator, "Collect", null, context);
                    var popUpFinish = PopUpManager.Instance.ShowPopUp<UIMiniGameFinialReward11005>("UIMiniGameFinialReward11005");
                    await popUpFinish.RefresUI();
                }
                else
                {
                    AudioUtil.Instance.PlayAudioFx("BonusJump_Win");
                    await XUtility.PlayAnimationAsync(animatorPlayer, "Trigger",context);
                    var popUp = PopUpManager.Instance.ShowPopUp<UIMiniGameReward11005>("UIMiniGameTargetSymbolReward11005");
                    await popUp.RefresUI((int)mapInfoProcess.StepWin);
                }
            }

            if (mapInfoProcess.StepPlus>0)
            {
                AudioUtil.Instance.PlayAudioFx("BonusJump_Win");
                await XUtility.PlayAnimationAsync(animatorPlayer, "Trigger",context);
                var popUp = PopUpManager.Instance.ShowPopUp<UIMiniGameOneMoreSpin11005>("UIMiniGameOneMoreSpin11005");
                await popUp.RefresUI();
                
                await StartRefreshUI(false);
            }
            else
            {
                AudioUtil.Instance.StopMusic();
            
                await context.WaitSeconds(1);
                _taskCompletionSource.SetResult(true);
                //AudioUtil.Instance.PlayMusic(context.machineConfig.audioConfig.GetBaseBackgroundMusicName());
                Close();
            }

        }

        protected async Task PlayerMove(uint nowStepId,uint moveStepId,int rowOld)
        {
            int rowMove = (int)(moveStepId / 10f);
            
            //最后两行不下沉，占满屏幕
            rowOld = Math.Min(7, rowOld);
            rowMove = Math.Min(7, rowMove);
            
            if (rowMove != rowOld)
            {

                if (nowStepId == 54)//火箭
                {
                    AudioUtil.Instance.PlayAudioFx("BonusRocket");
                    XUtility.PlayAnimation(animatorPlayer, "Rocket",null, context);
                    await context.WaitSeconds(0.15f);
                    BGMove(rowOld,rowMove,1.85f);
                    await XUtility.FlyLocalAsync(tranPlayer, tranPlayer.localPosition, listPoint[(int)moveStepId].localPosition, 0,1.85f,-1,Ease.Linear,context);
                    return;
                }


                int rowMin = rowOld - rowMove;
                if (rowMin >= 8)
                {
                    //长滑梯
                    AudioUtil.Instance.PlayAudioFx("BonusFall");
                    BGMove(rowOld,rowMove,3);
                    XUtility.PlayAnimation(animatorPlayer, "Slide2",null, context);
                    await XUtility.FlyLocalAsync(tranPlayer, tranPlayer.localPosition, listPoint[(int)moveStepId].localPosition, 0,3,-1,Ease.Linear,context);
                    return;
                    
                }
                else if(rowMin >= 1)
                {
                    //短滑梯
                    AudioUtil.Instance.PlayAudioFx("BonusFall");
                    BGMove(rowOld,rowMove,2);
                    XUtility.PlayAnimation(animatorPlayer, "Slide",null, context);
                    await XUtility.FlyLocalAsync(tranPlayer, tranPlayer.localPosition, listPoint[(int)moveStepId].localPosition, 0,2,-1,Ease.Linear,context);
                    return;
                }
                else if(rowMin < -1 || moveStepId == 78)
                {
                    //梯子
                    AudioUtil.Instance.PlayAudioFx("BonusRise");
                    BGMove(rowOld,rowMove,2.333f);
                    XUtility.PlayAnimation(animatorPlayer, "ladder",null, context);
                    await XUtility.FlyLocalAsync(tranPlayer, tranPlayer.localPosition, listPoint[(int)moveStepId].localPosition, 0,2.333f,-1,Ease.Linear,context);
                    return;
                }
                else
                {
                    BGMove(rowOld,rowMove,timeJump);
                }
            }
            
            
            AudioUtil.Instance.PlayAudioFx("BonusJump");
            XUtility.PlayAnimation(animatorPlayer, "Beat",null, context);
            await XUtility.FlyLocalAsync(tranPlayer, tranPlayer.localPosition, listPoint[(int)moveStepId].localPosition, 0,timeJump,-1,Ease.Linear,context);
            
            
        }


        protected async Task BGMove(int rowOld,int rowNew,float time)
        {
            var posOld = tranBG.localPosition;

            //最后两行不下沉，占满屏幕
            rowOld = Math.Min(7, rowOld);
            rowNew = Math.Min(7, rowNew);
            
            if (rowOld == rowNew)
            {
                posOld.y -= rowOld * offsetY;
                tranBG.localPosition = posOld;
            }
            else
            { posOld.y -= (rowNew - rowOld) * offsetY;
               await XUtility.FlyLocalAsync(tranBG, tranBG.localPosition, posOld, 0, time,-1,Ease.Linear,context);
            }

        }




    }
}