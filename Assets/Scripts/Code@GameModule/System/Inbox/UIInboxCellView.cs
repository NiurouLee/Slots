
namespace GameModule
{
    public abstract class UIInboxCellView : View<UIInboxCellViewController>
    {
        protected InboxItem itemData;

        public virtual void Set(InboxItem inItemData)
        {
            itemData = inItemData;
            
            ParseData();
            UpdateView();
        }

        public virtual void ParseData()
        {
            
        }
        
        public virtual void UpdateView()
        {
            
        }

        public virtual void Update() { }
    }

    public class UIInboxCellViewController : ViewController<UIInboxCellView>
    {
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            
            EnableUpdate(2);
            
            view.Update();
        }

        public override void Update()
        {
            view.Update();
        }
    }
}
