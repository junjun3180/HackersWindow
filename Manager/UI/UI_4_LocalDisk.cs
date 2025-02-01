using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UI_4_LocalDisk : MonoBehaviour
{
    #region Definition

    private static UI_4_LocalDisk instance = null;
    private FolderManager folderManager;
    public Player player;

    // UI Window
    public GameObject UI_W_LocalDisk = null;

    //주소관련
    [SerializeField]
    GameObject adressParent;
    [SerializeField]
    Adress_Button adressButton;
    public Text Address;

    #endregion

    #region UI Prefab List

    [Header("UI References")]
    public RectTransform content;       // 트리 UI의 Content 그룹
    public RectTransform linePrefab;    // 선으로 사용할 프리팹

    [Header("Base Folder Prefab")]
    public GameObject FoldPrefab;       // 노말 폴더 노드 프리팹
    public GameObject FoldHiddenPrefab;       // 노말 폴더 노드 프리팹
    public GameObject BossPrefab;       // 보스 폴더 노드 프리팹

    [Header("Shop, Download Prefab")]
    public GameObject DownloadPrefab;   // 다운로드 폴더 노드 프리팹
    public GameObject ShopPrefab;       // 상점 폴더 노드 프리팹

    [Header("Random Special Prefab")]
    public GameObject ChargeRoomPrefab; // 상점 폴더 노드 프리팹
    public GameObject GuardRoomPrefab;  // 상점 폴더 노드 프리팹
    public GameObject JuvaCafePrefab;   // 상점 폴더 노드 프리팹
    public GameObject TrashRoomPrefab;  // 상점 폴더 노드 프리팹

    #endregion

    #region UI Information

    public float ySpacing = 100f; // Y축 간격
    public float xSpacing = 200f; // X축 간격
    public float verticalGapBetweenChildren = 20f; // 자식 노드들 간의 간격

    private Dictionary<int, RectTransform> nodeUIMap;       // 노드 ID와 UI RectTransform 매핑
    private Dictionary<int, List<GameObject>> linesMap;     // 각 노드에 연결된 선을 저장

    public ScrollRect scrollRect;
    public RectTransform viewport;

    #endregion

    #region Default Function

    public static UI_4_LocalDisk Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_4_LocalDisk>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UI_4_LocalDisk).Name);
                    instance = singletonObject.AddComponent<UI_4_LocalDisk>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        folderManager = FolderManager.Instance;
    }

    #endregion

    #region Open/Close UI

    public void OpenUI()
    {
        if (UI_W_LocalDisk != null)
        {
            UI_W_LocalDisk.SetActive(true);
            UpdateNodeUIStates();
        }
    }

    public void CloseUI()
    {
        if (UI_W_LocalDisk != null)
        {
            UI_W_LocalDisk.SetActive(false);
        }
    }

    #endregion

    #region Generation/Update UI Element

    public void GenerateTreeUI(FolderNode rootFolder)
    {
        if (rootFolder == null)
        {
            Debug.LogError("Root folder is null. Cannot generate UI.");
            return;
        }

        // 기존 UI 초기화
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 초기화
        nodeUIMap = new Dictionary<int, RectTransform>();
        linesMap = new Dictionary<int, List<GameObject>>();

        // 거리 계산 및 배치 시작
        float rootSpacing = CalculateNodeSpacing(rootFolder);
        PlaceNodeUI(rootFolder, Vector2.zero);

        // Content 사이즈 조정
        UpdateContentSize();

    }

    private float CalculateNodeSpacing(FolderNode node)
    {
        if (node.Children == null || node.Children.Count == 0)
            return ySpacing;

        float maxChildSpacing = 0f;
        foreach (var child in node.Children)
        {
            float childSpacing = CalculateNodeSpacing(child);
            maxChildSpacing = Mathf.Max(maxChildSpacing, childSpacing);
        }

        // 부모 노드는 자식 간격보다 넓게 설정
        return maxChildSpacing * 1.5f; 
    }

    private void PlaceNodeUI(FolderNode node, Vector2 position)
    {
        // (1) 현재 노드를 UI로 생성
        GameObject prefab = GetPrefabForNode(node);
        GameObject newNodeUI = Instantiate(prefab, content);

        Button button = newNodeUI.GetComponentInChildren<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnNodeButtonClicked(node));
        }

        RectTransform rectTransform = newNodeUI.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;

        nodeUIMap[node.GetInstanceID()] = rectTransform;

        Text nodeText = newNodeUI.GetComponentInChildren<Text>();
        if (nodeText != null)
            nodeText.text = node.FolderName;

        // 자식이 없는 경우는 여기서 끝
        if (node.Children == null || node.Children.Count == 0)
            return;

        // 자식들 전체가 필요로 하는 높이를 계산 (서브트리 높이 합산)
        float verticalGapBetweenChildren = 20f;  // 자식 간 세로 간격
        float totalChildrenHeight = 0f;

        // 미리 자식 서브트리 높이를 구해둔다.
        float[] childHeights = new float[node.Children.Count];
        for (int i = 0; i < node.Children.Count; i++)
        {
            childHeights[i] = CalculateSubtreeHeight(node.Children[i]);
            totalChildrenHeight += childHeights[i];
        }
        // 자식 간격을 고려해 최종 합산
        totalChildrenHeight += verticalGapBetweenChildren * (node.Children.Count - 1);

        float currentY = position.y + totalChildrenHeight * 0.5f;

        // 자식 노드 순회하며 배치
        for (int i = 0; i < node.Children.Count; i++)
        {
            float childHeight = childHeights[i];

            // 자식 노드의 중심이 currentY - childHeight/2가 되도록 설정
            float childCenterY = currentY - childHeight * 0.5f;

            Vector2 childPosition = new Vector2(
                position.x + xSpacing,  // X는 일정 거리만큼 오른쪽
                childCenterY
            );

            DrawLine(rectTransform, childPosition, node);
            PlaceNodeUI(node.Children[i], childPosition);

            currentY -= (childHeight + verticalGapBetweenChildren);
        }
    }

    private float CalculateSubtreeHeight(FolderNode node)
    {
        // 자식이 없다면(리프 노드), 최소 높이를 ySpacing으로 가정
        if (node.Children == null || node.Children.Count == 0)
            return ySpacing;

        float totalHeight = 0f;

        // 모든 자식 서브트리 높이를 누적
        for (int i = 0; i < node.Children.Count; i++)
        {
            float childHeight = CalculateSubtreeHeight(node.Children[i]);
            totalHeight += childHeight;

            // 자식 사이마다 약간 간격을 준다
            if (i < node.Children.Count - 1)
                totalHeight += verticalGapBetweenChildren;
        }

        // 자기 자신이 최소로 차지해야 하는 ySpacing보다 작다면 보정 - 자식이 매우 작아도 본인 ySpacing은 확보
        return Mathf.Max(totalHeight, ySpacing);
    }

    private GameObject GetPrefabForNode(FolderNode node)
    {
        switch (node.Type)
        {
            case FolderType.Download:
                return DownloadPrefab;
            case FolderType.Shop:
                return ShopPrefab;
            case FolderType.Boss:
                return BossPrefab;
            case FolderType.RandomSpecial:
                string name = node.CurrentFolder.name;
                if (name == "Charge_room(Clone)")
                    return ChargeRoomPrefab;
                if (name == "Guard_room(Clone)")
                    return GuardRoomPrefab;
                if (name == "Juva_cafe(Clone)")
                    return JuvaCafePrefab;
                if (name == "Trash_room(Clone)")
                    return TrashRoomPrefab;
                Debug.LogError("Could not found Room Type");
                return null;
            default:
                return FoldPrefab;
        }
    }

    private void DrawLine(RectTransform parentRect, Vector2 childPosition, FolderNode parentNode)
    {
        // linePrefab으로 라인 생성
        GameObject line = Instantiate(linePrefab.gameObject, content);
        RectTransform lineRect = line.GetComponent<RectTransform>();

        Vector2 parentPosition = parentRect.anchoredPosition;
        Vector2 midPoint = (parentPosition + childPosition) / 2;

        lineRect.anchoredPosition = midPoint;
        lineRect.sizeDelta = new Vector2(Vector2.Distance(parentPosition, childPosition), 2f);
        lineRect.rotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(childPosition.y - parentPosition.y, childPosition.x - parentPosition.x) * Mathf.Rad2Deg);

        // linesMap에 저장
        int parentID = parentNode.GetInstanceID();
        if (!linesMap.ContainsKey(parentID))
        {
            linesMap[parentID] = new List<GameObject>();
        }
        linesMap[parentID].Add(line);
    }

    public void UpdateNodeUIStates()
    {
        Debug.Log("UpdateNodeUIStates");

        foreach (var nodePair in nodeUIMap)
        {
            int nodeID = nodePair.Key;
            RectTransform nodeUI = nodePair.Value;

            FolderNode node = FindNodeByID(nodeID);
            if (node == null)
            {
                Debug.LogError($"Node with ID {nodeID} not found.");
                continue;
            }

            // 부모 노드 상태 갱신
            // 맵 이동 아이템으로 이동 시 부모 노드가 미발견 상태인 경우 체크해준다.
            // 만약 이 노드가 탐색되었다면, 부모가 미탐색 상태인지 확인 후 발견 처리
            if (node.isDetectionDone)
            {
                MarkParentAsDiscovered(node);
            }

            // 기존 UI 처리 로직 
            GameObject nodeGameObject = nodeUI.gameObject;
            Image imageComponent = nodeGameObject.GetComponent<Image>();

            if (imageComponent == null)
            {
                Debug.LogError("Image Component is not found in node UI.");
                continue;
            }

            // 탐색되지 않은 경우 UI 비활성화
            if (!node.isDetectionDone)
            {
                nodeGameObject.SetActive(false);
            }
            // 발견 O + 클리어 X : 색상을 어둡게 처리
            else if (node.isDetectionDone && !node.IsCleared)
            {
                nodeGameObject.SetActive(true);
                imageComponent.color = new Color(0.45f, 0.45f, 0.45f, 1.0f);

                if (linesMap.ContainsKey(node.GetInstanceID()))
                {
                    List<GameObject> lines = linesMap[node.GetInstanceID()];

                    for (int i = 0; i < node.Children.Count; i++)
                    {
                        FolderNode child = node.Children[i];

                        // 자식이 클리어된 경우에만 해당 선을 활성화
                        // 자식 라인 프리펩이 순서대로 저장되있음이 보장되어야 함.
                        if (child.IsCleared && i < lines.Count)
                        {
                            lines[i].SetActive(true);
                        }
                    }
                }
            }
            // 발견 O + 클리어 O : 색상 원복, 선 활성화
            else if (node.isDetectionDone && node.IsCleared)
            {
                nodeGameObject.SetActive(true);
                imageComponent.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                // 연결된 선 활성화
                if (linesMap.ContainsKey(nodeID))
                {
                    foreach (GameObject line in linesMap[nodeID])
                    {
                        line.SetActive(true);
                    }
                }
            }
            else
            {
                Debug.LogError("Unexpected node state encountered.");
            }
        }
    }

    // 컨텐츠의 사이즈를 다시 계산해주는 함수
    // UI 요소들을 전부 배치한 후 실행해야 됨.
    private void UpdateContentSize()
    {
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (var kvp in nodeUIMap)
        {
            RectTransform rt = kvp.Value;
            // 노드의 좌표
            Vector2 pos = rt.anchoredPosition;

            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        // content의 pivot이 (0, 0.5)이므로, minX ~ maxX 범위, minY ~ maxY 범위를 고려해서
        // 폭과 높이를 계산
        float width = maxX - minX;
        float height = maxY - minY;

        // 여유 padding
        float padding = 50f;
        width += padding;
        height += padding;

        // 사이즈가 음수가 되지 않도록 보정
        width = Mathf.Max(width, 2000f);  // 최소 폭
        height = Mathf.Max(height, 2000f); // 최소 높이

        content.sizeDelta = new Vector2(width, height);
    }

    // UI에서 클릭시 해당 프리펩이 Viewport에 정 중앙에 위치시키는 함수
    public void CenterOnNode(RectTransform targetNode)
    {
        Vector3 targetLocalPosition = targetNode.localPosition;

        // X, Y 좌표 반전
        float invertedX = -targetLocalPosition.x;
        float invertedY = -targetLocalPosition.y;

        content.anchoredPosition = new Vector2(invertedX, invertedY);
    }

    private void MarkParentAsDiscovered(FolderNode node)
    {
        if (node == null) return;

        // 이미 부모가 없거나 발견된 상태라면 종료
        if (node.Parent == null || node.Parent.isDetectionDone)
            return;

        // 만약 부모가 발견 상태가 아니라면, 발견 처리
        node.Parent.isDetectionDone = true;

        // 그 부모의 부모도 재귀적으로 확인
        // MarkParentAsDiscovered(node.Parent);
    }

    private FolderNode FindNodeByID(int nodeID)
    {
        return FindNodeRecursive(folderManager.rootFolder, nodeID);
    }

    private FolderNode FindNodeRecursive(FolderNode currentNode, int nodeID)
    {
        if (currentNode == null) return null;

        if (currentNode.GetInstanceID() == nodeID)
            return currentNode;

        foreach (var child in currentNode.Children)
        {
            FolderNode result = FindNodeRecursive(child, nodeID);
            if (result != null)
                return result;
        }

        return null;
    }

    #endregion

    #region OnClick Function

    private void OnNodeButtonClicked(FolderNode node)
    {
        Debug.Log("Onclick!!");
        if (node == null)
        {
            Debug.LogError("Folder is null.");
            return;
        }

        if (!node.IsCleared)
        {
            Debug.Log($"Folder {node.FolderName} is not cleared.");
            return;
        }

        if (folderManager?.CurrentFolder == node)
        {
            Debug.Log("Folder is same");
            return;
        }

        Debug.Log($"Moving to Folder {node.FolderName}");
        folderManager.MoveToFolder(node);

        Transform teleportPoint = node.transform.Find("TeleportPoint");
        if (teleportPoint != null && player != null)
        {
            player.transform.position = teleportPoint.position;
            Debug.Log($"Player moved to {node.FolderName} at {teleportPoint.position}");
        }
        else
        {
            Debug.LogWarning("TeleportPoint not found or Player is null.");
        }

        folderManager.ResetCurrentPortal();

        
        CenterOnNode(nodeUIMap[node.GetInstanceID()]);

        UIManager.Instance.WindowUISetActive();
    }

    #endregion

    // 아래는 기존 동근이가 작성한 코드임
    #region Regacy Code 

    public void AdressReset()
    {
        for (int i = adressParent.transform.childCount - 1; i > 0; i--) // 0번째는 제외하고 역순으로
        {
            Destroy(adressParent.transform.GetChild(i).gameObject);
        }
        // adressList.Clear();
    }

    public void SetUIAdress(UI uiType)
    {
        switch (uiType)
        {
            case UI.UI_MyPC:
                Address.text = "내 PC";
                break;
            case UI.UI_DownLoad:
                Address.text = "다운로드";
                break;
            case UI.UI_MyDocument:
                Address.text = "내 문서";
                break;
            case UI.UI_LocalDisk:
                Address.text = "로컬 디스크";
                break;
            case UI.UI_Control:
                Address.text = "제어판";
                break;
            case UI.UI_Help:
                Address.text = "도움말";
                break;
        }
    }

    #endregion 
}
