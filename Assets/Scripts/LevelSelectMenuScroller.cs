using UnityEngine;

public class LevelSelectMenuScroller : MonoBehaviour
{ 
    [SerializeField] private float scrollSpeedInKmh = 100f;
    [SerializeField] private bool isStopped = true;
    private Renderer _rend;

    public void StartScrolling()
    {
        isStopped = false;
    }

    public void StopScrolling()
    {
        isStopped = true;
    }
    
    private void Awake()
    {
        _rend = GetComponent<Renderer>();
    }
    
    private void FixedUpdate()
    {
        if (isStopped) return;
        
        var distanceInMetersPerDeltaTime = (scrollSpeedInKmh * 1000) * (Time.deltaTime / 3600);
        var oneTileSizeInMeters = transform.localScale.z / _rend.material.mainTextureScale.y;
        _rend.material.mainTextureOffset += new Vector2(0, -(distanceInMetersPerDeltaTime / oneTileSizeInMeters));
    }
}
