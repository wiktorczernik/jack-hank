using System.Collections;
using UnityEngine;

public class PassengerInteraction_GUI : MonoBehaviour
{
    static PassengerInteraction_GUI inst;

    [SerializeField] Animation Catch;
    [SerializeField] Animation Smash;
    [SerializeField] Animation Expire;

    public float caughtUITime = 3.5f;
    public float smashedUITime = 3.5f;
    public float expiredUITime = 3.5f;

    private void OnEnable()
    {
        inst = this;
    }

    public static void PassengerCaught()
    {
        inst.Catch.Play();
    }
    public static void PassengerSmashed()
    {
        inst.Smash.Play();
    }
    public static void PassengerExpired()
    {
        inst.Expire.Play();
    }
}