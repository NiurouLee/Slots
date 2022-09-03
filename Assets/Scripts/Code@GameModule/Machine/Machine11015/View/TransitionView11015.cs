using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TransitionView11015 : TransformHolder
    {

        protected Animator animatorZeus;
        protected MeshRenderer meshZeus;
        protected Transform tranGroupZeus;

        protected Animator animatorIcon;

        protected Animator animatorTransition;

        protected Transform tranBaseBackground;
        protected Transform tranFreeBackground;
        
        
        
        public TransitionView11015(Transform inTransform) : base(inTransform)
        {
            tranGroupZeus = transform.Find("Wheels/WheelBaseGame/GroupZeus");
            animatorZeus = transform.Find("Wheels/WheelBaseGame/GroupZeus/Zeus").GetComponent<Animator>();
            meshZeus = animatorZeus.transform.Find("SpineZeus").GetComponent<MeshRenderer>();

            tranBaseBackground = transform.Find("BackgroundGroup/BaseBackground");
            tranFreeBackground = transform.Find("BackgroundGroup/FreeBackground");

            animatorIcon = transform.Find("Wheels/WheelBaseGame/GroupIcon/Icon").GetComponent<Animator>();
            animatorTransition = transform.Find("Transition").GetComponent<Animator>();
        }
        
        
        public async Task PlayZeusSuperior()
        {
            var pos = tranGroupZeus.localPosition;
            pos.x = 0;
            tranGroupZeus.localPosition = pos;
            meshZeus.sortingLayerID = SortingLayer.NameToID("Wheel");
            meshZeus.sortingOrder = 0;
            
            AudioUtil.Instance.PlayAudioFxOneShot("ZeusAudio");
            await XUtility.PlayAnimationAsync(animatorZeus, "Superior", context);
        }


        public async Task PlayZeusDown()
        {

            var wheel = context.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var listRoll = wheel.GetVisibleElementInfo();
            int firstWildWheel = -1;
            for (int x = 0; x < listRoll.Count; x++)
            {
                var roll = listRoll[x];
                for (int y = 0; y < roll.Count; y++)
                {
                    var element = roll[y];
                    if (element.config.id == Constant11015.ShieldElementId)
                    {
                        firstWildWheel = x;
                        break;
                    }
                }

                if (firstWildWheel != -1)
                {
                    break;
                }
            }

            
            meshZeus.sortingLayerID = SortingLayer.NameToID("LocalFx");
            meshZeus.sortingOrder = 30;


            var pos = tranGroupZeus.localPosition;
            

            if (firstWildWheel == 0 || firstWildWheel == 1)
            {
                pos.x = Constant11015.ZeusOffsetX * firstWildWheel;
                tranGroupZeus.localPosition = pos;
                await XUtility.PlayAnimationAsync(animatorZeus, "Left", context);
            }
            else
            {
                pos.x = -Constant11015.ZeusOffsetX * (listRoll.Count -1 -firstWildWheel);
                tranGroupZeus.localPosition = pos;
                await XUtility.PlayAnimationAsync(animatorZeus, "Right", context);
            }


        }
        
        
        public async Task PlayZeusAppear()
        {
            var pos = tranGroupZeus.localPosition;
            pos.x = 0;
            tranGroupZeus.localPosition = pos;
            meshZeus.sortingLayerID = SortingLayer.NameToID("Wheel");
            meshZeus.sortingOrder = 0;
            
            AudioUtil.Instance.PlayAudioFxOneShot("Zeus_Appear");
            await XUtility.PlayAnimationAsync(animatorZeus, "Appear", context);
        }


        public void OpenBaseBackground()
        {
            tranBaseBackground.gameObject.SetActive(true);
            tranFreeBackground.gameObject.SetActive(false);
        }
        
        public void OpenFreeBackground()
        {
            tranBaseBackground.gameObject.SetActive(false);
            tranFreeBackground.gameObject.SetActive(true);
        }

        public async void OpenIcon()
        {
            await XUtility.PlayAnimationAsync(animatorIcon, "Open");
            animatorIcon.Play("Logo",0,0);
        }
        
        
        public void CloseIcon()
        {
            animatorIcon.Play("Close",0,0);
        }

        public void OpenTransition()
        {
            
            animatorTransition.Play("Transition",0,0);
            
        }

    }
}