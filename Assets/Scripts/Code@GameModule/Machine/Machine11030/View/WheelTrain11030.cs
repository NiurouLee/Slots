using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class WheelTrain11030:Wheel
    {
        [ComponentBinder("LineFeature")] public Animator TrainLine;
        public bool showTrainLine;
        [ComponentBinder("BGGroup/BG5")] public Transform PurpleReel;
        public WheelTrain11030(Transform transform) : base(transform)
        {
            ComponentBinder.BindingComponent(this,transform);
            showTrainLine = false;
        }

        public void SetPurpleReel(bool activeFlag)
        {
            if (PurpleReel.gameObject.activeSelf != activeFlag)
            {
                PurpleReel.gameObject.SetActive(activeFlag);
            }
        }
        
        public async Task SetTrainLine(bool activeFlag)
        {
            if (showTrainLine != activeFlag)
            {
                showTrainLine = activeFlag;
                if (activeFlag)
                {
                    AudioUtil.Instance.PlayAudioFx("StopGo");
                    AudioUtil.Instance.PlayMusic(context.machineConfig.audioConfig.GetBonusBackgroundMusicName(),true,0,0.6f);
                    
                    TrainLine.gameObject.SetActive(true);
                    await XUtility.PlayAnimationAsync(TrainLine,"Start");
                }
                else
                {
                    AudioUtil.Instance.PlayAudioFx("AllowGo");
                    await XUtility.PlayAnimationAsync(TrainLine,"Close");
                    TrainLine.gameObject.SetActive(false);
                    AudioUtil.Instance.StopMusic();
                }
            }
        }
    }
}