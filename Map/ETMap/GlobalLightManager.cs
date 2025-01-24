using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GlobalLightManager Instance { get; private set; }

    // 글로벌 라이트를 연결하기 위한 변수
    public Light2D globalLight;

    private void Awake()
    {
        // 싱글톤 초기화: 현재 Instance가 비어 있으면 이 오브젝트를 Instance로 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 이미 Instance가 존재하면 중복 오브젝트를 제거
            Destroy(gameObject);
        }
    }
}