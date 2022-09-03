using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class UIJackpotBase11024:UIJackpotBase
    {
        public UIJackpotBase11024(Transform inTransform) : base(inTransform)
        {
            
        }
        
        public override async Task AutoClose()
        {
            await XUtility.PlayAnimationAsync(animator, "Open");
            // await XUtility.PlayAnimationAsync(animator, "Idle");
            // await XUtility.PlayAnimationAsync(animator, "Idle");
            // Close();
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            context.WaitSeconds(2f, () =>
            {
                Close();
            });
        }

        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("LinkGame_JACKPOT");
            base.OnOpen();
        }
    }
}