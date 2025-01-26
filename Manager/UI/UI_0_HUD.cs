using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_0_HUD : MonoBehaviour
{
    #region Manager

    private static UI_0_HUD instance = null;

    // UI Window
    public GameObject UI_W_HUD = null;

    private StatusManager statusManager = null;
    private FolderManager folderManager = null;
    private ItemManager itemManager = null;

    #endregion

    #region HP

    [SerializeField]
    private List<GameObject> hpPrefabsList;
    public List<GameObject> hpList = new();
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private int interval;
    private int hpNum = 0;

    #endregion

    #region Left Under Inforamtion

    // Left side text
    public Text KeyCount;
    public Text CoinCount;
    public Text BombCount;
    public Text MonsterCount;

    public GameObject HUDGroup;
    public GameObject MiniMap;

    #endregion

    #region Default Function
    public static UI_0_HUD Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_0_HUD>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UI_0_HUD).Name);
                    instance = singletonObject.AddComponent<UI_0_HUD>();
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
        statusManager = StatusManager.Instance;
        folderManager = FolderManager.Instance;
        itemManager = ItemManager.Instance;
        HpBarSet();
        UpdateHpUI();
    }

    #endregion

    #region Open/Close UI

    public void OpenUI()
    {
        if (UI_W_HUD != null)
        {
            UI_W_HUD.SetActive(true);
            HUDGroup.SetActive(true);
            MiniMap.SetActive(true);
            foreach (GameObject HPList in hpList)
            {
                HPList.SetActive(true);
            }
        }
    }

    public void CloseUI()
    {
        if (UI_W_HUD != null)
        {
            UI_W_HUD.SetActive(false);
            HUDGroup.SetActive(false);
            MiniMap.SetActive(false);
            foreach (GameObject HPList in hpList)
            {
                HPList.SetActive(false);
            }
        }
    }

    #endregion

    #region Update HUD

    public void UpdateHUD()
    {
        if (folderManager == null || itemManager == null)
        {
            Debug.Log("UpdateHUD find null");
            return;
        }

        KeyCount.text = itemManager.GetKeyCount().ToString();
        CoinCount.text = itemManager.GetCoinCount().ToString();
        BombCount.text = itemManager.GetBombCount().ToString();
        MonsterCount.text = folderManager.CurrentFolder.GetMonsterCount().ToString();
    }

    #endregion

    #region HP UI

    // HP UI를 초기화해주는 함수 - 시작할때만 사용
    public void HpBarSet()
    {
        foreach (var hp in hpList)
        {
            Destroy(hp);
        }
        hpList.Clear();
        hpNum = 0;

        if (hpPrefabsList.Count < 4)
        {
            Debug.LogError("hpPrefabsList에 필요한 프리팹이 부족합니다.");
            return;
        }

        // 플레이어의 체력 상황에따라 체력바 재생성
        // 최적화 코드
        // 체력 관련 데이터를 배열로 정의 - 새로운거 추가되면 여기에 추가해야 함.
        int HPTotal = ((int)statusManager.MaxHp / 3) + ((int)statusManager.MaxHp % 3 > 0 ? 1 : 0);
        int TempTotal = ((int)statusManager.TemHp / 3) + ((int)statusManager.TemHp % 3 > 0 ? 1 : 0);
        var hpData = new (int count, GameObject prefab)[]
        {
        ( HPTotal, hpPrefabsList[0]), // 일반 체력
        ( TempTotal, hpPrefabsList[1]), // 임시 체력
        ( (int)statusManager.Elect, hpPrefabsList[2]),     // 전기
        ( (int)statusManager.Shield, hpPrefabsList[3])   // 쉴드
        };

        // 체력바 생성
        foreach (var (count, prefab) in hpData)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject newHp = Instantiate(prefab, canvas.transform);
                RectTransform rectTransform = newHp.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // 위치 조정
                hpList.Add(newHp);
                hpNum++;
            }
        }

    }

    // HP를 제외하고 나머지를 초기화해주는 함수
    public void ExceptHp_BarSet()
    {
        for (int i = hpList.Count - 1; i >= 0; i--)
        {
            GameObject hp = hpList[i];
            if (hp.name != "R_Heart(Clone)")
            {
                hpList.RemoveAt(i);
                Destroy(hp);
                hpNum--;
            }
        }

        if (hpPrefabsList.Count < 4)
        {
            Debug.LogError("hpPrefabsList에 필요한 프리팹이 부족합니다.");
            return;
        }

        // hp를 제외한 나머지 체력 생성
        int TempTotal = ((int)statusManager.TemHp / 3) + ((int)statusManager.TemHp % 3 > 0 ? 1 : 0);
        var hpData = new (int count, GameObject prefab)[]
        {
        ( TempTotal, hpPrefabsList[1]), // 임시 체력
        ( (int)statusManager.Elect, hpPrefabsList[2]),     // 전기
        ( (int)statusManager.Shield, hpPrefabsList[3])   // 쉴드
        };

        // 체력바 생성
        foreach (var (count, prefab) in hpData)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject newHp = Instantiate(prefab, canvas.transform);
                RectTransform rectTransform = newHp.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // 위치 조정
                hpList.Add(newHp);
                hpNum++;
            }
        }

    }

    // HP UI를 최신화해주는 함수
    public void UpdateHpUI()
    {
        // 현재 체력 UI의 상태를 업데이트
        for (int i = 0; i < hpList.Count; i++)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                Transform hpSegment = hpList[i].transform;
                hpSegment.GetChild(1).gameObject.SetActive(false); // 첫 칸
                hpSegment.GetChild(2).gameObject.SetActive(false); // 두 번째 칸
                hpSegment.GetChild(3).gameObject.SetActive(false); // 세 번째 칸
            }
            if (hpList[i].name == "RDis_Heart(Clone)")
            {
                Transform hpSegment = hpList[i].transform;
                hpSegment.GetChild(0).gameObject.SetActive(false); // 첫 칸
                hpSegment.GetChild(1).gameObject.SetActive(false); // 두 번째 칸
                hpSegment.GetChild(2).gameObject.SetActive(false); // 세 번째 칸
            }
        }

        // 체력을 3씩 나눠 각 세그먼트를 활성화
        int remainingHp = (int)statusManager.CurrentHp;
        for (int i = 0; i < hpList.Count && remainingHp > 0; i++)
        {
            // 기본 체력에 경우에만 처리
            if (hpList[i].name != "R_Heart(Clone)")
                continue;

            Transform hpSegment = hpList[i].transform;

            if (remainingHp >= 3)
            {
                hpSegment.GetChild(1).gameObject.SetActive(true);
                hpSegment.GetChild(2).gameObject.SetActive(true);
                hpSegment.GetChild(3).gameObject.SetActive(true);
                remainingHp -= 3;
            }
            else if (remainingHp == 2)
            {
                hpSegment.GetChild(1).gameObject.SetActive(true);
                hpSegment.GetChild(2).gameObject.SetActive(true);
                remainingHp -= 2;
            }
            else if (remainingHp == 1)
            {
                hpSegment.GetChild(1).gameObject.SetActive(true);
                remainingHp -= 1;
            }
        }

        // 임시 체력을 3씩 나눠 각 세그먼트를 활성화
        int remainingTemp = (int)statusManager.TemHp;
        for (int i = 0; i < hpList.Count && remainingTemp > 0; i++)
        {
            // 임시체력의 경우에만 처리
            if (hpList[i].name != "RDis_Heart(Clone)")
                continue;

            Transform hpSegment = hpList[i].transform;

            if (remainingTemp >= 3)
            {
                hpSegment.GetChild(0).gameObject.SetActive(true);
                hpSegment.GetChild(1).gameObject.SetActive(true);
                hpSegment.GetChild(2).gameObject.SetActive(true);
                remainingTemp -= 3;
            }
            else if (remainingTemp == 2)
            {
                hpSegment.GetChild(0).gameObject.SetActive(true);
                hpSegment.GetChild(1).gameObject.SetActive(true);
                remainingTemp -= 2;
            }
            else if (remainingTemp == 1)
            {
                hpSegment.GetChild(0).gameObject.SetActive(true);
                remainingTemp -= 1;
            }
        }
    }

    // ShiledHP를 먹은 상태고 히트 상태가 될 때 HP에 Shiled를 씌워주는 함수
    public void ShiledSet()
    {
        // ShiledHP는 가장 오른쪽(리스트의 끝)에 있음.
        int index = hpNum - 1;
        if (index >= 0 && hpList[index].name == "Shield_Heart(Clone)")
        {
            GameObject removeHp = hpList[index];
            hpList.RemoveAt(index);
            Destroy(removeHp);
            hpNum--;
            SetRandomShiledNum();
            ShiledOn();
            return;
        }
    }

    // 쉴드가 씌워질 개수를 설정해주는 함수
    private void SetRandomShiledNum()
    {
        int RandomShiledNum = UnityEngine.Random.Range(1, (int)statusManager.CurrentHp / 3 + 1);

        Debug.Log($"배터리 개수 : {RandomShiledNum}");
        statusManager.ShieldHp = RandomShiledNum;
    }

    // HP에 Shiled를 씌워주는 함수
    private void ShiledOn()
    {
        int remainingShields = (int)statusManager.ShieldHp; // 활성화할 쉴드 개수
        int currentHp = (int)statusManager.CurrentHp;       // 현재 HP

        int ActiveShield = currentHp / 3 - 1 + (currentHp % 3 != 0 ? 1 : 0);
        // ActiveShield = ActiveShield >= 
        // 오른쪽부터 쉴드 적용
        for (int i = ActiveShield; i >= 0 && remainingShields > 0; i--)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                Transform segments = hpList[i].transform;
                if (!hpList[i].transform.GetChild(0).gameObject.activeSelf)
                {
                    hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                    remainingShields--;
                }
                else
                    continue;
            }
        }
    }

    // ShiledHP와 일반(빨간)HP를 처리하는 함수
    public void DamagedHP()
    {
        for (int i = hpList.Count - 1; i >= 0; i--)
        {
            if (hpList[i].name == "R_Heart(Clone)" && hpList[i].transform.GetChild(1).gameObject.activeSelf)
            {
                Transform segments = hpList[i].transform;

                if (segments.GetChild(0).gameObject.activeSelf)
                {
                    Debug.Log("쉴드 소모");
                    statusManager.ShieldHp--;

                    segments.GetChild(0).gameObject.SetActive(false); // 쉴드 감소

                    return;
                }
                else
                {
                    Debug.Log("일반 체력 소모");
                    statusManager.CurrentHp--;

                    if (segments.GetChild(3).gameObject.activeSelf)
                        segments.transform.GetChild(3).gameObject.SetActive(false);
                    else if(segments.GetChild(2).gameObject.activeSelf)
                        segments.transform.GetChild(2).gameObject.SetActive(false);
                    else if (segments.GetChild(1).gameObject.activeSelf)
                        segments.transform.GetChild(1).gameObject.SetActive(false);
                    
                    return;

                }
            }
        }
    }

    #endregion
}
