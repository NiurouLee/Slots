using System;
using UnityEngine;

using UnityEngine.EventSystems;
public class SelectEventCustomHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, ICancelHandler, IUpdateSelectedHandler
{
    
    private Action<BaseEventData> onSelectAction;
    private Action<BaseEventData> onCancelAction;
    private Action<BaseEventData> onUpdateSelectedAction;
    private Action<BaseEventData> onDeselectedAction;
    
    public void BindingSelectAction(Action<BaseEventData> selectAction)
    {
        onSelectAction = selectAction;
    }
    
    public void BindingCancelAction(Action<BaseEventData> cancelAction)
    {
        onCancelAction = cancelAction;
    }
    
    public void BindingUpdateSelectedAction(Action<BaseEventData> updateSelected)
    {
        onUpdateSelectedAction = updateSelected;
    }
    
    public void BindingDeselectedAction(Action<BaseEventData> deselectedAction)
    {
        onDeselectedAction = deselectedAction;
    }

    public void ResetBindingAction()
    {
        onSelectAction = null;
        onCancelAction = null;
        onDeselectedAction = null;
        onUpdateSelectedAction = null;
    }
    
    public void OnSelect(BaseEventData eventData)
    {
         onSelectAction?.Invoke(eventData);
    }

    public void OnCancel(BaseEventData eventData)
    {
        onCancelAction?.Invoke(eventData);
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
        onUpdateSelectedAction?.Invoke(eventData);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        onDeselectedAction?.Invoke(eventData);
    }
}
 