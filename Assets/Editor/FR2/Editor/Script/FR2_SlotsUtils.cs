using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace vietlabs.fr2
{
    public class FR2_SlotsUtils
    {
        public static void ReCreateCache()
        {
            FR2_Cache.DeleteCache();
            FR2_Cache.CreateCache();
        }

        public static Dictionary<string, FR2_Ref> FindUsage(string [] guids)
        {
            Dictionary<string, FR2_Ref> guid2refDict1 = FR2_Ref.FindUsage(guids);
            List<string> folderGuids = new List<string>();
            foreach(KeyValuePair<string, FR2_Ref> entry in guid2refDict1)
            {
                string guid = entry.Key;
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    folderGuids.Add(guid);
                }
            }
            foreach(string guid in folderGuids)
            {
                guid2refDict1.Remove(guid);
            }
            Dictionary<string, FR2_Ref> guid2refDict2 = FR2_Ref.FindUsage(folderGuids.ToArray());
            var merged = guid2refDict1.Concat(guid2refDict2)
            .ToLookup(x => x.Key, x => x.Value)
            .ToDictionary(x => x.Key, g => g.First());
            return merged;
        }

        public static Dictionary<string, FR2_Ref> FindUsedBy(string[] guids)
        {
            Dictionary<string, FR2_Ref> guid2refDict1 = FR2_Ref.FindUsedBy(guids);
            List<string> folderGuids = new List<string>();
            foreach (KeyValuePair<string, FR2_Ref> entry in guid2refDict1)
            {
                string guid = entry.Key;
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    folderGuids.Add(guid);
                }
            }
            foreach (string guid in folderGuids)
            {
                guid2refDict1.Remove(guid);
            }
            Dictionary<string, FR2_Ref> guid2refDict2 = FR2_Ref.FindUsedBy(folderGuids.ToArray());
            var merged = guid2refDict1.Concat(guid2refDict2)
            .ToLookup(x => x.Key, x => x.Value)
            .ToDictionary(x => x.Key, g => g.First());
            return merged;
        }
    }
}
