using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointSettings : MonoBehaviour
{
    [Header("Path Settings")]
    public List<Transform> waypoints;

    [Tooltip("How much curved path are blocks going to follow")]
    public float curveStrength = -99f;

    [Tooltip("Index of the waypoint that should track the previous block position dynamically")]
    public int waypointTrackingPreviousBlockTransform = -99;


    public Transform GetWaypointAtIndex(int index)
    {
        if (index >= 0 && index < waypoints.Count)
        {
            return waypoints[index];
        }
        return null;
    }

    public int GetWaypointCount()
    {
        return waypoints.Count;
    }
    public float GetCurveStrength()
    {
        if (curveStrength == -99f)
            Debug.Log("CurveStrength Not Set");

        return curveStrength;
    }

    public int GetTrackingWaypoint()
    {
        if (curveStrength == -99f)
            Debug.Log("Tracking WayPointIndex  Not Set");

        return waypointTrackingPreviousBlockTransform;
    }

    public void SetWaypointPositionAtIndex(int index, Vector3 p)
    {
        waypoints[index].position = p;
    }


}
