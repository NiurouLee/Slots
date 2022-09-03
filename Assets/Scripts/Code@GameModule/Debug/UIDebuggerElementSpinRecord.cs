#if UNITY_EDITOR || !PRODUCTION_PACKAGE
using System;
using GameModule;
using UnityEngine;
using UnityEngine.UI;

public class UIDebuggerElementSpinRecord:UIDebuggerElement
{
    [ComponentBinder("GameRecord")] 
    private RectTransform gameRecord;

    [ComponentBinder("GameRecord/RecordPanel/RecordName")] 
    private InputField spinRecordName;

    [ComponentBinder("GameRecord/RecordPanel/RemoveName")]
    private InputField removeRecordName;

    [ComponentBinder("GameRecord/RecordPanel/RecordingPoint")]
    private Transform recordingPoint;

    [ComponentBinder("GameRecord/RecordPanel/Dropdown")]
    protected Dropdown dropdown;

    [ComponentBinder("GameRecord/RecordPanel/Start")]
    private Button btnStart;
    
    [ComponentBinder("GameRecord/RecordPanel/Stop")]
    private Button btnStop;
    
    [ComponentBinder("GameRecord/RecordPanel/Save")]
    private Button btnSave;

    [ComponentBinder("GameRecord/RecordPanel/Load")]
    private Button btnLoad;
    
    [ComponentBinder("GameRecord/RecordPanel/Remove")]
    private Button btnRemove;
    
    [ComponentBinder("GameRecord/RecordPanel/Use")]
    private Button btnUse;
    
    [ComponentBinder("GameRecord/RecordPanel/UsingRecordCount")]
    private Transform usingRecordPoint;
    
    [ComponentBinder("GameRecord/RecordPanel/UsingRecordCount/Text")]
    private Text usingRecordLeftCount;

    public UIDebuggerElementSpinRecord(string inButtonText = "defaultBtnText", int inPriority = 10, Action inButtonCallback = null):base(inButtonText,inPriority, inButtonCallback)
    {
        buttonCallback = OnButtonRecord;
    }
    private static UIDebuggerElementSpinRecord instance;
    public static UIDebuggerElementSpinRecord Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UIDebuggerElementSpinRecord("数据重播",1);
            }

            return instance;
        }
    }
    public override void InitContext(Transform inDebuggerTransform)
    {
        base.InitContext(inDebuggerTransform);
        btnStart.onClick.RemoveAllListeners();
        btnStop.onClick.RemoveAllListeners();
        btnSave.onClick.RemoveAllListeners();
        btnLoad.onClick.RemoveAllListeners();
        btnStart.onClick.RemoveAllListeners();
        btnRemove.onClick.RemoveAllListeners();
        btnUse.onClick.RemoveAllListeners();
        
        btnStart.onClick.AddListener(StartRecord);
        btnStop.onClick.AddListener(StopRecord);
        btnSave.onClick.AddListener(SaveRecord);
        btnLoad.onClick.AddListener(LoadRecord);
        btnRemove.onClick.AddListener(RemoveRecord);
        btnUse.onClick.AddListener(UseRecord);
        SetDragPointAndObject(gameRecord.transform,gameRecord.transform);
    }
    
    private void OnButtonRecord()
    {
        gameRecord.pivot = GetContainerPivot();
        var btnPosition = btnElement.transform.position;
        gameRecord.position = btnPosition;
        gameRecord.gameObject.SetActive(!gameRecord.gameObject.activeSelf);

        dropdown.options = SpinDataRecord.GetAvailableRecord();
    }
    private void StartRecord()
    {
        SpinDataRecord.activeFeature = spinRecordName.text;
        SpinDataRecord.isRecording = true;
        recordingPoint.gameObject.SetActive(true);
        SpinDataRecord.ClearRecord(spinRecordName.text);
    }
    private void StopRecord()
    {
        SpinDataRecord.isRecording = false;
        recordingPoint.gameObject.SetActive(false);
        dropdown.options = SpinDataRecord.GetAvailableRecord();
    }
    private void SaveRecord()
    {
        SpinDataRecord.isRecording = false;
        recordingPoint.gameObject.SetActive(false);
        SpinDataRecord.SaveRecord();
    }

    private void LoadRecord()
    {
        SpinDataRecord.isRecording = false;
        recordingPoint.gameObject.SetActive(false);
        SpinDataRecord.LoadRecord();
        dropdown.options = SpinDataRecord.GetAvailableRecord();
    }

    private void RemoveRecord()
    {
        SpinDataRecord.RemoveRecord(removeRecordName.text);
        dropdown.options = SpinDataRecord.GetAvailableRecord();
    }
    
    private void UseRecord()
    {
        if (dropdown.options.Count > 0 && dropdown.options.Count > dropdown.value)
        {
            if (SpinDataRecord.usingRecord && SpinDataRecord.activeFeature == dropdown.options[dropdown.value].text)
            {
                SpinDataRecord.usingRecord = false;
                SpinDataRecord.currentIndex = 0;
                RefreshUsingRecordLeftCount();
            }
            else
            {
                SpinDataRecord.isRecording = false;
                recordingPoint.gameObject.SetActive(false);
                SpinDataRecord.usingRecord = true;
                SpinDataRecord.currentIndex = 0;
                SpinDataRecord.activeFeature = dropdown.options[dropdown.value].text;
                RefreshUsingRecordLeftCount();
            }
        }
    }

    public void RefreshUsingRecordLeftCount()
    {
        var leftCount = SpinDataRecord.GetLeftDataCount();
        usingRecordLeftCount.text = SpinDataRecord.GetLeftDataCount().ToString();
        if (leftCount > 0)
        {
            usingRecordPoint.gameObject.SetActive(true);
        }
        else
        {
            usingRecordPoint.gameObject.SetActive(false);
        }
    }
}
#endif