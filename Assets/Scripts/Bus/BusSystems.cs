using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusSystems : MonoBehaviour
{

    public bool areDoorsOpened = false;

    public void DoorsAction()
    {
        if (areDoorsOpened == true)
        {
            CloseDoors();
        }
        else
        {
            OpenDoors();
        }
    }
    
    private void OpenDoors()
    {
        areDoorsOpened = !areDoorsOpened;
    }

    private void CloseDoors()
    {
        areDoorsOpened = !areDoorsOpened;
    }
}
