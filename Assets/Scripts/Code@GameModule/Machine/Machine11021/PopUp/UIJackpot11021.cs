using UnityEngine;

namespace GameModule
{
    public class UIJackpot11021:UIJackpotBase
    {

        [ComponentBinder("BonusImage")]
        protected Transform tranBonusPic;

        [ComponentBinder("JackpotImage")]
        protected Transform tranJackpotPic;
        
        public UIJackpot11021(Transform inTransform) : base(inTransform)
        {
        }



        public void SetIsJackpot(bool isJackpot)
        {
            tranBonusPic.gameObject.SetActive(!isJackpot);
            tranJackpotPic.gameObject.SetActive(isJackpot);
        }
    }
}