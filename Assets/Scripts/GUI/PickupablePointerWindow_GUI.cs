using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PickupablePointerWindow_GUI : MonoBehaviour
{
    [SerializeField] private RectTransform pointerRect;
    [SerializeField] private Transform pickupableTransform;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite edgeSprite;
    [SerializeField] private Vector2 onScreenOffset = new Vector2(0f, 20f);

    private RectTransform parentRect;
    private Image pointerImage;
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
        if (!zone.pickupable) return;
        Debug.Log(zone.gameObject.name, zone);
        pickupableTransform = zone.pickupable.transform;
    }

    void OnZoneExit(PickupZone zone)
    {
        pickupableTransform = null;
    }

    private void Awake()
    {
        if (pointerRect == null || normalSprite == null || edgeSprite == null)
        {
            Debug.LogError("Pointer, Passenger или спрайты не назначены.");
            enabled = false;
            return;
        }

        parentRect = pointerRect.parent as RectTransform;
        if (parentRect == null)
        {
            Debug.LogError("Указатель должен находиться внутри UI-контейнера (RectTransform).");
            enabled = false;
            return;
        }

        pointerImage = pointerRect.GetComponent<Image>();
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
        if (pickupableTransform == null)
        {
            pointerRect.gameObject.SetActive(false);
            return;
        }

        Camera cam = Camera.main;
        if (cam == null) return;
        pointerRect.gameObject.SetActive(true);

        // Получаем рендерер, чтобы вычислить дополнительное смещение по высоте
        Renderer pickRenderer = pickupableTransform.GetComponentInChildren<Renderer>();
        Vector3 centerWorld = pickupableTransform.position;
        Vector3 topWorld = centerWorld;

        if (pickRenderer != null)
        {
            // Верхняя точка Bound'а
            topWorld = pickRenderer.bounds.center + Vector3.up * pickRenderer.bounds.extents.y;
        }

        // Экранные точки для центра и для верха
        Vector3 screenPoint = cam.WorldToScreenPoint(centerWorld);
        Vector3 topScreenPoint = cam.WorldToScreenPoint(topWorld);

        // Проверяем, на экране ли центр объекта
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

        if (isOnScreen)
        {
            // Если в кадре, используем верхнюю точку для размещения указателя
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
                pointerImage.sprite = normalSprite;
            }
        }
        else
        {
            // Offscreen-логика остается без изменений — кладем в экранные границы
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
                float clampedX = Mathf.Clamp(localClampedPos.x, minX, maxX);
                float clampedY = Mathf.Clamp(localClampedPos.y, minY, maxY);
                pointerRect.anchoredPosition = new Vector2(clampedX, clampedY);
                pointerImage.sprite = edgeSprite;
            }
        }
    }
}
