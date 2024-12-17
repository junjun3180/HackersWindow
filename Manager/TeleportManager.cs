//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TeleportManager : MonoBehaviour
//{
//    private static TeleportManager instance = null;
//    CameraManager cameraManager;
//    GameManager gameManager;
//    [SerializeField]
//    MapGenerator mapGenerator;
//    [Header("텔레포트 되는거리")]
//    [SerializeField]
//    float playerTelDisX;
//    [SerializeField]
//    float playerTelDisY;
//    private void Awake()
//    {
//        if (null == instance)
//        {
//            instance = this;
//            DontDestroyOnLoad(this.gameObject);
//        }
//        else
//        {
//            Destroy(this.gameObject);
//        }
//    }
//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.H))
//        {
//            HiddenTel();
//        }
//    }
//    public static TeleportManager Instance
//    {
//        get
//        {
//            if (null == instance)
//            {
//                return null;
//            }
//            return instance;
//        }
//    }
//    // Start is called before the first frame update
//    void Start()
//    {
//        cameraManager = CameraManager.Instance;
//        gameManager = GameManager.Instance;
//    }

    
//    public void PlayerTeleport(Player player,Portal currentPortal,Portal connectPortal)
//    {

//        // 부모 객체 (Map) 확인
//        Transform currentMap = currentPortal.transform.parent;
//        Transform connectMap = connectPortal.transform.parent;

//        // MapGenerator의 자식 객체로부터 index 확인
//        int currentMapIndex = currentMap.GetSiblingIndex();       // 현재 맵의 인덱스
//        int connectMapIndex = connectMap.GetSiblingIndex();       // 연결될 맵의 인덱스

//        // 기본 이동 위치 (connectPortal 위치)
//        Vector2 newPosition = connectPortal.transform.position;

//        // 인덱스를 비교하여 이동 방향 조정
//        if (connectMapIndex < currentMapIndex)
//        {
//            // connectMap의 인덱스가 더 낮음 → 왼쪽으로 이동
//            newPosition.x -= playerTelDisX; // 필요에 따라 값을 조정
//            newPosition.y += playerTelDisY;
//        }
//        else if (connectMapIndex > currentMapIndex)
//        {
//            // connectMap의 인덱스가 더 높음 → 오른쪽으로 이동
//            newPosition.x += playerTelDisX; // 필요에 따라 값을 조정
//            newPosition.y += playerTelDisY;

//        }

//        // 플레이어 이동
//        player.transform.position = newPosition;

//        // 포탈 사용 가능 여부 설정
//        // currentPortal.isUse = false;
//        // connectPortal.isUse = false;
//        if (connectMap.GetComponent<Map>().Type == Map.MapType.RandomSpecial || 
//            connectMap.GetComponent<Map>().Type == Map.MapType.Shop || 
//            connectMap.GetComponent<Map>().Type == Map.MapType.Download)
//        {
//            cameraManager.SpecialMapCamera(connectMap.GetComponent<Map>());
//        }
//    }
//    public void MapTeleportPortal(GameObject moveMap,bool isRightMove,GameObject currentMap)
//    {
//        BoxCollider2D collider = moveMap.GetComponent<BoxCollider2D>();
//        if (collider != null)
//        {
            
//            float distance = collider.size.x;
//            distance *= 1.5f;
//            if (isRightMove)
//            {

//                moveMap.transform.position = new Vector2(currentMap.transform.position.x, currentMap.transform.position.y);
//                moveMap.transform.position = new Vector2(moveMap.transform.position.x + distance, moveMap.transform.position.y);
//            }
//            else
//            {
//                moveMap.transform.position = new Vector2(currentMap.transform.position.x, currentMap.transform.position.y);
//                moveMap.transform.position = new Vector2(moveMap.transform.position.x - distance, moveMap.transform.position.y);
//            }
//        }
//        moveMap.SetActive(true);
//    }
//    private IEnumerator PlayerTeleport_Co(Portal currentPortal)
//    {
        
//        yield return new WaitForSeconds(1f);  // 1초 후 다시 포탈 활성화
//        // currentPortal.isUse = true;

//    }
//    public void LocalDisckTel(GameObject telMap)
//    {
//        int currentMapIndex =0;
//        for (int i = 0; i < mapGenerator.mapList.Count; i++)
//        {
//            Map map = mapGenerator.mapList[i];

//            // 현재 활성화된 맵인지 확인
//            if (map.transform.gameObject.activeSelf)
//            {
//                currentMapIndex = i;
//                continue; // 활성화된 맵의 인덱스 반환
//            }
//        }
//        int telMapIndex = mapGenerator.mapList.IndexOf(telMap.GetComponent<Map>());
//        if(currentMapIndex != telMapIndex)
//        {
//            mapGenerator.mapList[currentMapIndex].gameObject.SetActive(false);
//            telMap.SetActive(true);
//            Vector2 telPoint = telMap.transform.Find("TeleportPoint").transform.position;
//            gameManager.player.transform.position = telPoint;
//            cameraManager.CameraLimit(telMap);
//        }
//    }
//    public void PortalUse_co(Portal currentPortal)
//    {
//        StartCoroutine(PlayerTeleport_Co(currentPortal));
//    }
//    public void StartPlayerTel()
//    {
//        gameManager.player.transform.position = mapGenerator.mapList[0].transform.Find("TeleportPoint").transform.position;
//    }
//    public void HiddenTel()
//    {
//        int currentMapIndex = 0;
//        for (int i = 0; i < mapGenerator.mapList.Count; i++)
//        {
//            Map map = mapGenerator.mapList[i];

//            // 현재 활성화된 맵인지 확인
//            if (map.transform.gameObject.activeSelf)
//            {
//                currentMapIndex = i;
//                continue; // 활성화된 맵의 인덱스 반환
//            }
//        }
//        mapGenerator.mapList[currentMapIndex].gameObject.SetActive(false);

//        // 랜덤으로 히든 맵 선택
//        int hiddenIndex = Random.Range(0, mapGenerator.hiddenMap.transform.childCount);
//        GameObject hiddenMap = mapGenerator.hiddenMap.transform.GetChild(hiddenIndex).gameObject;
//        Debug.Log(hiddenMap);
//        // 선택한 히든 맵 활성화
//        hiddenMap.SetActive(true);

//        // 플레이어의 텔레포트 위치 설정
//        Vector2 telPoint = hiddenMap.transform.Find("TeleportPoint").transform.position;
//        gameManager.player.transform.position = telPoint;

//        // 카메라 제한 설정
//        cameraManager.CameraLimit(hiddenMap);
//    }
//}
