using System.Collections;
using UnityEngine;

public class SpecialAttack : MonoBehaviour
{
    #region Manager

    private static SpecialAttack instance;
    private ItemManager itemManager;

    #endregion

    #region Variables Declaration

    [Header("Base Element")]
    [SerializeField] private float Damage; // 데미지
    [SerializeField] private bool isSpecialAttackMode = false; // 특수 공격 모드 여부
    private float lastClickTime; // 더블 클릭 확인용

    [Header("Attack Area")]
    [SerializeField] private float maxSideLength = 10f; // 드래그 시 최대 변의 길이
    [SerializeField] private float squareSize = 5f; // 더블 클릭 시 나오는 정사각형 크기
    [SerializeField] private float damageDelay; // 데미지가 들어가기까지의 딜레이
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float doubleClickThreshold = 0.3f; // 더블 클릭으로 간주하는 시간 간격

    private Vector2 dragStartPosition; // 드래그 시작 위치
    private bool isDragging = false; // 드래그 중 여부
    private Rect currentAttackArea; // 현재 공격 영역

    #endregion

    #region Default Function

    public static SpecialAttack Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject specialAttackObject = new GameObject("SpecialAttack");
                instance = specialAttackObject.AddComponent<SpecialAttack>();
                DontDestroyOnLoad(specialAttackObject);
            }
            return instance;
        }
    }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer가 할당되지 않았습니다.");
        }
        else
        {
            lineRenderer.positionCount = 5; // 사각형 모양을 그리기 위해 꼭짓점 4개 + 시작점으로 복귀
            lineRenderer.enabled = false; // 초기에는 비활성화
        }

        itemManager = ItemManager.Instance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isSpecialAttackMode) // 모드 상태이면 아무것도 하지 않음
                return;

            if (itemManager.ForcedDeletionUse())
            {
                isSpecialAttackMode = true;
            }
        }

        if (isSpecialAttackMode)
        {
            HandleMouseInput();
        }
    }

    #endregion

    #region Line Renderer Updator

    void HandleMouseInput()
    {
        // 클릭 시작: 드래그 또는 더블 클릭 후보
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = false; // 드래그 초기화
            lastClickTime = Time.time; // 마지막 클릭 시간 갱신
        }

        // 드래그 중 영역 갱신
        if (Input.GetMouseButton(0))
        {
            float dragDuration = Time.time - lastClickTime;

            // 드래그 시간이 0.1초보다 길다면 드래그 상태로 간주
            if (dragDuration > 0.1f)
            {
                lineRenderer.enabled = true;

                isDragging = true;
                Vector2 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 size = new Vector2(
                    Mathf.Min(Mathf.Abs(currentMousePosition.x - dragStartPosition.x), maxSideLength),
                    Mathf.Min(Mathf.Abs(currentMousePosition.y - dragStartPosition.y), maxSideLength)
                );
                Vector2 center = dragStartPosition + size / 2 * new Vector2(
                    Mathf.Sign(currentMousePosition.x - dragStartPosition.x),
                    Mathf.Sign(currentMousePosition.y - dragStartPosition.y)
                );

                currentAttackArea = new Rect(center - size / 2, size);
                UpdateLineRenderer(currentAttackArea);
            }
        }

        // 클릭 또는 드래그 종료
        if (Input.GetMouseButtonUp(0))
        {
            float clickDuration = Time.time - lastClickTime;
            lastClickTime = Time.time;

            // 더블 클릭 처리
            if (clickDuration <= doubleClickThreshold && !isDragging)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                AttackSquare(mousePosition);
                isSpecialAttackMode = false;
            }
            // 드래그 종료 처리
            else if (isDragging)
            {
                isDragging = false;
                StartCoroutine(ApplyDamageAfterDelay(currentAttackArea));
                isSpecialAttackMode = false;
            }
        }
    }

    void AttackSquare(Vector2 center)
    {
        Rect attackArea = new Rect(center - Vector2.one * (squareSize / 2), Vector2.one * squareSize);
        UpdateLineRenderer(attackArea); // 더블 클릭 시에도 라인 렌더러로 영역 표시
        lineRenderer.enabled = true; // 라인 렌더러 활성화

        Debug.Log("더블 클릭 인식");
        StartCoroutine(ApplyDamageAfterDelay(attackArea));

        isSpecialAttackMode = false; // 스페셜 공격 모드 종료
    }

    void UpdateLineRenderer(Rect area)
    {
        Vector3[] corners = new Vector3[5]; // 꼭짓점 4개 + 시작점으로 복귀

        corners[0] = new Vector3(area.xMin, area.yMin, 0); // Bottom-left
        corners[1] = new Vector3(area.xMin, area.yMax, 0); // Top-left
        corners[2] = new Vector3(area.xMax, area.yMax, 0); // Top-right
        corners[3] = new Vector3(area.xMax, area.yMin, 0); // Bottom-right
        corners[4] = corners[0]; // 다시 Bottom-left로 복귀

        lineRenderer.SetPositions(corners);
    }

    #endregion

    #region Area Detection and Damaged Monster

    IEnumerator ApplyDamageAfterDelay(Rect attackArea)
    {
        yield return new WaitForSeconds(damageDelay);
        lineRenderer.enabled = false;

        // Put animation effects under this line
        // Animation Line

        Collider2D[] hitMonsters = Physics2D.OverlapAreaAll(
            new Vector2(attackArea.xMin, attackArea.yMin),
            new Vector2(attackArea.xMax, attackArea.yMax)
        );

        foreach (var monster in hitMonsters)
        {
            if (monster.CompareTag("Monster"))
            {
                monster.gameObject.GetComponent<MonsterBase>().Damaged(Damage);
            }
        }
    }

    #endregion

    #region Getter

    public bool GetAttackMode(){ return isSpecialAttackMode; }

    #endregion
}
