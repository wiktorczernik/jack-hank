using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class HeliBoss : BotVehicle, IBossBarApplicable
{
    [Header("Burst Fire State")]
    public bool isDuringBurst;
    public bool isBurstTimedOut = false;
    public bool isBurstCooldowned = false;
    public int missilesLeft = -1;
    public MissleCrosshair[] activeCrosshairs;
    public Missle[] shotMissiles;

    [Header("Heli Events")]
    public UnityEvent onBurstStart;
    public UnityEvent onBurstEnd;
    public UnityEvent onFire;

    [Header("Boss Properties")]
    public PlayerVehicle target;
    public Missle missilePrefab;
    public MissleCrosshair crosshairPrefab;
    public GameObject ground;
    public Transform missileShootAnchor;

    [Header("Heli Burst Fire")]
    public float burstTimeOut = 5f;
    public float timeBeforeBurst = 3f;
    public float burstMinDistance = 30f;
    public float burstFireTiming = 0.1f;
    public int burstMaxFirings = 5;
    public float burstCooldown = 5f;
    public float sideCrosshairDistance = 10f;

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

    public Missle CreateMissile(MissleCrosshair crosshair)
    {
        Missle instance = Instantiate(missilePrefab, missileShootAnchor.position, Quaternion.identity);

        crosshair.Show();

        instance.enabled = false;
        instance.homingTarget = crosshair.transform;
        instance.transform.LookAt(crosshair.transform);

        return instance;
    }
    public void BurstFire()
    {
        StartCoroutine(BurstFireCo());
    }
    private void CreateCrosshairs()
    {
        for (int i = 0; i < burstMaxFirings; ++i)
        {
            activeCrosshairs[i] = Instantiate(crosshairPrefab);
        }
    }
    private void DestroyCrosshairs()
    {
        foreach (var crosshair in activeCrosshairs)
        {
            Destroy(crosshair);
        }
    }
    IEnumerator BurstFireCo()
    {
        CreateCrosshairs();
        isBurstCooldowned = true;
        isDuringBurst = true;

        onBurstStart?.Invoke();

        missilesLeft = 0;
        yield return new WaitForSeconds(timeBeforeBurst);
        for (int i = 0; i < burstMaxFirings; ++i)
        {
            missilesLeft++;

            var crosshair = activeCrosshairs[i];
            var missile = CreateMissile(crosshair);

            void ExplosionEventHandler(ExplosionProperties p)
            {
                OnMissileExplode(missile, crosshair);
                missile.onSelfExplode -= ExplosionEventHandler;
            }

            missile.onSelfExplode += ExplosionEventHandler;
            missile.enabled = true;
            missile.Shoot();
            shotMissiles[i] = missile;

            onFire?.Invoke();

            yield return new WaitForSeconds(burstFireTiming);
        }

        Invoke(nameof(TimeOutBurst), burstTimeOut);

        yield return new WaitUntil(() => missilesLeft <= 0 || isBurstTimedOut == true);

        isBurstTimedOut = false;
        onBurstEnd?.Invoke();
        Invoke(nameof(ResetBurstCooldown), burstCooldown);
        DestroyCrosshairs();

        missilesLeft = -1;
    }
    public void TimeOutBurst()
    {
        isBurstTimedOut = true;
    }
    private void ResetBurstCooldown()
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
    private void OnMissileExplode(Missle missile, MissleCrosshair crosshair)
    {
        crosshair.Hide();
        missilesLeft--;
    }

    protected override void Awake()
    {
        base.Awake();

        shotMissiles = new Missle[burstMaxFirings];
        activeCrosshairs = new MissleCrosshair[burstMaxFirings];

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
    protected void LateUpdate()
    {
        if (!isDuringBurst) return;

        var targetPos = target.GetPosition();
        MissleCrosshair crosshair = activeCrosshairs[0];
        if (crosshair)
            crosshair.SetPosition(targetPos);

        // TODO: make it random?
        int sideCount = burstMaxFirings - 1;
        float angleOffset = 0;
        float stepAngle = 360 / sideCount;

        for (int i = 0; i < sideCount; ++i)
        {
            crosshair = activeCrosshairs[1 + i];
            float angle = i * stepAngle;
            angle += angleOffset;
            targetPos = target.GetPosition();
            targetPos += Quaternion.Euler(Vector3.up * angle) * Vector3.forward * sideCrosshairDistance;
            if (crosshair)
                crosshair.SetPosition(targetPos);
        }
    }
}
