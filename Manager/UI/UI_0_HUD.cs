using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_0_HUD : MonoBehaviour
{
    private static UI_0_HUD instance = null;

    // UI Window
    public GameObject UI_W_HUD = null;

    // Detail
    // HP
    [SerializeField]
    private List<GameObject> hpPrefabsList;
    private List<GameObject> hpList = new();
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private int interval;
    
    private int hpNum = 0;

    // Left side text
    public Text KeyCount;
    public Text CoinCount;
    public Text BombCount;
    public Text MonsterCount;

    public GameObject HUDGroup;
    public GameObject MiniMap;

    // Manager
    private StatusManager statusManager = null;
    private FolderManager folderManager = null;
    private ItemManager itemManager = null;
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
    }

    // Update is called once per frame
    void Update()
    {

    }

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

    // HP

    // HP Update Func
    public void HpBarSet()
    {
        //hp 체력바 리셋
        if (hpList.Count > 0)
        {
            for (int i = hpList.Count - 1; i >= 0; i--)
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
            }
            hpNum = 0;
        }

        if (hpPrefabsList.Count < 4)
        {
            Debug.LogError("hpPrefabsList에 필요한 프리팹이 부족합니다.");
            return;
        }

        //플레이어의 체력 상황에따라 체력바 재생성
        if (statusManager.MaxHp > 0)
        {
            //최대체력 3당 체력베터리 1개 생성후 리스트에 추가
            for (int i = 0; i < statusManager.MaxHp / 3; i++)
            {
                GameObject newHp = Instantiate(hpPrefabsList[0], canvas.transform);
                newHp.transform.SetParent(canvas.transform, false);
                RectTransform rectTransform = newHp.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(i * interval, 0); // 위치 조정 (임의로 설정)
                hpList.Add(newHp);
                hpNum += 1;
            }
            //임시체력 3당 임시체력베터리 1개 생성후 리스트에 추가
            if (statusManager.TemHp > 0)
            {
                for (int i = 0; i < statusManager.TemHp / 3; i++)
                {
                    GameObject newTemHp = Instantiate(hpPrefabsList[1], canvas.transform);
                    newTemHp.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = newTemHp.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // 위치 조정 (임의로 설정)
                    hpList.Add(newTemHp);
                    hpNum += 1;

                }
            }
            //전기1 전기베터리 1개 생성후 리스트에 추가

            if (statusManager.Elect > 0)
            {
                for (int i = 0; i < statusManager.Elect; i++)
                {
                    GameObject spark = Instantiate(hpPrefabsList[2], canvas.transform);
                    spark.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = spark.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // 위치 조정 (임의로 설정)
                    hpList.Add(spark);
                    hpNum += 1;

                }
            }
            //쉴드체력1당 체력베터리 1개 생성후 리스트에 추가
            if (statusManager.ShieldHp > 0)
            {
                for (int i = 0; i < statusManager.ShieldHp; i++)
                {
                    GameObject newShildHp = Instantiate(hpPrefabsList[3], canvas.transform);
                    newShildHp.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = newShildHp.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // 위치 조정 (임의로 설정)
                    hpList.Add(newShildHp);
                    hpNum += 1;

                }
            }
        }
    }
    public void HpBarPlus()
    {

    }
    public void ShiledSet()
    {
        //쉴드체력이 소모될때 쉴드체력을 삭제
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "Shield_Heart(Clone)")
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
                hpNum--;
                ShiledOn();
                return;
            }
        }
    }
    private void ShiledOn()
    {
        //Hp체력바의 쉴드를 활성화
        for (int i = 0; i < hpNum; i++)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                // 첫 번째 자식을 가져와 활성화
                if (hpList[i].transform.childCount > 0)
                {
                    hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }
    public void ShiledOff()
    {
        //hp체력바의 쉴드를 비활성화
        for (int i = hpNum - 1; i >= statusManager.Shield; i--)
        {
            if (hpList[i].name == "R_Heart(Clone)" && hpList[i].transform.GetChild(0).gameObject.activeSelf)
            {
                hpList[i].transform.GetChild(0).gameObject.SetActive(false);
                return;
            }
        }
    }
    public void HpSet()
    {
        if (statusManager.CurrentHp <= 3)
        {
            switch (statusManager.CurrentHp)
            {
                case 1:
                    hpList[0].transform.GetChild(1).gameObject.SetActive(true);
                    hpList[0].transform.GetChild(2).gameObject.SetActive(false);
                    hpList[0].transform.GetChild(3).gameObject.SetActive(false);
                    return;
                case 2:
                    hpList[0].transform.GetChild(1).gameObject.SetActive(true);
                    hpList[0].transform.GetChild(2).gameObject.SetActive(true);
                    hpList[0].transform.GetChild(3).gameObject.SetActive(false);
                    return;
                case 3:
                    hpList[0].transform.GetChild(1).gameObject.SetActive(true);
                    hpList[1].transform.GetChild(1).gameObject.SetActive(false);
                    hpList[0].transform.GetChild(2).gameObject.SetActive(true);
                    hpList[0].transform.GetChild(3).gameObject.SetActive(true);
                    return;
            }
        }
        switch (statusManager.CurrentHp % 3)
        {
            case 0:
                for (int i = 0; i < statusManager.MaxHp / 3; i++)
                {
                    hpList[i].transform.GetChild(1).gameObject.SetActive(false);
                    hpList[i].transform.GetChild(2).gameObject.SetActive(false);
                    hpList[i].transform.GetChild(3).gameObject.SetActive(false);
                }
                for (int i = 0; i < statusManager.CurrentHp / 3; i++)
                {
                    hpList[i].transform.GetChild(1).gameObject.SetActive(true);
                    hpList[i].transform.GetChild(2).gameObject.SetActive(true);
                    hpList[i].transform.GetChild(3).gameObject.SetActive(true);
                }
                break;
            case 1:
                hpList[((int)statusManager.CurrentHp / 3)].transform.GetChild(1).gameObject.SetActive(true);
                hpList[((int)statusManager.CurrentHp / 3)].transform.GetChild(2).gameObject.SetActive(false);
                hpList[((int)statusManager.CurrentHp / 3)].transform.GetChild(3).gameObject.SetActive(false);

                break;
            case 2:
                hpList[((int)statusManager.CurrentHp / 3)].transform.GetChild(1).gameObject.SetActive(true);
                hpList[((int)statusManager.CurrentHp / 3)].transform.GetChild(2).gameObject.SetActive(true);
                hpList[((int)statusManager.CurrentHp / 3)].transform.GetChild(3).gameObject.SetActive(false);

                break;
        }
    }
    public void TemHpSet()
    {
        //임시체력 관리 
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "RDis_Heart(Clone)")
            {
                switch (statusManager.TemHp % 3)
                {
                    case 0:
                        hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(1).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(2).gameObject.SetActive(true);
                        break;

                    case 1:
                        hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(1).gameObject.SetActive(false);
                        hpList[i].transform.GetChild(2).gameObject.SetActive(false);
                        break;

                    case 2:
                        hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(1).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(2).gameObject.SetActive(false);
                        break;
                }
                return;
            }
        }
    }
    public void TemHpDel()
    {
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "RDis_Heart(Clone)")
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
                hpNum--;
                return;
            }
        }
    }
    public void HpDel()
    {
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
                hpNum--;
                return;
            }
        }
    }
    public void ElectDel()
    {
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "Elect_Heart(Clone)")
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
                hpNum--;
                return;
            }
        }
    }

}
