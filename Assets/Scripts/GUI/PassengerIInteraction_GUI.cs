using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PassengerInteraction_GUI : MonoBehaviour
{
    static PassengerInteraction_GUI inst;

    [SerializeField] GameObject Catch;
    [SerializeField] GameObject Smash;
    [SerializeField] GameObject Expire;

    public float caughtUITime = 3.5f;
    public float smashedUITime = 3.5f;
    public float expiredUITime = 3.5f;

    private void Awake()
    {
        if (inst != null) Destroy(this);
        else inst = this;
    }

    public static void PassengerCaught()
    {
        inst.StartCoroutine(inst.ShowForTime(inst.Catch, inst.caughtUITime));
    }
    public static void PassengerSmashed()
    {
        inst.StartCoroutine(inst.ShowForTime(inst.Smash, inst.smashedUITime));
    }
    public static void PassengerExpired()
    {
        inst.StartCoroutine(inst.ShowForTime(inst.Expire, inst.expiredUITime));
    }

    IEnumerator ShowForTime(GameObject obj, float time)
    {
        obj.SetActive(true);
        while (time >= 0f)
        {
            yield return new WaitForEndOfFrame();
            time -= Time.deltaTime;
        }
        obj.SetActive(false);
    }
}