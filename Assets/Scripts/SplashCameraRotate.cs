using UnityEngine;

public class SplashCameraRotate : MonoBehaviour
{
    public float rotateSpeed = 10f;

    private void Update()
    {
        transform.eulerAngles += Vector3.up * rotateSpeed * Time.deltaTime;
    }
}
