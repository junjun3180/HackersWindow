using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_4_LocalDisk : MonoBehaviour
{
    private static UI_4_LocalDisk instance = null;

    // UI Window
    public GameObject UI_W_LocalDisk = null;

    // Manager
    private FolderManager folderManager;
    public Player player;

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


    public float ySpacing = 100f; // Y축 간격
    public float xSpacing = 200f; // X축 간격

    private Dictionary<int, List<FolderNode>> depthNodes;   // Depth별 노드 저장
    private Dictionary<int, RectTransform> nodeUIMap;       // 노드 ID와 UI RectTransform 매핑
    private Dictionary<int, List<GameObject>> linesMap;     // 각 노드에 연결된 선을 저장


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
        folderManager = FolderManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenUI()
    {
        Debug.Log("OpenUI");
        if (UI_W_LocalDisk != null)
        {
            UI_W_LocalDisk.SetActive(true);
            UpdateNodeUIStates();
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
        depthNodes = new Dictionary<int, List<FolderNode>>();
        nodeUIMap = new Dictionary<int, RectTransform>();
        linesMap = new Dictionary<int, List<GameObject>>();

        // Depth별로 노드 수집 (깊이 우선 탐색)
        CollectNodesByDepth(rootFolder, 0);

        // Depth별 노드 UI 생성
        GenerateUIFromDepthNodes();
    }

    private void CollectNodesByDepth(FolderNode node, int depth)
    {
        if (!depthNodes.ContainsKey(depth))
        {
            depthNodes[depth] = new List<FolderNode>();
        }

        // 현재 Depth에 노드 추가
        depthNodes[depth].Add(node);

        // 자식 노드 순회
        foreach (var child in node.Children)
        {
            CollectNodesByDepth(child, depth + 1);
        }
    }

    private void GenerateUIFromDepthNodes()
    {
        foreach (var depth in depthNodes.Keys)
        {
            List<FolderNode> currentDepthNodes = depthNodes[depth];

            // Y축 위치를 균등하게 배치하기 위한 시작점
            float yStart = (currentDepthNodes.Count - 1) * ySpacing / 2;

            for (int i = 0; i < currentDepthNodes.Count; i++)
            {
                var node = currentDepthNodes[i];

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
                    case FolderNode.FolderType.RandomSpecial:
                        string name = node.CurrentFolder.name;
                        if (name == "Charge_room(Clone)")
                            newNodeUI = Instantiate(ChargeRoomPrefab, content);
                        else if (name == "Guard_room(Clone)")
                            newNodeUI = Instantiate(GuardRoomPrefab, content);
                        else if (name == "Juva_cafe(Clone)")
                            newNodeUI = Instantiate(JuvaCafePrefab, content);
                        else if (name == "Trash_room(Clone)")
                            newNodeUI = Instantiate(TrashRoomPrefab, content);
                        else
                        {
                            Debug.LogError("Could not found Room Type");
                            return;
                        }
                        break;
                    default:
                        newNodeUI = Instantiate(FoldPrefab, content);
                        break;
                }

                RectTransform rectTransform = newNodeUI.GetComponent<RectTransform>();
                rectTransform.name = $"Node_{node.FolderName}";

                // X축 위치: Depth에 따라 오른쪽으로 이동
                float xPos = depth * xSpacing;

                // Y축 위치: 중앙 기준으로 위아래로 균등하게 분산
                float yPos = yStart - i * ySpacing;

                rectTransform.anchoredPosition = new Vector2(xPos, yPos);

                // 노드 정보 설정
                Text nodeText = newNodeUI.GetComponentInChildren<Text>();
                if (nodeText != null)
                {
                    nodeText.text = node.FolderName;
                }

                // 노드 UI 저장
                nodeUIMap[node.GetInstanceID()] = rectTransform;

                // 부모와 연결되는 선 그리기
                if (node.Parent != null && nodeUIMap.ContainsKey(node.Parent.GetInstanceID()))
                {
                    RectTransform parentUI = nodeUIMap[node.Parent.GetInstanceID()];
                    DrawLine(node.Parent, node, parentUI, rectTransform);
                }

                // 버튼 이벤트 
                Button button = newNodeUI.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnNodeButtonClicked(node));
                }
            }
        }
    }

    private void DrawLine(FolderNode startNode, FolderNode endNode, RectTransform startNodeUI, RectTransform endNodeUI)
    {
        // 라인을 생성 후 그룹 최상단에 위치시킴.
        RectTransform line = Instantiate(linePrefab, content);
        line.transform.SetSiblingIndex(0);
        line.name = "Line";

        Vector2 startPosition = startNodeUI.anchoredPosition;
        Vector2 endPosition = endNodeUI.anchoredPosition;

        float distance = Vector2.Distance(startPosition, endPosition);
        float angle = Mathf.Atan2(endPosition.y - startPosition.y, endPosition.x - startPosition.x) * Mathf.Rad2Deg;

        line.sizeDelta = new Vector2(distance, 2f); // 선 두께
        line.anchoredPosition = (startPosition + endPosition) / 2; // 선의 중심
        line.localRotation = Quaternion.Euler(0, 0, angle); // 선 회전
        line.gameObject.SetActive(false);

        // 선을 linesMap에 저장
        int startNodeID = startNode.GetInstanceID();
        int endNodeID = endNode.GetInstanceID();

        if (!linesMap.ContainsKey(startNodeID))
        {
            linesMap[startNodeID] = new List<GameObject>();
        }
        linesMap[startNodeID].Add(line.gameObject);

        if (!linesMap.ContainsKey(endNodeID))
        {
            linesMap[endNodeID] = new List<GameObject>();
        }
        linesMap[endNodeID].Add(line.gameObject);
    }

    private void OnNodeButtonClicked(FolderNode node)
    {
        if (node == null)
        {
            Debug.LogError("Folder is null.");
            return;
        }

        // 클리어 상태 확인
        if (!node.IsCleared)
        {
            Debug.Log($"Folder {node.FolderName} is not cleared.");
            return;
        }

        // 이동 로직 실행
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
        
        // 현재 포탈을 모두 활성화
        folderManager.ResetCurrentPortal();
    }



    public void UpdateNodeUIStates()
    {
        Debug.Log("UpdateNodeUIStates");
        foreach (var nodePair in nodeUIMap)
        {
            int nodeID = nodePair.Key;
            RectTransform nodeUI = nodePair.Value;

            // FolderNode를 가져오기 위해 ID를 기반으로 찾음
            FolderNode node = depthNodes.Values
                .SelectMany(list => list)
                .FirstOrDefault(n => n.GetInstanceID() == nodeID);

            if (node == null)
            {
                Debug.LogError($"Node with ID {nodeID} not found.");
                continue;
            }

            GameObject nodeGameObject = nodeUI.gameObject; 
            Image imageComponent = nodeGameObject.GetComponent<Image>();

            if (imageComponent == null)
            {
                Debug.LogError("Image Component is not found");
                return;
            }
            // 탐색되지 않은 경우 UI 비활성화
            if (node.isDetectionDone == false)
            {
                nodeGameObject.SetActive(false);
            }
            // 발견 O + 클리어 X : 색상을 어둡게 처리
            else if (node.isDetectionDone == true && node.IsCleared == false)
            {
                Debug.Log("발견 O + 클리어 X");
                nodeGameObject.SetActive(true);
                imageComponent.color = new Color(0.3f, 0.3f, 0.3f, 0.6f);
            }
            // 발견 O + 클리어 O : 색상 원복, 선 활성화
            else if (node.isDetectionDone == true && node.IsCleared == true)
            {
                Debug.Log("발견 O + 클리어 O");
                nodeGameObject.SetActive(true);
                imageComponent.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
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
                Debug.LogError("이상한 상태가 별견됨");
            }
            
        }
    }


    // 아래는 기존 동근이가 작성한 코드임

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
