using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BillboardBehaviour : MonoBehaviour
{
    [SerializeField] public bool pitch = true;
    [SerializeField] public bool yaw = true;
    [SerializeField] public bool roll = true;
    
    private void Update()
    {
        var currentCamera = Camera.main;

        if (!currentCamera) return;

        var lastRotation = transform.rotation.eulerAngles;
        
       transform.LookAt(currentCamera.transform.position);

       if (!(!pitch || !yaw || !roll)) return;
       
       var currentRotation = transform.rotation.eulerAngles;
       
       transform.rotation = Quaternion.Euler(
           pitch ? currentRotation.x : lastRotation.x, 
           yaw ? currentRotation.y : lastRotation.y, 
           roll ? currentRotation.z : lastRotation.z);
    }
}
