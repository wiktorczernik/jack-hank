using UnityEngine;

public class ScreenFadeRequester : MonoBehaviour
{
    [Header("Auto Start")]
    public bool startIn = false;
    public bool startOut = false;
    [Header("In")]
    public float inDuration = 1;
    public ScreenFadeType inType = ScreenFadeType.Default;
    [Header("Out")]
    public float outDuration = 1;
    public ScreenFadeType outType = ScreenFadeType.Default;

    public void In()
    {
        ScreenFade.In(inDuration, inType);
    }
    public void Out()
    {
        ScreenFade.Out(outDuration, outType);
    }

    private void Start()
    {
        if (startIn)
            In();
        else if (startOut)
            Out();
    }
}
