using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CounterAnimationForNumberField_GUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usedTextComponent;
    [Min(0)][SerializeField] private float durationInSeconds = 1;
    [Min(0)] [SerializeField] private int animationFPS = 60;

    public bool isBusy { get; private set; }
    public UnityEvent onAnimationStarted;
    public UnityEvent onAnimationEnded;

    public void StartNumericAnimation(double targetValue, double startValue = 0, bool useLeadingZeros = false, int digitsAfterPoint = 2)
    {
        if (isBusy || targetValue - startValue == double.Epsilon) return;
        isBusy = true;
        
        onAnimationStarted.Invoke();
        StartCoroutine(AnimationCo());

        IEnumerator AnimationCo()
        {
            var counter = startValue;
            var step = (targetValue - startValue) / animationFPS;
            var stepDuration = durationInSeconds / animationFPS;
            
            var format = useLeadingZeros ? 
                new string('0', GetMaximumDigitsFrom(targetValue, startValue)) + "." + new string('0', digitsAfterPoint): 
                "." + new string('0', digitsAfterPoint);

            for (var i = 0; i < animationFPS * durationInSeconds; i++)
            {
                counter += step;
                
                if (targetValue - startValue < 0 && counter <= targetValue) break;
                if (targetValue - startValue > 0 && counter >= targetValue) break;
                
                usedTextComponent.text = counter.ToString(format);
                yield return new WaitForSeconds(stepDuration);
            }

            usedTextComponent.text = targetValue.ToString(format);
            isBusy = false;
            onAnimationEnded.Invoke();
        }
    }
    
    public void StartNumericAnimation(int targetValue, int startValue = 0, bool useLeadingZeros = false)
    {
        if (isBusy || targetValue - startValue == 0) return;
        isBusy = true;
        
        onAnimationStarted.Invoke();
        StartCoroutine(AnimationCo());

        IEnumerator AnimationCo()
        {
            var counter = startValue;
            var stepsAmount = animationFPS * durationInSeconds;
            var step = (int)((targetValue - startValue) / stepsAmount);
            var format = useLeadingZeros ? new string('0', GetMaximumDigitsFrom(targetValue, startValue)) : "";

            if (step == 0) step = 1;
            
            var stepDuration = durationInSeconds / animationFPS;

            for (var i = 0; i < stepsAmount; i++)
            {
                counter += step;

                if (targetValue - startValue < 0 && counter <= targetValue) break;
                if (targetValue - startValue > 0 && counter >= targetValue) break;
                
                usedTextComponent.text = counter.ToString(format);
                
                yield return new WaitForSeconds(stepDuration);
            }

            usedTextComponent.text = targetValue.ToString(format);
            isBusy = false;
            onAnimationEnded.Invoke();
        }
    }

    public void StartTimestampAnimation(int targetTimestamp, int startTimestamp = 0)
    {
        if (isBusy || targetTimestamp - startTimestamp == 0 || targetTimestamp < 0 || startTimestamp < 0) return;
        isBusy = true;
        
        onAnimationStarted.Invoke();
        StartCoroutine(AnimationCo());

        IEnumerator AnimationCo()
        {
            var counter = startTimestamp;
            var step = (targetTimestamp - startTimestamp) / animationFPS;
            var stepDuration = durationInSeconds / animationFPS;
            var format = "{0:00}:{1:00}";

            for (var i = 0; i < animationFPS * durationInSeconds; i++)
            {
                counter += step;
                
                if (targetTimestamp - startTimestamp < 0 && counter <= targetTimestamp) break;
                if (targetTimestamp - startTimestamp > 0 && counter >= targetTimestamp) break;

                var seconds = (counter / 1000) % 60;
                var minutes = (counter / 60000);
                
                usedTextComponent.text = String.Format(format, minutes, seconds);
                yield return new WaitForSeconds(stepDuration);
            }

            var targetSeconds = (targetTimestamp / 1000) % 60;
            var targetMinutes = (targetTimestamp / 60000);
            
            usedTextComponent.text = String.Format(format, targetMinutes, targetSeconds);
            isBusy = false;
            onAnimationEnded.Invoke();
        }
    }

    private int GetMaximumDigitsFrom(double a, double b)
    {
        return ((int)Math.Max(Math.Abs(a), Math.Abs(b))).ToString().Length;
    }

    private void Awake()
    {
        if (usedTextComponent == null)
        {
            Debug.LogError("CounterAnimationForNumberField_GUI: usedTextComponent is null");
        }
    }
}