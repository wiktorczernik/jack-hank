using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MissleCrosshair : MonoBehaviour
{
    public bool isVisible = false;
    public Vector2 uvScale = Vector2.zero;

    [SerializeField] Animator animator;
    [SerializeField] DecalProjector decal;

    public bool Detached = false;
    public void Detach()
    {
        Detached = true;
    }
    
    public void SetPosition(Vector3 position)
    {
        if (Detached) return;
        transform.position = position;
    }
    public void Show()
    {
        if (isVisible)
        {
            Debug.LogError("Attemp of starting follow when its already following!", this);
        }
        isVisible = true;
    }
    public void Hide()
    {
        if (!isVisible)
        {
            Debug.LogError("Tried freeing crosshair that isn't following atm!", this);
        }
        isVisible = false;
    }
    private void LateUpdate()
    {
        animator.SetBool("isVisible", isVisible);

        Vector3 newSize = decal.gameObject.transform.localScale;
        newSize.x = uvScale.x;
        newSize.y = uvScale.y;

        decal.gameObject.transform.localScale = newSize;
    }
}