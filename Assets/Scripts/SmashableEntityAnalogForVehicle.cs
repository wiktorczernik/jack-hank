using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class SmashableEntityAnalogForVehicle : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private Rigidbody usedRigidbody;
    public int bountyPointsReward;
    public UnityEvent<SmashableEntityAnalogForVehicle> onHit;

    private bool _wasHit;

    private void Awake()
    {
        GetComponent<CollisionEventEmitter>().OnEnter.AddListener(HandleOnEnter);
    }

    private void HandleOnEnter(Collision col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        
        if (_wasHit) return;
        
        usedRigidbody.freezeRotation = true;
        usedRigidbody.constraints = RigidbodyConstraints.None;
        usedRigidbody.isKinematic = true;
        
        if (debrisPrefab != null)
        {
            var modTrans = model.transform;
            var debrisInst = Instantiate(debrisPrefab, modTrans.position, modTrans.rotation, transform);
            debrisInst.transform.localScale = modTrans.localScale;
            Destroy(model);
        }

        onHit?.Invoke(this);
        
        _wasHit = true;
    }
}