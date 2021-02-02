using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [field: SerializeField, Tooltip("Target to follow")] 
    private Transform target;
    
    [field: SerializeField, Tooltip("Height from the ground")] 
    private float height = 10f;
    
    [field: SerializeField, Tooltip("Distance from target")] 
    private float distance = 5f;
    
    private Transform _transform;
    
    void Start()
    {
        _transform = transform;
    }

    void Update()
    {
        HandleCamera();
    }

    /// <summary>
    /// Camera follows the target without rotation
    /// </summary>
    private void HandleCamera()
    {
        var worldPos = (Vector3.forward * -distance) + (Vector3.up * height);
        
        var flatTargetPos = target.position;
        flatTargetPos.y = 0;

        var finalPos = flatTargetPos + worldPos;

        _transform.position = finalPos;
    }
}
