using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace GameModule{
    public class WinLineAnimationController11312 : WinLineAnimationController
    {
        public override async Task BlinkFreeSpinTriggerLine()
        {
            List<string> listTriggerAudio = new List<string>();
            var freeSpinTriggerLines =  wheel.wheelState.GetFreeSpinTriggerLine();

            for (int i = 0; i < freeSpinTriggerLines.Count; i++)
            {
                var line = freeSpinTriggerLines[i];
                
                for (var index = 0; index < line.Positions.Count; index++)
                {
                    var pos = freeSpinTriggerLines[i].Positions[index];
                    var container =  wheel.GetWinLineElementContainer((int) pos.X,(int) pos.Y);
                    container.PlayElementAnimation("Trigger",true);
                    container.ShiftSortOrder(true);
                    container.GetElement().UpdateMaskInteraction(SpriteMaskInteraction.None);
                    if (!listTriggerAudio.Contains(container.sequenceElement.config.name))
                    {
                        listTriggerAudio.Add(container.sequenceElement.config.name);
                    }
                }
            }

            if (freeSpinTriggerLines.Count>0)
            {
                AudioUtil.Instance.StopMusic();
                foreach (var triggerAudio in listTriggerAudio)
                {
                    AudioUtil.Instance.PlayAudioFx(triggerAudio + "_Trigger");
                }
                await wheel.GetContext().WaitSeconds(wheel.wheelState.GetWheelConfig().bonusLineBlinkDuration); 
            }
        }


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
                    // AudioUtil.Instance.PlayAudioFx(container.sequenceElement.config.name + "_Trigger");
                    container.PlayElementAnimation("Trigger");
                    container.ShiftSortOrder(true);
                    if (Constant11312.ListCoinElementIds.Contains(container.sequenceElement.config.id)){
                        var IntegralText = container.GetElement().transform.Find("AnimRoot/IntegralGroup/IntegralText");
                        IntegralText.GetComponent<MeshRenderer>().material.SetFloat("_StencilComp", 8);
                    }
                    if (!listTriggerAudio.Contains(container.sequenceElement.config.name))
                    {
                        listTriggerAudio.Add(container.sequenceElement.config.name);
                    }
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

