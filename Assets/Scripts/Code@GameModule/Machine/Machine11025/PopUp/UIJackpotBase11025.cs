using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class UIJackpotBase11025:UIJackpotBase
    {
        public UIJackpotBase11025(Transform inTransform) : base(inTransform)
        {
            
        }
        
        public override async Task AutoClose()
        {
            await XUtility.PlayAnimationAsync(animator, "Open");
            await XUtility.PlayAnimationAsync(animator, "Idle");
            Close();
        }

        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("Jackpot");
            base.OnOpen();
        }
    }
}