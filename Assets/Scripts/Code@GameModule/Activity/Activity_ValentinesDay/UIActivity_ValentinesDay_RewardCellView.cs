using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class UIActivity_ValentinesDay_RewardCellView : View
    {
        public void Set(Item item)
        {
            if (item == null) { return; }
            XItemUtility.InitItemUI(
                       transform.parent,
                       item,
                       null,
                       transform.name);
        }
    }
}