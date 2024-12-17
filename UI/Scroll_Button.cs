using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Scroll_Button : MonoBehaviour
{
    public ScrollRect scrollRect; // Scroll Rect 연결
    public float scrollSpeed = 0.1f; // 스크롤 속도 설정

    // 스크롤을 위로 올리는 함수
    public void ScrollUp()
    {
        scrollRect.verticalNormalizedPosition += scrollSpeed;
        if (scrollRect.verticalNormalizedPosition > 1f)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    // 스크롤을 아래로 내리는 함수
    public void ScrollDown()
    {
        scrollRect.verticalNormalizedPosition -= scrollSpeed;
        if (scrollRect.verticalNormalizedPosition < 0f)
            scrollRect.verticalNormalizedPosition = 0f;
    }
}
