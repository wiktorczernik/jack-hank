using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class FinishText_GUI : MonoBehaviour
{
    public event Action OnEndAnimation;
    
    private TMP_Text _text;
    private FinishBonusTicket_GUI[] _finishBonusTickets;
    private Animation _anim;

    private void Awake()
    {
        if (!TryGetComponent(out _text)) Debug.LogError("FinishText_GUI: No TextMeshPro text found!");
        if (!TryGetComponent(out _anim)) Debug.LogError("FinishText_GUI: No Animation component found!");

        _finishBonusTickets = GetComponentsInChildren <FinishBonusTicket_GUI>();
        
        _text.alpha = 0;
    }
    
    public void ShowFinishMark(LevelCompletenessMark mark, Dictionary<PlayerBonusTypes, int> pointsByBonusType)
    {
        _text.text = $"{mark.ToString()} Rank";

        foreach (var (bonusType, points) in pointsByBonusType)
        {
            var ticket = _finishBonusTickets.FirstOrDefault(ticket => ticket.playerBonusType == bonusType);

            if (ticket == null)
            {
                Debug.LogError($"FinishText_GUI: No ticket for {bonusType} found!");
                continue;
            }
            
            ticket.SetBonusPoints(points);
            ticket.StartAnimation();
        }
        
        StartCoroutine(ShowFinishMarkCo());
    }

    private IEnumerator ShowFinishMarkCo()
    {
        _anim.Play();
        yield return new WaitForSeconds(_anim.clip.length);
        OnEndAnimation?.Invoke();
    }
}
