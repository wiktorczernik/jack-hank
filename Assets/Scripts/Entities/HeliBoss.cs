using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class HeliBoss : BotVehicle, IBossBarApplicable
{
    [Header("Burst Fire State")]
    public bool isDuringBurst;
    public bool isBurstCooldowned = true;

    [Header("Heli Events")]
    public UnityEvent onBurstStart;
    public UnityEvent onBurstEnd;
    public UnityEvent onFire;

    [Header("Boss Properties")]
    public PlayerVehicle target;
    public ArcadeMissile missilePrefab;
    public GameObject ground;
    public Transform missileShootAnchor;

    [Header("Heli Burst Fire")]
    public float burstPredictionTime = 1.5f;
    public float timeBeforeBurst = 3f;
    public float burstMinDistance = 30f;
    public float burstFireTiming = 0.1f;
    public int burstMaxFirings = 10;
    public float burstCooldown = 5f;

    [Header("Heli Follow Options")]
    public float currentHeight = 0;
    public float playerVerticalOffset = 20f;
    public float playerForwardDistance = 30f;
    public float verticalAlignSpeed = 1f;

    [Header("Bossbar")]
    public Color primaryColor;
    public Color secondaryColor;
    public string bossTitle;
    public Color PrimaryColor => primaryColor;
    public Color SecondaryColor => secondaryColor;
    public string BossTitle => bossTitle;
    public GameEntity Self => this;

    Vector3 PredictPlayerPosition(float time)
    {
        var rb = target.physics.bodyRigidbody;
        Vector3 startPos = rb.position;
        Vector3 deltaPos = rb.linearVelocity * time;
        Vector3 finalPos = startPos + deltaPos;
        return finalPos;
    }
    public void BurstFire()
    {
        StartCoroutine(BurstFireCo());
    }
    IEnumerator BurstFireCo()
    {
        isBurstCooldowned = true;
        isDuringBurst = true;

        onBurstStart?.Invoke();
        yield return new WaitForSeconds(timeBeforeBurst);
        for (int i = 0; i < burstMaxFirings; ++i)
        {
            Vector3 spawnPos = PredictPlayerPosition(burstPredictionTime) + Vector3.up * 50;
            Instantiate(missilePrefab, spawnPos, Quaternion.identity);
            onFire?.Invoke();

            yield return new WaitForSeconds(burstFireTiming);
        }

        onBurstEnd?.Invoke();
        DelayedResetBurstCooldown(burstCooldown);
    }
    public void DelayedResetBurstCooldown(float delay)
    {
        Invoke(nameof(ResetBurstCooldown), delay);
    }
    public void ResetBurstCooldown()
    {
        isBurstCooldowned = false;
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
        if (isBurstCooldowned) return;
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

        currentHeight = transform.position.y;
        ground.transform.SetParent(null);
        isBurstCooldowned = true;
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
