using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public bool IsPlayerHere;
    public bool IsVisited;

    private void Awake()
    {
        IsPlayerHere = false;
        IsVisited = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            IsPlayerHere = true;
            IsVisited = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            IsPlayerHere = false;
        }
    }
}
