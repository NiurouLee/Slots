using UnityEngine.UI;

namespace GameModule
{
    public class UICrazeSmashGetHammerPopup : Popup
    {
        [ComponentBinder("Root/ConutGroup/CountText")]
        public Text textCount;

        public UICrazeSmashGetHammerPopup(string address) : base(address) { }

        public void Set(uint count)
        {
            if (textCount != null)
            {
                textCount.text = $"+{count}";
            }
        }
    }
}