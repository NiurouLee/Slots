using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace GameModule
{
    public class FreeSpinStartPopUp11029 : FreeSpinStartPopUp
    {
        public FreeSpinStartPopUp11029(Transform transform) : base(transform)
        {
            
        }

        public override void OnOpen()
        {
	        var freeSpinState = context.state.Get<FreeSpinState>();
	        if (freeSpinState.freeSpinId == 0)
	        {
		        AudioUtil.Instance.PlayAudioFx("FreeGameStart_Open");
	        }
	        else if (freeSpinState.freeSpinId == 1)
	        {
		        AudioUtil.Instance.PlayAudioFx("FreeGameStart_Open");
	        }
	        else if (freeSpinState.freeSpinId == 2)
	        {
		        AudioUtil.Instance.PlayAudioFx("Map_ClassicStart_Open");
	        }
	        else if (freeSpinState.freeSpinId == 3 || freeSpinState.freeSpinId == 4 || freeSpinState.freeSpinId == 5)
	        {
		        AudioUtil.Instance.PlayAudioFx("Map_FreeGameStart_Open");
	        }
	        else
	        {
		       AudioUtil.Instance.PlayAudioFx("Map_FreeGameStart_Open");
	        }

	        if (animator != null && animator.HasState("Open"))
		        animator.Play("Open");
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
	        var freeSpinState = context.state.Get<FreeSpinState>();
            var _extraState = context.state.Get<ExtraState11029>();
            if (freeSpinState.freeSpinId == 3)
            {
                transform.Find("Root/MainGroup/Point1").transform.gameObject.SetActive(true);
                transform.Find("Root/MainGroup/Point3").transform.gameObject.SetActive(false);
                transform.Find("Root/MainGroup/Point5").transform.gameObject.SetActive(false);
            }
            else if (freeSpinState.freeSpinId == 4)
            {
                transform.Find("Root/MainGroup/Point1").transform.gameObject.SetActive(false);
                transform.Find("Root/MainGroup/Point3").transform.gameObject.SetActive(true);
                transform.Find("Root/MainGroup/Point5").transform.gameObject.SetActive(false);
            }
            else if (freeSpinState.freeSpinId == 5)
            {
                transform.Find("Root/MainGroup/Point1").transform.gameObject.SetActive(false);
                transform.Find("Root/MainGroup/Point3").transform.gameObject.SetActive(false);
                transform.Find("Root/MainGroup/Point5").transform.gameObject.SetActive(true);
            }
            else if (freeSpinState.freeSpinId == 6)
            {
                transform.Find("Root/MainGroup/ContentGroup/BG1/Point2").transform.gameObject.SetActive(true);
                transform.Find("Root/MainGroup/ContentGroup/BG1/Point4").transform.gameObject.SetActive(false);
                transform.Find("Root/MainGroup/ContentGroup/BG1/Point6").transform.gameObject.SetActive(false);
            }
            else if (freeSpinState.freeSpinId == 7)
            {
                transform.Find("Root/MainGroup/ContentGroup/BG1/Point2").transform.gameObject.SetActive(false);
                transform.Find("Root/MainGroup/ContentGroup/BG1/Point4").transform.gameObject.SetActive(true);
                transform.Find("Root/MainGroup/ContentGroup/BG1/Point6").transform.gameObject.SetActive(false);
            }
            else if (freeSpinState.freeSpinId == 8)
            {
                transform.Find("Root/MainGroup/ContentGroup/BG1/Point2").transform.gameObject.SetActive(false);
                transform.Find("Root/MainGroup/ContentGroup/BG1/Point4").transform.gameObject.SetActive(false);
                transform.Find("Root/MainGroup/ContentGroup/BG1/Point6").transform.gameObject.SetActive(true);
            }
        }
    }
}