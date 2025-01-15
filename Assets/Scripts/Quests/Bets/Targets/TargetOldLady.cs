using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetOldLady : Target
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            IsCompleted = true;
        }
    }
}
