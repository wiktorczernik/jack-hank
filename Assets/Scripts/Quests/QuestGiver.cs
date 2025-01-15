using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] private Bet bet;
    [SerializeField] private string description;
    [SerializeField] private bool isAccepted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" & !isAccepted)
        {
            Debug.Log(description);
            // WOLNE MIEJSCE na twoje okno dialogowe
            QuestManager.Instance.AddBet(bet);
            isAccepted = true;
        }
    }
}
