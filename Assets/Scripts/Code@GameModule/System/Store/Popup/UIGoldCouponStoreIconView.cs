
using TMPro;

namespace GameModule
{
    public class UIGoldCouponStoreIconView : View
    {
        [ComponentBinder("ContentGroup/PercentageText")]
        public TMP_Text tmpTextPercent;

        public void Set(float count)
        {
            if (tmpTextPercent != null)
            {
                tmpTextPercent.SetText(count + "%");
            }
        }
    }
}