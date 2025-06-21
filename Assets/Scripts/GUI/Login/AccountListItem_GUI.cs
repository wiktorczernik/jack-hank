using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountListItem_GUI : MonoBehaviour
{
     [SerializeField] private Button selectItemBtn;
     [SerializeField] private Button deleteItemBtn;
     
     public string accountName { get; private set; }
     
     [NonSerialized] public RectTransform rectTransform;
     
     public void Initialize(string accountName, Action<AccountListItem_GUI> onClick, Action<AccountListItem_GUI> onDelete)
     {
          this.accountName = accountName;
          selectItemBtn.GetComponentInChildren<TextMeshProUGUI>().text = accountName;
          rectTransform = GetComponent<RectTransform>();
          
          selectItemBtn.GetComponent<Button>().onClick.AddListener((() => onClick(this)));
          deleteItemBtn.GetComponent<Button>().onClick.AddListener((() => onDelete(this)));
     }

     public void SetPosition(Vector2 position)
     {
          rectTransform.anchoredPosition = position;
     }
}