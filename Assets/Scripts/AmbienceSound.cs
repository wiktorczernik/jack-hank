using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ============== INSTRUCTION ==============
// Create empty game object and add "Box Collider" component
// Set the "Box Collider" to "Is Trigger"
// Adjust its size to fit the ambience area
// Create another empty game object and add this script
// Select "Area" as well as "Player" in the inspector
// Add sound to the object

public class AmbienceSound : MonoBehaviour
{
    [Tooltip("Area of the sound to be in")]
    public Collider Area;
    [Tooltip("Character to track")]
    public GameObject Player;

    void Update()
    {
        // Locate closest point on the collider to the player
        Vector3 closestPoint = Area.ClosestPoint(Player.transform.position);
        // Set position to closest point to the player
        transform.position = closestPoint;
    }
}
