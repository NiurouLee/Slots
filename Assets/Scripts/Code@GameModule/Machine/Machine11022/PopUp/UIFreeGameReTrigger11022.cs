using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIFreeGameReTrigger11022:FreeSpinReTriggerPopUp
    {
        public UIFreeGameReTrigger11022(Transform transform) : base(transform)
        {
            _extraCountText = (Text)transform.Find("Root/MainGroup/Count/CountText").GetComponent(typeof(Text));
        }
        public override async void SetExtraCount(uint extraCount)
        {
            if(_extraCountText)
                _extraCountText.text = extraCount.ToString();
            
            await XUtility.PlayAnimationAsync(animator,"Open",context);
            // AudioUtil.Instance.PlayAudioFx("Close");
            Close();
        }
        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("FREERetrigger_Open");
            base.OnOpen();
        }
    }
}