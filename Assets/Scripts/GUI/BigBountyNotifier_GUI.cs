using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BigBountyNotifier_GUI : MonoBehaviour
{
    [SerializeField] TMP_Text _label;
    [SerializeField] Animation _animation;

    void OnEnable()
    {
        GameManager.OnBigBounty += QueueBounty;
    }

    private void QueueBounty(string message, int count)
    {
        StartCoroutine(PlayNotification(message, count));
    }
    IEnumerator PlayNotification(string message, int count)
    {
        _animation.Play();
        _label.text = $"{message}\n+{count}";
        yield return new WaitForSeconds(_animation.clip.length);
        _label.text = "";
    }
}
