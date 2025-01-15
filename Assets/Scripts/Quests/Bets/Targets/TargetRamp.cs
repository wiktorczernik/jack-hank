using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRamp : Target
{
    [SerializeField] private bool isOnRamp;
    [SerializeField] private float timeInAir;
    [SerializeField] private float height;
    [SerializeField] private float busPositionY;
    public Transform busPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isOnRamp = true;

        }
    }

    private void Update()
    {
        if (isOnRamp & !IsCompleted)
        {
            busPositionY = busPosition.position.y;
            if (busPositionY >= height)
            {
                IsCompleted = true;
            }
        }
    }
}
