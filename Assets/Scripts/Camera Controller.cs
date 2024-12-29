using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform target;
    [SerializeField] float followSpeed = 5f;

    [Header("Camera position for stack game")]
    [SerializeField] float xCameraOffset = 4f;
    [SerializeField] float yCameraOffset = 5f;
    [SerializeField] float zCameraOffset = 4f;

    [Header("Camera rotation for stack game")]
    [SerializeField] float xRotation;
    [SerializeField] float yRotation;
    [SerializeField] float zRotation;

    


    private void LateUpdate()
    {
        FollowBlock();
    }

    private void FollowBlock()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position;

            float targetX = target.position.x + xCameraOffset;
            float targetY = target.position.y + yCameraOffset;
            float targetZ = target.position.z + zCameraOffset;
            

            Vector3 newCameraPosition = new Vector3(targetX, Mathf.Max(target.position.y, targetY),targetZ);
            transform.position = Vector3.Lerp(transform.position, newCameraPosition, followSpeed * Time.deltaTime);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
