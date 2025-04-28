using System;
using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    public event Action onDeath;

    public void Activate()
    {
        OnActivate();
    }

    public void Die()
    {
        PrepareDie();
        onDeath?.Invoke();
    }

    protected abstract void PrepareDie();

    protected abstract void OnActivate();
}