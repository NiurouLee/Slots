// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/04/26/10:45
// Ver : 1.0.0
// Description : PerformanceTracker.cs
// ChangeLog :
// **********************************************


using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;

public static class PerformanceTracker
{
    public class TrackPoint
    {
        public string trackPointName;
        public float startTime;
        public float finishTime;
        public float costTime;
    }

    public static List<TrackPoint> trackPoints;
    public static Dictionary<string, TrackPoint> trackPointDict;

    static PerformanceTracker()
    {
        trackPoints = new List<TrackPoint>();
        trackPointDict = new Dictionary<string, TrackPoint>();
    }

    public static void ClearAllTrackPoint()
    {
        trackPoints.Clear();
        trackPointDict.Clear();
    }

    public static TrackPoint GetTrackPoint(int index)
    {
        if (trackPoints.Count > index)
        {
            return trackPoints[index];
        }
        return null;
    }
    
    public static TrackPoint GetTrackPoint(string trackPointName)
    {
        if (trackPointDict.ContainsKey(trackPointName))
        {
            return trackPointDict[trackPointName];
        }

        return null;
    }

    public static TrackPoint AddTrackPoint(string trackPointName)
    {
        var point = GetTrackPoint(trackPointName);

        if (point == null)
        {
            point = new TrackPoint();
            point.trackPointName = trackPointName;
            point.startTime = Time.realtimeSinceStartup;
            trackPoints.Add(point);
            trackPointDict.Add(trackPointName, point);
        }

        return point;
    }

    public static void FinishTrackPoint(string trackPointName)
    {
        var point = GetTrackPoint(trackPointName);

        if (point != null)
        {
            point.finishTime = Time.realtimeSinceStartup;
            point.costTime = point.finishTime - point.startTime;
            Debug.Log(
                $"[[ShowOnExceptionHandler]] TrackerPoint:{point.trackPointName}[costTime:{point.costTime}][FinishTime:{point.finishTime}]");
        }
    } 
}