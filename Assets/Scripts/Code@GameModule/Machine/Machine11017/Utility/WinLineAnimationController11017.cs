using System.Threading.Tasks;
namespace GameModule
{
    public class WinLineAnimationController11017: WinLineAnimationController
    {
        
        public override async Task BlinkBonusLine()
        { 
            var bonusWinLines =  wheel.wheelState.GetBonusWinLine();
           for (int i = 0; i < bonusWinLines.Count; i++)
           {
               for (var index = 0; index < bonusWinLines[i].Positions.Count; index++)
               {
                   var pos = bonusWinLines[i].Positions[index];
                   var container =  wheel.GetWinLineElementContainer((int) pos.X,(int) pos.Y);
                   //AudioUtil.Instance.PlayAudioFx(container.sequenceElement.config.name + "_Trigger");
                   container.PlayElementAnimation("Remove");
                   container.ShiftSortOrder(true);
               }
           }
        }

    }
}