using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using System.Threading.Tasks;

namespace GameModule
{
    public class WinLineAnimationController11031 : WinLineAnimationController
    {
        public override async Task BlinkWinLineAsync(WinLine winLine)
        {
            wheel.GetContext().state.Get<WheelsActiveState11031>().ShowRollsMasks(wheel);
            await base.BlinkWinLineAsync(winLine);
        }

        protected override void InitializePayLineLayer()
        {
            var winFrameAnimationObj =
                assetProvider.InstantiateGameObject("PayLine");
            if (winFrameAnimationObj != null)
            {
                payLineLayer = new PayLineLayer(wheel,
                    winFrameAnimationObj, "SoloElement");
            }
        }

        public override async Task BlinkReSpinLine()
        {
            var reSpinWinLine = wheel.wheelState.GetReSpinWinLine();
            List<string> listTriggerAudio = new List<string>();
            for (int i = 0; i < reSpinWinLine.Count; i++)
            {
                for (var index = 0; index < reSpinWinLine[i].Positions.Count; index++)
                {
                    var pos = reSpinWinLine[i].Positions[index];
                    var container = wheel.GetWinLineElementContainer((int) pos.X, (int) pos.Y);
                    //将绿色辣椒更换成红色辣椒
                    var isChange = !wheel.GetContext().state.Get<ExtraState11031>().IsFourLinkMode();
                    if (Constant11031.ListGreenChilli.Contains(container.sequenceElement.config.id) && isChange)
                    {
                        var oldId = container.sequenceElement.config.id;
                        var newId = Constant11031.ChangeGreenChilliId(oldId);
                        var elementConfigSet = wheel.GetContext().state.machineConfig.GetElementConfigSet();
                        var sequenceElement =
                            new SequenceElement(elementConfigSet.GetElementConfig(newId), wheel.GetContext());
                        container.UpdateElement(sequenceElement);
                    }
                    container.PlayElementAnimation("Trigger");
                    container.ShiftSortOrder(true);
                    // if (!listTriggerAudio.Contains(container.sequenceElement.config.name))
                    // {
                    //     listTriggerAudio.Add(container.sequenceElement.config.name);
                    // }
                }
            }

            if (reSpinWinLine.Count > 0)
            {
                var isFourLink = wheel.GetContext().state.Get<ExtraState11031>().IsFourLinkMode();
                if (isFourLink)
                {
                    AudioUtil.Instance.PlayAudioFx("J20-J25_Trigger"); 
                }
                else
                {
                    AudioUtil.Instance.PlayAudioFx("J20-J25_Trigger_NoRing");
                }
                // foreach (var triggerAudio in listTriggerAudio)
                // {
                //     AudioUtil.Instance.PlayAudioFx(triggerAudio + "_Trigger");
                // }

                await wheel.GetContext().WaitSeconds(2.5f);
            }
        }
    }
}