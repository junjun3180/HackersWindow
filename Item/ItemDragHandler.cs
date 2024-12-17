using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item item;  // 드래그할 아이템
    public GameObject windowUI;  // WindowUI 참조
    public GameObject Button_Drag_Prefab; // 드래그 시 표시할 프리팹
    private Transform originalParent;  // 드래그 전 원래 부모 저장
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private GameObject dragIcon; // 드래그 시 표시되는 아이콘
    UI_3_MyDocument ui_3_MyDocument;

    private ItemManager itemManager;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        ui_3_MyDocument = UI_3_MyDocument.Instance;
        itemManager = ItemManager.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Button_Drag_Prefab == null)
        {
            Debug.LogError("Button_Drag_Prefab si null");
            return;
        }

        // 드래그 시작 시 드래그용 아이콘 생성
        dragIcon = Instantiate(Button_Drag_Prefab, transform.root);
        dragIcon.transform.SetAsLastSibling();

        // 드래그 아이콘 이미지 설정
        Image dragImage = dragIcon.GetComponentInChildren<Image>();
        if (dragImage != null)
        {
            dragImage.sprite = GetItemSprite(item.itemType);
        }
        else
        {
            Debug.LogError("dragImage is null");
        }


        // 누른 버튼 투명도 조절
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 아이콘 삭제
        if (dragIcon != null)
        {
            Destroy(dragIcon);
        }

        if (!IsPointerInsideUI(eventData.position, windowUI))
        {
            // WindowUI 바깥으로 드롭했을 경우
            if (IsPointerInRightSide(eventData.position))
            {
                // 오른쪽 영역에 아이템 생성
                // Debug.Log("Right Side Generate Item");
                itemManager.DropItem(item, true);
            }
            else
            {
                // 왼쪽 영역에 아이템 생성
                // Debug.Log("Left Side Generate Item");
                itemManager.DropItem(item, false);
            }

            ui_3_MyDocument.UpdateStorage();
            ui_3_MyDocument.RemoveItemDetail();
            Destroy(gameObject);
        }
        else
        {
            ui_3_MyDocument.GenerateItemList();
        }

        canvasGroup.alpha = 1f; // 원래 투명도로 복구
        canvasGroup.blocksRaycasts = true;
    }

    private bool IsPointerInRightSide(Vector2 pointerPosition)
    {
        return pointerPosition.x > Screen.width / 2;
    }

    private bool IsPointerInsideUI(Vector2 pointerPosition, GameObject uiObject)
    {
        RectTransform uiRect = uiObject.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(uiRect, pointerPosition, null);
    }

    private Sprite GetItemSprite(Item.ItemType itemType)
    {
        string spriteSheetName = itemManager.GetSpriteSheetName(itemType);
        int spriteIndex = itemManager.GetImageIndex(itemType);
        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSheetName);
        if (spriteIndex >= 0 && spriteIndex < sprites.Length)
        {
            return sprites[spriteIndex];
        }
        else
        {
            Debug.LogError($"Sprite not found for item type: {itemType}");
            return null;
        }
    }
}
