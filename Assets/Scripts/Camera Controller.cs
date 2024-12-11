using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform target;
    [SerializeField] float followSpeed = 5f;
    [SerializeField] float verticalCameraOffset = 7f;
    [SerializeField] float horizontalCameraOffset = 5.9f;

    private void LateUpdate()
    {
        FollowBlock();
    }

    private void FollowBlock()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position;

            float targetY = target.position.y + verticalCameraOffset;
            float targetX = target.position.x + horizontalCameraOffset;

            Vector3 newCameraPosition = new Vector3(targetX, Mathf.Max(target.position.y, targetY), transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newCameraPosition, followSpeed * Time.deltaTime);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
