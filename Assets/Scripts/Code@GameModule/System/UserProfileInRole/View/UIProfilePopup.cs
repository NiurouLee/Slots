using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIProfileMain")]
    public class UIProfilePopup : Popup<UIProfileController>
    {

        [ComponentBinder("Root/ProfileGroup")]
        public Transform transProfileGroup;

        [ComponentBinder("Root/GamesGroup")]
        public Transform transGamesGroup;

        [ComponentBinder("Root/ShiftGroup/ProfileButton")]
        public Button buttonProfile;

        [ComponentBinder("Root/ShiftGroup/ProfileButton/BGMask")]
        public Transform transButtonProfileMask;

        [ComponentBinder("Root/ShiftGroup/GamesButton")]
        public Button buttonGames;

        [ComponentBinder("Root/ShiftGroup/GamesButton/BGMask")]
        public Transform transButtonGamesMask;

        public UIProfilePopupProfileGroup profileGroup;

        public UIProfilePopupGamesGroup gamesGroup;

        public enum State
        {
            None, Profile, Games
        }

        public State currentState { get; private set; } = State.None;
 
        public SpriteAtlas atlas;

        public UIProfilePopup(string address) : base(address)
        {
            contentDesignSize = new Vector2(1200, 690);
        }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            profileGroup = AddChild<UIProfilePopupProfileGroup>(transProfileGroup);
            gamesGroup = AddChild<UIProfilePopupGamesGroup>(transGamesGroup);
        }

        public override void OnOpen()
        {
            base.OnOpen();

            atlas = AssetHelper.GetResidentAsset<SpriteAtlas>("CommonUIAtlas");
            
            SetState(State.Profile, true);
        }
 
        public void SetState(State state, bool forceUpdate)
        {
            if (currentState == state && forceUpdate == false) { return; }
            currentState = state;

            switch (state)
            {
                case State.Profile:
                    profileGroup.Show();
                    gamesGroup.Hide();
                    buttonProfile.transform.SetAsLastSibling();
                    transButtonProfileMask.gameObject.SetActive(false);
                    transButtonGamesMask.gameObject.SetActive(true);

                    break;
                case State.Games:
                    profileGroup.Hide();
                    gamesGroup.Show();
                    buttonGames.transform.SetAsLastSibling();
                    transButtonProfileMask.gameObject.SetActive(true);
                    transButtonGamesMask.gameObject.SetActive(false);

                    break;
                case State.None:
                default:
                    SetState(State.Profile, forceUpdate);
                    break;
            }
        }
    }

    public class UIProfileController : ViewController<UIProfilePopup>
    {
        private UserProfileInRoleController _controller;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.buttonProfile.onClick.AddListener(OnProfileButtonClicked);
            view.buttonGames.onClick.AddListener(OnGamesButtonClicked);

            _controller = Client.Get<UserProfileInRoleController>();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventUpdateRoleInfo>(OnUpdateRoleInfo);
            SubscribeEvent<EventRefreshUserProfile>(OnEventRefreshUserProfile);
        }

        private void OnEventRefreshUserProfile(EventRefreshUserProfile obj)
        {
            view.profileGroup.Refresh();
        }

        private void OnUpdateRoleInfo(EventUpdateRoleInfo obj)
        {
            view.profileGroup.Refresh();
        }

        private void OnProfileButtonClicked()
        {
            if (view.currentState != UIProfilePopup.State.Profile)
            {
                SoundController.PlaySwitchFx();
            }

            view.SetState(UIProfilePopup.State.Profile, false);
        }

        private void OnGamesButtonClicked()
        {
            if (view.currentState != UIProfilePopup.State.Games)
            {
                SoundController.PlaySwitchFx();
            }

            view.SetState(UIProfilePopup.State.Games, false);
        }
    }
}