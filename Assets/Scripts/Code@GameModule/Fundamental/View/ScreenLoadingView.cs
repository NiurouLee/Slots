using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class ScreenLoadingView : View
    {
        [ComponentBinder("LoadingIndicator")] 
        private Image _loadingIndicator = null;

        private float angle = 0;


        private Animator _animator;

        public ScreenLoadingView(string url) : base(url)
        {
        }

        protected override void OnViewSetUpped()
        {
            var updateProxy = gameObject.AddComponent<MonoUpdateProxy>();
            updateProxy.BindingAction(Update);

            _animator = transform.GetComponent<Animator>();

            if (_animator)
            {
                _animator.keepAnimatorControllerStateOnDisable = true;
            }
        }
        
        public void DelayShow()
        {
            gameObject.SetActive(true);

            if (_animator)
            {
                _animator.Play("DelayShowMask",0,0);
            }
        }

        public override void Show()
        {
            base.Show();
            if (_animator)
            {
                _animator.Play("Idle",0,0);
            }
        }

        public void Update()
        {
            if (transform.gameObject.activeInHierarchy)
            {
                angle += 2;
                _loadingIndicator.transform.localEulerAngles = new Vector3(0, 0, -angle);
            }
        }
    }
}