using System.Collections.Generic;
using UnityEngine;

public class FolderGenerator : MonoBehaviour
{
    #region Definition

    [Header("MapCount")]
    public int GenerateMapCount;
    private int MaxFolderCount;

    [Header("Folder Prefab Lists")]
    public List<GameObject> StartFolderPrefabs1; // 오른쪽 포탈 1개 스타트맵
    public List<GameObject> StartFolderPrefabs2; // 오른쪽 포탈 2개 스타트맵
    public List<GameObject> StartFolderPrefabs3; // 오른쪽 포탈 3개 스타트맵
    public List<GameObject> NormalFolderPrefabs1; // 왼쪽에 포탈이 1개, 오른쪽에 포탈이 1개인 폴더 리스트
    public List<GameObject> NormalFolderPrefabs2; // 왼쪽에 포탈이 1개, 오른쪽에 포탈이 2개인 폴더 리스트
    public List<GameObject> NormalFolderPrefabs3; // 왼쪽에 포탈이 1개, 오른쪽에 포탈이 3개인 폴더 리스트
    public List<GameObject> DownloadFolderPrefabs;
    public List<GameObject> ShopFolderPrefabs;
    public List<GameObject> BossFolderPrefabs;
    public List<GameObject> SpecialFolderPrefabs;
    public List<GameObject> EndFolderPrefabs;
    public List<GameObject> HiddenFolderPrefabs;
    public GameObject SqualoMarketPrefab;

    [Header("Generated Map Ojbect")]
    private FolderNode rootFolder;
    public List<GameObject> hiddenFolderList = new List<GameObject>();

    [Header("Portal Prefabs")]
    public GameObject BossPortal;
    public GameObject ShopPortal;
    public GameObject DownloadPortal;
    public GameObject Charge_room;
    public GameObject Guard_room;
    public GameObject Juva_cafe;
    public GameObject Trash_room;
    public GameObject Default;

    [Header("Spacing Settings")]
    public float HorizontalSpacing;
    public float VerticalSpacing;

    public List<TreeNodeData> TreeTemplete;

    #endregion

    public void GenerateMap()
    {
        if (GenerateMapCount < 6)
        {
            Debug.LogError("MaxFolderCount must be at least 6 to ensure proper map generation.");
            return;
        }

        // 2(다운로드, 상점) + 1(보스방) + 2(스페셜) = 5개를 제외 시켜야 함.(시작 방은 포함되어 있음)
        MaxFolderCount = GenerateMapCount - 5;

        // 가장 먼저 트리 자료 구조를 생성
        TreeTemplete = GenerateTreeData(MaxFolderCount);

        // 시작 맵을 생성
        rootFolder = CreateStartMap();

        // 중간 부분 맵을 생성
        if (rootFolder != null)
        {
            TreeNodeData rootNode = TreeTemplete.Find(node => node.ParentId == null);
            CreateMiddleMaps(rootFolder, rootNode, Vector3.zero);
        }

        // Step 4: Connect Portals
        ConnectPortals(rootFolder);

        // Step 5 : Generate SpecialMap
        GenerateHiddenFolderList();

        //Debug.Log("Map generation complete.");
    }

    #region FolderNode

