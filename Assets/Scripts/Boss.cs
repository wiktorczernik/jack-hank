using System;
using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    public event Action onDeath;

    private void DieHandle()
    {
        PrepareDie();
        onDeath?.Invoke();
    }

    protected abstract void PrepareDie();
}