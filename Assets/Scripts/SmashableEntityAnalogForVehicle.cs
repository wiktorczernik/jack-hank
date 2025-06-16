using UnityEngine;
using UnityEngine.Events;

public class SmashableEntityAnalogForVehicle : MonoBehaviour
{
    public int bountyPointsReward;
    public UnityEvent<SmashableEntityAnalogForVehicle> onHit;

    private bool _wasHit;

    private void Awake()
    {
        GetComponent<CollisionEventEmitter>().OnEnter.AddListener(HandleOnEnter);
    }

    private void HandleOnEnter(Collision col)
    {
        if (_wasHit) return;

        onHit?.Invoke(this);
        
        _wasHit = true;
    }
}