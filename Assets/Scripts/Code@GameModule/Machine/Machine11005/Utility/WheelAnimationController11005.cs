namespace GameModule
{
    public class WheelAnimationController11005 : WheelAnimationController
    {

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
                   
                    

                    wheel.AttachGameObject("AnticipationAnimation", anticipationAnimationGo);
                }
            }

            if (anticipationAnimationGo)
            {
                // AudioUtil.Instance.StopAudioFx(wheelState.GetAnticipationSoundAssetName());
                // AudioUtil.Instance.PlayAudioFx(wheelState.GetAnticipationSoundAssetName());

                if (!anticipationAnimationGo.activeSelf)
                {
                    anticipationAnimationGo.SetActive(true);
                }

                anticipationAnimationGo.transform.localPosition = wheel.GetAnticipationAnimationPosition(rollIndex);
                AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
                AudioUtil.Instance.PlayAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());

            }
        }
    }
}