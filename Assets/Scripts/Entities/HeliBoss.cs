using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;
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
public class HeliBoss : GameEntity, IBossBarApplicable
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

    [Header("Heli Burst Fire")]
    public float predictionTime = 1.5f;
    public float timeBeforeBurst = 3f;
    public float burstMinDistance = 30f;
    public float burstFireTiming = 0.1f;
    public int burstMaxFirings = 10;
    public float burstCooldown = 5f;

    [Header("Heli Follow Options")]
    public NavMeshAgent agent;
    public float maxSampleDistance = 25f;
    public float maxRayHeight = 100f;
    public LayerMask groundMask;

    public float playerVerticalOffset = 50f;
    public float playerForwardDistance = 30f;

    [Header("Visuals")]
    public ParticleSystem[] prepareParticles;

    [Header("Audio Effects")]
    public EventReference propellerEventRef;
    public EventInstance propellerEventInstance;

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
        Vector3 destinationPoint = target.GetPosition();
        destinationPoint.y = transform.position.y;

        Vector3 destinationOffset = Quaternion.Euler(Vector3.up * target.transform.eulerAngles.y) * Vector3.forward;
        destinationOffset *= playerForwardDistance;
        Vector3 velocityOffset = target.physics.bodyRigidbody.linearVelocity;
        velocityOffset.y = 0;
        destinationOffset += velocityOffset;
        destinationPoint += destinationOffset;

        if (NavMesh.SamplePosition(destinationPoint, out NavMeshHit hit, maxSampleDistance, NavMesh.AllAreas))
        {
            Vector3 navPoint = hit.position;

            Ray ray = new Ray(navPoint + Vector3.up * maxRayHeight, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit rhit, maxRayHeight * 2f, groundMask))
            {
                navPoint.y = rhit.point.y;
            }

            agent.SetDestination(navPoint);
        }
    }
    private void Awake()
    {
        propellerEventInstance = RuntimeManager.CreateInstance(propellerEventRef);
        propellerEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        propellerEventInstance.start();
    }
    private void OnDestroy()
    {
        propellerEventInstance.release();
    }
    protected override void OnDeathInternal()
    {
        base.OnDeathInternal();
        propellerEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
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


    protected void FixedUpdate()
    {
        SeekPlayerView();
    }
}
