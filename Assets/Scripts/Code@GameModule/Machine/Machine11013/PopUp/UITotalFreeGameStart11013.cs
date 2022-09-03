using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UITotalFreeGameStart11013:MachinePopUp
    {
        [ComponentBinder("Root/MainGroup/CountText")]
        private Text _extraCountText;
        
        public UITotalFreeGameStart11013(Transform transform) 
            :base(transform)
        {
            
        }
        
        public async Task SetExtraCount(uint extraCount)
        {
            if(_extraCountText)
                _extraCountText.text = "0";
            
            XUtility.PlayAnimation(animator,"Open");

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