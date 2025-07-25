using System;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public static event Action OnEnemyReachedGoal;

    WaypointManager waypointPath;
    int currentTargetWaypointIndex  = 0;

    [SerializeField]float moveSpeed = 3f;

    private void Update()
    {
        MoveEnemy();
    }

    public void SetESpeed(float speed)
    { 
        moveSpeed = speed;
    }

    public void SetPath(WaypointManager path)
    { 
        waypointPath = path;
    }

    void MoveEnemy()
    {
        if (waypointPath == null) return;

        Transform targetWaypoint = waypointPath.GetWaypointAtIndex(currentTargetWaypointIndex);
        if (targetWaypoint != null)
        {
            Vector3 targetWaypointPosition = new Vector3(targetWaypoint.position.x, transform.position.y,
                                                    targetWaypoint.position.z);
            transform.position = Vector3.MoveTowards(transform.position,
                                                    targetWaypointPosition,
                                                    moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetWaypointPosition) < 0.1f)
            { 
                currentTargetWaypointIndex++;
                
                if (currentTargetWaypointIndex >= waypointPath.GetWaypointCount())
                {
                    OnReachGoal();
                }
            }
        }
    }

    void OnReachGoal()
    {
        OnEnemyReachedGoal?.Invoke();

        Debug.Log("reached goal");
        Destroy(gameObject);
    }
}
