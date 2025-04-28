using UnityEngine;

public class HitmanTrigger : MonoBehaviour
{
    private TriggerEventEmitter _emitter;

    private void Awake()
    {
        _emitter = GetComponent<TriggerEventEmitter>();
        _emitter.OnEnter.AddListener(Kill);
    }


    private void Kill(Collider col)
    {
        GameEntity entity;
        if (!col.gameObject.TryGetComponent(out entity))
        {
            entity = col.gameObject.GetComponentInParent<GameEntity>();
            if (entity == null) return;
        }

        Debug.Log("KILLLL!!!" + entity.Kill());
    }
}