using System;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Storage;

using UnityEngine;

namespace GameModule
{
    public class ShowPlayerInfo: MonoBehaviour
    {
        private ulong playerid;
        private MachineLogicController featureMachine;
        private void Start()
        {
            playerid = StorageManager.Instance.GetStorage<StorageCommon>().PlayerId;
            featureMachine = Client.Get<MachineLogicController>();
            _color = Color.green;
        }

        public bool IsShow { get; set; } = false;


        private Color _color;
        private void OnGUI()
        {
            if (IsShow)
            {
                if (Client.Get<UserController>() != null)
                {
                    var coinsCount = Client.Get<UserController>().GetCoinsCount();
                    var diamondCount = Client.Get<UserController>().GetDiamondCount();
                    // if (featureMachine.UserProfile != null)
                    {
                        int w = Screen.width, h = Screen.height;

                        GUIStyle style = new GUIStyle();
                        float height = h * 2 / 100;
                        Rect rect = new Rect(0, 100, w, height);
                        style.alignment = TextAnchor.UpperLeft;
                        style.fontSize = h / 25;
                        style.normal.textColor = _color;

                        string text = $"playerId:{playerid} coins:{coinsCount}, D:{diamondCount}";

                        GUI.Label(rect, text, style);
                    }
                }
            }

            
        }
    }
}