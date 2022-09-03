using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class LinkLockView11019: TransformHolder
    {
        protected Transform tranReSpinGroup;

        protected Animator animatorReSpin;

        
        protected TextMesh txtReSpinCount;

        protected Transform tranSpin;

        protected Transform tranSpins;
         
        protected List<LinkLockItem11019> listItem = new List<LinkLockItem11019>();
        public LinkLockView11019(Transform inTransform) : base(inTransform)
        {
            tranReSpinGroup = transform.parent.Find("SpinsRemaining");
            animatorReSpin = tranReSpinGroup.GetComponent<Animator>();
            txtReSpinCount = tranReSpinGroup.Find("GroupReSpin/txtReSpinCount").GetComponent<TextMesh>();
            tranSpin = tranReSpinGroup.Find("Spin");
            tranSpins = tranReSpinGroup.Find("Spins");
            
            tranReSpinGroup.gameObject.SetActive(false);
        }


        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            for (int i = 0; i < 4; i++)
            {
                var tranItem = transform.Find($"Lock{i + 1}");
                var linkLockItem = new LinkLockItem11019(tranItem);
                linkLockItem.Initialize(context);
                linkLockItem.SetData(i);
                listItem.Add(linkLockItem);
            }
        }



        public async Task RefreshUI(bool isSetupRoom)
        {
            List<Task> listTask = new List<Task>();
            foreach (var linkLockItem in listItem)
            {
                listTask.Add(linkLockItem.RefreshUI(isSetupRoom));
            }

            await Task.WhenAll(listTask);
        }

        public async Task Open()
        {
            Close();
            foreach (var linkLockItem in listItem)
            {
               linkLockItem.RefreshUI(true);
               await context.WaitSeconds(0.3f);
            }
            tranReSpinGroup.gameObject.SetActive(true);
            tranSpin.gameObject.SetActive(false);
            tranSpins.gameObject.SetActive(true);
            RefreshReSpinCount(false);
        }


        public void Close()
        {
            foreach (var linkLockItem in listItem)
            {
                linkLockItem.transform.gameObject.SetActive(false);
            }
        }

        public void OpenReSpin()
        {
            tranReSpinGroup.gameObject.SetActive(true);
        }

        public void CloseReSpin()
        {
            tranReSpinGroup.gameObject.SetActive(false);

        }


        public async Task RefreshReSpinCount(bool isStartSpin)
        {
            ReSpinState reSpinState = context.state.Get<ReSpinState>();

            if (reSpinState.IsInRespin || reSpinState.ReSpinTriggered)
            {
                if (isStartSpin)
                {
                    txtReSpinCount.text = (reSpinState.ReSpinCount - 1).ToString();
                    if (reSpinState.ReSpinCount - 1 <= 1)
                    {
                        tranSpin.gameObject.SetActive(true);
                        tranSpins.gameObject.SetActive(false);
                    }
                    else
                    {
                        tranSpin.gameObject.SetActive(false);
                        tranSpins.gameObject.SetActive(true);
                    }
                }
                else
                {
                    txtReSpinCount.text = reSpinState.ReSpinCount .ToString();
                    
                    if (reSpinState.ReSpinCount <= 1)
                    {
                        tranSpin.gameObject.SetActive(true);
                        tranSpins.gameObject.SetActive(false);
                    }
                    else
                    {
                        tranSpin.gameObject.SetActive(false);
                        tranSpins.gameObject.SetActive(true);
                    }
                    
                    if (reSpinState.ReSpinLimit == reSpinState.ReSpinCount)
                    {
                        AudioUtil.Instance.PlayAudioFx("Link_Reset");
                        await XUtility.PlayAnimationAsync(animatorReSpin, "Reset", context);
                    }
                    else
                    {
                        await context.WaitSeconds(1.2f);
                    }
                }
                
            }
        }


    }
}