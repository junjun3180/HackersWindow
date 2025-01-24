using Cinemachine;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class FolderNode : MonoBehaviour
{
    #region Definition

    [Header("폴더 기본정보")]
    public GameObject CurrentFolder;
    public string FolderName; // 폴더 이름
    public FolderNode Parent; // 부모 폴더
    public List<FolderNode> Children; // 자식 폴더들 (최대 3개)
    public bool isDetectionDone = false;
    public List<MonsterBase> monsters = new List<MonsterBase>();

    public enum FolderType
    {
        Start,           // 시작 지점
        Boss,            // 보스 폴더
        Shop,            // 상점 폴더
        Download,        // 다운로드 폴더
        RandomSpecial,   // 랜덤 특수 폴더
        Hidden,          // 숨겨진 폴더 (특정 조건에서만 진입 가능)
        Middle,          // 일반 폴더 (노멀 폴더)
        End,             // 엔드포인트 폴더 (오른쪽 포탈 없음)
        MiddleBoss       // 중간 보스 폴더
    }

    public FolderType Type; // 현재 폴더 타입

    [SerializeField] private int MonsterCount; // 현재 폴더의 몬스터 수
    public bool IsCleared = false; // 클리어 여부
    public float nowPosition;

    // 포탈
    [Header("왼쪽 포탈")]
    public Portal Left_Portal = null; // 왼쪽 포탈
    [Header("오른쪽 포탈 리스트")]
    public Portal[] Portals; // 오른쪽 포탈 리스트

    // 카메라
    public CinemachineVirtualCamera mapCamera;

    #endregion

    #region Default function

    private void Awake()
    {
        Children = new List<FolderNode>(); // 자식 폴더 초기화
    }
    void Update()
    {
        nowPosition = transform.position.x;
    }

    #endregion

    #region Setting FolderNode Connection

    // 부모 폴더를 설정
    public void SetParent(FolderNode parent)
    {
        Parent = parent;
    }

    // 자식 폴더 추가
    public bool AddChild(FolderNode child)
    {
        if (Children.Count >= 3) // 자식이 최대 3개인 경우 추가 불가
        {
            return false;
        }
        child.SetParent(this); // 자식의 부모 설정
        Children.Add(child);
        return true;
    }

    #endregion

    #region Activation FolderNodeElement
    // 현재 폴더를 활성화
    public void SetFolderActive()
    {
        if (CurrentFolder == null) return;

        // Debug.Log("SetFolderActive");
        CurrentFolder.SetActive(true);
    }

    // 현재 폴더를 비활성화
    public void SetFolderDeActive()
    {
        if (CurrentFolder == null) return;

        // Debug.Log("SetFolderDeActive");
        CurrentFolder.SetActive(false);
    }

    // 현재 폴더에 있는 포탈을 전부 활성화
    public void ActivePortal()
    {
        // Debug.Log("ActivePortal");

        // 왼쪽 포탈 활성화
        if (Left_Portal != null)
        {
            Left_Portal.isActive = true;
            Left_Portal.SetClearTrigger();
        }

        // 오른쪽 포탈들 활성화
        foreach (var portal in Portals)
        {
            portal.isActive = true;
            portal.SetClearTrigger();
        }
    }

    // 현재 폴더에 있는 포탈을 전부 비활성화
    public void DeActivePortal()
    {
        // Debug.Log("DeActivePortal");

        // 왼쪽 포탈 비활성화
        if (Left_Portal != null)
            Left_Portal.isActive = false;

        // 오른쪽 포탈들 비활성화
        foreach (var portal in Portals)
        {
            portal.DeActivePortal();
        }
    }

    // 클리어 여부와 몬스터 존재 여부를 체크 후 포탈을 활성화 시킴
    public void CheckCurrentFolder()
    {
        if (IsCleared)
        {
            ActivePortal();
            return;
        }

        if (MonsterCount > 0) return;
        else IsCleared = true;

        if (IsCleared == false) return;

        Debug.Log("Folder is cleared");
        ActivePortal();
    }

    #endregion

    #region Related to Monsters

    // 몬스터 처치 시 호출
    public void ChangeMonsterCount()
    {
        MonsterCount--;

        if (MonsterCount <= 0)
        {
            IsCleared = true;
            ActivePortal();
        }
    }

    public int GetMonsterCount()
    {
        return MonsterCount;
    }

    public List<MonsterBase> FindAndReturnMonster()
    {
        // 자식 오브젝트들 순회
        foreach (Transform child in transform)
        {
            // 자식 오브젝트에 MonsterBase 컴포넌트가 있는지 확인
            MonsterBase monster = child.GetComponent<MonsterBase>();

            // MonsterBase 타입이 있을 경우 리스트에 추가
            if (monster != null)
            {
                monsters.Add(monster);
            }
        }

        return monsters;
    }


    #endregion
}
