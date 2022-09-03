// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/24/18:25
// Ver : 1.0.0
// Description : SpinDataRecored.cs
// ChangeLog :
// **********************************************

#if UNITY_EDITOR || !PRODUCTION_PACKAGE

using System.Collections.Generic;
using System.Linq;
using Google.ilruntime.Protobuf;
using UnityEngine.UI;

namespace GameModule
{
    public static class SpinDataRecord
    {
        private static Dictionary<string, List<ByteString>>  recordMap;

        public static string activeFeature;

        public static bool isRecording = false;
        public static bool usingRecord = false;

        public static int currentIndex = 0;

        public static string recordFileName;

        public static string gameRecordPath = "record/";
        
        public static void SaveRecord(string name = null)
        {
            // if (name == null)
            // {
            //     name = recordFileName;
            // }
            // var tempSerializableRecordMap = new Dictionary<string, List<byte[]>>();
            var keyList = recordMap.Keys.ToList();
            for (var i = 0; i < keyList.Count; i++)
            {
                var key = keyList[i];
                var tempByteList = new List<byte[]>();
                var byteList = recordMap[key];
                for (var j = 0; j < byteList.Count; j++)
                {
                    tempByteList.Add(byteList[j].bytes);
                }
                SaveData.Save< List<byte[]>>(tempByteList, key,gameRecordPath);
                // tempSerializableRecordMap[key] = tempByteList;
            }
            // SaveData.Save<Dictionary<string, List<byte[]>>>(tempSerializableRecordMap, name);
        }

        public static void RemoveRecord(string activeName)
        {
            SaveData.Remove(activeName, gameRecordPath);
            ClearRecord(activeName);
        }
        
        public static void LoadRecord(string name = null)
        {
            // recordFileName = name;
            recordMap = new Dictionary<string, List<ByteString>>();
            // var tempSerializableRecordMap = SaveData.Load<Dictionary<string, List<byte[]>>>(name);
            var fileNameList = SaveData.GetFileNameList(gameRecordPath);
            for (var i = 0; i < fileNameList.Count; i++)
            {
                var key = fileNameList[i];
                var tempByteList = SaveData.Load<List<byte[]>>(key,gameRecordPath);
                var byteList = new List<ByteString>();
                for (var j = 0; j < tempByteList.Count; j++)
                {
                    byteList.Add(new ByteString(tempByteList[j]));
                }
                recordMap[key] = byteList;
            }
        }

        public static List<Dropdown.OptionData> GetAvailableRecord()
        {
            if (recordMap == null)
            {
                recordMap = new Dictionary<string, List<ByteString>>();
            }

            var list = recordMap.Keys.ToList();

            var listOption = new List<Dropdown.OptionData>();
         
            for (var i = 0; i < list.Count; i++)
            {
                listOption.Add(new Dropdown.OptionData(list[i]));
            }

            return listOption;
        }

        public static void ClearRecord(string recordName)
        {
            if (recordMap == null)
            {
                recordMap = new Dictionary<string, List<ByteString>>();
                return;
            }

            if (recordMap.ContainsKey(recordName))
            {
                recordMap.Remove(recordName);
            }
        }

        public static void Record(IMessage message)
        {
            if (recordMap == null)
            {
                recordMap = new Dictionary<string, List<ByteString>>();
            }

            if (!recordMap.ContainsKey(activeFeature))
            {
                recordMap[activeFeature] = new List<ByteString>();
            }

            recordMap[activeFeature].Add(message.ToByteString());
            
            // SaveRecord();
        }

        public  static T GetRecord<T>() where T : class, IMessage, new()
        {
            if (recordMap == null)
            {
                recordMap = new Dictionary<string, List<ByteString>>();
            }

            if (recordMap.ContainsKey(activeFeature) && recordMap[activeFeature].Count > currentIndex)
            {
                T record = new T();
                record.MergeFrom(recordMap[activeFeature][currentIndex++]);
                UIDebuggerElementSpinRecord.Instance.RefreshUsingRecordLeftCount();
                return record;
            }

            currentIndex = 0;
            SpinDataRecord.usingRecord = false;
            return null;
        }

        public static int GetLeftDataCount()
        {
            if (usingRecord && recordMap.ContainsKey(activeFeature))
            {
                return recordMap[activeFeature].Count - currentIndex;
            }
            return -1;
        }
    }
}

#endif