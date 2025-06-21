using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LeaderListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI bountyPointsText;
    [SerializeField] private TextMeshProUGUI passengersText;
    [SerializeField] private TextMeshProUGUI playTimeText;
    public RectTransform rectTransform;

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void SetBounty(int score)
    {
        bountyPointsText.text = score.ToString();
    }

    public void SetPassengers(int passengers)
    {
        passengersText.text = passengers.ToString();
    }

    public void SetPlayTime(int timestamp)
    {
        var seconds = (timestamp / 1000) % 60;
        var minutes = timestamp / 60000;
        
        playTimeText.text = $"{minutes:00}:{seconds:00}";
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}