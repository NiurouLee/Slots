
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class UIJackpotBase11022:UIJackpotBase
    {
        
        public UIJackpotBase11022(Transform inTransform) : base(inTransform)
        {
            
        }
        public override async Task AutoClose()
        {
            await XUtility.PlayAnimationAsync(animator, "Open",context);
            await context.WaitSeconds(1);
            // AudioUtil.Instance.PlayAudioFx("Close");
            Close();
        }

        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("Jackpot_Open");
            base.OnOpen();
        }
    }
}