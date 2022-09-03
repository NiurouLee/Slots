using UnityEngine;

namespace GameModule
{
    public class UIJackpot11009: UIJackpotBase
    {
        public UIJackpot11009(Transform inTransform) : base(inTransform)
        {
        }
        
        
        public override async void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            AudioUtil.Instance.PlayAudioFx("JackpotPanel_Open");
            await context.WaitSeconds(3.766f);
            Close();
        }
    }
}