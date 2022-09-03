using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class LinkLockItem11019: TransformHolder
    {

        [ComponentBinder("CountText")]
        protected TextMesh txtCount;

        [ComponentBinder("LockstGroup")]
        protected Animator animator;
        public LinkLockItem11019(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
        }


        protected int index = 0;

        public void SetData(int index)
        {
            this.index = index;
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
        }


        public async Task Open()
        {
            if (!this.transform.gameObject.activeInHierarchy)
            {
                this.transform.gameObject.SetActive(true);
                await XUtility.PlayAnimationAsync(animator, "Idle", context);
            }
            
        }

        public async Task Close()
        {
            if (this.transform.gameObject.activeInHierarchy)
            {
                AudioUtil.Instance.PlayAudioFx("Link_Unlock");
                await XUtility.PlayAnimationAsync(animator, "Open", context);
                this.transform.gameObject.SetActive(false);
            }
        }


        public async Task RefreshUI(bool isSetupRoom)
        {
            var extraState = context.state.Get<ExtraState11019>();


            int nowCount = extraState.GetPepperCount();
            int lastCount = extraState.GetLastPepperCount();
            int needCount = Constant11019.listLinkUnlockPaperNeedCount[this.index];


            if (nowCount != lastCount || isSetupRoom)
            {
                if (nowCount >= needCount)
                {

                    await Close();
                }
                else
                {

                    txtCount.text = (needCount - nowCount).ToString();
                    await Open();

                    AudioUtil.Instance.PlayAudioFx("Link_Lock");
                    await XUtility.PlayAnimationAsync(animator, "Win", context);
                    

                }
            }


        }
    }
}