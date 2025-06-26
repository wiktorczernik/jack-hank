using System;
using System.Collections;
using FMODUnity;
using UnityEngine;

public class PlayerTurret : MonoBehaviour
{
    public static bool canFire;
    public static int ammo;
    public static Action onFire;
    public static Action onAllowFire;
    public static Action onDisallowFire;
    public static Action onLoad;

    [Header("Parts")]
    [SerializeField] Transform Rotor;
    [SerializeField] Transform Head;
    [SerializeField] Transform Nozzle;
    [SerializeField] Transform RaycastOrigin;

    [Header("Turret Stats")]
    public float damage;
    [Tooltip("In rounds per second")] public float fireRate;
    public GameEntity fireTarget;
    public bool targetInProximity;

    [Header("Raycasting")]
    public LayerMask mask;

    [Header("Visual Effects")]
    public Animator animator;
    [SerializeField] ParticleSystem[] particles;
    public float maxNozzleAnimTime = 0.3f;
    public Vector3 nozzleStartPos;
    public Vector3 nozzleEndPos;
    public Light lightSource;

    [Header("Audio Effects")]
    public EventReference audioEventReference;

    private void Update()
    {
        LookAtTarget();
    }

    public void AllowFire()
    {
        if (canFire) return;
        animator.SetTrigger("On Enter");
        canFire = true;
        onAllowFire?.Invoke();
        InvokeRepeating(nameof(TryFire), 0f, 1f / fireRate);
    }
    public void DisallowFire()
    {
        if (!canFire) return;
        animator.SetTrigger("On Exit");
        canFire = false;
        onDisallowFire?.Invoke();
        CancelInvoke(nameof(TryFire));
    }
    private void TryFire()
    {
        if (ammo <= 0f)
        {
            return;
        }

        ForceFire();
    }
    public void ForceFire()
    {
        foreach (ParticleSystem system in particles) system.Play();
        ammo -= 1;
        onFire?.Invoke();
        StartCoroutine(NozzleAnimation(Mathf.Min(maxNozzleAnimTime, 1f / fireRate)));

        RuntimeManager.PlayOneShot(audioEventReference);

        fireTarget.Hurt(damage);

    }
    IEnumerator NozzleAnimation(float animTime)
    {
        lightSource.enabled = true;
        float timePassed = 0f;
        float shotPerc = 0.2f;

        while (timePassed < shotPerc * animTime)
        {
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;

            Nozzle.localPosition = Vector3.Lerp(nozzleStartPos, nozzleEndPos, timePassed / shotPerc / animTime);
        }

        lightSource.enabled = false;

        while (timePassed < animTime)
        {
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;

            Nozzle.localPosition = Vector3.Lerp(nozzleEndPos, nozzleStartPos,
                (timePassed - animTime * shotPerc) / (1 - shotPerc) / animTime);
        }
    }

    private void LookAtTarget()
    {
        if (fireTarget == null) return;

        Vector3 pos = fireTarget.transform.position;
        pos.y = Rotor.position.y;

        Rotor.LookAt(pos);

        Head.LookAt(fireTarget.transform.position);

        foreach (ParticleSystem system in particles)
        {
            var main = system.main;
            main.startRotationX = Head.rotation.eulerAngles.x * Mathf.Deg2Rad;
            main.startRotationY = Head.rotation.eulerAngles.y * Mathf.Deg2Rad;
            main.startRotationZ = Head.rotation.eulerAngles.z * Mathf.Deg2Rad;
        }
    }

    public void LoadAmmo(int amount)
    {
        if (amount <= 0) return;
        ammo += amount;
        onLoad?.Invoke();
    }
}
