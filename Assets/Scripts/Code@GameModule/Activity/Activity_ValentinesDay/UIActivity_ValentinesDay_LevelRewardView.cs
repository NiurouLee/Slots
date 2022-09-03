using System.Collections.Generic;
using UnityEngine;
using static DragonU3DSDK.Network.API.ILProtocol.SGetValentineMainPageInfo.Types;
using static DragonU3DSDK.Network.API.ILProtocol.SGetValentineMainPageInfo.Types.ValentineStepReward.Types;

namespace GameModule
{
    public class UIActivity_ValentinesDay_LevelRewardView : View
    {
        [ComponentBinder("LevelPoint/StateGroup/FinishState")]
        public Transform transformFinishState;

        [ComponentBinder("FreeRewardGroup/UIValentinesDayRewardCell")]
        public Transform transformFreeCellPrefab;

        [ComponentBinder("FreeRewardGroup")]
        public Transform transformFreeCellRoot;

        [ComponentBinder("ValentineGroup/UIValentinesDayRewardCell")]
        public Transform transformCellPrefab;

        [ComponentBinder("ValentineGroup")]
        public Transform transformCellRoot;

        [ComponentBinder("ValentineGroup/LockState")]
        public Transform transformLock;

        [ComponentBinder("LevelPoint")]
        public Animator animatorLevelPoint;

        private readonly List<UIActivity_ValentinesDay_MapPopup_RewardCellView> _specialCells
            = new List<UIActivity_ValentinesDay_MapPopup_RewardCellView>();

        private readonly List<UIActivity_ValentinesDay_MapPopup_RewardCellView> _normalCells
            = new List<UIActivity_ValentinesDay_MapPopup_RewardCellView>();

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            transformFreeCellPrefab.gameObject.SetActive(false);
            transformCellPrefab.gameObject.SetActive(false);
        }

        public void SetStepChecked(bool isChecked, bool withAnimation)
        {
            if (transformFinishState != null)
            {
                transformFinishState.gameObject.SetActive(isChecked);
            }
            var stateName = isChecked ? "Collect State" : "Empty";

            if (animatorLevelPoint != null)
            {
                animatorLevelPoint.Play(stateName, 0, withAnimation ? 0 : 1);
            }
        }

        public void SetNormalRewardChecked(bool isChecked, bool withAnimation)
        {
            for (int i = 0; i < _normalCells.Count; i++)
            {
                _normalCells[i].SetChecked(isChecked, withAnimation);
            }
        }

        public void SetSpecialRewardChecked(bool isChecked, bool withAnimation)
        {
            for (int i = 0; i < _specialCells.Count; i++)
            {
                _specialCells[i].SetChecked(isChecked, withAnimation);
            }
        }

        public void SetLocked(bool locked)
        {
            transformLock.gameObject.SetActive(locked);
        }

        public void Set(ValentineStepReward reward)
        {
            ClearCells();

            if (reward == null) { return; }

            var normalReward = reward.NormalReward;
            var normalStatus = reward.NormalRewardStatus;
            if (normalReward != null && normalReward.Items != null && normalReward.Items.count > 0)
            {
                var items = normalReward.Items;
                var itemCount = items.count;
                for (int i = 0; i < itemCount; i++)
                {
                    var go = Object.Instantiate(transformFreeCellPrefab.gameObject, transformFreeCellRoot);
                    var cell = AddChild<UIActivity_ValentinesDay_MapPopup_RewardCellView>(go.transform);
                    _normalCells.Add(cell);
                    cell.Set(items[i]);
                    cell.SetChecked(normalStatus == ValentineRewardStatus.Received, false);
                }
            }

            var specialReward = reward.SpecialReward;
            var specialStatus = reward.SpecialRewardStatus;
            if (specialReward != null && specialReward.Items != null && specialReward.Items.count > 0)
            {
                var items = specialReward.Items;
                var itemCount = items.count;
                for (int i = 0; i < itemCount; i++)
                {
                    var go = Object.Instantiate(transformFreeCellPrefab.gameObject, transformCellRoot);
                    var cell = AddChild<UIActivity_ValentinesDay_MapPopup_RewardCellView>(go.transform);
                    _specialCells.Add(cell);
                    cell.Set(items[i]);
                    cell.SetChecked(specialStatus == ValentineRewardStatus.Received, false);
                }
            }
        }

        private void ClearCells()
        {
            if (_normalCells != null && _normalCells.Count > 0)
            {
                foreach (var cell in _normalCells)
                {
                    RemoveChild(cell);
                    Object.Destroy(cell.transform.gameObject);
                }

                _normalCells.Clear();
            }

            if (_specialCells != null && _specialCells.Count > 0)
            {
                foreach (var cell in _specialCells)
                {
                    RemoveChild(cell);
                    Object.Destroy(cell.transform.gameObject);
                }

                _specialCells.Clear();
            }
        }
    }
}