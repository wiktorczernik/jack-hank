using UnityEngine;
using UnityEngine.Events;

public class RunFinish : MonoBehaviour
{
    public UnityEvent onPlayerFinish;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Vehicle")) return;
            
        Debug.Log("Player finished");
        onPlayerFinish.Invoke();
    }
}
