// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-05-20 3:52 PM
// Ver : 1.0.0
// Description : LocalStorableValue.cs
// 本地可存储的Long变量
// 方便玩家某些状态的存储和使用，让代码逻辑更加简洁
// 实现变量自动更新，自动加载
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class LocalStorableLong
    {
        private long value;
        private string storeName;
        private long defaultValue;
        public LocalStorableLong(string inStoreName, int inDefaultValue)
        {
            storeName = inStoreName;
             
            // if (Client.Player.IsValid())
            // {
            //     storeName = Client.Player.Id + "_" + storeName;
            // }
            
            defaultValue = inDefaultValue;
            
            LoadValue();
        }

        public long Value
        {
            get => value;
            set
            {
                this.value = value;
                Client.Storage.SetItem(storeName, value.ToString());
            }
        }
        
        public void LoadValue()
        {
            value = System.Convert.ToInt64(Client.Storage.GetItem(storeName, defaultValue.ToString()));
        }
        
        public static implicit operator long (LocalStorableLong m1)
        {
            return m1.value;
        }
        
        public void ResetValue()
        {
            value = defaultValue;
            Client.Storage.DeleteItem(storeName);
        }
    }
}