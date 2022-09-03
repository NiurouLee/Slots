using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidChooseMapPoint")]
    public class TreasureRaidChooseMapPopup : Popup<TreasureRaidChooseMapPopupController>
    {
        [ComponentBinder("Root/AdaptGroup/PathGroup")]
        private Transform pathGourp;

        [ComponentBinder("Root/TitleGroup/CloseButton")]
        private Button closeBtn;

        [ComponentBinder("Root/ToAnyWhereGroup/ToAnyWhereCell/PropertyGroup/PropertyText")]
        private Text portalText;

        [ComponentBinder("Root/GoButton")]
        private Button goBtn;

        [ComponentBinder("Root/Puzzle")]
        private Transform puzzlePrefab;
        
        [ComponentBinder("Root/UITreasureRaidChooseMapPointGridCell")]
        private Transform pathPointPrefab;
        
        [ComponentBinder("Root/AdaptGroup/UITreasureRaidChooseMapPointPlayerCell")]
        private Transform _player;

        [ComponentBinder("Root/AdaptGroup/UITreasureRaidChooseMapPointPlayerCell/Root/AvatarMask/Icon")]
        private RawImage rawImageAvatar;
        
        [ComponentBinder("Root/AdaptGroup/UITreasureRaidChooseMapPointArrowCell")]
        private Transform arrowCell;
        
        private Activity_TreasureRaid _activityTreasureRaid;

        private List<Animator> latticesAni;

        private int currentIndex;

        private int selectIndex;

        public TreasureRaidChooseMapPopup(string address) : base(address)
        {
            contentDesignSize = new Vector2(1250, 768);
        }

        protected override void OnViewSetUpped()
        {
            _activityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            base.OnViewSetUpped();
            closeBtn.onClick.AddListener(OnCloseClicked);
            goBtn.onClick.AddListener(OnGoBtnClicked);
            currentIndex = (int) _activityTreasureRaid.GetRoundInfo().CurrentLatticeIndex;
        }

        private void SetBtnState(bool interactable)
        {
            closeBtn.interactable = interactable;
            goBtn.interactable = interactable;
        }

        protected override void OnCloseClicked()
        {
            SetBtnState(false);
            HideSpine();
            base.OnCloseClicked();
        }

        private void HideSpine()
        {
            var sks = transform.GetComponentsInChildren<SkeletonGraphic>();
            var noColor = new Color(1, 1, 1, 0);
            foreach (var sk in sks)
            {
                sk.DOColor(noColor, 0.28f);
            }
        }

        private async void OnGoBtnClicked()
        {
            if (selectIndex == currentIndex)
            {
                XDebug.LogWarning("state error, check your code");
                return;
            }
            SetBtnState(false);
            foreach (var cell in latticesAni)
            {
                cell.transform.GetComponent<Button>().interactable = false;
            }

            var forwardStep = 0;
            if (selectIndex > currentIndex)
            {
                forwardStep = selectIndex - currentIndex;
            }
            else
            {
                forwardStep = 28 - currentIndex + selectIndex;
            }
            // 发送传送门协议
            var sMonopolyTeleport = await _activityTreasureRaid.MonopolyTeleport((uint) forwardStep);
            HideSpine();
            if (sMonopolyTeleport == null)
            {
                Close();
                return;
            }
            viewController._mainPopup.BeginTeleportAni(sMonopolyTeleport, selectIndex);
            Close();
        }

        public override void OnOpen()
        {
            base.OnOpen();
            latticesAni = new List<Animator>();
            for (int i = 0; i < 28; i++)
            {
                var path = pathGourp.Find($"Path{i}");

                var specialLattice = GetSpecialLattice((uint)i);
                if (specialLattice != null)
                {
                    var cell = GameObject.Instantiate(pathPointPrefab, path.Find("FloorCell/Area"), false);
                    UpdateSpecialLatticeUI(cell, specialLattice);
                }
                else
                {
                    var puzzleLattice = viewController._mainPopup.viewController.GetPuzzleSpecialLattice((uint) i);
                    if (puzzleLattice != null)
                    {
                        var cell = GameObject.Instantiate(puzzlePrefab, path.Find("FloorCell/Area"), false);
                        cell.localPosition = Vector3.zero;
                        cell.localScale = Vector3.one * 0.8f;
                        cell.gameObject.SetActive(true);
                    }
                }

                var floorCell = path.Find("FloorCell");
                
                var btn = floorCell.GetComponent<Button>();
                var index = i;
                btn.onClick.AddListener(() =>
                {
                    OnSelectLatticeClicked(index);
                });
                btn.interactable = i != currentIndex;
                
                var ani = floorCell.GetComponent<Animator>();
                XUtility.PlayAnimation(ani, currentIndex == i ? "PlayerArea" : "NormalArea");
                latticesAni.Add(ani);
            }

            portalText.SetText(_activityTreasureRaid.GetRoundInfo().MonopolyPortal.Amount.ToString());
            goBtn.interactable = false;
            UpdateUserAvatar(Client.Get<UserController>().GetUserAvatarID());
            _player.position = GetPlayPosInLattice(currentIndex);
        }

        public Vector3 GetPlayPosInLattice(int index)
        {
            var pos = latticesAni[index].transform.position;
            if (GetSpecialLattice((uint)index) != null)
            {
                pos += new Vector3(0, 5, 0);
            }
            return pos;
        }
        
        private void OnSelectLatticeClicked(int index)
        {
            for (int i = 0; i < latticesAni.Count; i++)
            {
                if (i == currentIndex)
                    continue;

                if (selectIndex == index)
                {
                    XUtility.PlayAnimation(latticesAni[i], "NormalArea");
                }
                else
                {
                    XUtility.PlayAnimation(latticesAni[i], i == index ? "ChooseArea" : "NormalArea");
                }
            }

            if (selectIndex == index)
            {
                selectIndex = currentIndex;
                goBtn.interactable = false;
                if (arrowCell.gameObject.activeSelf)
                {
                    arrowCell.gameObject.SetActive(false);
                }
            }
            else
            {
                selectIndex = index;
                goBtn.interactable = true;
                if (!arrowCell.gameObject.activeSelf)
                {
                    arrowCell.gameObject.SetActive(true);
                }
                arrowCell.position = latticesAni[index].transform.position;
            }
            // XDebug.LogWarning("Select Index: " + selectIndex);
        }

        private MonopolyRoundInfo.Types.SpecialLattice GetSpecialLattice(uint index)
        {
            var roundInfo = _activityTreasureRaid.GetRoundInfo();
            foreach (var lattice in roundInfo.SpecialLattices)
            {
                if (lattice.Index == index)
                {
                    return lattice;
                }
            }
            return null;
        }
        
        private static class LatticeName
        {
            public const string GiftBox = "BoxRewardType";
            public const string Weapon = "WeaponRewardType";
        }

        private void UpdateSpecialLatticeUI(Transform cell, MonopolyRoundInfo.Types.SpecialLattice lattice)
        {
            var childName = LatticeName.GiftBox;
            
            switch (lattice.Type)
            {
                case MonopolyRoundInfo.Types.SpecialLattice.Types.LatticeType.Giftbox:
                    childName = LatticeName.GiftBox;
                    break;
                case MonopolyRoundInfo.Types.SpecialLattice.Types.LatticeType.Cannon:
                    childName = LatticeName.Weapon;
                    break;
            }

            var latticeTrParent = cell.GetChild(0);
            for (int i = 0; i < latticeTrParent.childCount; i++)
            {
                var child = latticeTrParent.GetChild(i);
                child.gameObject.SetActive(child.name == childName);
                if (child.name == childName)
                {
                    for (int j = 0; j < child.childCount; j++)
                    {
                        var itemContainer = child.GetChild(j);
                        itemContainer.gameObject.SetActive((j + 1) == lattice.Level);
                        if (childName == LatticeName.Weapon)
                        {
                            var weaponIndex = GetWeaponIndex((int)lattice.Index);
                            for (int k = 0; k < itemContainer.childCount; k++)
                            {
                                itemContainer.GetChild(k).gameObject.SetActive(k == weaponIndex);
                            }
                        }
                    }
                }
            }
            cell.gameObject.SetActive(true);
            cell.localPosition = Vector3.zero;
        }

        private int GetWeaponIndex(int latticeIndex)
        {
            if (latticeIndex <= 7)
                return 3;
            else if (latticeIndex <= 14)
                return 1;
            else if (latticeIndex <= 21)
                return 0;

            return 2;
        }
        
        public void UpdateUserAvatar(uint avatarID)
        {
            if (rawImageAvatar != null)
            {
                rawImageAvatar.texture = AvatarController.defaultAvatar;

                AvatarController.GetSelfAvatar(avatarID, (t) =>
                {
                    var controller = Client.Get<UserController>();
                    if (rawImageAvatar != null && controller != null && avatarID == controller.GetUserAvatarID())
                    {
                        rawImageAvatar.texture = t;
                    }
                });
            }
        }
    }

    public class TreasureRaidChooseMapPopupController : ViewController<TreasureRaidChooseMapPopup>
    {
        public TreasureRaidMainPopup _mainPopup;
        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            var args = inExtraData as PopupArgs;
            if (args != null)
            {
                _mainPopup = args.extraArgs as TreasureRaidMainPopup;
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventActivityExpire>(OnActivityExpired);
        }
        
        private void OnActivityExpired(EventActivityExpire obj)
        {
            view.Close();
        }
    }
}