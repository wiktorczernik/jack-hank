using UnityEngine;

public class Loop : MonoBehaviour
{
    [SerializeField] private GameObject ter1;
    [SerializeField] private GameObject ter2;
    [SerializeField] private float scrollSpeedInKmh = 100f;

    private float start = 373f;
    private float end = -63;

    private void Update()
    {
        var distanceInMetersPerDeltaTime = (scrollSpeedInKmh * 1000) * (Time.deltaTime / 3600);
        var change = new Vector3(-(distanceInMetersPerDeltaTime),0, 0);

        ter1.transform.position = ter1.transform.position + change;
        ter2.transform.position = ter2.transform.position + change;

        if (ter2.transform.position.x <= start - 800)
        {
            Debug.Log("low");
            var vec = new Vector3(ter2.transform.position.x - 1000, ter1.transform.position.y, ter1.transform.position.z);
            ter1.transform.position = vec;
        }

        if (ter1.transform.position.x <= start - 800)
        {
            Debug.Log("low");
            var vec = new Vector3(ter1.transform.position.x - 1000, ter1.transform.position.y, ter1.transform.position.z);
            ter2.transform.position = vec;
        }


        Debug.Log("aaaa?");
    }

}