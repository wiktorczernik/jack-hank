using UnityEngine;

public class PlayBtn_GUI : MonoBehaviour
{
    bool activated = false;
   
    private void LateUpdate()
    {
        if (Input.anyKeyDown && !activated)
        {
            activated = true;

            if (AccountManager.UseDebugAccount) GameSceneManager.LoadFirstLevelAsync();
            else GameSceneManager.LoadLogin();
        }
    }
}
