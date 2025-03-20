using TMPro;
using UnityEngine;

public class BonusTicket_GUI : MonoBehaviour
{
    [SerializeField] private string bonusName;
    [SerializeField] private PlayerBonusTypes bonusType;
    [SerializeField] private float timeToShowWithoutUpdatesInSeconds;
    
    public PlayerBonusTypes BonusType => bonusType;
    
    private float _lastUpdateInSeconds;
    private TMP_Text _text;

    public void ChangeBonusValueOn(int value)
    {
        _lastUpdateInSeconds = Time.time;
        UpdateView(value);
    }
    
    public void ChangeBonusValueOn(int value, int comboValue)
    {
        if (bonusType != PlayerBonusTypes.DestructionCombo) 
            throw new System.Exception("This overload of ChangeBonusValueOn should be used only for PlayerBonusTypes.DestructionCombo!");
        
        _lastUpdateInSeconds = Time.time;
        UpdateView(value);
    }

    private void Start()
    {
        _text = GetComponent<TMP_Text>();
        gameObject.SetActive(false);
        enabled = false;
    }

    private void Update()
    {
        if (!(Time.time - _lastUpdateInSeconds > timeToShowWithoutUpdatesInSeconds)) return;
        
        gameObject.SetActive(false);
        enabled = false;
    }

    private void UpdateView(int value)
    {
        gameObject.SetActive(true);
        enabled = true;
        
        _text.text = $"{bonusName.ToUpper()}: {value}";
    }
}