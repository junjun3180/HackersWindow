using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_4_LocalDisk : MonoBehaviour
{
    private static UI_4_LocalDisk instance = null;

    // UI Window
    public GameObject UI_W_LocalDisk = null;

    //주소관련
    // public List<Map> adressList = new();
    [SerializeField]
    GameObject adressParent;
    [SerializeField]
    Adress_Button adressButton;
    public Text Address;

    // 신규 
    [Header("UI References")]
    public RectTransform content;       // 트리 UI의 Content 그룹
    public GameObject FoldPrefab;       // 노말 폴더 노드 프리팹
    public GameObject DownloadPrefab;   // 다운로드 폴더 노드 프리팹
    public GameObject ShopPrefab;       // 상점 폴더 노드 프리팹
    public GameObject BossPrefab;       // 보스 폴더 노드 프리팹
    public RectTransform linePrefab;    // 선으로 사용할 프리팹
    public float xOffset = 200f;        // X축 간격
    public float yOffset = 100f;        // Y축 간격

    private Dictionary<int, int> depthNodeCount; // 각 Depth의 노드 개수를 저장

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenUI()
    {
        if (UI_W_LocalDisk != null)
        {
            UI_W_LocalDisk.SetActive(true);
            // Debug.Log("OpenUI : UI_4_LocalDisk");
        }
    }

    public void CloseUI()
    {
        if (UI_W_LocalDisk != null)
        {
            UI_W_LocalDisk.SetActive(false);
            // Debug.Log("CloseUI : UI_4_LocalDisk");
        }
    }

    public void GenerateTreeUI(List<FolderGenerator.TreeNodeData> nodes)
    {
        if (nodes == null || nodes.Count == 0)
        {
            Debug.LogError("No nodes available for generating the UI.");
            return;
        }

        // 기존 UI 초기화
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Depth별 노드 개수 계산
        CalculateDepthNodeCount(nodes);

        // 루트 노드부터 시작
        var rootNode = nodes.Find(node => node.ParentId == null);
        if (rootNode == null)
        {
            Debug.LogError("Root node not found.");
            return;
        }

        // 재귀적으로 UI 생성 시작
        GenerateNodeUI(rootNode, nodes, null, 0, 0);
    }

    // Depth별 노드 개수를 계산하는 함수
    private void CalculateDepthNodeCount(List<FolderGenerator.TreeNodeData> nodes)
    {
        depthNodeCount = new Dictionary<int, int>();

        foreach (var node in nodes)
        {
            if (!depthNodeCount.ContainsKey(node.Depth))
            {
                depthNodeCount[node.Depth] = 0;
            }
            depthNodeCount[node.Depth]++;
        }
    }

    private void GenerateNodeUI(FolderGenerator.TreeNodeData node, List<FolderGenerator.TreeNodeData> nodes,
                                RectTransform parentUI, int depth, int siblingIndex)
    {
        // 현재 노드 UI 생성
        GameObject newNodeUI;

        // 노드 타입에 따라 프리팹 설정
        switch (node.Type)
        {
            case FolderNode.FolderType.Download:
                newNodeUI = Instantiate(DownloadPrefab, content);
                break;
            case FolderNode.FolderType.Shop:
                newNodeUI = Instantiate(ShopPrefab, content);
                break;
            case FolderNode.FolderType.Boss:
                newNodeUI = Instantiate(BossPrefab, content);
                break;
            default:
                newNodeUI = Instantiate(FoldPrefab, content);
                break;
        }

        RectTransform rectTransform = newNodeUI.GetComponent<RectTransform>();
        rectTransform.name = $"Node_{node.Id}";

        // X축 위치 계산
        float xPos = depth * xOffset;

        // Y축 위치 계산: Depth의 노드 개수에 따라 중앙에서 균등 분산
        int totalNodesAtDepth = depthNodeCount[depth];
        float ySpacing = yOffset; // 노드 간 간격
        float yStart = -(totalNodesAtDepth - 1) * ySpacing / 2; // 중앙 기준 시작점

        float yPos = yStart + siblingIndex * ySpacing;

        rectTransform.anchoredPosition = new Vector2(xPos, yPos);

        // 노드 정보 설정
        Text nodeText = newNodeUI.GetComponentInChildren<Text>();
        if (nodeText != null)
        {
            nodeText.text = $"ID: {node.Id}";
        }

        // 부모와 자식 사이에 선 그리기
        if (parentUI != null)
        {
            DrawLine(parentUI, rectTransform);
        }

        // 자식 노드 순회
        int childIndex = 0;
        foreach (var childId in node.Children)
        {
            var childNode = nodes.Find(n => n.Id == childId);
            if (childNode != null)
            {
                GenerateNodeUI(childNode, nodes, rectTransform, depth + 1, childIndex);
                childIndex++;
            }
        }
    }
    private void DrawLine(RectTransform startNode, RectTransform endNode)
    {
        // 선 프리팹 생성
        RectTransform line = Instantiate(linePrefab, content);
        line.name = "Line";

        Vector2 startPosition = startNode.anchoredPosition;
        Vector2 endPosition = endNode.anchoredPosition;

        // 선 길이와 각도 설정
        float distance = Vector2.Distance(startPosition, endPosition);
        float angle = Mathf.Atan2(endPosition.y - startPosition.y, endPosition.x - startPosition.x) * Mathf.Rad2Deg;

        line.sizeDelta = new Vector2(distance, 2f); // 선 두께
        line.anchoredPosition = (startPosition + endPosition) / 2; // 선의 중심
        line.localRotation = Quaternion.Euler(0, 0, angle); // 선 회전
    }





    // 아래는 기존 동근이가 작성한 코드임
    IEnumerator LayoutReset(RectTransform obj)
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(obj);

    }

    public void AdressReset()
    {
        for (int i = adressParent.transform.childCount - 1; i > 0; i--) // 0번째는 제외하고 역순으로
        {
            Destroy(adressParent.transform.GetChild(i).gameObject);
        }
        // adressList.Clear();
    }

    public void SetUIAdress(UIManager.UI uiType)
    {
        switch (uiType)
        {
            case UIManager.UI.UI_MyPC:
                Address.text = "내 PC";
                break;
            case UIManager.UI.UI_DownLoad:
                Address.text = "다운로드";
                break;
            case UIManager.UI.UI_MyDocument:
                Address.text = "내 문서";
                break;
            case UIManager.UI.UI_LocalDisk:
                Address.text = "로컬 디스크";
                break;
            case UIManager.UI.UI_Control:
                Address.text = "제어판";
                break;
            case UIManager.UI.UI_Help:
                Address.text = "도움말";
                break;
        }
    }
}
