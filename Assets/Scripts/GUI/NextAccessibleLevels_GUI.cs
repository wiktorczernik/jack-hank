using System;
using System.Collections.Generic;
using LevelManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextAccessibleLevels_GUI : MonoBehaviour
{
   public Button selectLevelButtonPrefab; 
   public event Action<LevelInfo> OnLevelSelected;

   private Button[] _createdButtons;
   
   public void InitializeWithNextLevels(LevelInfo[] levels)
   {
      _createdButtons = new Button[levels.Length];

      var i = 1;
      
      foreach (var level in levels)
      {
         var button = Instantiate(selectLevelButtonPrefab, transform);
         button.onClick.AddListener(() => OnLevelSelected?.Invoke(level));

         const int rightOffset = 20;
         const int bottomOffset = 20;
         
         button.GetComponentInChildren<TMP_Text>().text = level.LevelSceneName;
         var rect = button.GetComponent<RectTransform>();
         rect.anchorMax = new Vector2(1, 1);
         rect.anchorMin = new Vector2(1, 1);
         rect.anchoredPosition = new Vector2((-rightOffset - rect.localScale.x) * i, -bottomOffset - rect.localScale.y);
         i++;
      }
   }
}
