using UnityEditor;
using UnityEngine;

/// <summary>
/// 时间缩放工具
/// </summary>
public class TimeScaleTools
{
    [MenuItem("Tools/TimeScale/VerySlow")]
    public static void SetTimeVerySlow()
    {
        Time.timeScale = 0.1f;
    }

    [MenuItem("Tools/TimeScale/Slow")]
    public static void SetTimeSlow()
    {
        Time.timeScale = 0.5f;
    }

    [MenuItem("Tools/TimeScale/Normal")]
    public static void SetTimeNormal()
    {
        Time.timeScale = 1.0f;
    }

    [MenuItem("Tools/TimeScale/Turbo")]
    public static void SetTimeTurbo()
    {
        Time.timeScale = 5.0f;
    }
}
