using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunFinish : MonoBehaviour
{
    [SerializeField] private FinishText_GUI finishText;

    private void Start()
    {
        finishText.OnTextEndAnimation += () => SceneManager.LoadScene("XMenu");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Vehicle")) return;
            
        GameManager.FinishRun();
        finishText.ShowFinishMark(GameManager.GetMarkByBounty());
    }
}
