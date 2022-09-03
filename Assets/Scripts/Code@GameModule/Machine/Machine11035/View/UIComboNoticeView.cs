using System.Threading.Tasks;
using GameModule;
using UnityEngine;

public class UIComboNoticeView : TransformHolder
{
    private GameObject[] _combos = new GameObject[7];

    private Animator _animator;

    public UIComboNoticeView(Transform inTransform) : base(inTransform)
    {
        _animator = transform.GetComponent<Animator>();

        var root = transform.Find("GameObject/BetGroup");

        for (int i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);

            _combos[i] = child.gameObject;
        }
    }

    public async Task Set()
    {
        var extraState = context.state.Get<ExtraState11035>();

        var combo = extraState.GetCombo();

        var index = combo - 2;

        if (index < 0 || index >= _combos.Length)
        {
            Hide();
        }
        else
        {
            Show();

            _animator.Play("BetNotice", 0, 0);

            AudioUtil.Instance.PlayAudioFxIfNotPlaying("Win_Multipliy");

            for (int i = 0; i < _combos.Length; i++)
            {
                _combos[i].SetActive(i == index);
            }

            await XUtility.WaitSeconds(1.0f, context);
        }
    }
}