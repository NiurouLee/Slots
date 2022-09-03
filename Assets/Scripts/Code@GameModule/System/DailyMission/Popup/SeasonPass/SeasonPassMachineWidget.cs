//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-04 08:49
//  Ver : 1.0.0
//  Description : SeasonPassMachineWidget.cs
//  ChangeLog :
//  **********************************************

using TMPro;

namespace GameModule
{
    public class SeasonPassMachineWidget:Popup<SeasonPassMachineWidgetViewController>
    {
        [ComponentBinder("TimerText")]
        public TextMeshProUGUI timerText;


    }

    public class SeasonPassMachineWidgetViewController : ViewController<SeasonPassMachineWidget>
    {
        
    }
}