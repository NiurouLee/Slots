using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TransitionView11009: TransformHolder
    {

        [ComponentBinder("Green")]
        protected Transform tranGreen1;
        
        [ComponentBinder("Gules")]
        protected Transform tranRed1;
        
        [ComponentBinder("Violet")]
        protected Transform tranPurple1;
        
        [ComponentBinder("Golden")]
        protected Transform tranGolden1;
        
        
        
        [ComponentBinder("Green2")]
        protected Transform tranGreen2;
        
        [ComponentBinder("Gules2")]
        protected Transform tranRed2;
        
        [ComponentBinder("Violet2")]
        protected Transform tranPurple2;
        
        [ComponentBinder("Golden2")]
        protected Transform tranGolden2;


        
        protected Animator animator;
        
        public TransitionView11009(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
            animator = transform.GetComponent<Animator>();
        }


        


        public async Task RefreshUI()
        {
            this.transform.gameObject.SetActive(true);
            var extraInfo = context.state.Get<ExtraState11009>();

            bool isGreen = extraInfo.GetFreeGreenState();
            bool isRed = extraInfo.GetFreeRedState();
            bool isPurple = extraInfo.GetFreePurpleState();

            tranGreen1.gameObject.SetActive(false);
            tranRed1.gameObject.SetActive(false);
            tranPurple1.gameObject.SetActive(false);
            tranGolden1.gameObject.SetActive(false);
            tranGreen2.gameObject.SetActive(false);
            tranRed2.gameObject.SetActive(false);
            tranPurple2.gameObject.SetActive(false);
            tranGolden2.gameObject.SetActive(false);
            
            
            if (isGreen && isRed && isPurple)
            {
                //绿红紫
                tranGolden1.gameObject.SetActive(true);
                tranGolden2.gameObject.SetActive(true);
            }
            else if(isGreen && isRed)
            {
                //绿红
                tranGreen1.gameObject.SetActive(true);
                tranRed2.gameObject.SetActive(true);
            }
            else if(isRed && isPurple)
            {
                //红紫
                tranRed1.gameObject.SetActive(true);
                tranPurple2.gameObject.SetActive(true);
            }
            else if(isGreen && isPurple)
            {
                //绿紫
                tranGreen1.gameObject.SetActive(true);
                tranPurple2.gameObject.SetActive(true);
            }
            else if(isGreen)
            {
                //绿
                tranGreen1.gameObject.SetActive(true);
                tranGreen2.gameObject.SetActive(true);
            }
            else if(isRed)
            {
                //红
                tranRed1.gameObject.SetActive(true);
                tranRed2.gameObject.SetActive(true);
            }
            else if (isPurple)
            {
                //紫
                tranPurple1.gameObject.SetActive(true);
                tranPurple2.gameObject.SetActive(true);
            }

            AudioUtil.Instance.PlayAudioFx("Video");
            await XUtility.PlayAnimationAsync(animator, "Transition",context);
            this.transform.gameObject.SetActive(false);
        }
    }
}