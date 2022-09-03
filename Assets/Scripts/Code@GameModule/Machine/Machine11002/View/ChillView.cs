using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class ChillView : TransformHolder
    {
        public ChillView(Transform inTransform) : base(inTransform)
        {
        }

        private ExtraState11002 extraState;
        private BetState _betState;

        private List<ChillViewItem> listChillViewItem = new List<ChillViewItem>();

        private bool isFree = false;

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            extraState = inContext.state.Get<ExtraState11002>();
            _betState = inContext.state.Get<BetState>();

            if (transform.name == "Wheel1")
            {
                isFree = true;
            }

            for (int i = 1; i <= 5; i++)
            {
                Transform tran = transform.Find($"ChillGroup/Chill{i}");
                Animator tranRollAnim = null;

                var wPath = isFree ? $"Rolls/Active_W01_6_{i}" : $"Rolls/Active_W01_3_{i}";
                Transform wTrans = transform.Find(wPath);
                if (wTrans != null)
                {
                    tranRollAnim = wTrans.GetComponent<Animator>();
                }

                ChillViewItem item = new ChillViewItem(i - 1, tran, tranRollAnim, inContext, isFree);
                listChillViewItem.Add(item);
            }
        }

        public void PlayFullWildWin(int rollIndex)
        {
            var item = listChillViewItem[rollIndex];
            if (item.IsFullWildActive()) { item.PlayFullWildWin(); }
        }

        public void PlayFullWildIdle(int rollIndex)
        {
            var item = listChillViewItem[rollIndex];
            if (item.IsFullWildActive()) { item.PlayFullWildIdle(); }
        }

        public void StopAllFullWildAnimation()
        {
            foreach (var item in listChillViewItem)
            {
                if (item.IsFullWildActive()) { item.PlayFullWildIdle(); }
            }
        }

        public void PlayAllFullWildFade()
        {
            foreach (var item in listChillViewItem)
            {
                if (item.IsFullWildActive()) { item.PlayFullWildFade(); }
            }
        }

        public bool IsFullWild(int rollIndex)
        {
            var item = listChillViewItem[rollIndex];
            return item.IsFullWildActive();
        }

        // private int numRefresh = 0;
        public async Task RefreshUI(bool isStartSpin, bool isAnim = true,
            bool isReConnect = false, bool isChangeWild = true)
        {
            if (extraState.HasExtraInfo())
            {
                var listChillNum = extraState.GetChillState(_betState.totalBet);
                var listLastChillNum = extraState.GetChillLastState(_betState.totalBet);
                var listActive = extraState.GetChillActive(_betState.totalBet);

                // numRefresh = 0;

                // TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
                // for (int i = 0; i < 5; i++)
                // {
                //     RefreshItemUI(taskCompletionSource, i, listChillNum[i], listLastChillNum[i],
                //         listActive[i], isStartSpin, isAnim, isReConnect, isChangeWild);
                // }

                // await taskCompletionSource.Task;

                Task[] tasks = new Task[5];

                for (int i = 0; i < 5; i++)
                {
                    var item = listChillViewItem[i];

                    tasks[i] = item.RefreshUI(
                        listChillNum[i],
                        listLastChillNum[i],
                        listActive[i],
                        isStartSpin,
                        isAnim,
                        isReConnect,
                        isChangeWild);
                }

                await Task.WhenAll(tasks);
            }
        }

        // private async void RefreshItemUI(TaskCompletionSource<bool> task, int i,
        //     uint nowChill, uint lastChill, bool isActive,
        //     bool isStartSpin, bool isAnim, bool isReConnect, bool isChangeWild)
        // {
        //     if (listChillViewItem != null && listChillViewItem.Count > 0)
        //     {
        //         await listChillViewItem[i].RefreshUI(nowChill, lastChill, isActive, isStartSpin, isAnim, isReConnect, isChangeWild);
        //     }

        //     numRefresh++;
        //     if (numRefresh >= 5)
        //     {
        //         numRefresh = 0;
        //         task.SetResult(true);
        //     }
        // }
    }
}