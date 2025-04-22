using UnityEngine;

public class BillboardBehaviour : MonoBehaviour
{
    public bool pitch = true;
    public bool yaw = true;
    public bool roll = true;
    
    private void Update()
    {
        var currentCamera = Camera.main;

        if (!currentCamera) return;
        Debug.Log("BILLBOARD");

        var lastAngles = transform.eulerAngles;
        
        transform.LookAt(currentCamera.transform.position);
       
        var newAngles = transform.eulerAngles;
       
        transform.eulerAngles = new Vector3(
           pitch ? newAngles.x : lastAngles.x, 
           yaw ? newAngles.y : lastAngles.y, 
           roll ? newAngles.z : lastAngles.z
           );
    }
}
