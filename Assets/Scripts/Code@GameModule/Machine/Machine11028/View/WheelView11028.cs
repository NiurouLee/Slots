//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-12 14:13
//  Ver : 1.0.0
//  Description : WheelView11028.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class WheelView11028:TransformHolder
    {
        private Animator animator;
        [ComponentBinder("Root/MainGroup")]
        private Transform mainGroup;
        [ComponentBinder("Root/CenterGroup/WheelBonusButton")]
        private Button btnSpin;
        
        public Action wheelEndAction;
        

        private List<Text> listTxtLabels;
        public float anglePerFan = 0;
        private bool normalWheel;
        
        public WheelView11028(Transform inTransform,bool isNormalWheel):base(inTransform)
        {
            normalWheel = isNormalWheel;
            ComponentBinder.BindingComponent(this,inTransform);
            animator = transform.GetComponent<Animator>();
            InitWheelUI();
        }

        private void InitWheelUI()
        {
            if (btnSpin)
            {
                btnSpin.onClick.AddListener(OnSpinClicked);   
            }
            listTxtLabels = new List<Text>();
            for (int i = 0; i < 16; i++)
            {
                var transformCell = base.transform.Find($"Root/MainGroup/AnimControl/Cell{i + 1}/CountText");
                if (transformCell)
                {
                    listTxtLabels.Add(transformCell.gameObject.GetComponent<Text>());
                }
            }   
            PlayAnimation("Idle");
        }

        public void EnableButton(bool enabled = true)
        {
            if (btnSpin)
            {
                btnSpin.interactable = enabled;   
            }
        }

        public virtual void InitializeWheelView()
        {
            EnableButton();
            var dataList = normalWheel
                ? context.state.Get<ExtraState11028>().GetNormalWheelData()
                : context.state.Get<ExtraState11028>().GetMultiplierWheelData();
            anglePerFan = 360/ (float) dataList.Count;
            for (int i = 0; i < listTxtLabels.Count; i++)
            {
                if (listTxtLabels[i])
                {
                    listTxtLabels[i].text = dataList[i].ToString();
                }
            }
        }

        private void OnSpinClicked()
        {
            OnStartSpin();
            AudioUtil.Instance.PlayAudioFx("Wheel_Spin");
        }

        public async Task OnStartSpin()
        {
            AudioUtil.Instance.PlayAudioFx(normalWheel ? "Wheel_Turn" : "BonusWheel_Turn");
            EnableButton(false);
            PlayAnimation("Start");
            await SendBonusProcess();
            await context.WaitSeconds(2f);
            await StopWheel(GetWheelHitIndex());
            AudioUtil.Instance.PlayAudioFx(normalWheel ? "Wheel_TurnStop" : "BonusWheel_TurnStop");
            await ShowWheelWinAsync();
            await context.WaitSeconds(1);
            wheelEndAction?.Invoke();
        }

        private uint GetWheelHitIndex()
        {
            return normalWheel
                ? context.state.Get<ExtraState11028>().NormalWheelIndex
                : context.state.Get<ExtraState11028>().MultiplierWheelIndex;
        }

        public async Task PlayAnimationAsync(string animName)
        {
            await XUtility.PlayAnimationAsync(animator,animName);
        }
        public void PlayAnimation(string animName)
        {
            XUtility.PlayAnimation(animator,animName);
        }

        public async Task SendBonusProcess()
        {
            CBonusProcess cBonusProcess = new CBonusProcess();
            cBonusProcess.Json = "Night";
            await context.state.Get<ExtraState11028>().SendBonusProcess(context.state.Get<ExtraState11028>().IsNight ? cBonusProcess : null); 
        }
        public async Task StopWheel(uint hitWedgeId)
        {
            float targetAngle = anglePerFan * hitWedgeId;
            mainGroup.localEulerAngles = new Vector3(0, 0, targetAngle);
            await PlayAnimationAsync("Finish");
        }

        public async Task ShowWheelWinAsync()
        {
            await PlayAnimationAsync("Win");
            await context.WaitSeconds(0.5f);
        }
    }
}