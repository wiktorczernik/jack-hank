using UnityEngine;

public class SetParent : MonoBehaviour
{
    public Transform newParent;
    public Vector3 nullPosition;
    public Vector3 nullAngles;
    public Vector3 nullScale;

    public bool activated = false;
    private bool _previousActivated = false;


    private void Update()
    {
        if (activated == true && _previousActivated == false)
        {
            _previousActivated = true;
            Activate();
        }
    }
    public void Activate()
    {
        transform.SetParent(newParent);
    }
}