    // 트리에 따라 실제로 시작 맵을 생성시키는 함수
    private FolderNode CreateStartMap()
    {
        // 1. 루트 노드 식별
        TreeNodeData rootNode = TreeTemplete.Find(node => node.ParentId == null);
        if (rootNode == null)
        {
            Debug.LogError("Root node not found in TreeTemplete.");
            return null;
        }

        // 2. 자식 개수에 맞는 시작 맵 프리팹 선택
        int childCount = rootNode.ChildCount;
        GameObject startPrefab = null;

        switch (childCount)
        {
            case 1:
                startPrefab = StartFolderPrefabs1[UnityEngine.Random.Range(0, StartFolderPrefabs1.Count)];
                break;
            case 2:
                startPrefab = StartFolderPrefabs2[UnityEngine.Random.Range(0, StartFolderPrefabs2.Count)];
                break;
            case 3:
                startPrefab = StartFolderPrefabs3[UnityEngine.Random.Range(0, StartFolderPrefabs3.Count)];
                break;
            default:
                Debug.LogError($"No start prefab found for child count: {childCount}");
                break;
        }

        if (startPrefab == null)
        {
            Debug.LogError("startPrefab is null.");
            return null;
        }

        // 3. 시작 맵 생성
        GameObject startMap = Instantiate(startPrefab, Vector3.zero, Quaternion.identity);
        startMap.transform.SetParent(this.transform);

        // 4. FolderNode 스크립트 찾기
        FolderNode folderNode = startMap.GetComponent<FolderNode>();

        if (folderNode == null)
        {
            Debug.LogError("The selected start map prefab does not contain a FolderNode component.");
            return null;
        }

        // FolderNode가 성공적으로 찾은 경우
        // Debug.Log("FolderNode component found successfully.");

        return folderNode;
    }

    // 트리에 따라 실제로 중간, 엔드, 보스 방을 생성시키는 함수
    private void CreateMiddleMaps(FolderNode parentFolder, TreeNodeData parentNode, Vector3 parentPosition)
    {
        // 1. 자식 노드 탐색
        foreach (int childId in parentNode.Children)
        {
            // 자식 노드 데이터 가져오기
            TreeNodeData childNode = TreeTemplete.Find(node => node.Id == childId);
            if (childNode == null)
            {
                Debug.LogError($"Child node with ID {childId} not found in TreeTemplete.");
                continue;
            }

            // 자식의 자식 노드 수 확인
            int grandChildCount = childNode.ChildCount;

            // 2. 프리팹 선택
            GameObject middlePrefab = GetPrefabForFolderType(childNode.Type, grandChildCount);
            if (middlePrefab == null)
            {
                Debug.LogError($"No prefab found for FolderType {childNode.Type}.");
                continue;
            }

            // 3. 위치 계산
            Vector3 childPosition = parentPosition + new Vector3(HorizontalSpacing, 0, 0);

            // 4. 맵 생성
            GameObject middleMap = Instantiate(middlePrefab, childPosition, Quaternion.identity);
            middleMap.transform.SetParent(this.transform);

            // 5. FolderNode 설정
            FolderNode folderNode = middleMap.GetComponent<FolderNode>();
            if (folderNode == null)
            {
                Debug.LogError("The selected map prefab does not contain a FolderNode component.");
                continue;
            }

            folderNode.Children = new List<FolderNode>();

            // 부모와 자식 관계 설정
            parentFolder.AddChild(folderNode);

            // 6. 재귀 호출 (다음 레벨 생성)
            CreateMiddleMaps(folderNode, childNode, childPosition);
        }
    }

    public FolderNode GetRootNode() { return rootFolder; }

    //private FolderNode InstantiateNode(FolderNode node, Vector2 position, FolderNode parent, GameObject customPrefab = null)
    //{
    //    // 선택된 프리팹 사용
    //    GameObject prefab = customPrefab ?? GetPrefabForFolderType(node.Type);
    //    GameObject folderObject = Instantiate(prefab, position, Quaternion.identity);
    //    FolderNode instantiatedNode = folderObject.GetComponent<FolderNode>();

    //    instantiatedNode.Type = node.Type;
    //    instantiatedNode.FolderName = node.FolderName;

    //    if (parent != null)
    //    {
    //        parent.AddChild(instantiatedNode);
    //    }

    //    spawnedFolders.Add(instantiatedNode);

    //    float childOffset = -VerticalSpacing;
    //    foreach (var child in node.Children)
    //    {
    //        Vector2 childPosition = position + new Vector2(HorizontalSpacing, childOffset);
    //        InstantiateNode(child, childPosition, instantiatedNode);
    //        childOffset -= VerticalSpacing;
    //    }

    //    return instantiatedNode;
    //}

