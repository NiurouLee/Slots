using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine.UI;

namespace GameModule
{
    public class UIInboxRewardCellView : View
    {
        [ComponentBinder("")]
        public TMP_Text tmpText;

        [ComponentBinder("Icon")]
        public Image tmpTextTitle;

        private string GetTimeString(uint minutes)
        {
            string result = null;
            if (minutes < 60) { result = $"{minutes}MIN"; }
            else
            {
                var hours = minutes / 60;
                var minutesRemain = minutes % 60;
                if (minutesRemain == 0) { result = $"{hours}H"; }
                else { result = $"{hours}H{minutesRemain}MIN"; }
            }
            return result;
        }

        public void Set(Item item)
        {
            if (item != null)
            {
                var type = item.Type;
                var sprite = XItemUtility.GetItemSprite(type, item);
                tmpTextTitle.sprite = sprite;

                switch (type)
                {
                    case Item.Types.Type.Coin:
                        tmpText.text = item.Coin?.Amount.GetCommaFormat();
                        break;
                    case Item.Types.Type.Emerald:
                        tmpText.text = item.Emerald?.Amount.GetCommaFormat();
                        break;
                    case Item.Types.Type.VipPoints:
                        tmpText.text = item.VipPoints?.Amount.GetCommaFormat();
                        break;
                    case Item.Types.Type.ShopGiftBox:
                        tmpText.text = item.ShopGiftBox?.Amount.GetCommaFormat();
                        break;
                    case Item.Types.Type.SuperWheel:
                        tmpText.text = GetTimeString(item.SuperWheel == null ? 0 : item.SuperWheel.Amount);
                        break;
                    case Item.Types.Type.DoubleExp:
                        tmpText.text = GetTimeString(item.DoubleExp == null ? 0 : item.DoubleExp.Amount);
                        break;
                    case Item.Types.Type.LevelUpBurst:
                        tmpText.text = GetTimeString(item.LevelUpBurst == null ? 0 : item.LevelUpBurst.Amount);
                        break;
                }
            }
        }
    }
}
