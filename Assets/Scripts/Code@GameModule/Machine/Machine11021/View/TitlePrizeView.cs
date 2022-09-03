using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TitlePrizeView: TransformHolder
    {
        public TitlePrizeView(Transform inTransform) : base(inTransform)
        {
            
        }


        protected List<TitlePrizeViewItem> listItems = new List<TitlePrizeViewItem>();
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            for (int i = 0; i < 6; i++)
            {
                Transform tranCell = transform.Find($"PrizeDiskFG/Cell{i+1}");
                TitlePrizeViewItem item = new TitlePrizeViewItem(tranCell);
                item.SetData(i,this);
                item.Initialize(inContext);
                listItems.Add(item);
            }
            
        }



        public void ChangeListEnableState(bool isFree)
        {
            if (isFree)
            {
                for (int i = 0; i < 5; i++)
                {
                    listItems[i].SetEnableState(true);
                }
            }
            else
            {
                listItems[0].SetEnableState(true);
                listItems[1].SetEnableState(false);
                listItems[2].SetEnableState(true);
                listItems[3].SetEnableState(false);
                listItems[4].SetEnableState(true);
            }
        }

        
        public void RefreshInfo()
        {
            var freeSpinState = context.state.Get<FreeSpinState>();
            RefreshInfo(freeSpinState.IsInFreeSpin && !freeSpinState.IsOver);
        }


        public void RefreshInfo(bool isFree)
        {
            var extralInfo = context.state.Get<ExtraState11021>();
            var deskData = extralInfo.GetDiskData();

            if (isFree)
            {
                for (int i = 0; i < deskData.Items.Count; i++)
                {
                    listItems[i].RefreshInfo(deskData.FreeItems[i],isFree);
                }
            }
            else
            {
                for (int i = 0; i < deskData.Items.Count; i++)
                {
                    listItems[i].RefreshInfo(deskData.Items[i],isFree);
                }
            }

            
        }


        public async Task MoveToNext()
        {
            var listTask = new List<Task>();
            foreach (var item in listItems)
            {
                listTask.Add(item.MoveToNext());
            }
            await Task.WhenAll(listTask);
            
        }


        
        public async Task CollectItems(LogicStepProxy logicStep)
        {
            for (int i = 0; i < 5; i++)
            {
                await listItems[i].CollectReward(logicStep);
            }
            for (int i = 0; i < 5; i++)
            {
                await listItems[i].CollectRewardFree(logicStep);
            }
        }

        public void NiceWinComplet()
        {
            for (int i = 0; i < 5; i++)
            {
                listItems[i].NiceWinComplet();
            }
        }


        public void NoVisibleItemsAll()
        {
            for (int i = 0; i < 5; i++)
            {
                 listItems[i].SetVisible(false);
            }
        }
        
        
        public void VisibleItemsAll()
        {
            for (int i = 0; i < 5; i++)
            {
                
                listItems[i].SetVisible(true);
            }
        }


        public void ShowOneItem(int index)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == index)
                {
                    listItems[i].SetVisible(true);
                }
                else
                {
                    listItems[i].SetVisible(false);
                }
            }
        }



    }
}