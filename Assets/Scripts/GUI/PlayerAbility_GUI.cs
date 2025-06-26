using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbility_GUI : MonoBehaviour
{
    public PlayerVehicleAbility ability;

    [Header("Tweaks")]
    [SerializeField] float loadingRotateSpeed = 45f;

    [Header("Images")]
    [SerializeField] Image borderImage;
    [SerializeField] Image iconImage;
    [SerializeField] Image loadingImage;
    [SerializeField] RectTransform loadingRect;
    [SerializeField] Slider cooldownSlider;

    [Header("Sprites")]
    [SerializeField] Sprite borderActiveSprite;
    [SerializeField] Sprite borderInactiveSprite;
    [SerializeField] Sprite iconActiveSprite;
    [SerializeField] Sprite iconInactiveSprite;

    [SerializeField] bool initialized = false;

    public void Initialize(PlayerVehicleAbility ability)
    {
        if (!ability)
        {
            Debug.LogError("Can't initialize PlayerAbility GUI", this);
            return;
        }
        this.ability = ability;
        initialized = true;
    }
    public void Free()
    {
        initialized = false;
        ability = null;
    }

    private void UpdateBorder(PlayerVehicleAbility.AbilityState oldState, PlayerVehicleAbility.AbilityState newState)
    {
        borderImage.enabled = true;
        borderImage.sprite = ability.isReady ? borderActiveSprite : borderInactiveSprite;
    }
    private void UpdateIcon(PlayerVehicleAbility.AbilityState oldState, PlayerVehicleAbility.AbilityState newState)
    {
        iconImage.enabled = true;
        iconImage.sprite = ability.isBusy ? iconInactiveSprite : iconActiveSprite;
    }
    private void UpdateLoading(PlayerVehicleAbility.AbilityState oldState, PlayerVehicleAbility.AbilityState newState)
    {
        loadingImage.enabled = ability.isWorking;
    }
    private void UpdateCooldownSlider()
    {
        cooldownSlider.enabled = !(ability.isReady || ability.isWorking || ability.isCooldowned);
        cooldownSlider.value = ability.cooldownFraction;
    }
    private void Update()
    {
        if (!initialized) return;
        if (!ability) return;

        UpdateBorder(ability.state, ability.state);
        UpdateIcon(ability.state, ability.state);
        UpdateLoading(ability.state, ability.state);
        UpdateCooldownSlider();

        var rect = loadingRect;
        rect.Rotate(Vector3.forward * loadingRotateSpeed * Time.deltaTime);
    }
}
