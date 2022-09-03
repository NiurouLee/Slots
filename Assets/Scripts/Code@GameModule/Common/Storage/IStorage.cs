namespace GameModule
{
    public interface IStorage
    {
        void SetItem(string key, int value);
        void SetItem(string key, float value);
        void SetItem(string key, string value);
        int GetItem(string key, int defaultValue = 0);
        float GetItem(string key, float defaultValue = 0.0f);
        string GetItem(string key, string defaultValue = "");
        bool HasItem(string key);
        void DeleteItem(string key);
        void DeleteAll();
    }
}