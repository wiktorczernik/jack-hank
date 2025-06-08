using UnityEngine;
using UnityEngine.Video;

public class TutorialEvent : MonoBehaviour
{
    public string title;
    public string description;
    public VideoClip clip;
    public float timestretchDuration = 1f;
    public float readtime = 3f;

    [SerializeField] TriggerEventEmitter triggerEmitter;

    private void OnEnable()
    {
        triggerEmitter.OnEnter.AddListener(OnTriggerEnter);
    }
    private void OnDisable()
    {
        triggerEmitter.OnEnter.RemoveListener(OnTriggerEnter);
    }

    void OnTriggerEnter(Collider c)
    {
        if (!c) return;
        PlayerVehicle player = c.GetComponentInParent<PlayerVehicle>();
        if (!player) return;
        triggerEmitter.OnEnter.RemoveListener(OnTriggerEnter);
        TutorialMaster.TriggerEvent(this);
    }
    public void Byebye()
    {
        if (!this) return;
        if (!gameObject) return;
        Destroy(gameObject);
    }
}

