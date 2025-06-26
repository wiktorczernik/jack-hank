using UnityEngine;

public class WaterKiller : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null) return;
        if (collision.collider == null) return;

        PlayerVehicle player;
        if (collision.gameObject.TryGetComponent<PlayerVehicle>(out player))
        {
            player.Kill();
            return;
        }
        player = collision.gameObject.GetComponentInParent<PlayerVehicle>();
        if (player != null)
        {
            player.Kill();
        }
    }
}
