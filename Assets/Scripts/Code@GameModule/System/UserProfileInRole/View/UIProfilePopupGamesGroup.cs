using System.Text.RegularExpressions;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIProfilePopupGamesGroup : View<UIProfilePopupGamesGroupController>
    {
        [ComponentBinder("StatsGroup/TotalSpinsGroup/IntegralGroup/IntegralText")]
        private TMP_Text _tmpTextTotalSpins;

        [ComponentBinder("StatsGroup/TotalWinGroup/IntegralGroup/IntegralText")]
        private TMP_Text _tmpTextTotalWin;


        [ComponentBinder("StatsGroup/BiggestWinGroup/IntegralGroup/IntegralText")]
        private TMP_Text _tmpTextBiggestWin;


        [ComponentBinder("StatsGroup/JackpotHitGroup/IntegralGroup/IntegralText")]
        private TMP_Text _tmpTextJackpotHit;

        [ComponentBinder("StatsGroup/BigWinGroup/IntegralGroup/IntegralText")]
        private TMP_Text _tmpTextBigWin;


        private Image[] _imageSlotIcons = new Image[3];
        private AssetReference[] _assetReferences = new AssetReference[3];
        private GameObject[] _icons = new GameObject[3];

        private Vector3 _scale = Vector3.one * 0.81f;

        protected override void BindingComponent()
        {
            base.BindingComponent();
            for (int i = 0; i < 3; i++)
            {
                var trans = transform.Find($"MostPlayedGroup/MachineIconImage{i+1}");
                var image = trans.GetComponent<Image>();
                _imageSlotIcons[i] = image;
            }
        }

        public override void Show()
        {
            base.Show();
            Refresh();
        }

        public void Refresh()
        {
            var controller = Client.Get<UserProfileInRoleController>();
            var data = controller.sGetUserProfileInRoleData;
            if (data == null) { return; }

            var roleInfo = data?.RoleInfo;
            var gameStatInfo = roleInfo?.GameStatInfo;

            if (_tmpTextTotalSpins != null)
            {
                _tmpTextTotalSpins.text = gameStatInfo?.TotalSpin.GetCommaFormat();
            }

            if (_tmpTextTotalWin != null)
            {
                _tmpTextTotalWin.text = Regex.Replace(gameStatInfo?.TotalWinBigInt, "(\\d)(?=(\\d{3})+$)", "$1,");
            }

            if (_tmpTextBiggestWin != null)
            {
                _tmpTextBiggestWin.text = gameStatInfo?.BiggestWin.GetCommaFormat();
            }

            if (_tmpTextJackpotHit != null)
            {
                _tmpTextJackpotHit.text = gameStatInfo?.JackpotHit.GetCommaFormat();
            }

            if (_tmpTextBigWin != null)
            {
                _tmpTextBigWin.text = gameStatInfo?.BigWin.GetCommaFormat();
            }

            var icons = gameStatInfo?.MostPlayed;
            ReleaseAsset();

            if (icons == null || icons.count == 0)
            {
                foreach (var item in _imageSlotIcons)
                {
                    item.enabled = true;
                }
            }
            else
            {
                var iconsCount = icons.count;
                for (int i = 0; i < 3; i++)
                {
                    var image = _imageSlotIcons[i];
                    var index = i;

                    if (index < iconsCount)
                    {
                        image.enabled = false;

                        var iconAddress = $"Banner{icons[index].GameId}";
                        var assetReference = AssetHelper.PrepareAsset<GameObject>(iconAddress, (ar) =>
                        {
                            var prefab = ar.GetAsset<GameObject>();
                            if (prefab != null)
                            {
                                var go = GameObject.Instantiate(prefab, image.transform);
                                go.transform.localScale = _scale;
                                _icons[index] = go;
                            }
                            else
                            {
                                image.enabled = true;
                            }
                        });
                        _assetReferences[index] = assetReference;
                    }
                    else
                    {
                        image.enabled = true;
                    }
                }
            }
        }

        public void ReleaseAsset()
        {
            for (int i = 0; i < 3; i++)
            {
                var icon = _icons[i];
                if (icon != null) { GameObject.Destroy(icon); }
                _icons[i] = null;

                var assetReference = _assetReferences[i];
                if (assetReference != null) { assetReference.ReleaseOperation(); }
                _assetReferences[i] = null;
            }
        }
    }

    public class UIProfilePopupGamesGroupController : ViewController<UIProfilePopupGamesGroup>
    {
        public override void OnViewDestroy()
        {
            base.OnViewDestroy();
            view.ReleaseAsset();
        }
    }
}