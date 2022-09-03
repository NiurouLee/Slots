// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/05/12:58
// Ver : 1.0.0
// Description : LogicStepType.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public enum LogicStepType
    {
        STEP_START,
        STEP_MACHINE_SETUP,
        STEP_NEXT_SPIN_PREPARE,
        STEP_ROUND_START,
        STEP_SUBROUND_START,
        STEP_WHEEL_SPINNING,
        STEP_WHEEL_STOP_SPECIAL_EFFECT,
        STEP_EARLY_HIGH_LEVEL_WIN_EFFECT,
        STEP_WIN_LINE_BLINK,
        STEP_CONTROL_PANEL_WIN_UPDATE,
        STEP_SPECIAL_BONUS,
        STEP_SPECIAL_BONUS_WIN_NUM_INTERRUPT,
        STEP_LATE_HIGH_LEVEL_WIN_EFFECT,
        STEP_BONUS,
        STEP_BONUS_WIN_NUM_INTERRUPT,
        STEP_RE_SPIN,
        STEP_PRE_ROUND_END_WIN_NUM_INTERRUPT,
        STEP_SUBROUND_FINISH,
        STEP_FREE_GAME,
        STEP_ROUND_FINISH,
    }
}