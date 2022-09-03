using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;
using UnityEngine.UI;

namespace GameModule
{
	public class UISelectionFeature11022 : MachinePopUp
	{
		private int nChooseIndex;
		[ComponentBinder("Root/MainGroup/Safebox/Numberofrotations/num")]
		private Text LinkSpins;

		[ComponentBinder("Root/MainGroup/FreeGames/num")]
		private Text FreeSpins;
		
		[ComponentBinder("Root/MainGroup/Safebox")]
		private Button LinkButton;
		
		[ComponentBinder("Root/MainGroup/FreeGames")]
		private Button FreeButton;
		
		private Action<int> _chooseAction;
		public UISelectionFeature11022(Transform transform):base(transform)
		{
			LinkButton.onClick.AddListener(OnLinkBtnClicked);
			FreeButton.onClick.AddListener(OnFreeBtnClicked);
		}
		public override void Initialize(MachineContext inContext)
		{
			base.Initialize(inContext);

			var LinkSpinCount = inContext.state.Get<ExtraState11022>().GetChoosingReSpinCount();
			var FreeSpinCount = inContext.state.Get<ExtraState11022>().GetChoosingFreeSpinCount();
			LinkSpins.text = LinkSpinCount.ToString();
			FreeSpins.text = FreeSpinCount.ToString();
		}
		public async void OnLinkBtnClicked()
		{
			AudioUtil.Instance.StopMusic();
			LinkButton.interactable = false;
			FreeButton.interactable = false;
			
			nChooseIndex = 0;
			AudioUtil.Instance.PlayAudioFx("SelectionFeatureStart_Selected");
			// context.WaitSeconds(1f, () =>
			// {
			// 	AudioUtil.Instance.PlayAudioFx("Close");
			// });
			Close();
		}
		public async void OnFreeBtnClicked()
		{
			AudioUtil.Instance.StopMusic();
			LinkButton.interactable = false;
			FreeButton.interactable = false;
			
			nChooseIndex = 1;
			AudioUtil.Instance.PlayAudioFx("SelectionFeatureStart_Selected");
			// context.WaitSeconds(1f, () =>
			// {
			// 	AudioUtil.Instance.PlayAudioFx("Close");
			// });
			Close();
		}

		public override void OnOpen()
		{
			AudioUtil.Instance.PlayAudioFx("SelectionFeatureStart_Open");
			base.OnOpen();
		}
		public void SetChooseCallback(Action<int> chooseCallback)
		{
			_chooseAction = chooseCallback;
		}
		public  override async Task OnClose()
		{
			if (nChooseIndex == 0 && animator && animator.HasState("SafeBox"))
				await XUtility.PlayAnimationAsync(animator, "SafeBox",context);
			if (nChooseIndex == 1 && animator && animator.HasState("FreeGame"))
				await XUtility.PlayAnimationAsync(animator, "FreeGame",context);
			if (animator && animator.HasState("Close"))
				await XUtility.PlayAnimationAsync(animator, "Close",context);
			_chooseAction?.Invoke(nChooseIndex);
		}
	}
}