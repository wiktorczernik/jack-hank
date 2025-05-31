using UnityEngine;

[RequireComponent(typeof(TriggerEventEmitter))]
public class AbilitySwitchTrigger : MonoBehaviour
{
    [SerializeField] private bool hasJump;
    [SerializeField] private bool hasNitro;

    private void Awake()
    {
        GetComponent<TriggerEventEmitter>().OnEnter.AddListener(OnEnter);
    }

    private void OnEnter(Collider col)
    {
        var playerAbilities = col.gameObject.GetComponentInParent<PlayerVehicleAbilities>();
        
        if (playerAbilities == null) return;
        
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
}