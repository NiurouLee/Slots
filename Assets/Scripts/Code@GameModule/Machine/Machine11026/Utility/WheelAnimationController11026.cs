using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelAnimationController11026: WheelAnimationController
    {
        private string anticipationName = string.Empty;
        public override async void OnWheelStartSpinning()
        {
            base.OnWheelStartSpinning(); 
            await wheel.GetContext().WaitSeconds(0.1f); 
            wheel.GetContext().state.Get<WheelsActiveState11026>().ShowRollsMasks(wheel);
        }

        public override void OnRollStartBounceBack(int rollIndex)
        {
            base.OnRollStartBounceBack((rollIndex));
            wheel.GetContext().state.Get<WheelsActiveState11026>().FadeOutRollMask(wheel, rollIndex);
        }
        
        public override void ShowAnticipationAnimation(int rollIndex)
        {
            //  XDebug.Log("ShowDrum:" + rollView.RollIndex);

            var wheelState = wheel.wheelState;

            var anticipationAnimationGo = wheel.GetAttachedGameObject(GetAnticipationName(rollIndex));

             if (wheel.GetAttachedGameObject(GetAnticipationName(rollIndex)) == null)
            {
                // wheel.AttachGameObject(); =
                anticipationAnimationGo =
                     assetProvider.InstantiateGameObject(GetAnticipationName(rollIndex));

                if (anticipationAnimationGo)
                {
                    anticipationAnimationGo.transform.SetParent(wheel.transform.Find("Rolls"), false);
                    if (!anticipationAnimationGo.GetComponent<SortingGroup>())
                    {
                        var sortingGroup = anticipationAnimationGo.AddComponent<SortingGroup>();
                        sortingGroup.sortingLayerID = SortingLayer.NameToID("Element");
                        sortingGroup.sortingOrder = 400;
                    }

                    wheel.AttachGameObject(GetAnticipationName(rollIndex), anticipationAnimationGo);
                }
            }

            if (anticipationAnimationGo)
            {
                // AudioUtil.Instance.StopAudioFx(wheelState.GetAnticipationSoundAssetName());
                // AudioUtil.Instance.PlayAudioFx(wheelState.GetAnticipationSoundAssetName());

                anticipationAnimationGo.SetActive(false);
                // if (!anticipationAnimationGo.activeSelf)
                // {
                    anticipationAnimationGo.SetActive(true);
                //}

                XDebug.Log("PlayAnticipationSound:" + wheel.wheelState.GetAnticipationSoundAssetName());
                
                anticipationAnimationGo.transform.localPosition = wheel.GetAnticipationAnimationPosition(rollIndex);
                 if (rollIndex == 0 || rollIndex == 4) 
                 { 
                     anticipationAnimationGo.transform.localScale = new Vector3(1, 1, 1);
                 }
                 else 
                 { 
                     anticipationAnimationGo.transform.localScale = new Vector3(1, 1.32f, 1);
                 }
                 AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName()); 
                 AudioUtil.Instance.PlayAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
            }
        }
        
        public override void StopAnticipationAnimation(bool playStopSound = true)
        {
            var anticipationAnimationGo = wheel.GetAttachedGameObject("AnticipationAnimation");
            var anticipationAnimationGoJ01 = wheel.GetAttachedGameObject("AnticipationAnimationJ01");

            if (anticipationAnimationGo != null)
            {
                anticipationAnimationGo.SetActive(false);
                AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
            }
            
            if (anticipationAnimationGoJ01 != null)
            {
                anticipationAnimationGoJ01.SetActive(false);
                AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
            }
            
        }
        
        private string GetAnticipationName(int rollIndex)
        {
            int rapidCount = 0;
            int scatterCount = 0;
            for (int i = 0; i < rollIndex; i++)
            {
                var animationInfo = wheel.wheelState.GetBlinkAnimationInfo(i);
                for (int j = 0; j < animationInfo.Count; j++)
                {
                    var container = wheel.GetRoll(i).GetVisibleContainer((int) animationInfo[j]);
                    if (Constant11026.B01ElementId == container.sequenceElement.config.id)
                    {
                        scatterCount++;
                    }

                    if (Constant11026.ListBonusAllElementIds.Contains(container.sequenceElement.config.id))
                    {
                        rapidCount++;
                    }
                }
            }

            if (string.IsNullOrEmpty(anticipationName))
            {
                anticipationName = scatterCount >= 2 ? "AnticipationAnimation" : "AnticipationAnimationJ01";
            }
            return anticipationName;
        }
    }
}
