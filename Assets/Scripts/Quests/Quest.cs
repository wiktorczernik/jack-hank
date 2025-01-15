using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest : MonoBehaviour
{
    public bool IsCompleted;

    private void Awake()
    {
        IsCompleted = false;
    }
}
