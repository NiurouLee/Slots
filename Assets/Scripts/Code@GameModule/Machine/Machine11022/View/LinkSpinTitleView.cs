using UnityEngine;

namespace GameModule
{
    public class LinkSpinTitleView11022:TransformHolder
    {
        [ComponentBinder("TextSprite")] 
        private TextMesh _titleText; 
        [ComponentBinder("RoundPar")] 
        private Transform _animator; 
         
        public LinkSpinTitleView11022(Transform transform) : base(transform)
        {
            ComponentBinder.BindingComponent(this,transform);
        }
        
        public void UpdateContent(int leftSpins)
        {
            _titleText.SetText(leftSpins + " SPIN" + ((leftSpins > 1)?"S":"") + " REMAINING");
            if (leftSpins <= 0)
            {
                Hide();
            }
        }

        public void ShowLight()
        {
            _animator.gameObject.SetActive(true);
        }
    }
}