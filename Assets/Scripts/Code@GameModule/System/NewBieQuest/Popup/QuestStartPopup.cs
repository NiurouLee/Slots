// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/05/19:07
// Ver : 1.0.0
// Description : QuestFinishPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIQuestStart")]
    public class QuestStartPopup : Popup
    {
        [ComponentBinder("IntegralText")] 
        private TextMeshProUGUI _integralText;

        [ComponentBinder("StartButton")] 
        private Button _startButton;  
         
        [ComponentBinder("DescriptionText1")] 
        private TextMeshProUGUI _descriptionText1;
        
        [ComponentBinder("DescriptionText2")] 
        private TextMeshProUGUI _descriptionText2;
 
        [ComponentBinder("Root/MainGroup/TargetGroup1/Icon")]
        private Image _mission1Icon;
        
        [ComponentBinder("Root/MainGroup/TargetGroup2/Icon")]
        private Image _mission2Icon;
      

        [ComponentBinder("SlotIcon")] 
        private Transform iconContainer;

        private AssetReference atlasAssetReference;
        private AssetReference slotIconAssetReference;
 
        public QuestStartPopup(string address)
            : base(address)
        {
            
        }

        public override Vector3 CalculateScaleInfo()
        {
            if (ViewManager.Instance.IsPortrait)
            {
                return Vector3.one * 0.9f;
            }

            return Vector3.one;
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            
            _startButton.onClick.AddListener(OnStartButtonClicked);
            
            InitQuestDescriptionUI();
            
            if (iconContainer.childCount > 0)
            {
                int childCount = iconContainer.childCount;

                for (var i = childCount - 1; i >= 0; i--)
                {
                    var child = iconContainer.GetChild(i);
                    GameObject.Destroy(child);
                }
            }
            
            var quest = Client.Get<NewBieQuestController>().GetCurrentQuest();
            slotIconAssetReference = AssetHelper.PrepareAsset<GameObject>($"SlotIcon{quest.AssetId}Group", (handler) =>
                {
                    if (handler != null)
                    {
                        var machineIcon = handler.InstantiateAsset<GameObject>();
                        var iconAnimator = machineIcon.GetComponentInChildren<Animator>();
                        iconAnimator.enabled = false;
                        machineIcon.transform.SetParent(iconContainer,false);
                    }
                });
            
        }
        
        protected override void SetUpController(object inExtraData, object inExtraAsyncData = null)
        {
            if(inExtraAsyncData != null)
                atlasAssetReference = inExtraAsyncData as AssetReference;
        }
        
        protected void InitQuestDescriptionUI()
        {
            var missions = Client.Get<NewBieQuestController>().GetCurrentMission();

            if (missions.Count >= 2)
            {
                _descriptionText1.text = missions[0].GetContentDescText();
                _descriptionText2.text = missions[1].GetContentDescText();

                var spriteAtlas = atlasAssetReference.GetAsset<SpriteAtlas>();
                //TODO: Update Icon
                _mission1Icon.sprite = spriteAtlas.GetSprite(missions[0].GetMissionUI());
                _mission2Icon.sprite = spriteAtlas.GetSprite(missions[1].GetMissionUI());
                
                //TODO Update MachineIcon
            }
        }
        protected void OnStartButtonClicked()
        {
            SoundController.PlayButtonClick();
            Close();
        }

        public override void Destroy()
        {
            base.Destroy();
           
            if (atlasAssetReference != null)
            {
                atlasAssetReference.ReleaseOperation();
            }

            if (slotIconAssetReference != null)
            {
                slotIconAssetReference.ReleaseOperation();
            }
        }
    }
}