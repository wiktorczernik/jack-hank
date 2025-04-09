using System;
using UnityEngine;

[RequireComponent(typeof(PlayerVehicle))]
public class PlayerVehicleAbilities : MonoBehaviour
{
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

    private void TryUseAbility(PlayerVehicleAbility ability)
    {
        ability?.Use();
    }
}