    private GameObject GetPrefabForFolderType(FolderType type, int childCount = 0)
    {
        switch (type)
        {
            case FolderType.Middle:
                if (childCount == 1)
                    return NormalFolderPrefabs1[UnityEngine.Random.Range(0, NormalFolderPrefabs1.Count)];
                else if (childCount == 2)
                    return NormalFolderPrefabs2[UnityEngine.Random.Range(0, NormalFolderPrefabs2.Count)];
                else if (childCount == 3)
                    return NormalFolderPrefabs3[UnityEngine.Random.Range(0, NormalFolderPrefabs3.Count)];
                else
                    return null;
            case FolderType.Download:
                return DownloadFolderPrefabs[UnityEngine.Random.Range(0, DownloadFolderPrefabs.Count)];

            case FolderType.Shop:
                return ShopFolderPrefabs[UnityEngine.Random.Range(0, ShopFolderPrefabs.Count)];

            case FolderType.Boss:
                return BossFolderPrefabs[UnityEngine.Random.Range(0, BossFolderPrefabs.Count)];

            case FolderType.End:
                return EndFolderPrefabs[UnityEngine.Random.Range(0, EndFolderPrefabs.Count)];

            case FolderType.RandomSpecial:
                if (SpecialFolderPrefabs.Count == 0) // 리스트 비었을 경우 예외처리
                {
                    Debug.LogError("SpecialFolderPrefabs list is empty!");
                    return null;
                }

                // 랜덤 선택 후 중복 방지를 위해 리스트에서 제거
                int randomIndex = UnityEngine.Random.Range(0, SpecialFolderPrefabs.Count);
                GameObject specialPrefab = SpecialFolderPrefabs[randomIndex];
                SpecialFolderPrefabs.RemoveAt(randomIndex);
                return specialPrefab;

            default:
                return null;
        }
    }

    private void GenerateHiddenFolderList()
    {
        foreach(var prefab in HiddenFolderPrefabs)
        {
            GameObject newmap = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newmap.transform.SetParent(this.transform);
            hiddenFolderList.Add(newmap);
        }
        GameObject squaloMarket = Instantiate(SqualoMarketPrefab, Vector3.zero, Quaternion.identity);
        squaloMarket.SetActive(false);
        squaloMarket.transform.SetParent(this.transform);
        hiddenFolderList.Add(squaloMarket);
    }


    #endregion

    #region Portal

    // 실제로 포탈을 연결해주는 함수
    private void ConnectPortals(FolderNode node)
    {
        if (node == null) return;

        if (node.Portals == null)
        {
            Debug.LogError($"Node {node.name} has null Portals");
            return;
        }

        if (node.Children == null)
        {
            Debug.LogError($"Node {node.name} has null Children");
            return;
        }

        for (int i = 0; i < node.Children.Count; i++)
        {
            if (node.Portals[i] == null)
            {
                Debug.LogError($"Node {node.name} has null Portal at index {i}");
                continue;
            }

            if (node.Children[i] == null)
            {
                Debug.LogError($"Node {node.name} has null Child at index {i}");
                continue;
            }

            FolderNode childNode = node.Children[i];
            Portal rightPortal = node.Portals[i];


            // 연결된 자식 폴더 타입에 따라 포탈 프리펩 변경
            if (childNode.Type != FolderType.Middle)
            {
                GameObject newPortalPrefab = GetPortalPrefabsForFolderType(childNode);
                if (newPortalPrefab != null)
                {
                    // 기존 포탈 삭제
                    Transform parentTransform = rightPortal.transform.parent;
                    Vector3 oldPortalPosition = rightPortal.transform.localPosition;
                    Quaternion oldPortalRotation = rightPortal.transform.localRotation;

                    Destroy(rightPortal.gameObject);

                    // 새 포탈 생성
                    GameObject newPortal = Instantiate(newPortalPrefab, parentTransform);
                    newPortal.transform.localPosition = oldPortalPosition;
                    newPortal.transform.localRotation = oldPortalRotation;

                    // 새로 생성된 포탈의 Portal 컴포넌트 참조 설정
                    node.Portals[i] = newPortal.GetComponent<Portal>();
                    if (node.Portals[i] != null)
                    {
                        node.Portals[i].PortalIndex = i;
                        node.Portals[i].SetConnectedFolder(childNode, 0);
                    }
                }
            }

            // 현재 맵 -> 자식 맵 포탈 이어주기
            rightPortal.SetConnectedFolder(childNode, 0);

            // 자식 맵 -> 현재 맵 포탈 이어주기
            Portal leftPortal = childNode.Left_Portal;
            leftPortal.SetConnectedFolder(node, i);
            // node.Children[i].Left_Portal.SetConnectedFolder(node, i);
        }

        // 재귀적으로 자식 노드의 포탈 연결
        foreach (FolderNode child in node.Children)
        {
            ConnectPortals(child);
        }
    }

