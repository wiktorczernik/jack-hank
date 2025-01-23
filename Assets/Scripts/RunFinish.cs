using UnityEngine;

public class RunFinish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Vehicle")) return;
            
        GameManager.FinishRun();
        Debug.Log("Player finished");
    }
}
