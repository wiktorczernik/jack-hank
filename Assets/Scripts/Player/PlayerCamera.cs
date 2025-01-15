using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Vector3 position;
    public Vector3 viewAngles;
    public Quaternion viewRotation
    {
        get => Quaternion.Euler(viewAngles);
        set => viewAngles = value.eulerAngles;
    }
    public Vector3 forward => viewRotation * Vector3.forward;

    public Camera worldCam;
    public Camera vmCam;


    public Vector3 GetPosition(){
        return position;
    }
    public void SetPosition(Vector3 newPos){
        position = newPos;
    }
    public Vector3 GetViewAngles(){
        return transform.eulerAngles;
    }
    public void SetViewAngles(Vector3 newAngles){
        viewAngles = newAngles;
    }

    void Start()
    {
        transform.SetParent(null);
    }
    void LateUpdate()
    {
        transform.position = position;
        transform.eulerAngles = viewAngles;
    }
}
