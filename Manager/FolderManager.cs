using System.Collections.Generic;
using UnityEngine;

public class FolderManager : MonoBehaviour
{
    #region Manager

    // �̱��� �ν��Ͻ�
    private static FolderManager instance = null;

    private UI_0_HUD ui_0_HUD;
    private UI_4_LocalDisk localDiskUI;
    private CameraManager cameraManager;

    #endregion

    #region Definition

    public GameObject Player; // ĳ���� ��ġ
    public List<GameObject> specialFolderList = new List<GameObject>(); // ����� ���� ����Ʈ
    public FolderNode previousFolder; // ���� ����
    public FolderNode CurrentFolder; // ���� ����
    public FolderGenerator FolderGenerator; // ���� ������
    public int CurrentFolderMonsterCount = 0;
    public FolderNode rootFolder;

    [Header("Portal")]
    public Portal PreviousPortal = null;

    #endregion

    #region Base Function

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static FolderManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    void Start()
    {
        if (FolderGenerator == null)
        {
            FolderGenerator = FindObjectOfType<FolderGenerator>();
            if (FolderGenerator == null)
            {
                Debug.LogError("FolderGenerator not found!");
                return;
            }
        }

        ui_0_HUD = UI_0_HUD.Instance;
        localDiskUI = UI_4_LocalDisk.Instance;
        cameraManager = CameraManager.Instance;

        // �� ���� ����
        GenerateMap();

        AllFolderDeActivate();

        SetCurrentFolder(CurrentFolder);

        cameraManager.SetCameraFromFolderNode();

        if (localDiskUI != null && rootFolder != null)
        {
            localDiskUI.GenerateTreeUI(FolderGenerator.GetRootNode());
        }
    }

    #endregion

    #region Generate Map

    // �� ���� ����
    public void GenerateMap()
    {
        FolderGenerator.GenerateMap();
        rootFolder = FolderGenerator.GetRootNode();
        specialFolderList = FolderGenerator.hiddenFolderList;
         
        if (rootFolder == null)
        {
            Debug.LogError("Root folder is null. Map generation failed.");
            return;
        }

        CurrentFolder = rootFolder; // ��Ʈ ������ ���� ������ ����
    }

    #endregion

    #region Current Folder Setting, Getting Info

    // ���� ���� ����
    public void SetCurrentFolder(FolderNode folder)
    {
        if (folder == null) return;

        CurrentFolder = folder;
        CurrentFolder.SetFolderActive();

        // HUD ������Ʈ
        SetMonsterCount(folder);
        ui_0_HUD.UpdateHUD();

        // Ŭ���� ���ο� ���� ���� Ȯ�� �� ��Ż�� Ȱ��ȭ 
        CurrentFolder.DeActivePortal();
        CurrentFolder.CheckCurrentFolder(); 

        // ���� ������ �߰� ���·� ����.
        CurrentFolder.isDetectionDone = true;

        // ����� ������ ��� �߰� ���·� ����.
        if (CurrentFolder == null) return;
        if (CurrentFolder.Portals == null) return;

        foreach (Portal portal in CurrentFolder.Portals)
        {
            if (portal == null) continue;
            if (portal.ConnectedFolder == null) continue;
            
            portal.ConnectedFolder.isDetectionDone = true;
        }
    }

    public bool IsClear(){ return CurrentFolder.IsCleared; }

    #endregion

    #region Folder Move

    // ���� �̵�
    public void MoveToFolder(FolderNode folder)
    {
        // Debug.Log("MoveToFolder");
        if (folder == null) return;

        folder.SetFolderActive();
        CurrentFolder.SetFolderDeActive();

        SetCurrentFolder(folder);
        cameraManager.SetCameraFromFolderNode();
    }

    // ���� ������ �̵�(���� ��Ż)
    public void MoveToPreviousFolder(int ParentPortalIndex, Portal preportal)
    {
        if (CurrentFolder == null || CurrentFolder.Parent == null)
        {
            Debug.Log("No previous folder available.");
            return;
        }

        FolderNode PreviousFolderNode = CurrentFolder;
        PreviousPortal = preportal;

        // �÷��̾� ��ġ ����: ���� ������ ����� ������ ��Ż ��ó�� �̵�
        Portal CurrentPortal = CurrentFolder.Left_Portal;
        FolderNode DestinationFolder = CurrentPortal.ConnectedFolder;

        if (Player != null)
        {
            Vector3 newPosition = DestinationFolder.Portals[ParentPortalIndex].transform.position;
            newPosition.x -= 0.5f;
            newPosition.y -= 0.5f;
            Player.transform.position = newPosition;
        }

        MoveToFolder(DestinationFolder);
        PreviousFolderNode.DeActivePortal();
    }

