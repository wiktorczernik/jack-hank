using UnityEngine;

public abstract class PlayerModule : MonoBehaviour
{
    public Player parent { get; private set; }
    public bool initialized { get; private set; } = false;

    public bool Init(Player newParent){
        if (initialized){
            return false;
        }
        initialized = true;
        parent = newParent;
        OnInit();
        return true;
    }

    public virtual void OnInit() {}
    public virtual void OnUpdate(float deltaTime) {}
    public virtual void OnFixedUpdate(float deltaTime) {}
    public virtual void OnLateUpdate(float deltaTime) {}
}
