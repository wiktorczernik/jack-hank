using TMPro;
using UnityEngine;

public class FPS_GUI : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    float deltaTime = 0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9)) text.enabled = !text.enabled;

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        text.text = $"{Mathf.Round(100f / deltaTime) / 100f} FPS";
    }
}