    // ���� ������ �̵� (������ ��Ż�� �̿��ϴ� ���)
    public void MoveToNextFolder(int portalIndex, Portal preportal)
    {
        // Debug.Log("MoveToNextFolder");
        if (CurrentFolder == null) return;
        if (portalIndex < 0 || portalIndex >= CurrentFolder.Portals.Length)
        {
            Debug.Log("out of index");
            return;
        }

        FolderNode PreviousFolderNode = CurrentFolder;
        PreviousPortal = preportal;

        // �÷��̾� ��ġ ����: ���� ������ ���� ��Ż ��ó�� �̵�
        Portal CurrentPortal = CurrentFolder.Portals[portalIndex];
        FolderNode DestinationFolder = CurrentPortal.ConnectedFolder;

        if (CurrentPortal == null)
        {
            Debug.Log("CurrentPortal is null");
            return;
        }    
        if (CurrentPortal.ConnectedFolder == null)
        {
            Debug.Log("CurrentPortal.ConnectedFolder is null");
            return;
        }

        if (Player != null)
        {
            Vector3 newPosition;

            if (CurrentFolder.Type == FolderType.RandomSpecial 
                || CurrentFolder.Type == FolderType.Download 
                || CurrentFolder.Type == FolderType.Shop )
            {
                // Ư�� ��: Y�ุ 0.5 ���� ����
                newPosition = DestinationFolder.Left_Portal.transform.position;
                newPosition.y += 3.0f; // Y�� �̵�
            }
            else
            {
                // �Ϲ� ��: ���� ����
                newPosition = DestinationFolder.Left_Portal.transform.position;
                newPosition.x += 0.5f; // X�� �̵�
                newPosition.y -= 0.5f; // Y�� �̵�
            }
            Player.transform.position = newPosition;
        }

        MoveToFolder(DestinationFolder);
        PreviousFolderNode.DeActivePortal();
        DestinationFolder.Left_Portal.DelayisMovingFalse();
    }

    public void MoveHiddenFolder(string Name)
    {
        Debug.Log("MoveHiddenFolder");

        foreach (var cur in specialFolderList)
        {
            FolderNode folder = cur.GetComponent<FolderNode>();
            if (folder.FolderName == Name)
            {
                Debug.Log("Find");

                if (CurrentFolder.Type != FolderType.Hidden)
                {
                    Debug.Log("before folder is not hidden");
                    previousFolder = CurrentFolder;
                }

                folder.Left_Portal.ConnectedFolder = previousFolder;
                Player.transform.position = folder.transform.Find("TeleportPoint").position;
                MoveToFolder(folder);
                break;   
            }
        }
    }

    public void MoveHiddenToPre(FolderNode folder)
    {
        Debug.Log("MoveHiddenToPre");

        CurrentFolder.DeActivePortal();

        Player.transform.position = folder.transform.Find("TeleportPoint").position;
        MoveToFolder(folder);
    }

    public void MoveToSpecificFolder(string folderName)
    {
        Debug.Log($"Attempting to move to folder: {folderName}");

        FolderNode targetFolder = FindFolderByName(rootFolder, folderName);

        if (targetFolder == null)
        {
            Debug.LogError($"Folder with name '{folderName}' not found.");
            return;
        }

        MoveToFolder(targetFolder);

        Player.transform.position = targetFolder.transform.Find("TeleportPoint").position;
    }

    public void MoveToFolderByType(FolderType folderType)
    {
        FolderNode targetFolder = FindFolderByType(rootFolder, folderType);

        if (targetFolder == null)
        {
            Debug.LogError($"Folder of type '{folderType}' not found.");
            return;
        }

        // Move to the found folder
        MoveToFolder(targetFolder);

        Player.transform.position = targetFolder.transform.Find("TeleportPoint").position;
    }


    private FolderNode FindFolderByName(FolderNode root, string folderName)
    {
        if (root == null) return null;

        // Check if the current folder matches the name
        if (root.FolderName == folderName)
        {
            return root;
        }

        // Recursively search in children
        foreach (var child in root.Children)
        {
            var result = FindFolderByName(child, folderName);
            if (result != null)
            {
                return result;
            }
        }

        return null; // Folder not found
    }

    private FolderNode FindFolderByType(FolderNode root, FolderType folderType)
    {
        if (root == null) return null;

        // Check if the current folder matches the specified type
        if (root.Type == folderType)
        {
            return root;
        }

        // Recursively search in child folders
        foreach (var child in root.Children)
        {
            var result = FindFolderByType(child, folderType);
            if (result != null)
            {
                return result;
            }
        }

        return null; // Folder of the specified type not found
    }

    #endregion

    #region Activation Folder, Portal

    private void AllFolderDeActivate()
    {
        // ��� ������ ��Ȱ��ȭ
        foreach (FolderNode Folder in FindObjectsOfType<FolderNode>())
        {
            Folder.SetFolderDeActive();
        }
    }

    public void AllPortalActivate()
    {
        if (CurrentFolder.Left_Portal != null)
            CurrentFolder.Left_Portal.isActive = true;

        foreach (Portal portal in CurrentFolder.Portals)
        {
            portal.isActive = true;
        }
    }

    #endregion

    #region Portal Reset

    // ���� ���� ��Ż�� ��� �ʱ�ȭ���ִ� �Լ�
    public void ResetCurrentPortal()
    {
        if (CurrentFolder.Left_Portal != null)
            CurrentFolder.Left_Portal.isMoving = false;

        foreach (Portal portal in CurrentFolder.Portals)
        {
            portal.isMoving = false;
        }
    }

    #endregion

    #region Monster

    // �� ����� ������ ���� ������ �ҷ���
    public void SetMonsterCount(FolderNode folder)
    {
        CurrentFolderMonsterCount = folder.GetMonsterCount();
    }

    // ���� �� ����
    public void UpdateMonsterCount(int value)
    {
        CurrentFolderMonsterCount += value;
        CurrentFolder.ChangeMonsterCount();
        ui_0_HUD.UpdateHUD();
        CheckMonsterCount();
    }

    private void CheckMonsterCount()
    {
        if (CurrentFolderMonsterCount > 0) return;
        else
        {
            Debug.Log("Clear!!!");
            CurrentFolder.IsCleared = true;
            CurrentFolder.ActivePortal();
        }
    }

    #endregion

}
