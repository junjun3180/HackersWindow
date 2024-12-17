using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Material : MonoBehaviour
{
    public SpriteRenderer parentRenderer; // 부모 Sprite Renderer
    public Material animatedMaterial;     // 맞는 효과에 사용할 Material
    public float effectDuration;   // 효과 지속 시간

    private Material originalMaterial;    // 부모의 원래 Material
    private bool isEffectActive = false;  // 효과 실행 중인지 여부

    void Start()
    {
        // 부모의 원래 Material 저장
        if (parentRenderer != null)
        {
            originalMaterial = parentRenderer.material;
        }
    }

    public void TriggerHitEffect()
    {
        if (!isEffectActive) // 효과가 실행 중이 아닐 때만 실행
        {
            Debug.Log("넘어옴");

            StartCoroutine(HitEffectCoroutine());
        }
    }

    private IEnumerator HitEffectCoroutine()
    {
        isEffectActive = true;
        Debug.Log("코루틴 시작");

        if (parentRenderer != null)
        {
            // 부모의 Material을 맞는 효과 Material로 변경
            parentRenderer.material = animatedMaterial;
        }

        // 효과 지속 시간 동안 대기
        yield return new WaitForSeconds(effectDuration);

        if (parentRenderer != null)
        {
            // 부모의 Material을 원래 Material로 복구
            parentRenderer.material = originalMaterial;
        }

        isEffectActive = false;
    }
}
