using System.Collections;
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

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => ability != null);

        ability.onStateUpdate.AddListener(UpdateBorder);
        ability.onStateUpdate.AddListener(UpdateIcon);
        ability.onStateUpdate.AddListener(UpdateLoading);
        ability.onCooldownTick.AddListener(UpdateCooldownSlider);
        ability.onCooldownEnd.AddListener(UpdateCooldownSlider);
    }
    private void OnDisable()
    {
        ability.onStateUpdate.RemoveListener(UpdateBorder);
        ability.onStateUpdate.RemoveListener(UpdateIcon);
        ability.onStateUpdate.RemoveListener(UpdateLoading);
        ability.onCooldownTick.RemoveListener(UpdateCooldownSlider);
        ability.onCooldownEnd.RemoveListener(UpdateCooldownSlider);
    }

    private void UpdateBorder(PlayerVehicleAbility.AbilityState oldState, PlayerVehicleAbility.AbilityState newState)
    {
        borderImage.sprite = ability.isReady ? borderActiveSprite : borderInactiveSprite;
    }
    private void UpdateIcon(PlayerVehicleAbility.AbilityState oldState, PlayerVehicleAbility.AbilityState newState)
    {
        iconImage.sprite = ability.isBusy ? iconInactiveSprite : iconActiveSprite;
    }
    private void UpdateLoading(PlayerVehicleAbility.AbilityState oldState, PlayerVehicleAbility.AbilityState newState)
    {
        loadingImage.enabled = ability.isWorking;
    }
    private void UpdateCooldownSlider()
    {
        cooldownSlider.value = ability.cooldownFraction;
    }
    private void Update()
    {
        var rect = loadingRect;
        rect.Rotate(Vector3.forward * loadingRotateSpeed * Time.deltaTime);
    }
}
