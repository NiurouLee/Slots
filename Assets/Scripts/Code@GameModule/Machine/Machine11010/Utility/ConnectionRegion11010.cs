//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-01 14:35
//  Ver : 1.0.0
//  Description : ConnectionRegion11010.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Google.ilruntime.Protobuf.Collections;
using Sirenix.Utilities;
using UnityEngine;

namespace GameModule
{
    public class ConnectionRegion11010
    {
        private List<Block> _listBlocks;
        public List<Block> ListBlocks => _listBlocks;

        public ConnectionRegion11010()
        {
            _listBlocks = new List<Block>();
        }

        public bool IsBlockId(int posId)
        {
            for (int i = 0; i < ListBlocks.Count; i++)
            {
                var block = ListBlocks[i];
                if (block.ListIds.Contains(posId))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasBlock(RepeatedField<uint> block)
        {
            for (int i = 0; i < ListBlocks.Count; i++)
            {
                var blockItem = ListBlocks[i];
                if (blockItem.IsSameBlock(block))
                {
                    return true;
                }
            }
            return false;
        }

        public List<Block> GetNewBlocks()
        {
            List<Block> listBlocks = new List<Block>();
            for (int i = 0; i < ListBlocks.Count; i++)
            {
                var blockItem = ListBlocks[i];
                if (blockItem.IsNewBlock)
                {
                    listBlocks.Add(blockItem);
                }
            }
            return listBlocks;
        }

        public bool CreateBlock(RepeatedField<uint> listBlockIds)
        {
            ClearUpBlock(listBlockIds);
            var newBlock = new Block(listBlockIds);
            newBlock.GenerateBlockBorders();
            _listBlocks.Add(newBlock);
            return true;
        }

        public void ClearUpBlock(RepeatedField<uint> listBlockIds)
        {
            for (int i = ListBlocks.Count-1; i >= 0; i--)
            {
                var tmpBlock = ListBlocks[i];
                if (tmpBlock.IsSimilarBlock(listBlockIds))
                {
                    tmpBlock.ClearUp();
                    ListBlocks.Remove(tmpBlock);
                }
            }
        }

        public void ForceClearUpBlock()
        {
            if (ListBlocks != null && ListBlocks.Count>0)
            {
                for (int i = 0; i < ListBlocks.Count; i++)
                {
                    if (ListBlocks[i] != null)
                    {
                        ListBlocks[i].ClearUp();
                    }
                }
                ListBlocks.Clear();
            }
        }
        
        public class Block
        {
            private Tuple<int, int> _leftTopId;
            private bool _isNewBlock;

            public bool IsNewBlock
            {
                get
                {
                    return _isNewBlock;
                }
                set
                {
                    _isNewBlock = value;
                }
            }
            private readonly List<int> _listIds;
            private readonly List<Tuple<int, int>> _listCoords;

            public List<int> ListIds => _listIds;
            public List<Tuple<int, int>> ListCoords
            {
                get
                {
                    _listCoords.Sort(Comparater);
                    return _listCoords;
                }
            }
            private List<Tuple<int,Frame.BorderDirection>> _listNeedFillBorder;
            private readonly List<Frame> _ListFrames;
            public List<Frame> ListFrames => _ListFrames;

            public Block(RepeatedField<uint> region)
            {
                _listIds = new List<int>();
                _listCoords = new List<Tuple<int, int>>();
                for (int i = 0; i < region.Count; i++)
                {
                    int id = (int)region[i];
                    _listIds.Add(id);
                    _listCoords.Add(new Tuple<int, int>(id/3, id%3));
                }
                _leftTopId = GetLeftTopId();
                _ListFrames = new List<Frame>();
                _listNeedFillBorder = new List<Tuple<int,Frame.BorderDirection>>();
            }

            private Tuple<int, int> GetLeftTopId()
            {
                _listCoords.Sort(Comparater);
                return _listCoords[0];
            }

            public static int Comparater(Tuple<int, int> i, Tuple<int, int> i1)
            {
                if (i.Item2.CompareTo(i1.Item2)==0)
                {
                    return i.Item1.CompareTo(i1.Item1);
                }
                return i.Item2.CompareTo(i1.Item2);
            }
            public bool IsSameBlock(RepeatedField<uint> region)
            {
                var tmpRegionIds = new List<int>();
                for (int i = 0; i < region.Count; i++)
                {
                    tmpRegionIds.Add((int)region[i]);
                }
                return tmpRegionIds.Intersect(_listIds).Count()>0 && tmpRegionIds.Except(_listIds).Count()<=0;
            }
            
            public bool IsSimilarBlock(RepeatedField<uint> region)
            {
                var tmpRegionIds = new List<int>();
                for (int i = 0; i < region.Count; i++)
                {
                    tmpRegionIds.Add((int)region[i]);
                }
                return tmpRegionIds.Intersect(_listIds).Count()>0;
            }

            public void ClearUp()
            {
                for (int i = 0; i < ListFrames.Count; i++)
                {
                    var frame = ListFrames[i];
                    frame.ClearUp();
                }
                _listIds.Clear();
                ListFrames.Clear();
                _listCoords.Clear();
                _listNeedFillBorder.Clear();
            }
            private bool ContainsId(Tuple<int, int> coord)
            {
                if (coord.Item1 < 0 || coord.Item2 < 0)
                {
                    return false;
                }
                return _listCoords.Any(item => item.Item1 == coord.Item1 && item.Item2 == coord.Item2);
            }

            private List<Tuple<int, int>> ListDirections = new List<Tuple<int, int>>{
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(0, -1),
                new Tuple<int, int>(1, 0),
                new Tuple<int, int>(-1, 0)
            };
            /// <summary>
            /// 获取所有需要填充的边界，包括外框
            /// </summary>
            private void CalculateAllFillBorders()
            {
                _listNeedFillBorder.Clear();
                for (int i = 0; i < _listCoords.Count; i++)
                {
                    var id = _listCoords[i];
                    for (int j = 0; j < ListDirections.Count; j++)
                    {
                        var offet = ListDirections[j];
                        if (!ContainsId(new Tuple<int, int>(id.Item1+offet.Item1,id.Item2+offet.Item2)))
                        {
                            var posId = id.Item1 * 3 + id.Item2;
                            if (offet.Item2 == -1)
                            {
                                _listNeedFillBorder.Add(new Tuple<int,Frame.BorderDirection>(posId,Frame.BorderDirection.Top));    
                            }
                            if (offet.Item2 == 1)
                            {
                                _listNeedFillBorder.Add(new Tuple<int,Frame.BorderDirection>(posId,Frame.BorderDirection.Bottom));
                            }
                            if (offet.Item1 == 1)
                            {
                                _listNeedFillBorder.Add(new Tuple<int,Frame.BorderDirection>(posId,Frame.BorderDirection.Right));
                            }
                            if (offet.Item1 == -1)
                            {
                                _listNeedFillBorder.Add(new Tuple<int,Frame.BorderDirection>(posId,Frame.BorderDirection.Left));
                            }
                        }
                    }
                }
            }
            
            public void GenerateBlockBorders()
            {
                IsNewBlock = true;
                _ListFrames.Clear();

                CalculateAllFillBorders();
                
                //计算外围框
                Frame outerBorder = new Frame();
                CalculateTopBorders(_leftTopId, in outerBorder);
                _ListFrames.Add(outerBorder);
                
                //计算内框
                while (_listNeedFillBorder.Count > 0 && ListCoords.Count>0)
                {
                    _leftTopId = GetLeftBottomId();
                    if (_leftTopId==null) break;
                    Frame innerBorder = new Frame(Frame.BorderType.Inner);
                    CalculateInnerBottomBorders(_leftTopId, in innerBorder);
                    _ListFrames.Add(innerBorder);
                }
            }
            #region Calculate Out Border
            private void CalculateTopBorders(Tuple<int, int> id, in Frame border)
            {
                if (!ContainsId(id) && !IsBorderNotFilled(id.Item1*3+id.Item2, Frame.BorderDirection.Top)) return;
                RemoveFillBorderKey(id.Item1*3+id.Item2, Frame.BorderDirection.Top);
                border.ListBorders.Add(new Tuple<int,Frame.BorderDirection>(id.Item1*3+id.Item2,Frame.BorderDirection.Top));
                var idRight = new Tuple<int, int>(id.Item1+1, id.Item2);
                var idRightTop = new Tuple<int, int>(id.Item1+1, id.Item2-1);
                if (ContainsId(idRightTop)) //右上有图标
                    CalculateLeftBorders(idRightTop, in border);
                else if (ContainsId(idRight)) //右有图标
                    CalculateTopBorders(idRight, in border);
                else
                    CalculateRightBorders(id,border);  
            }

            private void CalculateRightBorders(Tuple<int, int> id, in Frame border)
            {
                var posId = id.Item1 * 3 + id.Item2;
                if (!ContainsId(id) && !IsBorderNotFilled(posId, Frame.BorderDirection.Right)) return;
                RemoveFillBorderKey(posId, Frame.BorderDirection.Right);
                border.ListBorders.Add(new Tuple<int,Frame.BorderDirection>(posId,Frame.BorderDirection.Right));
                var idBottom = new Tuple<int, int>(id.Item1,id.Item2+1);
                var idRightBottom = new Tuple<int, int>(id.Item1+1,id.Item2+1);
                if (ContainsId(idRightBottom))
                    CalculateTopBorders(idRightBottom,in border);
                else if(ContainsId(idBottom))
                    CalculateRightBorders(idBottom,in border);
                else
                    CalculateBottomBorders(id, in border);
            }

            private void CalculateLeftBorders(Tuple<int, int> id, in Frame border)
            {
                var posId = id.Item1 * 3 + id.Item2;
                if (!ContainsId(id) && !IsBorderNotFilled(posId, Frame.BorderDirection.Left)) return;
                RemoveFillBorderKey(posId, Frame.BorderDirection.Left);
                border.ListBorders.Add(new Tuple<int,Frame.BorderDirection>(posId,Frame.BorderDirection.Left));
                if (id.Item1 == _leftTopId.Item1 && id.Item2 == _leftTopId.Item2) return;
                var idTop = new Tuple<int, int>(id.Item1, id.Item2-1);
                var idLeftTop = new Tuple<int, int>(id.Item1-1, id.Item2-1);
                if (ContainsId(idLeftTop))
                    CalculateBottomBorders(idLeftTop, in border);
                else if (ContainsId(idTop))
                    CalculateLeftBorders(idTop, in border);
                else
                    CalculateTopBorders(id,in border);
            }

            private void CalculateBottomBorders(Tuple<int, int> id, in Frame border)
            {
                var posId = id.Item1 * 3 + id.Item2;
                if (!ContainsId(id) && !IsBorderNotFilled(posId, Frame.BorderDirection.Bottom)) return;
                RemoveFillBorderKey(posId, Frame.BorderDirection.Bottom);
                border.ListBorders.Add(new Tuple<int,Frame.BorderDirection>(posId, Frame.BorderDirection.Bottom));
                var idLeft = new Tuple<int, int>(id.Item1 - 1, id.Item2);
                var idLeftBottom = new Tuple<int, int>(id.Item1 - 1, id.Item2+1);
                if (ContainsId(idLeftBottom))
                    CalculateRightBorders(idLeftBottom, in border);
                else if (ContainsId(idLeft))
                    CalculateBottomBorders(idLeft, in border);
                else
                    CalculateLeftBorders(id, in border);
            }
            #endregion

            #region Calculate Inner Border
            private Tuple<int, int> GetLeftBottomId()
            {
                List<Tuple<int,int>> listIds = new List<Tuple<int,int>>();
                for (int i = 0; i < _listNeedFillBorder.Count; i++)
                {
                    var item = _listNeedFillBorder[i];
                    if (item.Item2 == Frame.BorderDirection.Bottom)
                    {
                        listIds.Add(new Tuple<int, int>(item.Item1/3,item.Item1%3));
                    }
                }
                listIds.Sort(Comparater);
                if (listIds.Count>0)
                {
                    return listIds[0];   
                }
                return null;
            }

            private void CalculateInnerTopBorders(Tuple<int, int> id, in Frame border)
            {
                var posId = id.Item1 * 3 + id.Item2;
                if (!ContainsId(id) && !IsBorderNotFilled(posId, Frame.BorderDirection.Top)) return;
                RemoveFillBorderKey(posId, Frame.BorderDirection.Top);
                border.ListBorders.Add(new Tuple<int,Frame.BorderDirection>(posId,Frame.BorderDirection.Top));
                var leftId = new Tuple<int, int>(id.Item1-1, id.Item2);
                var leftTopId = new Tuple<int, int>(id.Item1-1, id.Item2-1);
                if (ContainsId(leftTopId))
                    CalculateInnerRightBorders(leftTopId, in border);
                else if(ContainsId(leftId))
                    CalculateInnerTopBorders(leftId, in border);
                else
                    CalculateLeftBorders(id,in border);
            }

            private void CalculateInnerRightBorders(Tuple<int, int> id, in Frame border)
            {
                var posId = id.Item1 * 3 + id.Item2;
                if (!ContainsId(id) && !IsBorderNotFilled(posId, Frame.BorderDirection.Right)) return;
                RemoveFillBorderKey(posId, Frame.BorderDirection.Right);
                border.ListBorders.Add(new Tuple<int,Frame.BorderDirection>(posId,Frame.BorderDirection.Right));
                var idTop = new Tuple<int, int>(id.Item1, id.Item2-1);
                var idRightTop = new Tuple<int, int>(id.Item1+1, id.Item2-1);
                if (ContainsId(idRightTop))
                    CalculateInnerBottomBorders(idRightTop,in border);
                else if(ContainsId(idTop))
                    CalculateInnerRightBorders(idTop,in border);
                else
                    CalculateInnerTopBorders(id,in border);
            }

            private void CalculateInnerLeftBorders(Tuple<int, int> id, in Frame border)
            {
                var posId = id.Item1 * 3 + id.Item2;
                if (!ContainsId(id) && !IsBorderNotFilled(posId, Frame.BorderDirection.Left)) return;
                RemoveFillBorderKey(posId, Frame.BorderDirection.Left);
                border.ListBorders.Add(new Tuple<int,Frame.BorderDirection>(posId,Frame.BorderDirection.Left));
                var leftBottomId = new Tuple<int, int>(id.Item1-1, id.Item2+1);
                var bottomId = new Tuple<int, int>(id.Item1, id.Item2+1);
                if (ContainsId(leftBottomId))
                    CalculateInnerTopBorders(leftBottomId, in border);
                else if(ContainsId(bottomId))
                    CalculateInnerLeftBorders(bottomId, in border);
                else 
                    CalculateInnerBottomBorders(id, in border);
            }

            private void CalculateInnerBottomBorders(Tuple<int, int> id, in Frame border)
            {
                int posId = id.Item1 * 3 + id.Item2;
                if (!IsBorderNotFilled(posId, Frame.BorderDirection.Bottom)) return;
                if (!ContainsId(id) && !IsBorderNotFilled(posId, Frame.BorderDirection.Bottom)) return;
                RemoveFillBorderKey(posId, Frame.BorderDirection.Bottom);
                border.ListBorders.Add(new Tuple<int,Frame.BorderDirection>(posId, Frame.BorderDirection.Bottom));
                var idRight = new Tuple<int, int>(id.Item1+1, id.Item2);
                var idRightBottom = new Tuple<int, int>(id.Item1+1,id.Item2+1);
                if (ContainsId(idRightBottom))
                    CalculateInnerLeftBorders(idRightBottom,in border);
                else if (ContainsId(idRight))
                    CalculateInnerBottomBorders(idRight, in border);
                else
                    CalculateRightBorders(id,in border);
            }

            #endregion

            private void RemoveFillBorderKey(int id, Frame.BorderDirection dir)
            {
                _listNeedFillBorder.RemoveAll(p => p.Item1 == id && p.Item2 == dir);
            }

            private bool IsBorderNotFilled(int id, Frame.BorderDirection dir)
            {
                return _listNeedFillBorder.Any(p=>p.Item1 == id && p.Item2 == dir);
            }
            public class Frame
            {
                private BorderType _type;
                public BorderType Type => _type;
                private readonly List<Tuple<int,BorderDirection>> _listBorders;
                private readonly List<GameObject> _listCornerBorders;
                public List<Tuple<int,BorderDirection>> ListBorders => _listBorders;
                public enum BorderDirection
                {
                    Top=1,
                    Right=2,
                    Bottom=3,
                    Left=4,
                    Corner=5
                }
                public enum BorderType
                {
                    Outer,
                    Inner
                }

                public Frame(BorderType type = BorderType.Outer)
                {
                    _type = type;
                    _listBorders = new List<Tuple<int,BorderDirection>>();
                    _listCornerBorders = new List<GameObject>();
                }

                public void AddBorderGameobject(GameObject gameObject)
                {
                    _listCornerBorders.Add(gameObject);
                }

                public void ClearUp()
                {
                    for (int i = 0; i < _listCornerBorders.Count; i++)
                    {
                        var item = _listCornerBorders[i];
                        if (item)
                        {
                            GameObject.Destroy(item.gameObject);
                        }
                    }
                    _listCornerBorders.Clear();
                    _listBorders.Clear();
                }
            }
        }
    }
}