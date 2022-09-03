#if UNITY_EDITOR || !PRODUCTION_PACKAGE
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using GameModule;
using UnityEngine;
using UnityEngine.UI;
public class UIDebuggerElementSetCoins:UIDebuggerElement
{
    [ComponentBinder("SetCoins")] 
    protected RectTransform setCoinsPanel;
    [ComponentBinder("SetCoins/CoinsNum")] 
    protected InputField CoinsNum;
    [ComponentBinder("SetCoins/btnCommit")] 
    protected Button btnCommit;

    private bool waitForData = false;
    public UIDebuggerElementSetCoins(string inButtonText = "defaultBtnText", int inPriority = 10, Action inButtonCallback = null):base(inButtonText, inPriority, inButtonCallback)
    {
        buttonCallback = ShowSetCoinsPanel;
    }
    public override void InitContext(Transform inDebuggerTransform)
    {
        base.InitContext(inDebuggerTransform);
        btnCommit.onClick.AddListener(SetCoins);
        SetDragPointAndObject(setCoinsPanel.transform,setCoinsPanel.transform);
    }
    private static UIDebuggerElementSetCoins instance;
    public static UIDebuggerElementSetCoins Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UIDebuggerElementSetCoins("设置金币数量",2);
            }
            return instance;
        }
    }

    public void ShowSetCoinsPanel()
    {
        setCoinsPanel.pivot = GetContainerPivot();
        var btnPosition = btnElement.transform.position;
        setCoinsPanel.position = btnPosition;
        setCoinsPanel.gameObject.SetActive(!setCoinsPanel.gameObject.activeSelf);
    }
    public async void SetCoins()
    {
        if (waitForData)
        {
            return;
        }
        ulong numCoins = ulong.Parse(CoinsNum.text);
        CSetCoins coins = new CSetCoins();
        coins.Coins = numCoins;
        btnCommit.transform.Find("Text").GetComponent<Text>().text = "修改中";
        waitForData = true;
        var request = await APIManagerGameModule.Instance.SendAsync<CSetCoins, SSetCoins>(coins);
        waitForData = false;
        if (request.ErrorCode == ErrorCode.Success)
        {
            Debug.Log($"==========设置金币成功:{numCoins}");
            btnCommit.transform.Find("Text").GetComponent<Text>().text = "修改成功";
        }
        else
        {
            btnCommit.transform.Find("Text").GetComponent<Text>().text = "修改失败";
        }
    }
}
#endif