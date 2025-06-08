using System;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<WaypointStage> stages;
    [SerializeField] int stage;

    TriggerEventEmitter emitter;

    private void Awake()
    {
        emitter = GetComponent<TriggerEventEmitter>();
    }
    private void OnEnable()
    {
        emitter.OnEnter.AddListener(OnEnter);
    }
    private void OnDisable()
    {
        emitter.OnEnter.RemoveListener(OnEnter);
    }

    void OnEnter(Collider collider)
    {
        Transform tr = collider.transform.parent;
        if (tr == null) return;
        if (!tr.TryGetComponent(out PlayerVehicle _)) return;

        BacktrackPenalty bp = tr.GetComponent<BacktrackPenalty>();
        bp.current = this;
    }


    public Waypoint GetNext()
    {
        if (stages.Count == 0) return null;
        return stages[stage].target;
    }
    public float GetMultiplier()
    {
        if (stages.Count == 0) return 0f;
        return stages[stage].multiplier;
    }
    public float GetValue(Vector3 linearVelocity)
    {
        if (stages.Count == 0) return 0f;
        return Vector3.Dot((GetNext().transform.position - transform.position).normalized, linearVelocity) * GetMultiplier();
    }
    public int NextStage()
    {
        if (stage + 1 >= stages.Count) return stage;
        return ++stage;
    }
    public void SetStage(int value)
    {
        if (value < 0 || value >= stages.Count) return;
        stage = value;
    }
    public bool IsValid()
    {
        return stages.Count > 0;
    }

    [Serializable] public struct WaypointStage
    {
        public Waypoint target;
        public float multiplier;

        public WaypointStage(Waypoint target, float multiplier = 1f)
        {
            this.target = target;
            this.multiplier = multiplier;
        }
    }
}
