using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule{
    public class WheelAnimationController11008 : WheelAnimationController
    {
        public override void OnWheelStartSpinning()
        {
            base.OnWheelStartSpinning();
            var bgView = wheel.GetContext().view.Get<BgView11008>();
            var baseView = wheel.GetContext().view.Get<WheelBaseGame11008>();
            var freeView = wheel.GetContext().view.Get<WheelFreeGame11008>();
            var freeSpin = wheel.GetContext().state.Get<FreeSpinState>();
            bgView.PlayBgViewAnim("Base");
            if(freeSpin.IsOver){
                baseView.PlayBgViewAnim("Base");
            }else{
                freeView.PlayBgViewAnim("Base");
            }
                
        }
        public override void ShowAnticipationAnimation(int rollIndex)
        {
            //  XDebug.Log("ShowDrum:" + rollView.RollIndex);

            var wheelState = wheel.wheelState;

            var anticipationAnimationGo = wheel.GetAttachedGameObject("AnticipationAnimation");

            if (wheel.GetAttachedGameObject("AnticipationAnimation") == null)
            {
                // wheel.AttachGameObject(); =
                anticipationAnimationGo =
                    assetProvider.InstantiateGameObject(wheelState.GetAnticipationAnimationAssetName());

                if (anticipationAnimationGo)
                {
                    anticipationAnimationGo.transform.SetParent(wheel.transform.Find("Rolls"), false);

                    if (!anticipationAnimationGo.GetComponent<SortingGroup>())
                    {
                        var sortingGroup = anticipationAnimationGo.AddComponent<SortingGroup>();
                        sortingGroup.sortingLayerID = SortingLayer.NameToID("Wheel");
                        sortingGroup.sortingOrder = 400;
                    }

                    wheel.AttachGameObject("AnticipationAnimation", anticipationAnimationGo);
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
                AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
                AudioUtil.Instance.PlayAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
            }
        }
    }
}   
