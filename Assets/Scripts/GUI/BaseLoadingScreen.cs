using System.Collections;
using UnityEngine;

public abstract class BaseLoadingScreen : MonoBehaviour
{
    private bool _isLoading;

    #region Event subscribing and unsubscribing
    protected abstract void SubscribeBeginEvent();
    protected abstract void SubscribeEndEvent();
    private void SubscribeTickEvent() => GameSceneManager.onLoadTick += OnTick;
    protected abstract void UnsubscribeBeginEvent();
    protected abstract void UnsubscribeEndEvent();
    private void UnsubscribeTickEvent() => GameSceneManager.onLoadTick -= OnTick;
    #endregion

    #region Event handlers
    protected virtual IEnumerator OnBeginEventAsync(object arg = null)
    {
        yield return null;
    }
    protected virtual void OnTickEvent()
    {

    }
    protected virtual IEnumerator OnEndEventAsync(object arg = null)
    {
        yield return null;
    }
    #endregion

    #region Helpers
    protected void OnBegin(object arg = null) 
    {
        _isLoading = true;
        StartCoroutine(OnBeginEventAsync(arg));
    }
    protected void OnEnd(object arg = null)
    {
        StartCoroutine(OnEndEventAsync(arg));
        _isLoading = false;
    }
    private void OnTick()
    {
        if (_isLoading)
        {
            OnTickEvent();
        }
    }
    #endregion

    #region Unity messages
    private void OnEnable()
    {
        SubscribeBeginEvent();
        SubscribeEndEvent();
        SubscribeTickEvent();
    }
    private void OnDisable()
    {
        UnsubscribeBeginEvent();
        UnsubscribeEndEvent();
        UnsubscribeTickEvent();
    }
    #endregion
}
