using UnityEngine;

namespace GameModule
{
    public class UIJackpot11005: UIJackpotBase
    {
        public UIJackpot11005(Transform inTransform) : base(inTransform)
        {
        }

        public override async void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            AudioUtil.Instance.PlayAudioFx("J01_Settlement");
            await context.WaitSeconds(2.7f);
            Close();
        }
    }
}