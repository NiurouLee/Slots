//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-06 12:49
//  Ver : 1.0.0
//  Description : SmallWheel11016.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class SmallWheel11016:Wheel
    {
        private Panel _panel;
        public Action ActionSpinEnd;
        private TextMesh txtWin;
        private Transform transJackpot;
        private Animator _animatorSpin;
        private int _index;
        private int _gameId;
        
        private Dictionary<int, SpriteRenderer> _dictElements;
        private List<List<SpriteRenderer>> _listSpritesEnd;
        private List<List<SpriteRenderer>> _listSpritesStart;

        public ElementContainer ElementContainer;
        public SmallWheel11016(Transform transform, int index, int gameId) : base(transform)
        {
            _index = index;
            _gameId = gameId;
            _listSpritesEnd = new List<List<SpriteRenderer>>();
            _listSpritesStart = new List<List<SpriteRenderer>>();
            _animatorSpin = transform.Find("Animation").GetComponent<Animator>();
            txtWin = transform.Find("WinGroup/IntegralText").GetComponent<TextMesh>();
            transJackpot = transform.Find("WinGroup/SpriteJackpot").transform;
            for (int i = 0; i < 3; i++)
            {
                _listSpritesEnd.Add(new List<SpriteRenderer>());
                _listSpritesStart.Add(new List<SpriteRenderer>());
                for (int j = 0; j < 3; j++)
                {
                    var transformRow = _animatorSpin.transform.Find($"Rolls/Row{i + 1}");
                    _listSpritesStart[i].Add(transformRow.transform.Find($"Roll1_{j+1}").GetComponent<SpriteRenderer>());
                    _listSpritesEnd[i].Add(transformRow.transform.Find($"Roll1_{j+6}").GetComponent<SpriteRenderer>());
                }
            }

            _dictElements = new Dictionary<int, SpriteRenderer>();
            var transPool = transform.Find("ElementsPool");
            for (int i = 0; i < transPool.childCount; i++)
            {
                var transElement = transPool.GetChild(i);
                var id = transElement.name.ToInt();
                _dictElements.Add(id,transElement.GetComponent<SpriteRenderer>());
            }
        }

        public void InitializeWheel()
        {
            Show();
            var listStartSpin = GetSmallWheelState().ListStartSpin;
            for (int i = 0; i < listStartSpin.Count; i++)
            {
                for (int j = 0; j < listStartSpin[i].Count; j++)
                {
                    var elementId = listStartSpin[i][j];
                    _listSpritesEnd[i][j].sprite = _dictElements[elementId].sprite;
                    _listSpritesStart[i][j].sprite = _dictElements[elementId].sprite;
                }
            }
        }

        public SmallWheelState11016 GetSmallWheelState()
        {
            return wheelState as SmallWheelState11016;
        }

        public void OnSpinEnd()
        {
            ResetSmallWheelOrder();
            ActionSpinEnd?.Invoke();
        }

        public void PlaySpinAnimation(string stateName)
        {
            XUtility.PlayAnimation(_animatorSpin,stateName);
        }
        public void ShowResult()
        {
            var listResultSpin = GetSmallWheelState().ListResultSpin;
            for (int i = 0; i < listResultSpin.Count; i++)
            {
                for (int j = 0; j < listResultSpin[i].Count; j++)
                {
                    var elementId = listResultSpin[i][j];
                    _listSpritesEnd[i][j].sprite = _dictElements[elementId].sprite;
                    _listSpritesStart[i][j].sprite =  _dictElements[elementId].sprite;
                }
            }
        }
        
        protected override void InitializeWheelMaskSortOrder()
        {         
            wheelMask.backSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            wheelMask.frontSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            wheelMask.frontSortingOrder = 1100 + (_index + _gameId - 1) * 100;
            wheelMask.backSortingOrder = 1010 + (_index + _gameId - 1) * 100;

            var children = transform.GetComponentsInChildren<SortingGroup>(true);
            for (int i = 0; i < children.Length; i++)
            {
                children[i].sortingOrder = children[i].sortingOrder + _index * 100;   
            }
            for (int i = 0; i < 3; i++)
            {
                var spriteRenderer = _animatorSpin.transform.Find($"Rolls/mmohu_0{i+1}").GetComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = wheelMask.backSortingOrder + 4 + i;
            }
        }

        private void ResetSmallWheelOrder()
        {
            wheelMask.backSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            wheelMask.frontSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            wheelMask.frontSortingOrder = 1100 + (_gameId - 1)*100;
            wheelMask.backSortingOrder = 1010 + (_gameId - 1)*100;
            
            var children = transform.GetComponentsInChildren<SortingGroup>(true);
            for (int i = 0; i < children.Length; i++)
            {
                children[i].sortingOrder = children[i].sortingOrder - _index * 100;   
            }

            for (int i = 0; i < 3; i++)
            {
                var spriteRenderer = _animatorSpin.transform.Find($"Rolls/mmohu_0{i+1}").GetComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = wheelMask.backSortingOrder + 10 + i;
            }  
        }

        public void UpdateTotalWin()
        {
            txtWin.gameObject.SetActive(true);
            transJackpot.gameObject.SetActive(false);
            var state = wheelState as SmallWheelState11016;
            if (wheelState.GetJackpotWinLines().Count>0)
            {
                txtWin.gameObject.SetActive(false);
                transJackpot.gameObject.SetActive(true);
            }
            txtWin.text = state.TotalSmallWin.GetAbbreviationFormat();
        }
        
        
        public void ResetSpritesInsideMask()
        {
            var transAnimation = transform.Find("Animation");
            var listAllSprites = transAnimation.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < listAllSprites.Length; i++)
            {
                listAllSprites[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        public void ShowWinLines(List<WinLine> winLines, bool visible)
        {
            for (int i = 0; i < winLines.Count; i++)
            {
                transform.Find($"PayLineGroup/PayLine{winLines[i].PayLineId-1}").gameObject.SetActive(visible);
            }
        }
    }
}