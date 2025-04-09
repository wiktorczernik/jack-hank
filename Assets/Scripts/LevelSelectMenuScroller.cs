using UnityEngine;

public class LevelSelectMenuScroller : MonoBehaviour
{ 
    [SerializeField] private float scrollSpeedInKmh = 100f;
    private Renderer _rend;
    private bool _isStopped = true;

    public void StartScrolling()
    {
        _isStopped = false;
    }

    public void StopScrolling()
    {
        _isStopped = true;
    }
    
    private void Awake()
    {
        _rend = GetComponent<Renderer>();
    }
    
    private void FixedUpdate()
    {
        if (_isStopped) return;
        
        var distanceInMetersPerDeltaTime = (scrollSpeedInKmh * 1000) * (Time.deltaTime / 3600);
        _rend.material.mainTextureOffset += new Vector2(0, -(distanceInMetersPerDeltaTime / transform.localScale.z));
    }
}
