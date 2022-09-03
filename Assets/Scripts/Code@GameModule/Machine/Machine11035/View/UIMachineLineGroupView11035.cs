using GameModule;
using UnityEngine;

public class UIMachineLineGroupView11035 : TransformHolder
{
    private Animator _animator;

    public UIMachineLineGroupView11035(Transform inTransform) : base(inTransform)
    {
        _animator = inTransform.GetComponent<Animator>();
    }

    public void PlayIdle()
    {
        _animator.Play("New State", 0, 0);
    }

    public void PlayLoop()
    {
        _animator.Play("MachineLineGroup", 0, 0);
    }
}