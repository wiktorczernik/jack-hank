using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialMaster : MonoBehaviour
{
    public static string title { get; private set; } = string.Empty;
    public static string description { get; private set; } = string.Empty;
    public static VideoClip clip { get; private set; } = null;
    public static float readtime { get; private set; } = 1f;
    public static float timestretchDuration { get; private set; } = 0.1f;

    public static bool isActive { get; private set; } = false;

    [SerializeField] Animator windowAnimator;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] VideoPlayer videoPlayer;

    static TutorialMaster _main;


    public static void TriggerEvent(TutorialEvent tutorialEvent)
    {
        _main.StartCoroutine(_main.EventSequence(tutorialEvent));
    }

    private void Awake()
    {
        _main = this;
    }

    IEnumerator EventSequence(TutorialEvent tutorialEvent)
    {
        timestretchDuration = tutorialEvent.timestretchDuration;
        readtime = tutorialEvent.readtime;

        title = tutorialEvent.title;
        description = tutorialEvent.description;
        clip = tutorialEvent.clip;

        titleText.text = title;
        descriptionText.text = description;

        isActive = true;

        Time.timeScale = 1f;
        yield return new WaitForEndOfFrame();

        float timer = 0;
        while (timer < timestretchDuration)
        {
            timer += Time.unscaledDeltaTime;
            float timeFactor = Mathf.Clamp01(timer / timestretchDuration);
            Time.timeScale = Mathf.Lerp(1f, 0f, timeFactor);
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 0f;

        videoPlayer.clip = clip;
        videoPlayer.Play();

        windowAnimator.SetBool("visible", true);

        yield return new WaitForSecondsRealtime(readtime);


        while (!Input.anyKeyDown) yield return null;

        windowAnimator.SetBool("visible", false);

        Time.timeScale = 0f;
        yield return new WaitForEndOfFrame();

        timer = 0;
        while (timer < timestretchDuration)
        {
            timer += Time.unscaledDeltaTime;
            float timeFactor = Mathf.Clamp01(timer / timestretchDuration);
            Time.timeScale = Mathf.Lerp(0f, 1f, timeFactor);
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 1f;

        videoPlayer.Stop();
        videoPlayer.clip = null;

        isActive = false;
        if (tutorialEvent)
            tutorialEvent.Byebye();
    }
}