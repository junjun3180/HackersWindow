using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishController : MonoBehaviour
{
    public bool IsMoving = false; // Moving 상태 여부
    public bool IsFlipped = false; // X 플립 여부
    public float Speed = 0f; // 움직임 속도
    public float MaxPlusSpeed;
    public float MinPlusSpeed;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = IsFlipped; // X 플립 상태 동기화
        }
    }
}

