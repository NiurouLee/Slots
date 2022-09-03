using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace GameModule
{
    public class WinGroupFeature11031 : TransformHolder
    {
        private Animator animator;

        [ComponentBinder("Root/NumGP")] protected TextMesh numGroupsNodes;

        [ComponentBinder("Root/Num")] protected TextMesh _textMeshProUGUIPepperWinNum;

        private List<Transform> numGroupList;


        public WinGroupFeature11031(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            // var childCount = 11;
            // numGroupList = new List<Transform>(childCount);
            // for (var i = 0; i < childCount; i++)
            // {
            //     var lightGroup = numGroupsNodes.GetChild(i);
            //     numGroupList.Add(lightGroup);
            // }
        }

        public async Task Open()
        {
            AudioUtil.Instance.PlayAudioFx("SideJackpot_Open");
            var index = context.state.Get<ExtraState11031>().GetLinkPepperCount();
            // ulong winRate;
            // if (index >= 40)
            // {
            //     winRate = context.state.Get<ExtraState11031>().GetLinkPepperJackpotPay();
            // }
            // else
            // {
            //     winRate = context.state.Get<ExtraState11031>().GetLinkPepperWinRate();
            // }
            var winRate = context.state.Get<ExtraState11031>().GetLinkPepperJackpotPay() +
                          context.state.Get<ExtraState11031>().GetLinkPepperWinRate();

            var chips = context.state.Get<BetState>().GetPayWinChips(winRate);
            _textMeshProUGUIPepperWinNum.gameObject.GetComponent<TextMesh>().SetText(chips.GetCommaFormat());
            transform.gameObject.SetActive(true);
            numGroupsNodes.SetText(index.ToString());
            await XUtility.PlayAnimationAsync(animator, "UIJackpot_Open");
            // AudioUtil.Instance.PlayAudioFx("Close");
            await XUtility.PlayAnimationAsync(animator, "UIJackpot_Close");
        }

        public async Task InBaseOpen(int pay)
        {
            AudioUtil.Instance.PlayAudioFx("SideJackpot_Open");
            var index = context.state.Get<ExtraState11031>().GetLinkPepperCount();
            long chips;
            // if (index >= 40)
            // {
            //     var jackpotPay = context.state.Get<JackpotInfoState>().GetJackpotValue(6);
            //     chips = context.state.Get<BetState>().GetPayWinChips((long)jackpotPay);
            //     
            // }
            // else
            // {
            //    chips = context.state.Get<BetState>().GetPayWinChips(pay);
            // }
            var winRate = context.state.Get<ExtraState11031>().GetLinkPepperJackpotPay() +
                          context.state.Get<ExtraState11031>().GetLinkPepperWinRate();
            chips = context.state.Get<BetState>().GetPayWinChips(pay);
            _textMeshProUGUIPepperWinNum.gameObject.GetComponent<TextMesh>().SetText(chips.GetCommaFormat());
            transform.gameObject.SetActive(true);
            numGroupsNodes.SetText(index.ToString());
            await XUtility.PlayAnimationAsync(animator, "UIJackpot_Open");
            // AudioUtil.Instance.PlayAudioFx("Close");
            await XUtility.PlayAnimationAsync(animator, "UIJackpot_Close");
        }

        public async Task InBaseHighOpen(int pay)
        {
            AudioUtil.Instance.PlayAudioFx("SideJackpot_Open");
            var index = 40;
            long chips = context.state.Get<BetState>().GetPayWinChips((long) pay);
            _textMeshProUGUIPepperWinNum.SetText(chips.GetCommaFormat());
            transform.gameObject.SetActive(true);
            numGroupsNodes.SetText(index.ToString());
            await XUtility.PlayAnimationAsync(animator, "UIJackpot_Open");
            // AudioUtil.Instance.PlayAudioFx("Close");
            await XUtility.PlayAnimationAsync(animator, "UIJackpot_Close");
        }

        public async Task Close()
        {
            await XUtility.PlayAnimationAsync(animator, "UIJackpot_Close");
            transform.gameObject.SetActive(false);
        }
    }
}