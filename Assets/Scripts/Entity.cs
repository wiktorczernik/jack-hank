using UnityEngine;

public class Entity : MonoBehaviour
{
    public virtual void SetPosition(Vector3 newPos){
        transform.position = newPos;
    }
    public virtual Vector3 GetPosition()
    {
        return transform.position;
    }
}
