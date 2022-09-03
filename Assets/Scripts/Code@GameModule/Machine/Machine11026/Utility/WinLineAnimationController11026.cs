using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace GameModule
{
    public class WinLineAnimationController11026: WinLineAnimationController
    {
        
        public override async Task BlinkReSpinLine()
        {
            var reSpinWinLine =  wheel.wheelState.GetReSpinWinLine();
            List<string> listTriggerAudio = new List<string>();
            for (int i = 0; i < reSpinWinLine.Count; i++)
            {
                
                for (var index = 0; index < reSpinWinLine[i].Positions.Count; index++)
                {
                    var pos = reSpinWinLine[i].Positions[index];
                    var container =  wheel.GetWinLineElementContainer((int) pos.X,(int) pos.Y);
                    //AudioUtil.Instance.PlayAudioFx(container.sequenceElement.config.name + "_Trigger");
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
                // foreach (var triggerAudio in listTriggerAudio)
                // {
                    AudioUtil.Instance.PlayAudioFx("J01_Trigger");
                // }
                await wheel.GetContext().WaitSeconds(wheel.wheelState.GetWheelConfig().bonusLineBlinkDuration);
            }
        }

    }
}