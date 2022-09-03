using GameModule;
using UnityEngine;

public class UIFireJackpotNoticeView : TransformHolder
{
    private Animator _animator;
    public UIFireJackpotNoticeView(Transform inTransform) : base(inTransform)
    {
        _animator = transform.GetComponent<Animator>();
    }

    public void Open()
    {
        Show();

        _animator.Play("Open", 0, 0);
    }
}