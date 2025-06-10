using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class TutorialEvent : MonoBehaviour
{
    public string title;
    public string description;
    public VideoClip clip;
    public float timestretchDuration = 1f;
    public float readtime = 3f;

    public bool wasShown = false;
    public float triggerDelay = 10f;

    public UnityEvent onByebye;

    [SerializeField] TriggerEventEmitter triggerEmitter;

    public void DelayedTrigger()
    {
        if (wasShown) return;
        Invoke(nameof(Trigger), triggerDelay);
    }

    private void OnEnable()
    {
        wasShown = PlayerPrefs.HasKey($"tutorial_{title}");
        triggerEmitter.OnEnter.AddListener(TriggerEnter);
    }
    private void OnDisable()
    {
        triggerEmitter.OnEnter.RemoveListener(TriggerEnter);
    }
    void Trigger()
    {
        TutorialMaster.TriggerEvent(this);
    }
    void TriggerEnter(Collider c)
    {
        if (wasShown) return;
        if (!c) return;
        PlayerVehicle player = c.GetComponentInParent<PlayerVehicle>();
        if (!player) return;
        triggerEmitter.OnEnter.RemoveListener(TriggerEnter);
        Trigger();
    }
    public void Byebye()
    {
        wasShown = true;
        PlayerPrefs.SetInt($"tutorial_{title}", 1);
        onByebye?.Invoke();
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey($"tutorial_{title}");
    }
}

