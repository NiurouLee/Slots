using DG.Tweening;
using UnityEngine;

namespace GameModule.PopUp
{
    public class FreeSpinReTriggerPopUp11013: FreeSpinReTriggerPopUp
    {
        public FreeSpinReTriggerPopUp11013(Transform transform) : base(transform)
        {
        }
        
        
        
        public override async void SetExtraCount(uint extraCount)
        {
            if(_extraCountText)
                _extraCountText.text ="0";
            
             XUtility.PlayAnimation(animator,"Retrigger");
            
             await context.WaitSeconds(0.433f);

             AudioUtil.Instance.PlayAudioFx("Trigger");
             var tweener = DOTween.To(() => 0, (nowNum) =>
             {
                 if(_extraCountText)
                     _extraCountText.text = nowNum.ToString();
             },(int)extraCount,0.9f);
            
             context.AddTweener(tweener);

             tweener.onComplete += () =>
             {
                 context.RemoveTweener(tweener);
                 if(_extraCountText)
                     _extraCountText.text = extraCount.ToString();
             };
            
            
             await context.WaitSeconds(2.9f-0.433f);
            
            
            Close();
        }
    }
}