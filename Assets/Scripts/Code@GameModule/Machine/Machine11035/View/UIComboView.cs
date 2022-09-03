using GameModule;
using UnityEngine;

public class UIComboView : TransformHolder
{
    private GameObject[] _fills = new GameObject[9];

    public UIComboView(Transform inTransform) : base(inTransform)
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).Find("Fill");

            _fills[i - 1] = child.gameObject;
        }
    }

    public void Set(bool byCombo = false)
    {
        var extraState = context.state.Get<ExtraState11035>();
        var combo = (int)extraState.GetCombo();

        for (int i = 0; i < _fills.Length; i++)
        {
            var fill = _fills[i];

            fill.SetActive(i < combo || (extraState.HasJackpotWheel() && !byCombo));
        }
    }
}