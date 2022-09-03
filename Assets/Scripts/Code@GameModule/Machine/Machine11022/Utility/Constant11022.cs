using System.Collections.Generic;
using SRF;
using UnityEngine;

namespace GameModule
{
    public class Constant11022
    {
        public static bool debugType = false;
        public static string NormalLinkReels = "NormalLinkReels";
        public static string SelectLinkReels = "SelectLinkReels";
        public static string SelectInitialLinkReels = "SelectInitialLinkReels";
        public static Dictionary<int,string> FreeReelsNameDictionary = new Dictionary<int,string>()
        {
            {3,"Free3Reels"},
            {4,"Free4Reels"},
            {5,"Free5Reels"},
        };
        public static Dictionary<string,int> SingleWheelReelIndexDictionary = new Dictionary<string,int>()
        {
            {"Wheel2X2",0},
            {"Wheel2X3",1},
            {"Wheel3X2",2},
            {"Wheel2X4",3},
            {"Wheel3X3",4},
            {"Wheel2X5",5},
            {"Wheel3X4",6},
            {"Wheel3X5",7},
        };

        public static List<string> jackpotName = new List<string>{"","MINI","Minor","Major","Grand"};
        public static string ShapeLinkReels = "ShapeLinkReels";
        public static List<uint> JackpotList = new List<uint>() {30,31,35,39,43,47,50,57,58,65,66,72,75,78,83,91,94};
        public static List<uint> ValueList = new List<uint>() {14,15,16,25,26,27,28,29,32,33,34,36,37,38,40,41,42,44,45,46,48,49,51,52,53,54,55,56,59,60,61,62,63,64,67,68,69,70,71,73,74,76,77,79,80,81,82,84,85,86,87,88,89,90,92,93};
        public static List<uint> BoxList = new List<uint>() {14,15,16,17,18,19,20,21,22,23,24};
        public static List<uint> LongWildList = new List<uint>() {2,201,202};
        public static List<uint> NormalList = new List<uint>() {1,3,4,5,6,7,8,9,10,11};
        public static List<uint> LinkList = new List<uint>() {13};
        public static uint GetRandomNormalElementId()
        {
            return NormalList[Random.Range(0, NormalList.Count)];
            // return NormalList.Random();
        }

        public static string GetSingleWheelName(int width,int height)
        {
            return "Wheel" + height + "X" + width;
        }
    }
}