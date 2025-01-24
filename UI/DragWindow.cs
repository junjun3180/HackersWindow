using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private RectTransform rectTransform;
    private Vector2 offset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // 드래그 시작 시 마우스와 창의 상대적 위치를 계산
    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out offset
        );
    }

    // 드래그 중 창의 위치 업데이트
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out mousePosition
        );

        // 창의 새로운 위치 계산
        Vector2 newPosition = mousePosition - offset;

        // 부모 RectTransform 가져오기
        RectTransform parentRect = rectTransform.parent as RectTransform;

        if (parentRect != null)
        {
            // 이동 범위 제한
            float minX = -parentRect.rect.width / 2 + rectTransform.rect.width / 2;
            float maxX = parentRect.rect.width / 2 - rectTransform.rect.width / 2;
            float minY = -parentRect.rect.height / 2 + rectTransform.rect.height / 2;
            float maxY = parentRect.rect.height / 2 - rectTransform.rect.height / 2;

            rectTransform.anchoredPosition = new Vector2(
                Mathf.Clamp(newPosition.x, minX, maxX),
                Mathf.Clamp(newPosition.y, minY, maxY)
            );
        }
    }
}

