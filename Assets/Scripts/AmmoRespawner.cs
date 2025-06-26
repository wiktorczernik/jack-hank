using System.Collections;
using UnityEngine;

public class AmmoRespawner : MonoBehaviour
{
    public Pickupable ammoPrefab;
    [SerializeField] private int ammoAmount;
    [SerializeField] private Transform ammoSpawn;
    [SerializeField] private float delayBeforeRespawn = 20f;
    [SerializeField] private PickupZone pickupZone;

    private Pickupable currentAmmo;
    private float timeFromLastPickUp;

    private void Start()
    {
        Spawn();
    }

    private void OnPickup()
    {
        currentAmmo = null;
        StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeRespawn);
        Spawn();
    }

    private void Spawn()
    {
        currentAmmo = Instantiate(ammoPrefab, transform);
        currentAmmo.transform.SetPositionAndRotation(ammoSpawn.position, ammoSpawn.rotation);
        currentAmmo.GetComponent<AmmoIncrementer>().amount = ammoAmount;
        currentAmmo.onPickup.AddListener(() => OnPickup());
        pickupZone.target = currentAmmo;
    }
}