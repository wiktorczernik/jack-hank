using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class StatisticsListItem_GUI : MonoBehaviour
{
    [SerializeField] private Animation animationComponent;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private TextMeshProUGUI valueField;

    public UnityEvent onItemAppearStarted;
    public UnityEvent onItemAppearEnded;

    public UnityEvent onItemDisappearStarted;
    public UnityEvent onItemDisappearEnded;

    private bool _isBusy;

    public IEnumerator StartAppearing(string value)
    {
        if (_isBusy) yield break;
        _isBusy = true;

        onItemAppearStarted.Invoke();
        animationComponent.Play();

        yield return new WaitWhile(() => animationComponent.isPlaying);

        valueField.GetComponent<TextMeshProUGUI>().text = value;
        var valueFieldAnimation = valueField.GetComponent<Animation>();
        valueFieldAnimation.Play();
        
        yield return new WaitWhile(() => valueFieldAnimation.isPlaying);

        _isBusy = false;
        onItemAppearEnded.Invoke();
    }

    public IEnumerator StartAppearing(int value)
    {
        if (_isBusy) yield break;
        _isBusy = true;

        onItemAppearStarted.Invoke();
        animationComponent.Play();

        yield return new WaitWhile(() => animationComponent.isPlaying);

        var numericAnim = valueField.GetComponent<CounterAnimationForNumberField_GUI>();

        yield return numericAnim.StartNumericAnimationCo(value, 0, true);
        
        _isBusy = false;
        onItemAppearEnded.Invoke();
    }

    public IEnumerator StartAppearingInTimeField(int timestamp)
    {
        if (_isBusy) yield break;
        _isBusy = true;

        onItemAppearStarted.Invoke();
        animationComponent.Play();

        yield return new WaitWhile(() => animationComponent.isPlaying);

        var numericAnim = valueField.GetComponent<CounterAnimationForNumberField_GUI>();
        
        yield return numericAnim.StartTimestampAnimationCo(timestamp, 0);
        
        _isBusy = false;
        onItemAppearEnded.Invoke();
    }

    public IEnumerator StartDisappear()
    {
        if (_isBusy) yield break;
        _isBusy = true;

        onItemDisappearStarted.Invoke();
        
        animationComponent.Play("Disappear");
        
        yield return new WaitWhile(() => animationComponent.isPlaying);

        gameObject.SetActive(false);
        _isBusy = false;
        onItemDisappearEnded.Invoke();
    }
}