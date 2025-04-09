using UnityEngine;

[RequireComponent(typeof(PlayerVehicle))]
public class PlayerVehicleAbilities : MonoBehaviour
{
    [Header("Mana points")]
    public int availableMana
    {
        get => _availableMana;
        set => _availableMana = Mathf.Clamp(value, 0, _maxMana);
    }
    public int maxMana => _maxMana;

    [Header("Abilities")]
    public PlayerNitroAbility nitro;
    public PlayerJumpAbility jump;

    private PlayerVehicle parent;

    private void Awake()
    {
        parent = GetComponent<PlayerVehicle>();
        nitro?.Init(parent);
        jump?.Init(parent);
    }

    public void OnNitro() => TryUseAbility(nitro);
    public void OnJump() => TryUseAbility(jump);

    private bool TryUseAbility(PlayerVehicleAbility ability)
    {
        if (!ability) return false;
        if (ability.requiredMana > availableMana) return false;
        
        if (ability.Use())
        {
            availableMana -= ability.requiredMana;
            return true;
        }

        return false;
    }

    [Header("Settings")]
    [SerializeField] int _availableMana = 0;
    [SerializeField] int _maxMana = 500;
}