using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;
using UnityEngine.UI;

namespace GameModule
{
	public class UISelectionFeature11030 : MachinePopUp
	{
		private int nChooseIndex;

		[ComponentBinder("Root/MainGroup/FreeGameButton/CountText")]
		private Text FreeSpins;
		
		[ComponentBinder("Root/MainGroup/LineButton")]
		private Button LinkButton;
		
		[ComponentBinder("Root/MainGroup/FreeGameButton")]
		private Button FreeButton;
		
		private Action<int> _chooseAction;
		public UISelectionFeature11030(Transform transform):base(transform)
		{
			LinkButton.onClick.AddListener(OnLinkBtnClicked);
			FreeButton.onClick.AddListener(OnFreeBtnClicked);
		}
		public override void Initialize(MachineContext inContext)
		{
			base.Initialize(inContext);

			var FreeSpinCount = inContext.state.Get<ExtraState11030>().GetFreeSpinCount();
			FreeSpins.text = FreeSpinCount.ToString();
		}
		public async void OnLinkBtnClicked()
		{
			LinkButton.interactable = false;
			FreeButton.interactable = false;
			
			nChooseIndex = 0;
			AudioUtil.Instance.PlayAudioFx("SelectOneFeature");
			// context.WaitSeconds(1f, () =>
			// {
			// 	AudioUtil.Instance.PlayAudioFx("Close");
			// });
			Close();
		}
		public async void OnFreeBtnClicked()
		{
			LinkButton.interactable = false;
			FreeButton.interactable = false;
			
			nChooseIndex = 1;
			AudioUtil.Instance.PlayAudioFx("SelectOneFeature");
			// context.WaitSeconds(1f, () =>
			// {
			// 	AudioUtil.Instance.PlayAudioFx("Close");
			// });
			Close();
		}

		public override void OnOpen()
		{
			base.OnOpen();
		}
		public void SetChooseCallback(Action<int> chooseCallback)
		{
			_chooseAction = chooseCallback;
		}
		public  override async Task OnClose()
		{
			if (nChooseIndex == 0 && animator && animator.HasState("Line"))
			{
				await XUtility.PlayAnimationAsync(animator, "Line",context);
				_chooseAction?.Invoke(nChooseIndex);
				await XUtility.PlayAnimationAsync(animator, "LineClose",context);
			}
			if (nChooseIndex == 1 && animator && animator.HasState("FreeGame"))
			{
				await XUtility.PlayAnimationAsync(animator, "FreeGame",context);
				_chooseAction?.Invoke(nChooseIndex);
				await XUtility.PlayAnimationAsync(animator, "FGClose",context);
			}
		}
	}
}