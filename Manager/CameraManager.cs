using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance; // 싱글톤 인스턴스

    public CinemachineVirtualCamera primaryCamera;
    [SerializeField] private CinemachineVirtualCamera currentCamera;
    [SerializeField] private CinemachineConfiner2D confiner2D;
    [SerializeField] private Player playerController;

    private FolderManager folderManager;


    #region Lagacy Code
    /*
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
    */
    #endregion

    #region Default Function

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
        // gameManager = GameManager.Instance;
        currentCamera = primaryCamera;
        folderManager = FolderManager.Instance;
    }

    #endregion


    public void switcherCamera(CinemachineVirtualCamera newCamera)
    {
        currentCamera.gameObject.SetActive(false);
        newCamera.gameObject.SetActive(true);
        currentCamera = newCamera;
        currentCamera.Follow = playerController.transform;
    }

    public void switchPrimaryCamera()
    {
        currentCamera.gameObject.SetActive(false );
        primaryCamera.gameObject.SetActive(true);
        currentCamera = primaryCamera;
    }

    public void SetCollider()
    {
        Collider2D curCollider = folderManager?.CurrentFolder.GetComponent<Collider2D>();

        if (curCollider == null)
        {
            Debug.LogWarning("CurrentFolder에 Collider2D가 없습니다.");
            return;
        }

        if (confiner2D != null)
        {
            confiner2D.m_BoundingShape2D = curCollider;
            Debug.Log("Collider가 성공적으로 할당되었습니다.");
        }
        else
        {
            Debug.LogError("confiner2D is null");
        }
    }

}

