using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayBtn_GUI : MonoBehaviour
{
   private void Awake()
   {
      GetComponent<Button>().onClick.AddListener(HandleClick);
   }

   private void HandleClick()
   {
      GameSceneManager.LoadLogin();
   }
}
