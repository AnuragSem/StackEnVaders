using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [SerializeField] List<Transform> waypointPath;

    public Transform GetWaypointAtIndex(int index)
    {
        if (index >= 0 && index < waypointPath.Count)
        { 
            return waypointPath[index];
        }
        return null;
    }

    public int GetWaypointCount()
    { 
        return waypointPath.Count;
    }
}
