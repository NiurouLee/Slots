using UnityEngine;

namespace GameModule
{
    public class BaseInformationView : TipView
    {
        public BaseInformationView(Transform inTransform) : base(inTransform,true)
        {
        }


        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("Tips_Open");
            base.OnOpen();
        }


        public override void Close()
        {
            AudioUtil.Instance.PlayAudioFx("Tips_Close");
            base.Close();
        }
    }
}