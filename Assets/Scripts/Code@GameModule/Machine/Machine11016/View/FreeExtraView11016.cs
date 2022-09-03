using UnityEngine;

namespace GameModule
{
    public class FreeExtraView11016:TransformHolder
    {
        private Animator _animator;
        [ComponentBinder("Root/SpinSprite")] private Transform transSpinSprite;
        [ComponentBinder("Root/SpinsSprite")] private Transform transSpinsSprite;
        [ComponentBinder("Root/CountText")] private TextMesh txtCount;
        public FreeExtraView11016(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, inTransform);
            _animator = inTransform.GetComponent<Animator>();
        }

        public void UpdateExtraFreeCount(int nExtraFree)
        {
            txtCount.text = $"+{nExtraFree} ";
            transSpinSprite.gameObject.SetActive(nExtraFree<=1);
            transSpinsSprite.gameObject.SetActive(nExtraFree>1);
            XUtility.PlayAnimation(_animator,"freespin");
        }
    }
}