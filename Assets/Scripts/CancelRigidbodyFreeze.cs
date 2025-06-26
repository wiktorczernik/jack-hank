using UnityEngine;

public class CancelRigidbodyFreeze : MonoBehaviour
{
    public void Proceed()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = false;
        rigidbody.constraints = RigidbodyConstraints.None;
    }
}
