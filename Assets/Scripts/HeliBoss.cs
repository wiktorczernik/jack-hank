using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HeliBoss : BotVehicle
{
    [Header("Heli Events")]
    public UnityEvent onBurstPrepare;
    public UnityEvent onBurstStart;
    public UnityEvent onBurstEnd;
    public UnityEvent onSuccessFire;
    public UnityEvent onFailFire;

    [Header("Boss Properties")]
    public PlayerVehicle target;
    public Missle missilePrefab;
    public GameObject ground;
    public Transform missileShootAnchor;

    [Header("Heli Burst Fire")]
    public float timeBeforeBurst = 3f;
    public float burstMinDistance = 30f;
    public float burstFireTiming = 0.1f;
    public int burstFireCount = 5;
    public float burstCooldown = 5f;
    public bool burstCooldowned = false;

    [Header("Heli Follow Options")]
    public float currentHeight = 0;
    public float playerVerticalOffset = 20f;
    public float playerForwardDistance = 30f;
    public float verticalAlignSpeed = 1f;

    public void Fire()
    {
        Missle instance = Instantiate(missilePrefab, missileShootAnchor.position, Quaternion.identity);
        instance.homingTarget = target.transform;
        instance.transform.LookAt(target.transform);
        instance.Shoot();
    }
    public void BurstFire()
    {
        StartCoroutine(BurstFireCo());
    }
    IEnumerator BurstFireCo()
    {
        burstCooldowned = true;
        onBurstPrepare?.Invoke();
        yield return new WaitForSeconds(timeBeforeBurst);
        for (int i = 0; i < burstFireCount; ++i)
        {
            float targetDistance = Vector3.Distance(transform.position, target.GetPosition());
            if (targetDistance >= burstMinDistance)
            {
                onSuccessFire?.Invoke();
                Fire();
            }
            else
            {
                onFailFire?.Invoke();
            }
            yield return new WaitForSeconds(burstFireTiming);
        }
        onBurstEnd?.Invoke();
        Invoke(nameof(ResetBurstCooldown), burstCooldown);
        yield return null;
    }
    private void ResetBurstCooldown()
    {
        burstCooldowned = false;
    }
    private void SeekPlayerView()
    {
        arrived = false;
        isFollowing = true;
        destinationPoint = target.GetPosition();
        destinationPoint.y = currentHeight;

        Vector3 destinationOffset = Quaternion.Euler(Vector3.up * target.transform.eulerAngles.y) * Vector3.forward;
        destinationOffset *= playerForwardDistance;
        Vector3 velocityOffset = target.physics.bodyRigidbody.linearVelocity;
        velocityOffset.y = 0;
        destinationOffset += velocityOffset;
        destinationPoint += destinationOffset;
    }
    private void TryBurstFire()
    {
        if (burstCooldowned) return;
        float targetDistance = Vector3.Distance(transform.position, target.GetPosition());
        if (targetDistance > burstMinDistance)
        {
            BurstFire();
        }
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
        //GameObject.Find("TestBus").GetComponent<PlayerVehicle>().playerTurret.AllowFire();
        currentHeight = transform.position.y;
        burstCooldowned = true;
        Invoke(nameof(ResetBurstCooldown), burstCooldown);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        AlignGround();
        SeekPlayerView();
        TryBurstFire();
    }
}
