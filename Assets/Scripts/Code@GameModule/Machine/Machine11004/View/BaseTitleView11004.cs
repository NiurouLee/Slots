using UnityEngine;

namespace GameModule
{
    public class BaseTitleView11004: TransformHolder
    {

        [ComponentBinder("CountText","ElementGroup1")]
        protected TextMesh txtNumRed;

        [ComponentBinder("CountText","ElementGroup2")]
        protected TextMesh txtNumGreen;

        protected Animator animator;
        
        public BaseTitleView11004(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
            animator = transform.GetComponent<Animator>();
        }


        protected ExtraState11004 extraState;
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            extraState = context.state.Get<ExtraState11004>();
        }


        public void Open()
        {
            this.transform.gameObject.SetActive(true);
        }

        public void Close()
        {
            this.transform.gameObject.SetActive(false);
        }


        protected int numRed;
        protected int numGreen;
        public void RefreshUI()
        {
           
                
                
                RefreshRedUI();
                RefreshGreenUI();
            

            
        }


        public void ClearUI()
        {
            numRed = 0;
            txtNumRed.text = numRed.ToString();
            numGreen = 0;
            txtNumGreen.text = numGreen.ToString();
        }


        public void RefreshRedUI()
        {
            numRed = extraState.GetRedLockNum();
            txtNumRed.text = numRed.ToString();
        }

        public void RefreshGreenUI()
        {
            numGreen = extraState.GetGreenLockNum();
            txtNumGreen.text = numGreen.ToString();
        }

        public void AddRedUI(int numAdd)
        {
            numRed += numAdd;
            txtNumRed.text = numRed.ToString();
        }

        public void AddGreenUI(int numAdd)
        {
            numGreen += numAdd;
            txtNumGreen.text = numGreen.ToString();
        }


        public void PlayBaseAnim()
        {
            
            animator.Play("BaseGame_Loop");
            
        }

        public void PlayLinkPreAnim()
        {
            if (numGreen >= 6)
            {
                animator.Play("Text");
            }
        }

    }
}