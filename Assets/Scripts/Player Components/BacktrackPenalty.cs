using UnityEngine;

public class BacktrackPenalty : MonoBehaviour
{
    public Waypoint current;
    PlayerVehicle player;

    public float eventScore = 0f;
    public float passiveDrain = 5f;
    [Tooltip("amount of distance you need to backtrack in one second to receive penalty")] public float penaltyScore = 50f;

    private void Awake()
    {
        player = GetComponent<PlayerVehicle>();
    }

    private void FixedUpdate()
    {
        if (current == null) return;

        float score = current.GetValue(player.physics.bodyRigidbody.linearVelocity);
        Debug.Log(score);
        if (score >= 0f)
        {
            eventScore -= Time.fixedDeltaTime * passiveDrain;
            if (eventScore < 0f) eventScore = 0f;
            return;
        }
        eventScore -= score * Time.fixedDeltaTime;

        if (eventScore < penaltyScore) return;
        player.Kill();
    }
}
