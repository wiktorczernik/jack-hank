using UnityEngine;

public class BusPickupGuidance : MonoBehaviour
{
    [SerializeField] PlayerVehicle player;
    [SerializeField] float switchSideDelay = 0.5f;
    [SerializeField] float unreachableMinAngle = 130;
    [SerializeField] Animator leftAnimator;
    [SerializeField] Animator rightAnimator;

    bool justEntered = false;
    bool lastRight = false;
    bool canSwitchSide = true;

    GameObject pickupable;
    

    private void OnEnable()
    {
        leftAnimator.SetBool("isVisible", false);
        rightAnimator.SetBool("isVisible", false);
        player.onPickupZoneEnter.AddListener(OnZoneEnter);
        player.onPickupZoneExit.AddListener(OnZoneExit);
    }
    private void OnDisable()
    {
        leftAnimator.SetBool("isVisible", false);
        rightAnimator.SetBool("isVisible", false);
        player.onPickupZoneEnter.RemoveListener(OnZoneEnter);
        player.onPickupZoneExit.RemoveListener(OnZoneExit);
    }

    void OnZoneEnter(PickupZone zone)
    {
        justEntered = true;
        pickupable = zone.pickupable;
    }
    void OnZoneExit(PickupZone zone)
    {
        ResetSideSwitchBlock();
        pickupable = null;
    }

    private void Update()
    {
        if (!pickupable)
        {
            leftAnimator.SetBool("isVisible", false);
            rightAnimator.SetBool("isVisible", false);
            return;
        }

        Vector3 origin = transform.position;
        origin.y = 0;
        Vector3 destination = pickupable.transform.position;
        destination.y = 0;
        Vector3 direction = (destination - origin).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 lookAngles = lookRotation.eulerAngles;
        float targetY = lookAngles.y;
        float currentY = player.transform.eulerAngles.y;

        float delta = Mathf.DeltaAngle(currentY, targetY);
        float deltaAbs = Mathf.Abs(delta);

        bool isRight = delta > 0;

        if (deltaAbs > unreachableMinAngle)
        {
            leftAnimator.SetBool("isVisible", false);
            rightAnimator.SetBool("isVisible", false);
        }
        else if ((lastRight != isRight || justEntered) && canSwitchSide)
        {
            leftAnimator.SetBool("isVisible", !isRight);
            rightAnimator.SetBool("isVisible", isRight);
            canSwitchSide = false;
            justEntered = false;
            lastRight = isRight;
            Invoke(nameof(ResetSideSwitchBlock), switchSideDelay);
        }
    }
    private void ResetSideSwitchBlock()
    {
        canSwitchSide = true;
    }
}
