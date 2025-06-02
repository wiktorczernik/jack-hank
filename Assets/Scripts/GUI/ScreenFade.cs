using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum ScreenFadeType
{
    Default,
    Circle,
    Skull
}

[Serializable]
public class ScreenFadeDefaultConfig
{
    public Image image;
    public AnimationCurve inCurve;
    public AnimationCurve outCurve;
}
[Serializable]
public class ScreenFadeShapeConfig
{
    public ScreenFadeType type;
    public RectTransform shapeRect;
    public AnimationCurve inCurve;
    public AnimationCurve outCurve;
    public float unfadedSizeMultiplier = 1f;
}

public class ScreenFade : MonoBehaviour
{
    public static bool isBusy { get; private set; } = false;
    public static bool isFaded { get; private set; } = true;

    public static event Action onBeforeIn;
    public static event Action onAfterIn;
    public static event Action onBeforeOut;
    public static event Action onAfterOut;

    static ScreenFade instance;

    [SerializeField] RectTransform _holeCanvas;
    [Header("Shapes")]
    [SerializeField] ScreenFadeDefaultConfig defaultConfig;
    [SerializeField] ScreenFadeShapeConfig[] shapeConfigs;

    public static void In(float duration = 1f, ScreenFadeType type = ScreenFadeType.Default)
    {
        if (isFaded) return;
        if (isBusy) return;

        if (duration < float.Epsilon) duration = 0.01f;

        foreach (var config in instance.shapeConfigs)
            config.shapeRect.sizeDelta = Vector3.zero;

        if (type == ScreenFadeType.Default)
        {
            var image = instance.defaultConfig.image;
            var defaultCurve = instance.defaultConfig.inCurve;
            Color startColor = image.color;
            startColor.a = 0;
            Color finishColor = startColor;
            finishColor.a = 1;
            instance.StartCoroutine(instance.PerformDefaultTransition(duration, image, startColor, finishColor, defaultCurve, true));
            return;
        }

        ScreenFadeShapeConfig shapeConfig = instance.shapeConfigs.First(x => x.type == type);
        var background = instance.defaultConfig.image.color;
        background.a = 1;
        instance.defaultConfig.image.color = background;

        Vector2 canvasSize = instance._holeCanvas.sizeDelta;
        float finalLength = Mathf.Sqrt(canvasSize.x * canvasSize.x + canvasSize.y * canvasSize.y);
        Vector3 startSize = new Vector3(finalLength, finalLength, 0);
        startSize *= shapeConfig.unfadedSizeMultiplier;
        startSize.z = 1;
        Vector3 finishSize = new Vector3(0, 0, 1);
        var shape = shapeConfig.shapeRect;
        var curve = shapeConfig.inCurve;

        instance.StartCoroutine(instance.PerformShapeTransition(duration, shape, startSize, finishSize, curve, true));
    }
    public static void Out(float duration = 1f, ScreenFadeType type = ScreenFadeType.Default)
    {
        if (!isFaded) return;
        if (isBusy) return;

        if (duration < float.Epsilon) duration = 0.01f;

        foreach (var config in instance.shapeConfigs)
            config.shapeRect.sizeDelta = Vector3.zero;

        if (type == ScreenFadeType.Default)
        {
            var image = instance.defaultConfig.image;
            var defaultCurve = instance.defaultConfig.outCurve;
            Color startColor = image.color;
            startColor.a = 1;
            Color finishColor = startColor;
            finishColor.a = 0;
            instance.StartCoroutine(instance.PerformDefaultTransition(duration, image, startColor, finishColor, defaultCurve, false));
            return;
        }

        ScreenFadeShapeConfig shapeConfig = instance.shapeConfigs.First(x => x.type == type);
        var background = instance.defaultConfig.image.color;
        background.a = 1;
        instance.defaultConfig.image.color = background;

        Vector3 startSize = new Vector3(0, 0, 1);
        Vector2 canvasSize = instance._holeCanvas.sizeDelta;
        float finalLength = Mathf.Sqrt(canvasSize.x * canvasSize.x + canvasSize.y * canvasSize.y);
        Vector3 finishSize = new Vector3(finalLength, finalLength, 0);
        finishSize *= shapeConfig.unfadedSizeMultiplier;
        finishSize.z = 1;
        var shape = shapeConfig.shapeRect;
        var curve = shapeConfig.outCurve;

        instance.StartCoroutine(instance.PerformShapeTransition(duration, shape, startSize, finishSize, curve, false));
    }

    private IEnumerator PerformShapeTransition(float duration, RectTransform shape, Vector3 startSize, 
                                               Vector3 finishSize, AnimationCurve curve, bool fadedValue)
    {
        if (fadedValue)
            onBeforeIn?.Invoke();
        else
            onBeforeOut?.Invoke();

        shape.sizeDelta = startSize;
        isBusy = true;

        yield return new WaitForEndOfFrame();

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float timeFactor = Mathf.Clamp01(timer / duration);
            float curveFactor = curve.Evaluate(timeFactor);
            shape.sizeDelta = Vector2.Lerp(startSize, finishSize, curveFactor);
            yield return new WaitForEndOfFrame();
        }
        shape.sizeDelta = finishSize;

        isBusy = false;
        isFaded = fadedValue;

        if (fadedValue)
            onAfterIn?.Invoke();
        else
            onAfterOut?.Invoke();
    }
    private IEnumerator PerformDefaultTransition(float duration, Image image, Color startColor, Color finishColor, 
                                                 AnimationCurve curve, bool fadedValue)
    {
        if (fadedValue)
            onBeforeIn?.Invoke();
        else
            onBeforeOut?.Invoke();
        isBusy = true;

        image.color = startColor;
        yield return new WaitForEndOfFrame();

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float timeFactor = Mathf.Clamp01(timer / duration);
            float curveFactor = curve.Evaluate(timeFactor);
            image.color = Color.Lerp(startColor, finishColor, curveFactor);
            yield return new WaitForEndOfFrame();
        }
        image.color = finishColor;
        isBusy = false;
        isFaded = fadedValue;
        if (fadedValue)
            onAfterIn?.Invoke();
        else
            onAfterOut?.Invoke();
    }

    private void Awake()
    {
        instance = this;
        isBusy = false;
        isFaded = true;
        Color c = defaultConfig.image.color;
        c.a = 1;
        defaultConfig.image.color = c;
    }
}
