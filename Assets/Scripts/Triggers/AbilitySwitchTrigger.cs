using UnityEngine;

[RequireComponent(typeof(TriggerEventEmitter))]
public class AbilitySwitchTrigger : MonoBehaviour
{
    [SerializeField] private bool hasJump;
    [SerializeField] private bool hasNitro;

    public float triggerDelay = 5f;

    private void Awake()
    {
        GetComponent<TriggerEventEmitter>().OnEnter.AddListener(OnEnter);
    }

    public void DelayedTrigger()
    {
        Invoke(nameof(Trigger), triggerDelay);
    }
    public void Trigger()
    {
        var playerAbilities = GameManager.PlayerVehicle.GetComponent<PlayerVehicleAbilities>();

        if (playerAbilities.jump)
        {
            if (hasJump) playerAbilities.jump.TurnOn();
            else playerAbilities.jump.TurnOff();
        }

        if (playerAbilities.nitro)
        {
            if (hasNitro) playerAbilities.nitro.TurnOn();
            else playerAbilities.nitro.TurnOff();
        }
    }
    private void OnEnter(Collider col)
    {
        var playerAbilities = col.gameObject.GetComponentInParent<PlayerVehicleAbilities>();
        if (playerAbilities)
            Trigger();
    }
}