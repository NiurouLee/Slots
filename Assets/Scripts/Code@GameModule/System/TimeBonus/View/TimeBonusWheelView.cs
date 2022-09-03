// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/14/16:23
// Ver : 1.0.0
// Description : TimeBonusWheelView.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class TimeBonusWheelView : View<ViewController>
    {
        [ComponentBinder("WheelMainGroup")] 
        public Transform wheelMainGroup;
        
        [ComponentBinder("MainGroup")] 
        public Transform mainGroup;

        [ComponentBinder("RewardGroup")] 
        public Transform rewardGroup;
      
        [ComponentBinder("SpinButton")] 
        public Button spinButton;
        
        [ComponentBinder("SpinBuffGroup")] 
        public Transform spinBuffGroup;

        public Animator animator;
        
        public float anglePerFan = 0;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            
            animator = wheelMainGroup.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        public virtual void InitializeWheelView(CommonWheel commonWheel, int numberOfDigitsToSimply, double coinScaleFactor = 1.0f)
        {
            anglePerFan = 360/ (float) commonWheel.Wedge.Count;
            WheelUIInitializeHelper.InitializeWheelUI(rewardGroup, commonWheel, numberOfDigitsToSimply, coinScaleFactor);

            if (spinBuffGroup)
            {
                spinBuffGroup.gameObject.SetActive(false);
            }
        }

        public void StartSpinWheel()
        {
            animator.Play("Start");
        }

        public async Task StopWheel(int hitWedgeId)
        {
            float targetAngle = anglePerFan * hitWedgeId;
            
            mainGroup.localEulerAngles = new Vector3(0, 0, targetAngle);
            
            await XUtility.PlayAnimationAsync(animator, "Finish");
        }

        public async Task ShowWinEffect()
        {
            await XUtility.PlayAnimationAsync(animator, "Win");
        }
  
        public async Task ShowSpinBuffAdditionAnimation(float buff, CommonWheel commonWheel, int numberOfDigitsToSimply, double coinScaleFactor = 1.0f)
        {
            if (spinBuffGroup != null)
            {
                
                spinBuffGroup.Find("PercentText").GetComponent<Text>().text = (buff * 100) + "%";
                spinBuffGroup.gameObject.SetActive(true);
                
                var spinBuffAnimator = spinBuffGroup.GetComponent<Animator>();
               
                spinBuffGroup.Find("PercentText").GetComponent<Text>().text = (buff * 100) + "%";
                await XUtility.PlayAnimationAsync(spinBuffAnimator,"Open");
             
                viewController.WaitForSeconds(2f, () =>
                {
                    WheelUIInitializeHelper.InitializeWheelUI(rewardGroup, commonWheel, numberOfDigitsToSimply,
                        coinScaleFactor);
                });
                
                await XUtility.PlayAnimationAsync(spinBuffAnimator, "Fly");
              
                spinBuffGroup.gameObject.SetActive(false);
               
            }
        }
    }
}