    // 연결된 폴더에 따른 포탈 프리펩을 반환해주는 함수
    private GameObject GetPortalPrefabsForFolderType(FolderNode node)
    {
        switch (node.Type)
        {
            case FolderType.Boss:
                return BossPortal;
            case FolderType.Shop:
                return ShopPortal;
            case FolderType.Download:
                return DownloadPortal;
            case FolderType.RandomSpecial:
                string name = node.name;
                if (name == "Charge_room(Clone)")
                    return Charge_room;
                else if (name == "Guard_room(Clone)")
                    return Guard_room;
                else if (name == "Juva_cafe(Clone)")
                    return Juva_cafe;
                else if (name == "Trash_room(Clone)")
                    return Trash_room;
                else
                {
                    Debug.LogError($"Unknown Room Type: {name}");
                    return null;
                }
            default:
                return null; // 기본 포탈 이미지
        }
    }

    #endregion

    #region FolderNodeList Base Generator

    // 트리 구조체 정보를 담고있는 클래스
    public class TreeNodeData
    {
        public int Id { get; set; }               // 고유 ID
        public int? ParentId { get; set; }        // 부모 ID (루트 노드는 null)
        public int ChildCount { get; set; }       // 자식 개수
        public List<int> Children { get; set; }   // 자식 노드 ID 목록
        public FolderType Type { get; set; } // 노드 타입
        public bool IsLeaf { get; set; }          // 리프 노드 여부
        public int Depth { get; set; }            // 트리 깊이

        public TreeNodeData(int id, int? parentId, FolderType type, int depth)
        {
            Id = id;
            ParentId = parentId;
            Type = type;
            Depth = depth;
            Children = new List<int>();
            ChildCount = 0;
            IsLeaf = true; // 초기화 시 기본적으로 리프 노드로 설정
        }
    }

