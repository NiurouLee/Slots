// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/26/11:47
// Ver : 1.0.0
// Description : ServerSelector.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code;
using DragonU3DSDK.Account;
using UnityEngine;
using UnityEngine.UI;

namespace Tool
{
    public class ServerSelector:MonoBehaviour
    {
        private Dropdown _resourceDropdown;
        private Dropdown _serverDropdown;
        private Button _button;

        private TaskCompletionSource<bool> waitSelectTask;
        
        private string[] availableServerUrl = new[]
        {
            "https://fortune-x-api-alpha.ustargames.com|foobar",
            "http://52.81.148.216:9000|foobar",
            "https://cash-craze-api.casualjoygames.com|D9r4HORNmlZxOEBHpJAFX5bWP7bIix055OgINSTU",
            "http://52.81.148.216:9999|foobar",
            "https://fortune-x-simulator-api-alpha.ustargames.com|foobar",
            "http://52.81.148.216:9998|foobar",
            
        };

        private string[] listResServerUrl =
        {
            "https://res.starcdn.cn/cashcraze/ServerData/",
            "https://res.starcdn.cn/cashcraze_res/ServerData/",
            "https://res.starcdn.cn/cashcraze_pre_release/ServerData/",
        };

        private void Awake()
        {
            _resourceDropdown = transform.Find("Content/ResourceServerDropList").GetComponent<Dropdown>();
            _resourceDropdown.options = new List<Dropdown.OptionData>(1);
            _resourceDropdown.options.Add(new Dropdown.OptionData("开发资源服"));
            _resourceDropdown.options.Add(new Dropdown.OptionData("美术仓库资源服"));
            _resourceDropdown.options.Add(new Dropdown.OptionData("预发包资源服"));

            _resourceDropdown.value = 0;
            
            _serverDropdown = transform.Find("Content/ServerDropList").GetComponent<Dropdown>();
            _serverDropdown.options = new List<Dropdown.OptionData>(3);
            _serverDropdown.options.Add(new Dropdown.OptionData("港服"));
            _serverDropdown.options.Add(new Dropdown.OptionData("国服"));
            _serverDropdown.options.Add(new Dropdown.OptionData("正式服"));
            _serverDropdown.options.Add(new Dropdown.OptionData("数值测试专用服"));
            _serverDropdown.options.Add(new Dropdown.OptionData("香港模拟器服"));
            _serverDropdown.options.Add(new Dropdown.OptionData("临时国服"));
            
            _serverDropdown.value = 0;
    #if UNITY_EDITOR        
            _serverDropdown.value = 1;
    #endif
            _button = transform.Find("Content/ConfirmButton").GetComponent<Button>();
            _button.onClick.AddListener(OnConfirmClicked);
        }

        public async Task WaitServerSelection()
        {
            waitSelectTask = new TaskCompletionSource<bool>();
            await waitSelectTask.Task;
        }

        private void OnConfirmClicked()
        {
            var url = availableServerUrl[_serverDropdown.value];
            ConfigurationController.Instance.API_Server_URL_Beta = url.Split('|')[0];
            
            ConfigurationController.Instance.API_Server_Secret_Beta = url.Split('|')[1];

            ConfigurationController.Instance.Res_Server_URL_Beta = listResServerUrl[_resourceDropdown.value];
            int lastServerDropdownValue = -1;
            if (PlayerPrefs.HasKey("ServerDropdownValue"))
            {
                lastServerDropdownValue = PlayerPrefs.GetInt("ServerDropdownValue");
            }

            //need reset
            if (lastServerDropdownValue != _serverDropdown.value)
            {
                if (PlayerPrefs.HasKey("token"))
                {
                    PlayerPrefs.DeleteKey("token");
                    AccountManager.Instance.OnTokenExpire();
                }

                if (PlayerPrefs.HasKey("refreshToken"))
                {
                    PlayerPrefs.DeleteKey("refreshToken");
                    AccountManager.Instance.OnRefreshTokenExpire();
                }

                if (PlayerPrefs.HasKey("tokenExpire"))
                {
                    PlayerPrefs.DeleteKey("tokenExpire");
                }
           
                if (PlayerPrefs.HasKey("RemoteStorageVersion"))
                {
                    PlayerPrefs.DeleteKey("RemoteStorageVersion");
                }
                PlayerPrefs.SetInt("ServerDropdownValue", _serverDropdown.value);
                
                Debug.Log("reset server info in ServerSelector");
            }
            
         
            
            if (waitSelectTask != null)
                waitSelectTask.SetResult(true);
            GameObject.Destroy(gameObject);
        }
    }
}