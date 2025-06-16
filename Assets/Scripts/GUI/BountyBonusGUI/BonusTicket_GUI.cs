using TMPro;
using UnityEngine;

[ExecuteAlways]
public class BonusTicket_GUI : MonoBehaviour
{
    [SerializeField] private string bonusName;
    [SerializeField] private PlayerBonusTypes bonusType;
    [SerializeField] private float timeToShowWithoutUpdatesInSeconds;
    [SerializeField] private float offsetBetweenTextAndBonus;
    [SerializeField] private TMP_Text bonusValueText;
    [SerializeField] private TMP_Text bonusDescriptionText;
    
    public PlayerBonusTypes BonusType => bonusType;
    
    private float _lastUpdateInSeconds;
    private Animation _anim;

    public void ChangeBonusValueOn(int value)
    {
        _lastUpdateInSeconds = Time.time;
        gameObject.SetActive(true);
        enabled = true;

        _anim.Play("valueChanged");

        bonusValueText.text = $"+{value}";
        bonusDescriptionText.text = bonusName;
        UpdateTextPosition();
    }
    
    public void ChangeBonusValueOn(int value, int comboValue)
    {
        if (bonusType != PlayerBonusTypes.DestructionCombo && bonusType != PlayerBonusTypes.VehicleDestruction)
        {
            Debug.LogError(
                "This overload of ChangeBonusValueOn should be used only for PlayerBonusTypes.DestructionCombo and PlayerBonusTypes.VehicleDestruction!");
            return;
        }
        
        _lastUpdateInSeconds = Time.time;
        gameObject.SetActive(true);
        enabled = true;
        
        _anim.Play("valueChanged");
        
        bonusValueText.text = $"X{comboValue} +{value}";
        bonusDescriptionText.text = bonusName;
        UpdateTextPosition();
    }

    private void UpdateTextPosition()
    {
        bonusDescriptionText.rectTransform.anchoredPosition = new Vector2(bonusValueText.preferredWidth + offsetBetweenTextAndBonus, 0);
    }

    private void Awake()
    {
        if (!Application.isPlaying) return;
        
        gameObject.SetActive(false);
        enabled = false;
        _anim = GetComponentInChildren<Animation>();
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            if (!(Time.time - _lastUpdateInSeconds > timeToShowWithoutUpdatesInSeconds)) return;
        
            gameObject.SetActive(false);
            enabled = false;
        }
        UpdateTextPosition();
    }
}