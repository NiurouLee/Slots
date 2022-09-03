using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Dlugin;
using UnityEditor;

public class SaveData
{    
    public static string defaultPath = "/saves/";
    /// <summary>
    /// Save object with a string identifier
    /// </summary>
    /// <typeparam name="T">Type of object to save</typeparam>
    /// <param name="objectToSave">Object to save</param>
    /// <param name="key">String identifier for the data to load</param>
    /// <param name="internalPath">String identifier for the internal path</param>
    public static void Save<T>(T objectToSave, string key,string internalPath="")
    {
        SaveToFile<T>(objectToSave, key,internalPath);
    }

    /// <summary>
    /// Save object with a string identifier
    /// </summary>
    /// <param name="objectToSave">Object to save</param>
    /// <param name="key">String identifier for the data to load</param>
    /// <param name="internalPath">String identifier for the internal path</param>
    public static void Save(Object objectToSave, string key,string internalPath="")
    {
        SaveToFile<Object>(objectToSave, key,internalPath);
    }

    public static string GetBasePath()
    {
#if UNITY_EDITOR
        return EditorApplication.applicationPath;
#else
        return Application.persistentDataPath;
#endif
    }

    /// <summary>
    /// Handle saving data to File
    /// </summary>
    /// <typeparam name="T">Type of object to save</typeparam>
    /// <param name="objectToSave">Object to serialize</param>
    /// <param name="fileName">Name of file to save to</param>
    /// <param name="internalPath">internal path of file to save to</param>
    private static void SaveToFile<T>(T objectToSave, string fileName,string internalPath = "")
    {
        // Set the path to the persistent data path (works on most devices by default)
        string path = GetBasePath() + defaultPath + internalPath;
        // Create the directory IF it doesn't already exist
        Directory.CreateDirectory(path);
        // Grab an instance of the BinaryFormatter that will handle serializing our data
        BinaryFormatter formatter = new BinaryFormatter();
        // Open up a filestream, combining the path and object key
        FileStream fileStream = new FileStream(path + fileName + ".txt", FileMode.Create);

        // Try/Catch/Finally block that will attempt to serialize/write-to-stream, closing stream when complete
        try
        {
            formatter.Serialize(fileStream, objectToSave);
        }
        catch (SerializationException exception)
        {
            Debug.Log("Save failed. Error: " + exception.Message);
        }
        finally
        {
            fileStream.Close();
        }
    }

    public static List<string> GetFileNameList(string internalPath = "")
    {
        string path = GetBasePath() + defaultPath + internalPath;
        if (Directory.Exists(path))
        {
            var fileNameList = Directory.GetFiles(path);
            for (var i = 0; i < fileNameList.Length; i++)
            {
                var tempFileName = fileNameList[i];
                tempFileName = tempFileName.Remove(0, path.Length);
                tempFileName = tempFileName.Remove(tempFileName.Length - 4, 4);
                fileNameList[i] = tempFileName;
            }
            return fileNameList.ToList();   
        }
        return new List<string>();
    }

    public static bool Remove(string fileName,string internalPath = "")
    {
        string path = GetBasePath() + defaultPath + internalPath;
        string fileFullPath = path + fileName + ".txt";
        if (File.Exists(fileFullPath))
        {
            File.Delete(fileFullPath);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Load data using a string identifier
    /// </summary>
    /// <typeparam name="T">Type of object to load</typeparam>
    /// <param name="key">String identifier for the data to load</param>
    /// <param name="internalPath">internal path of data to load</param>
    /// <returns></returns>
    public static T Load<T>(string key,string internalPath = "") where T: new()
    {
        // Set the path to the persistent data path (works on most devices by default)
        string path = GetBasePath() + defaultPath + internalPath;
        // Grab an instance of the BinaryFormatter that will handle serializing our data
        BinaryFormatter formatter = new BinaryFormatter();

        if (!File.Exists(path + key + ".txt"))
        {
            return new T();
        }
        
        // Open up a filestream, combining the path and object key
        FileStream fileStream = new FileStream(path + key + ".txt", FileMode.Open);
        // Initialize a variable with the default value of whatever type we're using
        T returnValue = default(T);
        /* 
         * Try/Catch/Finally block that will attempt to deserialize the data
         * If we fail to successfully deserialize the data, we'll just return the default value for Type
         */
        try
        {
            returnValue = (T)formatter.Deserialize(fileStream);
        }
        catch (SerializationException exception)
        {
            Debug.Log("Load failed. Error: " + exception.Message);
        }
        finally
        {
            fileStream.Close();
        }

        return returnValue;
    }
}