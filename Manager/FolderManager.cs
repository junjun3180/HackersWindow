using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class FolderManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static FolderManager instance = null;

    public GameObject Player; // 캐릭터 위치

    public List<FolderNode> AllFolders = new List<FolderNode>(); // 모든 폴더 리스트
    public FolderNode CurrentFolder; // 현재 폴더
    public FolderGenerator FolderGenerator; // 폴더 생성기
    public int CurrentFolderMonsterCount = 0;
    public FolderNode rootFolder;

    public Portal PreviousPortal = null;

    private UI_0_HUD ui_0_HUD; // HUD를 갱신하기 위한 참조
    private UI_4_LocalDisk localDiskUI;
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

        // 맵 생성 시작
        GenerateMap();

        AllFolderDeActivate();

        SetCurrentFolder(CurrentFolder);

        if (localDiskUI != null && rootFolder != null)
        {
            localDiskUI.GenerateTreeUI(FolderGenerator.GetRootNode());
        }
    }

    // 맵 생성 시작
    public void GenerateMap()
    {
        FolderGenerator.GenerateMap();
        rootFolder = FolderGenerator.GetRootNode();

        if (rootFolder == null)
        {
            Debug.LogError("Root folder is null. Map generation failed.");
            return;
        }

        CurrentFolder = rootFolder; // 루트 폴더를 현재 폴더로 설정
        // Debug.Log("Map generation completed.");
    }

    // 현재 폴더 설정
    public void SetCurrentFolder(FolderNode folder)
    {
        if (folder == null) return;

        CurrentFolder = folder;
        CurrentFolder.SetFolderActive();

        // HUD 업데이트
        SetMonsterCount(folder);
        // Debug.Log($"Current MonsterCount: {CurrentFolderMonsterCount}");
        ui_0_HUD.UpdateHUD();

        // 클리어 여부와 몬스터 수를 확인 후 포탈을 활성화 
        CurrentFolder.DeActivePortal();
        CurrentFolder.CheckCurrentFolder(); 
        // Debug.Log($"Current folder set to: {CurrentFolder.FolderName}");

        // 현재 폴더를 발견 상태로 만듦.
        CurrentFolder.isDetectionDone = true;

        // 연결된 폴더도 모두 발견 상태로 만듦.
        if (CurrentFolder == null)
        {
            Debug.LogError("CurrentFolder is null");
            return;
        }

        if (CurrentFolder.Portals == null)
        {
            Debug.LogError("CurrentFolder.Portals is null");
            return;
        }

        foreach (Portal portal in CurrentFolder.Portals)
        {
            if (portal == null)
            {
                Debug.LogWarning("Portal is null.");
                continue;
            }

            if (portal.ConnectedFolder == null)
            {
                Debug.LogWarning($"Portal {portal.name} does not have a connected folder.");
                continue;
            }

            portal.ConnectedFolder.isDetectionDone = true;
        }
    }

    // 폴더 이동
    public void MoveToFolder(FolderNode folder)
    {
        if (folder == null) return;

        folder.SetFolderActive();
        CurrentFolder.SetFolderDeActive();

        SetCurrentFolder(folder);
    }

    // 상위 폴더로 이동(왼쪽 포탈)
    public void MoveToPreviousFolder(int ParentPortalIndex, Portal preportal)
    {
        if (CurrentFolder == null || CurrentFolder.Parent == null)
        {
            Debug.Log("No previous folder available.");
            return;
        }

        FolderNode PreviousFolderNode = CurrentFolder;
        PreviousPortal = preportal;

        // 플레이어 위치 조정: 상위 폴더의 연결된 오른쪽 포탈 근처로 이동
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

    // 하위 폴더로 이동 (오른쪽 포탈을 이용하는 경우)
    public void MoveToNextFolder(int portalIndex, Portal preportal)
    {
        Debug.Log("MoveToNextFolder");
        if (CurrentFolder == null) return;
        if (portalIndex < 0 || portalIndex >= CurrentFolder.Portals.Length)
        {
            Debug.Log("out of index");
            return;
        }

        FolderNode PreviousFolderNode = CurrentFolder;
        PreviousPortal = preportal;

        // 플레이어 위치 조정: 하위 폴더의 왼쪽 포탈 근처로 이동
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

            if (CurrentFolder.Type == FolderNode.FolderType.RandomSpecial 
                || CurrentFolder.Type == FolderNode.FolderType.Download 
                || CurrentFolder.Type == FolderNode.FolderType.Shop )
            {
                // 특수 방: Y축만 0.5 위로 조정
                newPosition = DestinationFolder.Left_Portal.transform.position;
                newPosition.y += 3.0f; // Y축 이동
            }
            else
            {
                // 일반 방: 기존 로직
                newPosition = DestinationFolder.Left_Portal.transform.position;
                newPosition.x += 0.5f; // X축 이동
                newPosition.y -= 0.5f; // Y축 이동
            }
            Player.transform.position = newPosition;
        }

        MoveToFolder(DestinationFolder);
        PreviousFolderNode.DeActivePortal();
        DestinationFolder.Left_Portal.DelayisMovingFalse();
    }

    private void AllFolderDeActivate()
    {
        // 모든 폴더를 비활성화
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

    // 현재 맵의 포탈을 모두 초기화해주는 함수
    public void ResetCurrentPortal()
    {
        if (CurrentFolder.Left_Portal != null)
            CurrentFolder.Left_Portal.isMoving = false;

        foreach (Portal portal in CurrentFolder.Portals)
        {
            portal.isMoving = false;
        }
    }
    // 이동 후 일정 시간이 지난다음에 이전 방의 포탈의 isMoving을 false로 바꿔줘야 하는데,.,,

    // 맵 입장시 지정된 몬스터 개수를 불러옴
    public void SetMonsterCount(FolderNode folder)
    {
        CurrentFolderMonsterCount = folder.GetMonsterCount();
    }

    // 몬스터 수 갱신
    public void UpdateMonsterCount(int value)
    {
        CurrentFolderMonsterCount += value;
        CurrentFolder.ChangeMonsterCount();
        ui_0_HUD.UpdateHUD();
        // Debug.Log($"Current MonsterCount: {CurrentFolderMonsterCount}");
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

    public bool IsClear()
    { return CurrentFolder.IsCleared; }

}
