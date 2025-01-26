using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    #region Manager

    public static CameraManager Instance; // �̱��� �ν��Ͻ�
    private FolderManager folderManager;

    #endregion

    #region Definition

    public CinemachineVirtualCamera primaryCamera;
    [SerializeField] private CinemachineVirtualCamera currentCamera;
    [SerializeField] private CinemachineConfiner2D confiner2D;
    [SerializeField] private Player playerController;

    #endregion

    #region Lagacy Code
    /*
    [SerializeField]
    private Camera playerCamera;  // �� ī�޶�
    [SerializeField]
    private CinemachineConfiner2D cinemachine;
    [SerializeField]
    private CinemachineVirtualCamera cinemachineVir;
    GameManager gameManager;
    [Header("Ư���� ī�޶� ����")]
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
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // �Ŵ����� �ٸ� ������ �Ѿ�� �ı����� �ʰ� ����
        }
        else
        {
            Destroy(gameObject);  // �̹� �����ϸ� ���ο� �Ŵ����� �ı�
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

    #region Camera Setting

    public void SetCameraFromFolderNode()
    {
        
        switch(folderManager.CurrentFolder.Type)
        {
            case FolderType.RandomSpecial:
            case FolderType.Hidden:
                switcherCamera(folderManager.CurrentFolder.mapCamera);
                break;
            default:
                switchPrimaryCamera();
                break;
        }
        
        /*
        if (folderManager.CurrentFolder.Type == FolderType.RandomSpecial)
        {
            // Special Setting Camera
            switcherCamera(folderManager.CurrentFolder.mapCamera);
        }
        else
        { 
            // Normal Type Camera
            switchPrimaryCamera();
        }
        */

        SetCollider();
    }

    public void switcherCamera(CinemachineVirtualCamera newCamera)
    {
        currentCamera.gameObject.SetActive(false);
        newCamera.gameObject.SetActive(true);
        currentCamera = newCamera;
        currentCamera.Follow = playerController.transform;
        confiner2D = newCamera.GetComponent<CinemachineConfiner2D>();
    }

    public void switchPrimaryCamera()
    {
        currentCamera.gameObject.SetActive(false );
        primaryCamera.gameObject.SetActive(true);
        currentCamera = primaryCamera;
        confiner2D = primaryCamera.GetComponent<CinemachineConfiner2D>();
    }

    public void SetCollider()
    {
        // Debug.Log("SetCollider");
        PolygonCollider2D curCollider = folderManager?.CurrentFolder.GetComponent<PolygonCollider2D>();

        if (curCollider == null)
        {
            Debug.LogWarning("Current Folder does not have Collider2D.");
            return;
        }

        if (confiner2D != null)
        {
            confiner2D.m_BoundingShape2D = curCollider;
            Debug.Log("Collider has been assigned successfully.");
        }
        else
        {
            Debug.LogError("confiner2D is null");
        }
    }

    #endregion
}

