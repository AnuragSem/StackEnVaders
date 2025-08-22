using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [SerializeField] List<Transform> waypoints;

    [Tooltip("How much curved path are blocks/enemies going to follow")]
    [SerializeField] private float curveStrength = 0f;

    [Tooltip("Index of the waypoint that should track the previous block position dynamically (-1 = none)")]
    [SerializeField] private int indexOfWaypointTrackingPreviousBlockTransform = -1;

    public Transform GetWaypointAtIndex(int index)
    {
        if (index >= 0 && index < waypoints.Count)
        {
            return waypoints[index];
        }
        return null;
    }

    public List<Transform> GetCurrentPath()
    {
        return waypoints;
    }

    public int GetWaypointCount()
    {
        return waypoints.Count;
    }

    public float GetCurveStrength()
    {
        return curveStrength;
    }

    public int GetTrackingIndex()
    {
        return indexOfWaypointTrackingPreviousBlockTransform;
    }

    public void SetWaypointPositionAtTrackedIndex(Vector3 p)
    {
        if (indexOfWaypointTrackingPreviousBlockTransform >= 0 && indexOfWaypointTrackingPreviousBlockTransform < waypoints.Count)
        {
            waypoints[indexOfWaypointTrackingPreviousBlockTransform].position = p;
        }
    }

    public void SetCurveStrength(float strength)
    {
        curveStrength = strength;
    }
}
