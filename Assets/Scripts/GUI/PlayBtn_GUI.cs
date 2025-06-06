using System.Collections;
using UnityEngine;

public class PlayBtn_GUI : MonoBehaviour
{
    bool activated = false;
    float startDelay = 3f;
    bool waiting = false;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);
        waiting = true;
    }

    private void LateUpdate()
    {
        if (Input.anyKeyDown && !activated && waiting)
        {
            activated = true;
            ScreenFade.In(0.5f, ScreenFadeType.Circle);
            GameSceneManager.LoadLogin();
        }
    }
}
