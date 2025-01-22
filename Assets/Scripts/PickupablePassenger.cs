using UnityEngine;

public class PickupablePassenger : SmashableEntity
{
    public bool isAlive = true;

    protected override void OnHitEvent()
    {
        base.OnHitEvent();
        isAlive = false;
    }
}
