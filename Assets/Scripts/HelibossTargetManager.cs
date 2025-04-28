using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HelibossTargetManager : MonoBehaviour
{
    public UnityEvent OnDetach;
    public VehiclePhysics targetVehicle;
    [SerializeField] Transform positionsList;
    public GameObject crosshairPrefab;
    
    private int loadedMissles = 0;
    private bool queuedForDeletion = false;

    public Transform FindNextTarget(out int id)
    {
        id = -1;
        if (positionsList.childCount == 0) return null;

        Transform randomPos = positionsList.GetChild(Random.Range(0, positionsList.childCount));
        id = int.Parse(randomPos.name);
        randomPos.parent = transform;
        GameObject crosshair = Instantiate(crosshairPrefab, randomPos);
        return crosshair.transform;
    }

    public void RegisterMissle(Missle missle, int id)
    {
        loadedMissles++;
        transform.Find($"{id}").GetComponent<HelibossTargetController>().assignedMissle = missle;
        missle.onSelfExplode += (_) => CancelTarget(id);
    }

    void CancelTarget(int id)
    {
        loadedMissles--;
        try { Destroy(transform.Find($"{id}").gameObject); }
        catch { }
    }

    public void QueueForDeletion()
    {
        queuedForDeletion = true;
    }

    private void Update()
    {
        if (loadedMissles == 0 && queuedForDeletion) Destroy(gameObject);

        if (targetVehicle == null) return;
        if (targetVehicle.isGrounded) return;

        OnDetach?.Invoke();
        transform.parent = null;
        targetVehicle = null;
    }
}
