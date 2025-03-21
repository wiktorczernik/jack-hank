using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BigBonusBoard_GUI : MonoBehaviour
{
    public bool IsShowing => isShowing;
    public event Action OnShowingEnded;
    
    [SerializeField] private TMP_Text label;
    [SerializeField] private Animation animation;

    private bool isShowing;

    public void ShowBigBonus(int bonus, string message)
    {
        if (isShowing) return;
        
        isShowing = true;
        StartCoroutine(PlayNotification(message, bonus));
    }

    public void StopShowing()
    {
        isShowing = false;
        animation.Stop();
        label.text = "";
    }
    
    IEnumerator PlayNotification(string message, int count)
    {
        animation.Play();
        label.text = $"{message}\n{(count > 0 ? "+" : "")}{count}";
        yield return new WaitForSeconds(animation.clip.length);
        label.text = "";
        isShowing = false;
        OnShowingEnded?.Invoke();
    }
}
