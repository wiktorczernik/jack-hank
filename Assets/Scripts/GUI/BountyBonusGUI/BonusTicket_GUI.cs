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
    [SerializeField] private RectTransform rect;
    
    public PlayerBonusTypes BonusType => bonusType;
    
    private float _lastUpdateInSeconds;
    private Animation _anim;
    private bool _hasUsualDirection = true;

    public void ChangeBonusValueOn(int value)
    {
        _lastUpdateInSeconds = Time.time;
        gameObject.SetActive(true);
        enabled = true;

        _anim.Play("valueChanged");

        bonusValueText.text = $"+{value}";
        bonusDescriptionText.text = bonusName;
        UpdateOffset();
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
        UpdateOffset();
    }

    public void SetReverseDirection()
    {
        if (!_hasUsualDirection) return;
        
        bonusDescriptionText.rectTransform.pivot = new Vector2(1, 1);
        bonusDescriptionText.rectTransform.anchorMax = new Vector2(0, 1);
        bonusDescriptionText.rectTransform.anchorMin = new Vector2(0, 1);
        
        bonusDescriptionText.rectTransform.sizeDelta = 
            new Vector2(bonusDescriptionText.preferredWidth, bonusDescriptionText.preferredHeight);

        _hasUsualDirection = false;
        
        UpdateOffset();
    }
    
    public void SetUsualDirection()
    {
        if (_hasUsualDirection) return;

        bonusDescriptionText.rectTransform.pivot = new Vector2(0, 1);
        bonusDescriptionText.rectTransform.anchorMax = new Vector2(1, 1);
        bonusDescriptionText.rectTransform.anchorMin = new Vector2(1, 1);
        
        bonusDescriptionText.rectTransform.sizeDelta = 
            new Vector2(bonusDescriptionText.preferredWidth, bonusDescriptionText.preferredHeight);
        
        _hasUsualDirection = true;
        
        UpdateOffset();
    }

    private void UpdateOffset()
    {
        rect.sizeDelta = 
            new Vector2(bonusValueText.preferredWidth, bonusValueText.preferredHeight);
        
        bonusDescriptionText.rectTransform.anchoredPosition = 
            new Vector2(_hasUsualDirection ? offsetBetweenTextAndBonus : -offsetBetweenTextAndBonus, 0);
    }

    private void Awake()
    {
        if (!Application.isPlaying) return;
        
        gameObject.SetActive(false);
        enabled = true;
        _anim = GetComponentInChildren<Animation>();
        
        bonusDescriptionText.rectTransform.sizeDelta = 
            new Vector2(bonusDescriptionText.preferredWidth, bonusDescriptionText.preferredHeight);
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            if (!(Time.time - _lastUpdateInSeconds > timeToShowWithoutUpdatesInSeconds)) return;
        
            gameObject.SetActive(false);
            enabled = false;
        }
        UpdateOffset();
    }
}