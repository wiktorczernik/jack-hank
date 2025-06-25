using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupablePointerWindow_GUI : MonoBehaviour
{
    [Serializable]
    public class PointerSpriteConfig
    {
        public PickupableType pickupableType;
        public Sprite onScreenSprite;
        public Sprite offScreenSprite;
    }

    [SerializeField] private PointerSpriteConfig[] spriteConfigs;
    [SerializeField] private RectTransform pointerRect;
    [SerializeField] private Image pointerImage;
    [SerializeField] private Vector2 onScreenOffset = new Vector2(0f, 20f);
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private GameObject distanceObject;

    private PointerSpriteConfig activeConfig;
    private Pickupable target;
    private RectTransform parentRect;
    private PlayerVehicle player;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.PlayerVehicle != null);
        player = GameManager.PlayerVehicle;
        if (!player) yield break;
        player.onPickupZoneEnter.AddListener(OnZoneEnter);
        player.onPickupZoneExit.AddListener(OnZoneExit);
    }

    private void OnEnable()
    {
        if (player) return;
        player = GameManager.PlayerVehicle;
        if (!player) return;
        player.onPickupZoneEnter.AddListener(OnZoneEnter);
        player.onPickupZoneExit.AddListener(OnZoneExit);
    }

    private void OnDisable()
    {
        if (!player) return;
        player.onPickupZoneEnter.RemoveListener(OnZoneEnter);
        player.onPickupZoneExit.RemoveListener(OnZoneExit);
        player = null;
    }

    void OnZoneEnter(PickupZone zone)
    {
        if (!zone) return;
        if (!zone.target) return;
        Debug.Log(zone.gameObject.name, zone);
        target = zone.target;
        activeConfig = spriteConfigs.First(x => x.pickupableType == target.type);
    }

    void OnZoneExit(PickupZone zone)
    {
        target = null;
        activeConfig = null;
    }

    private void Awake()
    {
        parentRect = pointerRect.parent as RectTransform;
        if (parentRect == null)
        {
            Debug.LogError("Указатель должен находиться внутри UI-контейнера (RectTransform).");
            enabled = false;
            return;
        }

        if (pointerImage == null)
        {
            Debug.LogError("На указателе должен быть компонент Image.");
            enabled = false;
            return;
        }

        pointerRect.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (target == null || target.smashed || target.expired || target.pickedUp || activeConfig == null)
        {
            pointerRect.gameObject.SetActive(false);
            return;
        }

        Camera cam = Camera.main;
        if (cam == null) return;
        pointerRect.gameObject.SetActive(true);

        Renderer pickRenderer = target.GetComponentInChildren<Renderer>();
        Vector3 centerWorld = target.transform.position;
        Vector3 topWorld = centerWorld;

        if (pickRenderer != null)
        {
            topWorld = pickRenderer.bounds.center + Vector3.up * pickRenderer.bounds.extents.y;
        }

        Vector3 screenPoint = cam.WorldToScreenPoint(centerWorld);
        Vector3 topScreenPoint = cam.WorldToScreenPoint(topWorld);

        bool isOnScreen = screenPoint.z >= 0f
                          && screenPoint.x >= 0f && screenPoint.x <= Screen.width
                          && screenPoint.y >= 0f && screenPoint.y <= Screen.height;

        Rect parentBounds = parentRect.rect;
        Vector2 pivot = pointerRect.pivot;
        Vector2 size = pointerRect.rect.size;

        float minX = parentBounds.xMin + size.x * pivot.x;
        float maxX = parentBounds.xMax - size.x * (1f - pivot.x);
        float minY = parentBounds.yMin + size.y * pivot.y;
        float maxY = parentBounds.yMax - size.y * (1f - pivot.y);

        distanceObject.SetActive(!isOnScreen);

        if (isOnScreen)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    topScreenPoint,
                    null,
                    out Vector2 localTopPos))
            {
                Vector2 desiredPos = localTopPos + onScreenOffset;
                float clampedX = Mathf.Clamp(desiredPos.x, minX, maxX);
                float clampedY = Mathf.Clamp(desiredPos.y, minY, maxY);
                pointerRect.anchoredPosition = new Vector2(clampedX, clampedY);
                pointerImage.sprite = activeConfig.onScreenSprite;
            }
        }
        else
        {
            int distance = (int)Vector3.Distance(cam.transform.position, target.transform.position);
            distanceText.text = $"<mspace=0.85em>{distance} m</mspace>";
            Vector2 displayScreen = new Vector2(screenPoint.x, screenPoint.y);
            if (screenPoint.z < 0f)
            {
                displayScreen.x = Screen.width - displayScreen.x;
                displayScreen.y = Screen.height - displayScreen.y;
            }

            displayScreen.x = Mathf.Clamp(displayScreen.x, 0f, Screen.width);
            displayScreen.y = Mathf.Clamp(displayScreen.y, 0f, Screen.height);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    displayScreen,
                    null,
                    out Vector2 localClampedPos))
            {
                // apply the same onScreenOffset even when off-screen
                Vector2 desiredPos = localClampedPos + onScreenOffset;
                float clampedX = Mathf.Clamp(desiredPos.x, minX, maxX);
                float clampedY = Mathf.Clamp(desiredPos.y, minY, maxY);
                pointerRect.anchoredPosition = new Vector2(clampedX, clampedY);
                pointerImage.sprite = activeConfig.offScreenSprite;
            }
        }
    }
}
