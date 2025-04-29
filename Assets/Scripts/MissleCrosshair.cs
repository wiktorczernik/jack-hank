using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MissleCrosshair : MonoBehaviour
{
    public bool isVisible = false;

    DecalProjector decal;

    
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
    public void Show()
    {
        if (isVisible)
        {
            Debug.LogError("Attemp of starting follow when its already following!", this);
        }
        isVisible = true;
        decal.enabled = true;
    }
    public void Hide()
    {
        if (!isVisible)
        {
            Debug.LogError("Tried freeing crosshair that isn't following atm!", this);
        }
        isVisible = false;
        decal.enabled = false;
        Destroy(gameObject);
    }

    private void Awake()
    {
        decal = GetComponentInChildren<DecalProjector>();
        decal.enabled = false;
    }
}