
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameModule
{
    public class WheelView11020 : TransformHolder
    {

        private List<uint> wheelData;
        private List<uint> bonusWheelData;

        private Transform wheelTrans;
        private Animator animatorWheel;
        private Animator animatorWin;

        private Transform bonusWheelTrans;

        private bool isSuperBonusGame;
        private bool firstRunning = false;

        private float timeDelta = -1.0f;

        public WheelView11020(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
            wheelData = new List<uint>(){2,6,3,4,3,5,6,10,7,8,2,9,2,10,5};

            bonusWheelData = new List<uint>(){1,15,14,13,12,11,10,9,8,7,6,5,4,3,2};
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            EnableUpdate();
        }

        public async void StartGame(Wheel wheel)
        {
            if (wheel.wheelName == Constant11020.baseWheelName)
            {
                wheelTrans = null;

                return;
            }

            firstRunning     = true;
            isSuperBonusGame = wheel.wheelName == Constant11020.superBonusWheelName;

            animatorWin = wheel.transform.Find("ep_WheelBaseGame_Win").GetComponent<Animator>();
            animatorWin.gameObject.SetActive(true);
            XUtility.PlayAnimation(animatorWin, "Idle");

            wheelTrans = wheel.transform.Find("WheelBonus");
            animatorWheel = wheelTrans.GetComponent<Animator>();

            for (var i = 1; i < 16; ++i)
            {
                wheelTrans.Find("Root/TurnTable/Point" + i + "/Value" + i).GetComponent<TextMesh>().text ="" + wheelData[i-1];
            }

            if (isSuperBonusGame)
            {
                bonusWheelTrans = wheel.transform.Find("WheelBonusSelect");

                var extraState = context.state.Get<ExtraState11020>();
                int index = extraState.GetRouletteWheelId();
                
                bonusWheelTrans.gameObject.SetActive(index > 0);
                wheelTrans.gameObject.SetActive(index == 0);
            }
            else
            {
                bonusWheelTrans = null;
                wheelTrans.gameObject.SetActive(true);
            }
        }

        public bool IsActive()
        {
            return wheelTrans != null;
        }

        public async void RunWheel()
        {
            if (!IsActive())
            {
                return;
            }
            timeDelta = 0;

            if (bonusWheelTrans != null)
            {
                bonusWheelTrans.gameObject.SetActive(false);
            }
            
            wheelTrans.gameObject.SetActive(true);

            // 开播第一段音效：蓄势加速
            AudioUtil.Instance.PlayAudioFx("Wheel_Start");

            await XUtility.PlayAnimationAsync(animatorWheel, "Start");
            
            // 停播第一段音效：蓄势加速
            AudioUtil.Instance.StopAudioFx("Wheel_Start");
            
            // 开播第二段：匀速
            AudioUtil.Instance.PlayAudioFx("Wheel_Loop", true);
        } 

        public async void StopWheel(Action finishAction, float delaySeconds)
        {   
            
            if (timeDelta < 0)
            {
                timeDelta = 0.0f;
            }

            float minTimeSeconds = 2.0f + delaySeconds;

            if (timeDelta < minTimeSeconds)
            {
                timeDelta = -1.0f;
                await context.WaitSeconds(minTimeSeconds-timeDelta);
            }
            else
            {
                timeDelta = -1.0f;
            }

            float targetAngle = GetTargetAngle();

            wheelTrans.Find("Root").localEulerAngles = new Vector3(0, 0, targetAngle);

            // 停播第二段：匀速
            AudioUtil.Instance.StopAudioFx("Wheel_Loop");
            
            ShowPointerFrame();
            
            
            // 开播第三段：减速
            AudioUtil.Instance.PlayAudioFx("Wheel_Finish");
            
            await XUtility.PlayAnimationAsync(animatorWheel, "Finish", context);
            
            //停播第三段：减速
            AudioUtil.Instance.StopAudioFx("Wheel_Finish");

            finishAction?.Invoke();
        } 

        public bool HasEnableSuperBonusWheel()
        {
            if (isSuperBonusGame)
            {
                var extraState = context.state.Get<ExtraState11020>();
                int index = extraState.GetRouletteWheelId();
                if (index > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task StartSuperBonusWheel()
        {
            if (!isSuperBonusGame)
            {
                await context.WaitSeconds(1.0f);

                return;
            }

            if (wheelTrans != null)
            {
                wheelTrans.gameObject.SetActive(false);
            }

            var extraState = context.state.Get<ExtraState11020>();
            int index = extraState.GetRouletteWheelId();
            if (index > 0)
            {
                firstRunning = false;

                bonusWheelTrans.gameObject.SetActive(true);

                var animator = bonusWheelTrans.GetComponent<Animator>();
                
                await context.WaitSeconds(2.0f);

                AudioUtil.Instance.PlayAudioFx("Wheel_Start");
                await XUtility.PlayAnimationAsync(animator, "Start", context);

                AudioUtil.Instance.PlayAudioFx("Wheel_Loop",true);

                await context.WaitSeconds(2.0f);
                
                float targetAngle = GetBonusTargetAngle();

                bonusWheelTrans.Find("Root").localEulerAngles = new Vector3(0, 0, targetAngle);

                ShowPointerFrame();
                AudioUtil.Instance.StopAudioFx("Wheel_Loop");
                AudioUtil.Instance.PlayAudioFx("Wheel_Finish");
                await XUtility.PlayAnimationAsync(animator, "Finish", context);
                AudioUtil.Instance.StopAudioFx("Wheel_Finish");

                await context.WaitSeconds(2.0f);
            }
            else
            {
                bonusWheelTrans.gameObject.SetActive(false);

                if (wheelTrans != null)
                {
                    wheelTrans.gameObject.SetActive(true);
                }
            }

            context.view.Get<LockedFramesView11020>().ShowSuperBonusLockedFrames();

            await context.WaitSeconds(1.5f);

            bonusWheelTrans.gameObject.SetActive(false);

            if (wheelTrans != null)
            {
                wheelTrans.gameObject.SetActive(true);
            }
        }

        private async void ShowPointerFrame(float time = 2.0f)
        {
            await context.WaitSeconds(time);
            AudioUtil.Instance.PlayAudioFx("Wheel_Frames_land");
            XUtility.PlayAnimation(animatorWin, "Open", ()=>{
                XUtility.PlayAnimation(animatorWin, "Idle");
            });
        }

        private float GetTargetAngle()
        {
            var extraState = context.state.Get<ExtraState11020>();

            uint count = (uint)extraState.GetNewFrameCount();
            
            int index = 0;

            for (var i = 0; i < wheelData.Count; ++i)
            {
                if (wheelData[i] == count)
                {
                    index = i;
                    break; 
                }
            }

            // Debug.Log("================================GetNewFrameCount: " + count + ", index: " + index);

            return 360 - index * (360/wheelData.Count);
        }

        private float GetBonusTargetAngle()
        {
            var extraState = context.state.Get<ExtraState11020>();

            int count = extraState.GetRouletteWheelId();
            
            int index = 0;

            for (var i = 0; i < bonusWheelData.Count; ++i)
            {
                if (bonusWheelData[i] == count)
                {
                    index = i;
                    break; 
                }
            }

            // Debug.Log("================================GetRouletteWheelId: " + count);

            return 360-index * (360/bonusWheelData.Count);
        }

        public override void Update()
        {
            if (timeDelta < 0)
            {
                return;
            }

            timeDelta += Time.deltaTime;
        }
    }
    
}
