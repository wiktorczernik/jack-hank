using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeTransition_GUI : MonoBehaviour
{
    public event Action OnFadeInEnded;
    public event Action OnFadeOutEnded;
    public event Action OnFadeOutStarted;
    private Image _image;
    private Animation _anim;

    private bool _isBusy;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _anim = GetComponent<Animation>();
        gameObject.SetActive(false);
    }

    public void PrepareFadeOut()
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1);
        gameObject.SetActive(true);
    }

    public void StartFadeIn()
    {
        gameObject.SetActive(true);
        if (_isBusy)
        {
            Debug.LogWarning("FadeTransition_GUI: Fade transition animation is already in progress!");
            return;
        }

        _isBusy = true;
        StartCoroutine(StartFadeInCo());
    }

    private IEnumerator StartFadeInCo()
    {
        _anim.Play("FadeIn");

        yield return new WaitForSeconds(_anim["FadeIn"].length);
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0);

        _isBusy = false;
        OnFadeInEnded?.Invoke();
    }

    public void StartFadeOut()
    {
        if (_isBusy)
        {
            Debug.LogWarning("FadeTransition_GUI: Fade transition animation is already in progress!");
            return;
        }
        
        _isBusy = true;
        StartCoroutine(StartFadeOutCo());
    }

    private IEnumerator StartFadeOutCo()
    {
        _anim.Play("FadeOut");
        OnFadeOutStarted?.Invoke();
        yield return new WaitForSeconds(_anim["FadeOut"].length);
        _isBusy = false;
        OnFadeOutEnded?.Invoke();
    }
}
