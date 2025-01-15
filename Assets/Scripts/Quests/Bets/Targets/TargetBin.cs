using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBin : Target
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            IsCompleted = true;
        }
    }
}
