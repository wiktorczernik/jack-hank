using UnityEngine;

public class HeliBoss : BotVehicle
{
    [Header("Boss Properties")]
    public PlayerVehicle target;
    public Missle missilePrefab;
    public GameObject ground;

    [Header("Boss Follow Options")]
    public float currentHeight = 0;
    public float playerVerticalOffset = 20f;
    public float playerForwardDistance = 30f;
    public float verticalAlignSpeed = 1f;

    public void FuckingShoot()
    {
        Missle instance = Instantiate(missilePrefab, transform.position + Vector3.down * 5, Quaternion.identity);
        instance.homingTarget = target.transform;
        instance.Shoot();
    }

    private void SeekPlayerView()
    {
        arrived = false;
        isFollowing = true;
        destinationPoint = target.GetPosition();
        destinationPoint.y = currentHeight;

        Vector3 destinationOffset = Quaternion.Euler(Vector3.up * target.transform.eulerAngles.y) * Vector3.forward;
        destinationOffset *= playerForwardDistance;

        destinationPoint += destinationOffset;
    }
    private void AlignGround()
    {
        float targetHeight = target.transform.position.y + playerVerticalOffset;

        currentHeight = Mathf.Lerp(currentHeight, targetHeight, verticalAlignSpeed * Time.fixedDeltaTime);

        Vector3 newPos = transform.position;
        newPos.y = currentHeight;

        ground.transform.position = newPos;
    }

    protected override void Awake()
    {
        base.Awake();
        currentHeight = transform.position.y;
        //InvokeRepeating(nameof(FuckingShoot), 1f, 1f);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        AlignGround();
        SeekPlayerView();
    }
}
