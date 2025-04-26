using UnityEngine;

public class HeliBoss : BotVehicle
{
    [Header("Boss Properties")]
    public PlayerVehicle target;
    public Missle missilePrefab;

    public void FuckingShoot()
    {
        Missle instance = Instantiate(missilePrefab, transform.position + Vector3.down * 5, Quaternion.identity);
        instance.homingTarget = target.transform;
        instance.Shoot();
    }

    protected void Start()
    {
        InvokeRepeating(nameof(FuckingShoot), 2, 2);
    }
}
