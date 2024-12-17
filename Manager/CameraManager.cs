using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance; // 싱글톤 인스턴스
    [SerializeField]
    private Camera playerCamera;  // 주 카메라
    [SerializeField]
    private CinemachineConfiner2D cinemachine;
    [SerializeField]
    private CinemachineVirtualCamera cinemachineVir;
    GameManager gameManager;
    [Header("특수맵 카메라 조정")]
    [SerializeField]
    private float chargeCamera;
    [SerializeField]
    private float guardCamera;
    [SerializeField]
    private float trashCamera;
    [SerializeField]
    private float cafeCamera;
    [SerializeField]
    private float downloadCamera;
    [SerializeField]
    private float shopCamera;
    

    private void Awake()
    {
        // 싱글톤 패턴 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 매니저가 다른 씬으로 넘어가도 파괴되지 않게 설정
        }
        else
        {
            Destroy(gameObject);  // 이미 존재하면 새로운 매니저는 파괴
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    
    public void CameraLimit(GameObject map)
    {
        PolygonCollider2D collider = map.GetComponent<PolygonCollider2D>();
        cinemachine.m_BoundingShape2D = collider;
    }
    public void CameraPlayerSet()
    {
        playerCamera.transform.position = 
            new Vector3(gameManager.player.transform.position.x,gameManager.player.transform.position.y,playerCamera.transform.position.z);
    }

    //public void SpecialMapCamera(Map specialMap)
    //{
    //    switch (specialMap.mapName)
    //    {
    //        case "전원 옵션":
    //            //숫자를 변경하면됨 기본 6f
    //            cinemachineVir.m_Lens.OrthographicSize = chargeCamera;
    //            break;
    //        case "Window 방화벽":
    //            //숫자를 변경하면됨 기본 6f
    //            cinemachineVir.m_Lens.OrthographicSize = guardCamera;
    //            break;
    //        case "JuvaCafe":
    //            //숫자를 변경하면됨 기본 6f
    //            cinemachineVir.m_Lens.OrthographicSize = cafeCamera;
    //            break;
    //        case "휴지통":
    //            //숫자를 변경하면됨 기본 6f
    //            cinemachineVir.m_Lens.OrthographicSize = trashCamera;
    //            break;
    //        case "Download":
    //            //숫자를 변경하면됨 기본 6f
    //            cinemachineVir.m_Lens.OrthographicSize = downloadCamera;
    //            break;
    //        case "Shop":
    //            //숫자를 변경하면됨 기본 6f
    //            cinemachineVir.m_Lens.OrthographicSize = shopCamera;
    //            break;
    //    }
    //}
}

