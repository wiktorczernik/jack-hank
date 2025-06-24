using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public enum ActiveMenuWindow
{ 
    Leaderboard,
    Play,
    Credits
}
public class MainMenu_GUI : MonoBehaviour
{
    public ActiveMenuWindow activeWindow = ActiveMenuWindow.Leaderboard;
    public bool requestedQuit = false;
    public Color selectedColor = Color.white;
    public Color unselectedColor = Color.white;

    [Header("Button")]
    [SerializeField] Button playButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button quitButton;

    [Header("Text")]
    [SerializeField] TMP_Text playText;
    [SerializeField] TMP_Text creditsText;
    [SerializeField] TMP_Text quitText;

    [Header("Animator")]
    [SerializeField] Animator playAnimator;
    [SerializeField] Animator creditsAnimator;


    public void OnRequestPlay()
    {
        if (activeWindow == ActiveMenuWindow.Play)
            activeWindow = ActiveMenuWindow.Leaderboard;
        else
            activeWindow = ActiveMenuWindow.Play;
    }
    public void OnRequestCredits()
    {
        if (activeWindow == ActiveMenuWindow.Credits)
            activeWindow = ActiveMenuWindow.Leaderboard;
        else
            activeWindow = ActiveMenuWindow.Credits;
    }
    public void OnRequestQuit()
    {
        requestedQuit = true;
        playButton.enabled = false;
        creditsButton.enabled = false;
        quitButton.enabled = false;

        ScreenFade.onAfterIn += () =>
        {
            Application.Quit();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        };
        ScreenFade.In(1, ScreenFadeType.Circle);
    }

    private void Update()
    {
        playText.color = unselectedColor;
        creditsText.color = unselectedColor;

        playAnimator.SetBool("isVisible", activeWindow == ActiveMenuWindow.Play);
        creditsAnimator.SetBool("isVisible", activeWindow == ActiveMenuWindow.Credits);

        TMP_Text targetText = null;
        switch(activeWindow)
        {
            case ActiveMenuWindow.Play:
                targetText = playText;
                break;
            case ActiveMenuWindow.Credits:
                targetText = creditsText;
                break;
        }
        if (targetText) targetText.color = selectedColor;
        if (requestedQuit) quitText.color = selectedColor;
    }
}
