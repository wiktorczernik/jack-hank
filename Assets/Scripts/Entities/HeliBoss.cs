using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct HeliBossWaveConfig
{
    [Header("General")]
    public float cooldownTime;
    /// <summary>
    /// How long does it shows that rockets will fire soon
    /// </summary>
    public float prepareDuration;
    /// <summary>
    /// How much predictive rockets will be fired.
    /// </summary>
    [Header("Predictive Rockets")]
    public int predictiveRocketsCount;
    /// <summary>
    /// How much time will helicopter wait after attack start, before it starts shooting predictive rockets
    /// </summary>
    public float predictiveBurstDelay;
    /// <summary>
    /// How much time will helicopter wait before shooting next rocket
    /// </summary>
    public float predictiveFirePause;
}
public class HeliBoss : BotVehicle, IBossBarApplicable
{
    [Header("Boss State")]
    public PlayerVehicle target;
    public int currentWave = -1;
    public bool isAttacking = false;
    public bool isCooldowned = true;

    [Header("Boss Events")]
    public UnityEvent onAttackPrepare;
    public UnityEvent onAttackBegin;
    public UnityEvent onAttackEnd;

    [Header("Boss Config")]
    public HeliBossWaveConfig[] waves;
    public ArcadeMissile staticMissilePrefab;
    public ArcadeMissile predictiveMissilePrefab;
    public GameObject ground;

    [Header("Heli Burst Fire")]
    public float predictionTime = 1.5f;
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

    [Header("Visuals")]
    public ParticleSystem[] prepareParticles;

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
    public void StartTotalFlyingDestruction()
    {
        StartCoroutine(WaveCoroutine());
    }
    IEnumerator WaveCoroutine()
    {
        currentWave = 0;
        
        while (isAlive)
        {
            HeliBossWaveConfig wave = waves[currentWave];

            isAttacking = false;
            isCooldowned = true;
            yield return new WaitForSeconds(wave.cooldownTime);
            yield return new WaitUntil(() => Vector3.Distance(transform.position, target.GetPosition()) > burstMinDistance);

            isAttacking = true;
            isCooldowned = false;
            foreach(ParticleSystem particle in prepareParticles)
            {
                particle.Stop();
                var main = particle.main;
                main.duration = wave.prepareDuration;
                particle.Play();
            }
            onAttackPrepare?.Invoke();
            yield return new WaitForSeconds(wave.prepareDuration);

            onAttackBegin?.Invoke();
            StartCoroutine(PredictiveRocketsCoroutine(wave));

            float attackDuration = wave.predictiveBurstDelay + wave.predictiveFirePause * wave.predictiveRocketsCount;
            yield return new WaitForSeconds(attackDuration);
            onAttackEnd?.Invoke();

            currentWave = Mathf.Clamp(currentWave + 1, 0, waves.Length - 1);

        }
    }
    private IEnumerator PredictiveRocketsCoroutine(HeliBossWaveConfig wave)
    {
        yield return new WaitForSeconds(wave.predictiveBurstDelay);
        for (int i = 0; i < wave.predictiveRocketsCount; i++)
        {
            Vector3 spawnPos = PredictPlayerPosition(predictionTime) + Vector3.up * 50;
            Instantiate(predictiveMissilePrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(wave.predictiveFirePause);
        }
    }
    private void AlignGround()
    {
        float targetHeight = target.transform.position.y + playerVerticalOffset;

        float oldHeight = currentHeight;
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
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        AlignGround();
        SeekPlayerView();
    }
}
