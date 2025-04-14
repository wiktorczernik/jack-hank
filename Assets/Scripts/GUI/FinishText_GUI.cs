using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class FinishText_GUI : MonoBehaviour
{
    public event Action OnTextEndAnimation;
    
    private TMP_Text _text;
    private Animator _animator;

    private void Awake()
    {
        if (!TryGetComponent(out _text)) Debug.LogError("FinishText_GUI: No TextMeshPro text found!");
        if (!TryGetComponent(out _animator)) Debug.LogError("FinishText_GUI: No Animation component found!");
        _text.alpha = 0;
        _animator.enabled = false;
    }
    
    public void ShowFinishMark(LevelCompletenessMark mark)
    {
        _text.text = $"{mark.ToString()} Rank";
        StartCoroutine(ShowFinishMarkCo());
    }

    private IEnumerator ShowFinishMarkCo()
    {
        _animator.enabled = true;
        yield return new WaitForSeconds(_animator.runtimeAnimatorController.animationClips[0].length);
        OnTextEndAnimation?.Invoke();
    }
}
