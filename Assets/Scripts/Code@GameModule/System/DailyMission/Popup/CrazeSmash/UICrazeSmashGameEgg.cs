using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;
using static DragonU3DSDK.Network.API.ILProtocol.EggInfo.Types;

namespace GameModule
{
    public class UICrazeSmashGameEgg : View<ViewController>
    {
        [ComponentBinder("StateGroup")]
        public Animator animator;
 
        [ComponentBinder("StateGroup/RewardState/RewardContent")]
        public Transform rewardContent;
 
        private Button _button;
 
        private Action _onClick;

        protected override void BindingComponent()
        {
            base.BindingComponent();
            var image = gameObject.AddComponent<Image>();
            image.color = Color.clear;

            _button = gameObject.AddComponent<Button>();
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            _onClick?.Invoke();
        }

        public void SetOnClick(Action onClick)
        {
            _onClick = onClick;
        }

        public async Task Smash(Egg egg)
        {
            if (egg == null || egg.Open == false)
            {
                return;
            }

            _button.interactable = false;
            animator.Play("EggCell_smash");
            await viewController.WaitForSeconds(0.5f);

            var sfx = egg.Win ? "WINSMASH_01" : "SMASH_01";
            SoundController.PlaySfx(sfx);
            
            SetReward(egg);

            await viewController.WaitForSeconds(1.5f);
        }

        public void SetReward(Egg egg)
        {
            var childCount = rewardContent.childCount;
            
            for (var i = 0; i < childCount; i++)
            {
                rewardContent.GetChild(i).gameObject.SetActive(false);
            }
            
            if (egg.Win)
            {
                rewardContent.Find("Win").gameObject.SetActive(true);
            }
            else
            {
                if (egg.Items.Count > 0)
                {
                    switch (egg.Items[0].Type)
                    {
                        case Item.Types.Type.CardPackage:
                            rewardContent.Find("CardPackage").gameObject.SetActive(true);
                            break;
                        case Item.Types.Type.Emerald:
                            rewardContent.Find("Emerald").gameObject.SetActive(true);
                            break;
                        case Item.Types.Type.Coin:
                            rewardContent.Find("Coin").gameObject.SetActive(true);
                            break;
                    }
                }
            }
        }

        public void Set(Egg egg)
        {
            if (egg == null) { return; }

            if (egg.Open == false)
            {
                animator.Play("EggCell", 0, 1);
                _button.interactable = true;
            }
            else
            {
                _button.interactable = false;
                animator.Play("EggCell_smash_Loop", 0, 1);
                SetReward(egg);
            }
        }
    }
}