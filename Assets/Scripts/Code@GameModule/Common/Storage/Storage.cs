using UnityEngine;

namespace GameModule
{
    public class Storage : IStorage
    {
        public void SetItem(string key, int value)
        {
            PlayerPrefs.SetInt(key,value);
            PlayerPrefs.Save();
        }
        
        public void SetItem(string key, float value)
        {
            PlayerPrefs.SetFloat(key,value);
            PlayerPrefs.Save();
        }
        
        public void SetItem(string key, string value)
        {
            PlayerPrefs.SetString(key,value);
            PlayerPrefs.Save();
        }

        public int GetItem(string key, int defaultValue = 0)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key);
            }

            return defaultValue;
        }
        
        public float GetItem(string key, float defaultValue = 0.0f)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetFloat(key);
            }

            return defaultValue;
        }
        
        public string GetItem(string key, string defaultValue = "")
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetString(key);
            }

            return defaultValue;
        } 

        public bool HasItem(string key)
        {
           return PlayerPrefs.HasKey(key);
        }

        public void DeleteItem(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}