    private List<TreeNodeData> GenerateTreeData(int nodeCount)
    {
        System.Random random = new System.Random();

        var nodes = new List<TreeNodeData>();
        int nextId = 1;
        int leafCount = 0;

        // 1. 루트 노드 생성 및 초기화
        var root = new TreeNodeData(nextId++, null, FolderType.Start, 0);
        nodes.Add(root);
        // Debug.Log($"Root Node Created - ID: {root.Id}, ChildCount: {root.ChildCount}");

        var validParents = new List<TreeNodeData> { root };

        // 2.트리 생성 루프
        while (nextId <= nodeCount || leafCount < 5)
        {
            // 부모 노드 선택
            if (validParents.Count == 0)
            {
                Debug.LogWarning("No valid parents available. Tree generation terminated early.");
                break;
            }

            // 부모 노드 선택
            var parent = validParents[random.Next(validParents.Count)];

            //int maxChildren = Mathf.Min(3, nodeCount - nextId);
            int maxChildren = Mathf.Min(3 - parent.ChildCount, nodeCount - nextId);

            int childCount = random.Next(1, maxChildren + 1);

            // Debug.Log($"Parent Node ID: {parent.Id}, Adding {childCount} Children");

            for (int i = 0; i < childCount && nextId <= nodeCount; i++)
            {
                if (parent.ChildCount >= 3) break; // 자식이 최대 개수를 초과하면 중단

                // 자식 노드 생성
                var child = new TreeNodeData(nextId++, parent.Id, FolderType.Middle, parent.Depth + 1);
                parent.Children.Add(child.Id);
                parent.ChildCount++;
                parent.IsLeaf = false; // 부모가 되었으므로 리프 노드가 아님

                nodes.Add(child);
                validParents.Add(child);

                // Debug.Log($"Child Node Created - ID: {child.Id}, Parent ID: {parent.Id}");
            }

            // 부모가 최대 자식을 가졌다면 유효 부모 리스트에서 제거
            if (parent.ChildCount >= 3)
            {
                validParents.Remove(parent);
                // Debug.Log($"Parent Node ID: {parent.Id} removed from validParents (Max Children Reached)");
            }

            // 리프 노드 개수 보장
            if (nextId > nodeCount && leafCount < 5)
            {
                foreach (var parentNode in validParents)
                {
                    var extraLeaf = new TreeNodeData(nextId++, parentNode.Id, FolderType.Hidden, parentNode.Depth + 1);
                    parentNode.Children.Add(extraLeaf.Id);
                    parentNode.ChildCount++;
                    nodes.Add(extraLeaf);
                    leafCount++;

                    // Debug.Log($"Extra Leaf Created - ID: {extraLeaf.Id}, Parent ID: {parentNode.Id}");

                    if (leafCount >= 5) break;
                }
            }
        }

        // 3. 노드 타입 재설정
        TreeNodeData bossNode = null;
        int maxDepth = -1; // 가장 깊은 리프 노드를 찾기 위한 변수
        List<TreeNodeData> leafNodes = new List<TreeNodeData>();

        foreach (var node in nodes)
        {
            if (node.Children.Count == 0 && node.ParentId != null) // 리프 노드
            {
                leafNodes.Add(node);
                node.Type = FolderType.End;

                // 가장 깊은 리프 노드 찾기
                if (node.Depth > maxDepth)
                {
                    maxDepth = node.Depth;
                    bossNode = node;
                }
            }
            else if (node.ParentId != null) // 중간 노드
            {
                node.Type = FolderType.Middle;
            }
        }

        // 4. 가장 깊은 리프 노드 타입을 Boss로 설정
        if (bossNode != null)
        {
            bossNode.Type = FolderType.Boss;
            leafNodes.Remove(bossNode);
            // Debug.Log($"Boss Node Assigned - ID: {bossNode.Id}, Depth: {bossNode.Depth}");
        }

        // 5. 특수 타입 방 지정
        if (leafNodes.Count >= 4)
        {
            // 다운로드 1개
            var downloadNode = leafNodes[random.Next(leafNodes.Count)];
            downloadNode.Type = FolderType.Download;
            leafNodes.Remove(downloadNode);
            // Debug.Log($"Download Node Assigned - ID: {downloadNode.Id}");

            // 상점 1개
            var storeNode = leafNodes[random.Next(leafNodes.Count)];
            storeNode.Type = FolderType.Shop;
            leafNodes.Remove(storeNode);
            // Debug.Log($"Store Node Assigned - ID: {storeNode.Id}");

            // 스페셜 2개
            for (int i = 0; i < 2; i++)
            {
                var specialNode = leafNodes[random.Next(leafNodes.Count)];
                specialNode.Type = FolderType.RandomSpecial;
                leafNodes.Remove(specialNode);
                // Debug.Log($"Special Node Assigned - ID: {specialNode.Id}");
            }
        }

        // 6. 디버깅: 최종 트리 구조 출력
        //foreach (var node in nodes)
        //{
        //    Debug.Log($"Node ID: {node.Id}, Parent ID: {node.ParentId}, ChildCount: {node.ChildCount}, Depth: {node.Depth}, Type: {node.Type}");
        //}

        return nodes;
    }

    #endregion
}
