using System;
using LevelManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelButton_GUI : MonoBehaviour
{
    private LevelInfo _levelInfo;
    
    public void Initialize(int index, LevelInfo levelInfo)
    {
        _levelInfo = levelInfo;
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = $"{levelInfo.LevelSceneName}; Status: {levelInfo.Status}";
        gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
        var rectTransform = GetComponent<RectTransform>();
        const int offset = 30;
        
        rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width / 2, -(rectTransform.rect.height + offset) * index);
    }

    private void OnClick()
    {
        if (_levelInfo.Status == LevelStatus.Unavailable) return;

        Debug.Log($"Hard opening of level called {_levelInfo.LevelSceneName}");
        GameSceneManager.LoadLevel(_levelInfo);
    }
}
