using System.Linq;
using UnityEngine;

public class LargeDestructableSite : MonoBehaviour
{
    public SmashableEntity[] supportEntities;
    public bool isBeingDestroyed = false;
    public int supportHitCount = 0;
    public Rigidbody[] usedRigidbodies;

    private void OnEnable()
    {
        foreach (var entity in supportEntities)
        {
            entity.OnHit.AddListener(OnSupportSmashed);
        }
    }

    private void OnSupportSmashed(SmashableEntity support)
    {
        supportHitCount++;
        if (supportHitCount == supportEntities.Length)
        {
            BeginDestruction();
        }
    }

    private void BeginDestruction()
    {
        isBeingDestroyed = true;

        foreach(Rigidbody rb in usedRigidbodies)
        {
            rb.freezeRotation = false;
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}
