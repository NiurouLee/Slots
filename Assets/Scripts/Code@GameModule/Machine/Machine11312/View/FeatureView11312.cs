using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
namespace GameModule{
    public class FeatureView11312 : TransformHolder
    {
        public List<GameObject> S01Symbols;
        public List<GameObject> BombFxs;

        public List<GameObject> WildSymbols;
        public List<GameObject> ScOrCoinSymbols;
        public List<GameObject> FishFxs;
        public FeatureView11312(Transform inTransform) : base(inTransform)
        {
            S01Symbols = new List<GameObject>();
            BombFxs = new List<GameObject>();
            WildSymbols = new List<GameObject>();
            ScOrCoinSymbols = new List<GameObject>();
            FishFxs = new List<GameObject>();

        }
        
        public void ReplaceS01Symbols(){
            if(S01Symbols.Count==0) return;
            foreach (var item in S01Symbols)
            {
                var posArr = item.name.Split('_');
                var curVisibleSymbol = context.view.Get<Wheel>().GetRoll(int.Parse(posArr[1])).GetVisibleContainer(int.Parse(posArr[2]));
                var elementConfigSet = context.state.machineConfig.GetElementConfigSet();
                var seqElement = new SequenceElement(elementConfigSet.GetElementConfig(uint.Parse(posArr[3])), context);
                curVisibleSymbol.UpdateElement(seqElement);
                GameObject.Destroy(item.gameObject);
            }
            S01Symbols = new List<GameObject>();
        }
        public void ReplaceWildSymbols(){
            if(WildSymbols.Count==0) return;
            foreach (var item in WildSymbols)
            {
                var posArr = item.name.Split('_');
                var curVisibleSymbol = context.view.Get<Wheel>().GetRoll(int.Parse(posArr[1])).GetVisibleContainer(int.Parse(posArr[2]));
                var elementConfigSet = context.state.machineConfig.GetElementConfigSet();
                var seqElement = new SequenceElement(elementConfigSet.GetElementConfig(uint.Parse(posArr[3])), context);
                curVisibleSymbol.UpdateElement(seqElement);
                GameObject.Destroy(item.gameObject);
            }
            WildSymbols = new List<GameObject>();
        }
        public void ReplaceScOrCoinSymbols(){
            if(ScOrCoinSymbols.Count==0) return;
            foreach (var item in ScOrCoinSymbols)
            {
                // var posArr = item.name.Split('_');
                // var curVisibleSymbol = context.view.Get<Wheel>().GetRoll(int.Parse(posArr[1])).GetVisibleContainer(int.Parse(posArr[2]));
                // var elementConfigSet = context.state.machineConfig.GetElementConfigSet();
                // var seqElement = new SequenceElement(elementConfigSet.GetElementConfig(uint.Parse(posArr[3])), context);
                // curVisibleSymbol.UpdateElement(seqElement,true);
                GameObject.Destroy(item.gameObject);
            }
            ScOrCoinSymbols = new List<GameObject>();
        }
        public void ClearAllBombFxs(){
            if(BombFxs.Count==0) return;
            foreach (var item in BombFxs)
            {
                item.transform.DOKill();
                GameObject.Destroy(item.gameObject);
            }
            BombFxs = new List<GameObject>();
        }
        public void ClearAllFishFxs(){
            if(FishFxs.Count==0) return;
            foreach (var item in FishFxs)
            {
                item.transform.DOKill();
                GameObject.Destroy(item.gameObject);
            }
            FishFxs = new List<GameObject>();
        }
        public void SymbolsStartBound(int rollIndex,List<GameObject> Symbols){
            if(Symbols.Count==0) return;
            foreach (var item in Symbols){
                var col = int.Parse(item.name.Split('_')[1]);
                if(col == rollIndex)
                    item.gameObject.SetActive(false);
            }
            
        }
      
    }
}

