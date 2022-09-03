using System;
using UnityEngine.EventSystems;
using UnityEngine;
public class MiscEventCustomHandler:MonoBehaviour, ISubmitHandler, IMoveHandler
{
    private Action<AxisEventData> onMoveAction;
    private Action<BaseEventData> onSubmitAction;


    public void BindingMoveAction(Action<AxisEventData> moveAction)
    {
        onMoveAction = moveAction;
    }

    public void BindSubmitAction(Action<BaseEventData> submitAction)
    {
        onSubmitAction = submitAction;
    }
    
    public void ResetBindingAction()
    {
        onMoveAction = null;
        onSubmitAction = null;
    }
    
    public void OnMove(AxisEventData eventData)
    {
        onMoveAction?.Invoke(eventData);
    }
 
    public void OnSubmit(BaseEventData eventData)
    {
        onSubmitAction?.Invoke(eventData);
    }
}