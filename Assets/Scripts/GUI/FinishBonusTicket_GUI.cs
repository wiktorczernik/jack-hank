using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FinishBonusTicket_GUI : MonoBehaviour
{
    [SerializeField] private string bonusName;
    [SerializeField] public PlayerBonusTypes playerBonusType;

    public event Action OnEndAnimation;

    private TMP_Text _text;
    private Animation _anim;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _anim = GetComponent<Animation>();
        _text.alpha = 0;
    }
    
    public void SetBonusPoints(int points)
    {
        _text.text = $"{bonusName}: {points}";
    }

    public void StartAnimation()
    {
        StartCoroutine(StartAnimationCo());
    }

    private IEnumerator StartAnimationCo()
    {
        _anim.Play();
        yield return new WaitForSeconds(_anim.clip.length);
        OnEndAnimation?.Invoke();
    }
}
