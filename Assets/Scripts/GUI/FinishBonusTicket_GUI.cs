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
    private Animator _animator;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
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
        _animator.enabled = true;
        yield return new WaitForSeconds(_animator.runtimeAnimatorController.animationClips[0].length);
        OnEndAnimation?.Invoke();
    }
}
