using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectAccountButton_GUI : MonoBehaviour
{
     public void Initialize(int index, string accountName, Action<string> onSuccess, Action onFail)
     {
          gameObject.GetComponentInChildren<TextMeshProUGUI>().text = accountName;
          var rectTransform = GetComponent<RectTransform>();
          const int offset = 30;

          rectTransform.anchoredPosition = new Vector2(0, -(rectTransform.rect.height + offset) * index);
          
          gameObject.GetComponent<Button>().onClick.AddListener((() =>
          {
               if (!AccountManager.ExistsSavedAccount(accountName)) onFail();

               onSuccess(accountName);
          }));
     }
}