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

    //�ּҰ���
    [SerializeField]
    GameObject adressParent;
    [SerializeField]
    Adress_Button adressButton;
    public Text Address;

    #endregion

    #region UI Prefab List

    [Header("UI References")]
    public RectTransform content;       // Ʈ�� UI�� Content �׷�
    public RectTransform linePrefab;    // ������ ����� ������

    [Header("Base Folder Prefab")]
    public GameObject FoldPrefab;       // �븻 ���� ��� ������
    public GameObject FoldHiddenPrefab;       // �븻 ���� ��� ������
    public GameObject BossPrefab;       // ���� ���� ��� ������

    [Header("Shop, Download Prefab")]
    public GameObject DownloadPrefab;   // �ٿ�ε� ���� ��� ������
    public GameObject ShopPrefab;       // ���� ���� ��� ������

    [Header("Random Special Prefab")]
    public GameObject ChargeRoomPrefab; // ���� ���� ��� ������
    public GameObject GuardRoomPrefab;  // ���� ���� ��� ������
    public GameObject JuvaCafePrefab;   // ���� ���� ��� ������
    public GameObject TrashRoomPrefab;  // ���� ���� ��� ������

    #endregion

    #region UI Information

    public float ySpacing = 100f; // Y�� ����
    public float xSpacing = 200f; // X�� ����
    public float verticalGapBetweenChildren = 20f; // �ڽ� ���� ���� ����

    private Dictionary<int, RectTransform> nodeUIMap;       // ��� ID�� UI RectTransform ����
    private Dictionary<int, List<GameObject>> linesMap;     // �� ��忡 ����� ���� ����

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

        // ���� UI �ʱ�ȭ
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // �ʱ�ȭ
        nodeUIMap = new Dictionary<int, RectTransform>();
        linesMap = new Dictionary<int, List<GameObject>>();

        // �Ÿ� ��� �� ��ġ ����
        float rootSpacing = CalculateNodeSpacing(rootFolder);
        PlaceNodeUI(rootFolder, Vector2.zero);

        // Content ������ ����
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

        // �θ� ���� �ڽ� ���ݺ��� �а� ����
        return maxChildSpacing * 1.5f; 
    }

    private void PlaceNodeUI(FolderNode node, Vector2 position)
    {
        // (1) ���� ��带 UI�� ����
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

        // �ڽ��� ���� ���� ���⼭ ��
        if (node.Children == null || node.Children.Count == 0)
            return;

        // �ڽĵ� ��ü�� �ʿ�� �ϴ� ���̸� ��� (����Ʈ�� ���� �ջ�)
        float verticalGapBetweenChildren = 20f;  // �ڽ� �� ���� ����
        float totalChildrenHeight = 0f;

        // �̸� �ڽ� ����Ʈ�� ���̸� ���صд�.
        float[] childHeights = new float[node.Children.Count];
        for (int i = 0; i < node.Children.Count; i++)
        {
            childHeights[i] = CalculateSubtreeHeight(node.Children[i]);
            totalChildrenHeight += childHeights[i];
        }
        // �ڽ� ������ ����� ���� �ջ�
        totalChildrenHeight += verticalGapBetweenChildren * (node.Children.Count - 1);

        float currentY = position.y + totalChildrenHeight * 0.5f;

        // �ڽ� ��� ��ȸ�ϸ� ��ġ
        for (int i = 0; i < node.Children.Count; i++)
        {
            float childHeight = childHeights[i];

            // �ڽ� ����� �߽��� currentY - childHeight/2�� �ǵ��� ����
            float childCenterY = currentY - childHeight * 0.5f;

            Vector2 childPosition = new Vector2(
                position.x + xSpacing,  // X�� ���� �Ÿ���ŭ ������
                childCenterY
            );

            DrawLine(rectTransform, childPosition, node);
            PlaceNodeUI(node.Children[i], childPosition);

            currentY -= (childHeight + verticalGapBetweenChildren);
        }
    }

    private float CalculateSubtreeHeight(FolderNode node)
    {
        // �ڽ��� ���ٸ�(���� ���), �ּ� ���̸� ySpacing���� ����
        if (node.Children == null || node.Children.Count == 0)
            return ySpacing;

        float totalHeight = 0f;

        // ��� �ڽ� ����Ʈ�� ���̸� ����
        for (int i = 0; i < node.Children.Count; i++)
        {
            float childHeight = CalculateSubtreeHeight(node.Children[i]);
            totalHeight += childHeight;

            // �ڽ� ���̸��� �ణ ������ �ش�
            if (i < node.Children.Count - 1)
                totalHeight += verticalGapBetweenChildren;
        }

        // �ڱ� �ڽ��� �ּҷ� �����ؾ� �ϴ� ySpacing���� �۴ٸ� ���� - �ڽ��� �ſ� �۾Ƶ� ���� ySpacing�� Ȯ��
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
        // linePrefab���� ���� ����
        GameObject line = Instantiate(linePrefab.gameObject, content);
        RectTransform lineRect = line.GetComponent<RectTransform>();

        Vector2 parentPosition = parentRect.anchoredPosition;
        Vector2 midPoint = (parentPosition + childPosition) / 2;

        lineRect.anchoredPosition = midPoint;
        lineRect.sizeDelta = new Vector2(Vector2.Distance(parentPosition, childPosition), 2f);
        lineRect.rotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(childPosition.y - parentPosition.y, childPosition.x - parentPosition.x) * Mathf.Rad2Deg);

        // linesMap�� ����
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

            // �θ� ��� ���� ����
            // �� �̵� ���������� �̵� �� �θ� ��尡 �̹߰� ������ ��� üũ���ش�.
            // ���� �� ��尡 Ž���Ǿ��ٸ�, �θ� ��Ž�� �������� Ȯ�� �� �߰� ó��
            if (node.isDetectionDone)
            {
                MarkParentAsDiscovered(node);
            }

            // ���� UI ó�� ���� 
            GameObject nodeGameObject = nodeUI.gameObject;
            Image imageComponent = nodeGameObject.GetComponent<Image>();

            if (imageComponent == null)
            {
                Debug.LogError("Image Component is not found in node UI.");
                continue;
            }

            // Ž������ ���� ��� UI ��Ȱ��ȭ
            if (!node.isDetectionDone)
            {
                nodeGameObject.SetActive(false);
            }
            // �߰� O + Ŭ���� X : ������ ��Ӱ� ó��
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

                        // �ڽ��� Ŭ����� ��쿡�� �ش� ���� Ȱ��ȭ
                        // �ڽ� ���� �������� ������� ����������� ����Ǿ�� ��.
                        if (child.IsCleared && i < lines.Count)
                        {
                            lines[i].SetActive(true);
                        }
                    }
                }
            }
            // �߰� O + Ŭ���� O : ���� ����, �� Ȱ��ȭ
            else if (node.isDetectionDone && node.IsCleared)
            {
                nodeGameObject.SetActive(true);
                imageComponent.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                // ����� �� Ȱ��ȭ
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

    // �������� ����� �ٽ� ������ִ� �Լ�
    // UI ��ҵ��� ���� ��ġ�� �� �����ؾ� ��.
    private void UpdateContentSize()
    {
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (var kvp in nodeUIMap)
        {
            RectTransform rt = kvp.Value;
            // ����� ��ǥ
            Vector2 pos = rt.anchoredPosition;

            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        // content�� pivot�� (0, 0.5)�̹Ƿ�, minX ~ maxX ����, minY ~ maxY ������ ����ؼ�
        // ���� ���̸� ���
        float width = maxX - minX;
        float height = maxY - minY;

        // ���� padding
        float padding = 50f;
        width += padding;
        height += padding;

        // ����� ������ ���� �ʵ��� ����
        width = Mathf.Max(width, 2000f);  // �ּ� ��
        height = Mathf.Max(height, 2000f); // �ּ� ����

        content.sizeDelta = new Vector2(width, height);
    }

    // UI���� Ŭ���� �ش� �������� Viewport�� �� �߾ӿ� ��ġ��Ű�� �Լ�
    public void CenterOnNode(RectTransform targetNode)
    {
        Vector3 targetLocalPosition = targetNode.localPosition;

        // X, Y ��ǥ ����
        float invertedX = -targetLocalPosition.x;
        float invertedY = -targetLocalPosition.y;

        content.anchoredPosition = new Vector2(invertedX, invertedY);
    }

    private void MarkParentAsDiscovered(FolderNode node)
    {
        if (node == null) return;

        // �̹� �θ� ���ų� �߰ߵ� ���¶�� ����
        if (node.Parent == null || node.Parent.isDetectionDone)
            return;

        // ���� �θ� �߰� ���°� �ƴ϶��, �߰� ó��
        node.Parent.isDetectionDone = true;

        // �� �θ��� �θ� ��������� Ȯ��
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

    // �Ʒ��� ���� �����̰� �ۼ��� �ڵ���
    #region Regacy Code 

    public void AdressReset()
    {
        for (int i = adressParent.transform.childCount - 1; i > 0; i--) // 0��°�� �����ϰ� ��������
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
                Address.text = "�� PC";
                break;
            case UI.UI_DownLoad:
                Address.text = "�ٿ�ε�";
                break;
            case UI.UI_MyDocument:
                Address.text = "�� ����";
                break;
            case UI.UI_LocalDisk:
                Address.text = "���� ��ũ";
                break;
            case UI.UI_Control:
                Address.text = "������";
                break;
            case UI.UI_Help:
                Address.text = "����";
                break;
        }
    }

    #endregion 
}
