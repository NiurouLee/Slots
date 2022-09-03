// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/06/16:53
// Ver : 1.0.0
// Description : MachineSystemWidgetView.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class MachineSystemWidgetView:TransformHolder
    {
        private SystemWidgetContainerView _systemWidget;

        public MachineSystemWidgetView(Transform transform)
            : base(transform)
        {
        
        }

        public async Task LoadContainerView()
        {
            _systemWidget = await View.CreateView<SystemWidgetContainerView>("SystemWidgetContainer", transform);
            _systemWidget.SetMachineContext(context);
            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();

            context.AddWaitTask(waitTask, null);
            _systemWidget.CollectSystemWidget(() =>
            {
                context.RemoveTask(waitTask);
                waitTask.SetResult(true);
            });

            await waitTask.Task;
        }

        public override void OnDestroy()
        {
            _systemWidget?.Destroy();
        }

        public void SetActive(bool activeState)
        {
            if (activeState && !transform.gameObject.activeSelf)
            {
                Show();
            }
            else if (!activeState && transform.gameObject.activeSelf)
            {
                Hide();
            }
        }
    }
}