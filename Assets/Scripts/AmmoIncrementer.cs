using UnityEngine;

public class AmmoIncrementer : MonoBehaviour
{
    public int amount;

    public void Increment()
    {
        PlayerTurret.ammo += amount;
    }
}
