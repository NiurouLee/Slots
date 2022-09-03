//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-27 18:37
//  Ver : 1.0.0
//  Description : PiggyBankEntranceView.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class PiggyBankEntranceView: View<PiggyBankEntranceViewController>
    {
        public Animator Animator;
        public PiggyBankEntranceView(): base(null)
        {

        }

        protected override void OnViewSetUpped()
        {
            Animator = transform.GetComponent<Animator>();
            base.OnViewSetUpped();
        }

        protected override void EnableView()
        {
            if (!viewController.PiggyBankController.IsLocked)
            {
                base.EnableView();
            }
        }
    }

    public class PiggyBankEntranceViewController : ViewController<PiggyBankEntranceView>
    {
        private ulong nLastPiggyCoin;
        private bool isPiggyFull = false;
        public PiggyBankController PiggyBankController;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            PiggyBankController = Client.Get<PiggyBankController>();
            EnableUpdate(1);
            InitPiggyState();
            view.transform.GetComponent<Button>().onClick.AddListener(OnBtnPiggyBankClicked);
            view.Hide();
        }

        private async void InitPiggyState()
        {
            if (PiggyBankController.IsLocked) return;
            nLastPiggyCoin = PiggyBankController.CurrentCoins;
            isPiggyFull = PiggyBankController.IsPiggyFull;
            await XUtility.WaitSeconds(0.1f, this);
            XUtility.PlayAnimation(view.Animator, isPiggyFull ? "Full":"Idle");
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSpinSuccess>(OnSpinSuccess);
            SubscribeEvent<EventPiggyUnLock>(OnPiggyUnLock);
        }

        private void OnPiggyUnLock(EventPiggyUnLock evt)
        {
            if (!PiggyBankController.IsLocked)
            {
                view.Show();
            }
        }

        public override void Update()
        {
            base.Update();
            if (PiggyBankController.IsLocked) return;
            var tmpPiggyFull = PiggyBankController.IsPiggyFull;
            if (!tmpPiggyFull && isPiggyFull)
            {
                nLastPiggyCoin = PiggyBankController.CurrentCoins;
                XUtility.PlayAnimation(view.Animator, "Idle");
                isPiggyFull = false;
            }
        }

        public override void OnViewDestroy()
        {
            base.OnViewDestroy();
            UnsubscribeEvent<EventSpinSuccess>(OnSpinSuccess);
        }
        
        private async void OnSpinSuccess(EventSpinSuccess evt)
        {
            if (PiggyBankController.IsLocked) return;
            ulong nNewPiggyCoins = Client.Get<PiggyBankController>().CurrentCoins;
            if (nLastPiggyCoin < nNewPiggyCoins)
            {
                nLastPiggyCoin = nNewPiggyCoins;
                await XUtility.PlayAnimationAsync(view.Animator, "Collect",this);
                if (!isPiggyFull && Client.Get<PiggyBankController>().IsPiggyFull)
                {
                    XUtility.PlayAnimation(view.Animator, "Full");
                    isPiggyFull = true;
                }   
            }
        }
        
        private void OnBtnPiggyBankClicked()
        {
            SoundController.PlayButtonClick();
            if (Client.Get<PiggyBankController>().IsLocked) return;
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(PiggyBankMainPopup), "TopPanel")));
        }
    }
}