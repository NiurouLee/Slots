using GameModule;
using UnityEngine;

public class UITransitionView11035 : TransformHolder
{
    private Animator _animator;

    public UITransitionView11035(Transform inTransform) : base(inTransform)
    {
        _animator = inTransform.GetComponent<Animator>();
    }

    public void PlayIdle()
    {
        _animator.Play("Idle1", 0, 0);
    }

    public void PlayLoop()
    {
        _animator.Play("Idle2", 0, 0);
    }
}