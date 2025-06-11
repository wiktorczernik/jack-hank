using System;
using System.Collections;
using UnityEngine;

public class BloodSplatter_GUI : MonoBehaviour
{
    [SerializeField] float scaleLerp = 3f;
    [SerializeField] AnimationCurve scaleCurve;

    RectTransform rect;
    [SerializeField] PlayerVehicle player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        rect = GetComponent<RectTransform>();
        if (!GameManager.PlayerVehicle)
            yield return new WaitUntil(() => GameManager.PlayerVehicle != null);
        player = GameManager.PlayerVehicle;
    }

    private void Update()
    {
        if (!player) return;
        rect.localScale = Vector3.Lerp(rect.localScale, Vector3.one * scaleCurve.Evaluate(player.health), scaleLerp * Time.deltaTime);
    }
}
