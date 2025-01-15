using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackVerifer : MonoBehaviour // ten skrypt musi znaleœæ siê w autobusie
{
    public bool isOnPavement;
    [SerializeField] public Vector3 startMoving;
    [SerializeField] public Vector3 endMoving;

    private void Awake()
    {
        isOnPavement = false;
    }

    private void Update()
    {
        if(isOnPavement)
        {
            if(startMoving == Vector3.zero)
            {
                startMoving = transform.position;
            }
        }
    }
}
