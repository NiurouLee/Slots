using System.Collections.Generic;

namespace GameModule
{
    public static class Constant11006
    {
        public static readonly int highElementId = 18;
        public static readonly uint normalElementId = 1;
        public static readonly string freeSeqName = "FreeReels";
        public static readonly uint scatterElement = 17;

        public static readonly List<uint> listBuffaloLevel2ElementId = new List<uint>()
        {
            2,
            3,
            4,
            5,
        };
        
        public static readonly List<string> listBuffaloLevel2SpriteName = new List<string>()
        {
            "S02",
            "S03",
            "S04",
            "S05",
        };
        
        public static readonly List<int> listBuffaloLevel2Upgrade = new List<int>()
        {
            4,
            7,
            13,
            15,
        };
        
        
        public static readonly List<List<uint>> listBuffaloLevel2ChangeId = new List<List<uint>>
        {
            new List<uint>(){2},
            new List<uint>(){2,3},
            new List<uint>(){2,3,4},
            new List<uint>(){2,3,4,5},
        };
        
        
        public static readonly List<string> listBuffaloLevel2SequenceName = new List<string>()
        {
            "Buffalo0",
            "Buffalo1",
            "Buffalo2",
            "Buffalo3",
        };

        public static readonly List<uint> listBaseMultiplierElements = new List<uint>()
        {
            13,14
        };
        
        public static readonly List<uint> listFreeMultiplierElements = new List<uint>()
        {
            15,16
        };
    }
}