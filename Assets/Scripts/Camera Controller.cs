using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    Transform target;
    [SerializeField] float followSpeed = 5f;

    [Header("Camera position for stack game")]
    [SerializeField] float xCameraOffset = 4f;
    [SerializeField] float yCameraOffset = 5f;
    [SerializeField] float zCameraOffset = 4f;

    [Header("Camera Zoom and Pan")]
    public float minZoom = 15f; // Minimum field of view
    public float maxZoom = 90f; // Maximum field of view
    public float defaultFOV = 60f;
    public float zoomSensitivity = 175f; // Speed of zooming

    [SerializeField] public bool canControlCamera;

    //inputs
    private float scrollVal;
    private float panVal;

    private Vector2 previousMousePosition;
    private Vector2 CurrentMousePosition;
    private Vector2 panDelta;
    private bool isPanning;
    public float panSpeed = 4f; // Speed of edge panning

                //clamps
    [SerializeField] Transform panClampA;
    [SerializeField] Transform panClampB;
    [SerializeField] Transform panDownClampC;

    private Vector3 initialCameraPosition;
    private Vector3 initialProjectedPoint;

    Vector3 cameraOffset;
    Vector3 direction;
    Vector3 unitVectorOfCameraLine;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (panClampA != null && panClampB != null)
        {
            unitVectorOfCameraLine = direction = (panClampB.position - panClampA.position).normalized;

            initialCameraPosition = mainCamera.transform.position;

            initialProjectedPoint = ProjectOntoLine(initialCameraPosition, panClampA.position, unitVectorOfCameraLine);

            cameraOffset = initialCameraPosition - initialProjectedPoint;

            //Debug.Log("Initial camera position: " + initialCameraPosition);
            //Debug.Log("Initial projected point: " + initialProjectedPoint);
            //Debug.Log("Camera offset from line: " + cameraOffset);
        }

    }

    private void LateUpdate()
    {
        if (!canControlCamera)
        {
            FollowBlock();
        }
        else if (isPanning)
        { 
            HandlePanning();
        }
    }

    //sends camera horizontal bounds for line rendere to draw a line at max enemy spawing height 
    public List<Vector3> GetTheHorizontalBounds()
    {
        List<Vector3> bounds = new List<Vector3>();
        if (panClampA != null && panClampB != null)
        { 
            bounds.Add(panClampA.position);
            bounds.Add(panClampB.position);
        }
        return bounds;
    }


    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    public void EnableCameraControls(bool enable)
    {
        canControlCamera = enable;
        if (!enable)
        {
            FollowBlock(); // Reset to follow mode
        }
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
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newCameraPosition, followSpeed * Time.deltaTime);
        }
    }

    public void Zoom(float value)
    {
        scrollVal = value;
        HandleZoom();

    }
    public void SetScrollValue(float sv)
    { 
        scrollVal = sv;
    }

    private void HandleZoom()
    {
        if (canControlCamera)
        { 
            float fov = mainCamera.fieldOfView;
            fov -=scrollVal *zoomSensitivity*Time.deltaTime;
            fov = Mathf.Clamp(fov,minZoom,maxZoom);
            mainCamera.fieldOfView = fov;
            
        }
    
    }
    public void SetPanValue(float pv)
    {
        panVal = pv;
    }

    public void OnPanStart()
    {
        if (canControlCamera)
        {
            isPanning = true;
            previousMousePosition = Mouse.current.position.ReadValue();
        }

    }
    public void HandlePanning()
    {
        if (isPanning)
        {

            CurrentMousePosition = Mouse.current.position.ReadValue();
            panDelta = CurrentMousePosition - previousMousePosition;
            previousMousePosition = CurrentMousePosition;

            panDelta = -panDelta;

            float movementAlongLine = panDelta.x * panSpeed * Time.deltaTime;

            float verticalMovement = panDelta.y * panSpeed * Time.deltaTime;

            Vector3 horizontalOffset = unitVectorOfCameraLine * movementAlongLine;
            Vector3 verticalOffset = Vector3.up * verticalMovement;

            Vector3 newPosition = mainCamera.transform.position + horizontalOffset + verticalOffset;

            // Apply clamping
            mainCamera.transform.position = ClampCameraPosition(newPosition);
        }
    }

    Vector3 ClampCameraPosition(Vector3 targetPosition)
    {
        Vector3 lineStart = panClampA.position;
        Vector3 lineEnd = panClampB.position;
        Vector3 lineDir = (lineEnd - lineStart).normalized;
        float lineLength = Vector3.Distance(lineStart, lineEnd);

        Vector3 projectedTarget = ProjectOntoLine(targetPosition, lineStart, lineDir);


        float distanceAlongLine = Vector3.Distance(lineStart, projectedTarget);
        if (Vector3.Dot(projectedTarget - lineStart, lineDir) < 0)
            distanceAlongLine *= -1; 

        distanceAlongLine = Mathf.Clamp(distanceAlongLine, 0, lineLength);

        Vector3 clampedLinePoint = lineStart + lineDir * distanceAlongLine;

        Vector3 finalPosition = clampedLinePoint + new Vector3(cameraOffset.x, 0, cameraOffset.z);

        finalPosition.y = targetPosition.y;

        finalPosition.y = Mathf.Max(finalPosition.y, panDownClampC.position.y);

        return finalPosition;
    }

    private Vector3 ProjectOntoLine(Vector3 point, Vector3 lineStart, Vector3 lineDir)
    {
        return lineStart + Vector3.Project(point - lineStart, lineDir);
    }

    public void OnPanStop()
    {
        isPanning = false;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(panClampA.position, panClampB.position);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(panClampA.position, direction);

    //    if (Application.isPlaying)
    //    {
    //        Vector3 projectedPoint = ProjectOntoLine(transform.position, panClampA.position, unitVectorOfCameraLine);
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawSphere(projectedPoint, 0.5f);
    //        Gizmos.DrawLine(transform.position, projectedPoint); // Line from camera to projection
    //    }
    //}

    public void ResetCameraPositionToMostRecentStackBase(List<Transform>stackBasePositions)
    {
        //make a seprate canvas for paning and scrollig related button or something .. disable it later idk
        //fix it to filter inactive base blocks
        Vector3 currentCamPos = mainCamera.transform.position;
        var activeStackBases = stackBasePositions
        .Where(t => t.childCount > 0 && t.GetChild(0).gameObject.activeInHierarchy)
        .ToList();

        if (activeStackBases.Count == 0)
        {
            Debug.LogWarning("No stack base with an active first child found.");
            return;
        }

        // Find the closest active stack base (based on the base's position)
        Transform closest = activeStackBases
            .OrderBy(t => Vector3.SqrMagnitude(t.position - mainCamera.transform.position))
            .First();

        Vector3 closestStackPos = closest.position;
        float targetX = closestStackPos.x + xCameraOffset;
        float targetY = closestStackPos.y + yCameraOffset;
        float targetZ = closestStackPos.z + zCameraOffset;

        mainCamera.transform.position = new Vector3(targetX, targetY, targetZ);
    

    }

    private void HandleEdgeMovement()
    { 
        
    }
}
