using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Google.ilruntime.Protobuf.Collections;
using Spine.Unity;
using UnityEngine;
namespace GameModule
{
    public class Constant11312
    {
        public static UIRespinFeature11312 UIRespinFeature = null;
        public static bool LastHasRandomS01 = false;
        public static bool LastRespinIsHasSmall=false;
        public static bool IsReconnection = false;
        // public static bool IsHasAnticipation=false;
        public static readonly uint ScSymbolId = 11;
        public static bool LinkIsSmall = false;
        public static readonly float ReboundValue = 0.265f;
        public static readonly string LastLinkEasingConfig = "LastLink";
        // 所有小轮盘的额外加金币的图标
        public static readonly List<uint> AllListSmallBlueCoinElementId = new List<uint>(){
            40,41,42,43,44,45
        };
        public static readonly List<uint> AllListSmallGoldCoinElementId = new List<uint>(){
            46,47,48
        };
        //币的图标id
        public static readonly List<uint> AllListCoinElementId = new List<uint>(){
            20,21,22,23,24,25,26,27,28,29,30,31
        };
        public static readonly List<uint> ListCoinElementIds = new List<uint>(){
            20,21,22,23,24,25,26,29,30,31
        };
        // 蓝币jackpot
        public static readonly List<uint> ListCoinElementIdsJackot = new List<uint>(){
            27,28
        };
        // 黄币
        public static readonly List<uint> ListCoinGoldElementIds = new List<uint>(){
            29,30,31
        };
        // free  蓝币对应的赢钱倍率
        public static readonly List<uint> ListCoinElementWinRates = new List<uint>(){
            30,50,100,150,200,300,500,1500,2000,2500
        };
        //小转轮结果是随机翻倍类型
        public static readonly List<uint> SmallWheelResForRandom = new List<uint>(){
            3,5,8,10
        };
        //小转轮结果是全部累加类型
        public static readonly List<uint> SmallWheelResForAllAdd = new List<uint>(){
            50,80,100,150,200,250
        };
        //小转轮结果是黄币 累加类型
        public static readonly List<uint> SmallWheelResForGoldOneAdd = new List<uint>(){
            100,200,300
        };
        //小转轮结果是黄币 jackpot类型
        public static readonly List<uint> SmallWheelResForGoldJackpot = new List<uint>(){
            3,4,5
        };
        public static readonly List<uint> GlodSymbolWinRate = new List<uint>(){
            1500,2000,2500
        };

        public static readonly List<uint> lowLevelSymbol = new List<uint>()
        {
            5, 6, 7, 8, 9, 10
        };

        public static readonly List<string> WheelName = new List<string>(){"WheelBaseGame","WheelFreeGame","WheelLinkGame","WheelSmallGame"};
        public static readonly List<string> SmallSeqWheelName = new List<string>(){"BlueCoinReels","BlueCoinNoAddRowReels","BlueCoinFor6X5Reels","YellowCoinReels"};

        /// <summary>
        /// 转化获取的金币索引到两个轮盘上的位置，转成两个轮盘上的pos
        /// </summary>
        public static List<int> ConversionDataToPos(int data,int rolNum){
            List<int> arr=new List<int>();
            var col = Math.Floor(((decimal)data / rolNum));
            //获取当前列的第几行
            var row = data - rolNum * col;
            
            arr.Add((int)col);
            arr.Add((int)row);
            return arr;
        }

        public static void SetStencilCompValue(Transform node,int value){
            MeshRenderer meshRenderer = node.GetComponent<MeshRenderer>();
            meshRenderer.material.SetFloat("_StencilComp", value);
        }

        public static void UpdateElementMaskInteraction(Transform transform,SpriteMaskInteraction interaction){
            var spriteMasks = transform.GetComponentsInChildren<SpriteMask>(true);
            var spriteList = transform.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < spriteList.Length; i++)
            {
                //如果这个sprite本身有被mask操作，那么不更改它的maskInteraction
                if (!IsSpriteMaskedBySpriteMask(spriteList[i].sortingOrder, spriteMasks))
                {
                    spriteList[i].maskInteraction = interaction;
                }
            }

            var spineSkeletonList = transform.GetComponentsInChildren<SkeletonAnimation>(true);

            for (int i = 0; i < spineSkeletonList.Length; i++)
            {
                if (!IsSpriteMaskedBySpriteMask(spineSkeletonList[i].GetComponent<MeshRenderer>().sortingOrder, spriteMasks))
                {
                    spineSkeletonList[i].maskInteraction = interaction;
                }
            }

            //粒子
            var particleList = transform.GetComponentsInChildren<ParticleSystemRenderer>(true);
            if (particleList != null && particleList.Length > 0)
            {
                for (int i = 0; i < particleList.Length; i++)
                {
                    if (!IsSpriteMaskedBySpriteMask(particleList[i].sortingOrder, spriteMasks))
                    {
                        particleList[i].maskInteraction = interaction;
                    }
                }
            }
        }

        private static bool IsSpriteMaskedBySpriteMask(int sortingOrder, SpriteMask[] spriteMasks)
        {
            if (spriteMasks != null && spriteMasks.Length > 0)
            {
                for (var i = 0; i < spriteMasks.Length; i++)
                {
                    if (spriteMasks[i].backSortingOrder < sortingOrder &&
                        spriteMasks[i].frontSortingOrder >= sortingOrder)
                        return true;
                }
            }
            
            return false;
        }

        // public static void SymbolRebound(Transform node){
            // var endValue = node.transform.localPosition.y - ReboundValue;
            // node.transform.DOLocalMoveY(endValue,0.1f).SetLoops(2,LoopType.Yoyo).OnComplete(()=>{

            // });
        // }
    }
}

