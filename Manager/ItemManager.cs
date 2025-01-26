using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    #region Manager

    public static ItemManager Instance;
    private FolderManager folderManager;
    private StatusManager statusManager;
    private UIManager uIManager;
    private UI_0_HUD ui_0_HUD;

    #endregion

    #region Variable Elment

    // public 
    public SortedDictionary<string, List<Item>> itemList;

    // private
    private Player player;
    private Rigidbody2D rb;

    #endregion

    #region Item Prefab

    [Header("Item Prefab")]
    public GameObject P_Coin1;
    public GameObject P_Coin5;
    public GameObject P_Coin10;
    public GameObject P_Coin15;
    public GameObject P_Coin100;
    public GameObject P_Key;
    public GameObject P_CardPack;
    public GameObject P_ForcedDeletion;
    public GameObject P_ProgramRemove;
    public GameObject P_ExpansionKit_1;
    public GameObject P_ExpansionKit_2;
    public GameObject P_ExpansionKit_3;
    public GameObject P_ProgramRecycle;
    public GameObject P_Card_Clover;
    public GameObject P_Card_Spade;
    public GameObject P_Card_Hearth;
    public GameObject P_Card_Dia;
    public GameObject P_Ticket_Random;
    public GameObject P_Ticket_Down;
    public GameObject P_Ticket_Shop;
    public GameObject P_Ticket_Special;
    public GameObject P_Ticket_BlackShop;
    public GameObject P_Ticket_Boss;

    public GameObject P_Heal;
    public GameObject P_TemHp;
    public GameObject P_Shiled;
    public GameObject P_Spark;

    private GameObject SpawnObject = null;
    private GameObject droppedItem = null;

    #endregion

    #region Sorting Item 

    // 아이템 이름
    private string Coin1_Name;
    private string Coin5_Name;
    private string Coin10_Name;
    private string Coin15_Name;
    private string Coin100_Name;
    private string KeyName;
    private string ForcedDeletionName;
    private string ProgramDeleteItemName;
    private string ProgramRecycleName;


    // ItemType을 키로, 이미지 인덱스를 값으로 저장하는 Dictionary
    public static Dictionary<ItemType, int> ImageIndexMap = new Dictionary<ItemType, int>()
    {
        { ItemType.Coin1, 0 },
        { ItemType.Coin5, 1 },
        { ItemType.Coin10, 6 },
        { ItemType.Coin15, 11 },
        { ItemType.Coin100, 16 },
        { ItemType.Key, 2 },
        { ItemType.CardPack, 1 },
        { ItemType.ForcedDeletion, 4 },
        { ItemType.ProgramRemove, 0 },
        { ItemType.ExpansionKit_1, 17 },
        { ItemType.ExpansionKit_2, 18 },
        { ItemType.ExpansionKit_3, 19 },
        { ItemType.ProgramRecycle, 3 },
        { ItemType.Card_Spade, 5 },
        { ItemType.Card_Clover, 6 },
        { ItemType.Card_Hearth, 7 },
        { ItemType.Card_Dia, 8 },
        { ItemType.Ticket_Shop, 9 },
        { ItemType.Ticket_Down, 10 },
        { ItemType.Ticket_Special, 11 },
        { ItemType.Ticket_Random, 12 },
        { ItemType.Ticket_BlackShop, 13 },
        { ItemType.Ticket_Boss, 14 }


    };

    // 코인 순서에 맞춰 정렬하기 위한 비교기
    public class CoinComparer : IComparer<string>
    {
        private Dictionary<string, int> customOrder;

        public CoinComparer(ItemManager manager)
        {
            customOrder = new Dictionary<string, int>
            {
                { manager.Coin1_Name, 1 },
                { manager.Coin5_Name, 2 },
                { manager.Coin10_Name, 3 },
                { manager.Coin15_Name, 4 },
                { manager.Coin100_Name, 5 }
            };
        }

        public int Compare(string x, string y)
        {
            // 커스텀 정렬 순서가 있는 경우, 해당 순서로 정렬
            if (customOrder.ContainsKey(x) && customOrder.ContainsKey(y))
            {
                return customOrder[x].CompareTo(customOrder[y]);
            }
            // 코인이 아닌 경우 사전 순으로 정렬
            return x.CompareTo(y);
        }
    }

    // 내림차순 정렬을 위한 클래스
    public class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T x, T y)
        {
            return y.CompareTo(x);
        }
    }

    #endregion

    #region Default Function

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 이름 지정
        Coin1_Name = P_Coin1.GetComponent<Item>().ItemName;
        Coin5_Name = P_Coin5.GetComponent<Item>().ItemName;
        Coin10_Name = P_Coin10.GetComponent<Item>().ItemName;
        Coin15_Name = P_Coin15.GetComponent<Item>().ItemName;
        Coin100_Name = P_Coin100.GetComponent<Item>().ItemName;
        KeyName = P_Key.GetComponent<Item>().ItemName;
        ForcedDeletionName = P_ForcedDeletion.GetComponent<Item>().ItemName;
        ProgramDeleteItemName = P_ProgramRemove.GetComponent<Item>().ItemName;
        ProgramRecycleName = P_ProgramRecycle.GetComponent<Item>().ItemName;

        if (itemList == null)
        {
            itemList = new SortedDictionary<string, List<Item>>(new ItemManager.CoinComparer(this));
        }
    }

    void Start()
    {
        statusManager = StatusManager.Instance;
        uIManager = UIManager.Instance;
        ui_0_HUD = UI_0_HUD.Instance;
        player = GameManager.Instance.GetPlayer();
        folderManager = FolderManager.Instance;
    }

    #endregion

    #region Item Add/Drop

    public bool AddItem(Item item)
    {
        if (statusManager.CurrentStorage + item.ItemSize > statusManager.MaxStorage)
        {
            Debug.Log("공간부족으로 아이템 먹기 불가능");
            return false;
        }

        // Debug.Log("ItemManager AddItem"); 
        if (itemList.ContainsKey(item.ItemName))
        {
            // 같은 이름의 아이템이 이미 존재하는 경우 리스트에 추가
            itemList[item.ItemName].Add(item);
        }
        else
        {
            // 새로운 이름의 아이템이면 리스트 생성 후 추가
            itemList[item.ItemName] = new List<Item> { item };
        }

        ui_0_HUD.UpdateHUD();
        statusManager.CurrentStorage += item.ItemSize;
        return true;
    }

    public void RemoveItem(Item item)
    {
        if (itemList.ContainsKey(item.ItemName))
        {
            statusManager.CurrentStorage -= item.ItemSize;
            itemList[item.ItemName].Remove(item);
            if (itemList[item.ItemName].Count == 0)
            {
                itemList.Remove(item.ItemName);
                ui_0_HUD.UpdateHUD();
            }
        }
    }

    public void DropItem(Item item, bool isRight)
    {
        RemoveItem(item);

        DesignateGameObject(item.itemType);

        Vector3 offset = player.transform.position + (isRight ? new Vector3(0.9f, 0, 0) : new Vector3(-0.8f, 0, 0));
        droppedItem = Instantiate(SpawnObject, offset, Quaternion.identity);

        droppedItem.GetComponent<Item>().isDroped = true;

        Vector3 pushDirection = (droppedItem.transform.position - player.transform.position).normalized;
        rb = droppedItem.GetComponent<Rigidbody2D>();
        rb.drag = statusManager.DragForce;
        rb.AddForce(pushDirection * statusManager.DropForce, ForceMode2D.Impulse);
        StartCoroutine(StopAfterDelay(0.3f));
    }

    public void DropDownItem(Item item)
    {
        RemoveItem(item);

        DesignateGameObject(item.itemType);

        Vector3 offset = player.transform.position + new Vector3(0, -1.2f, 0); 
        droppedItem = Instantiate(SpawnObject, offset, Quaternion.identity);

        droppedItem.GetComponent<Item>().isDroped = true;

        Vector3 pushDirection = (droppedItem.transform.position - player.transform.position).normalized;
        rb = droppedItem.GetComponent<Rigidbody2D>();
        rb.AddForce(pushDirection * 1.5f, ForceMode2D.Impulse);
        
        StartCoroutine(StopAfterDelay(0.3f));
    }

    private void DesignateGameObject(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Coin1: SpawnObject = P_Coin1; break;
            case ItemType.Coin5: SpawnObject = P_Coin5; break;
            case ItemType.Coin10: SpawnObject = P_Coin10; break;
            case ItemType.Coin15: SpawnObject = P_Coin15; break;
            case ItemType.Coin100: SpawnObject = P_Coin100; break;
            case ItemType.Key: SpawnObject = P_Key; break;
            case ItemType.CardPack: SpawnObject = P_CardPack; break;
            case ItemType.ForcedDeletion: SpawnObject = P_ForcedDeletion; break;
            case ItemType.ProgramRemove: SpawnObject = P_ProgramRemove; break;
            case ItemType.ExpansionKit_1: SpawnObject = P_ExpansionKit_1; break;
            case ItemType.ExpansionKit_2: SpawnObject = P_ExpansionKit_2; break;
            case ItemType.ExpansionKit_3: SpawnObject = P_ExpansionKit_3; break;
            case ItemType.ProgramRecycle: SpawnObject = P_ProgramRecycle; break;
            case ItemType.Card_Clover: SpawnObject = P_Card_Clover; break;
            case ItemType.Card_Spade: SpawnObject = P_Card_Spade; break;
            case ItemType.Card_Hearth: SpawnObject = P_Card_Hearth; break;
            case ItemType.Card_Dia: SpawnObject = P_Card_Dia; break;
            case ItemType.Ticket_Random: SpawnObject = P_Ticket_Random; break;
            case ItemType.Ticket_Down: SpawnObject = P_Ticket_Down; break;
            case ItemType.Ticket_Shop: SpawnObject = P_Ticket_Shop; break;
            case ItemType.Ticket_Special: SpawnObject = P_Ticket_Special; break;
            case ItemType.Ticket_BlackShop: SpawnObject = P_Ticket_BlackShop; break;
            case ItemType.Ticket_Boss: SpawnObject = P_Ticket_Boss; break;
            case ItemType.Heal: SpawnObject = P_Heal; break;
            case ItemType.TemHp: SpawnObject = P_TemHp; break;
            case ItemType.Shiled: SpawnObject = P_Shiled; break;
            case ItemType.Spark: SpawnObject = P_Spark; break;
        }
    }

    IEnumerator StopAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        droppedItem.GetComponent<Item>().isDroped = false;
        rb.velocity = Vector2.zero;
    }

    #endregion

    #region Get Item Count

    public int GetKeyCount()
    {
        int KeyCount = 0;
        if (itemList.ContainsKey(KeyName))
        {
            KeyCount = itemList[KeyName].Count;
        }
        return KeyCount;
    }

    public int GetCoinCount()
    {
        int CoinCount = 0;
        if (itemList.ContainsKey(Coin1_Name))
        {
            CoinCount += itemList[Coin1_Name].Count;
        }
        if (itemList.ContainsKey(Coin5_Name))
        {
            CoinCount += 5 * itemList[Coin5_Name].Count;
        }
        if (itemList.ContainsKey(Coin10_Name))
        {
            CoinCount += 10 * itemList[Coin10_Name].Count;
        }
        if (itemList.ContainsKey(Coin15_Name))
        {
            CoinCount += 15 * itemList[Coin15_Name].Count;
        }
        if (itemList.ContainsKey(Coin100_Name))
        {
            CoinCount += 100 * itemList[Coin100_Name].Count;
        }
        return CoinCount;
    }

    public int GetBombCount()
    {
        int BombCount = 0;

        if (itemList.ContainsKey(ForcedDeletionName))
        {
            BombCount = itemList[ForcedDeletionName].Count;
        }
        return BombCount;
    }

    #endregion

    #region Item Use

    private bool ConsumeItem(string itemName, bool updateHUD = false)
    {
        // Check if the item exists and has at least one instance
        if (itemList.ContainsKey(itemName) && itemList[itemName].Count > 0)
        {
            // Retrieve and remove the first item
            Item item = itemList[itemName][0];
            statusManager.CurrentStorage -= item.ItemSize;
            itemList[itemName].Remove(item);

            // If the list is now empty, remove the key from the dictionary
            if (itemList[itemName].Count == 0)
            {
                itemList.Remove(itemName);
            }

            // Optionally update the HUD
            if (updateHUD)
            {
                ui_0_HUD.UpdateHUD();
            }

            Debug.Log($"{itemName} 아이템 사용됨");
            return true; // Successful usage
        }
        else
        {
            Debug.Log($"{itemName} 아이템을 사용할 수 없습니다.");
            return false; // Failed usage
        }
    }


    public void UseItem(Item item)
    {
        Debug.Log("UseItem");

        switch (item.itemType)
        {
            case ItemType.Card_Clover:
                folderManager.MoveHiddenFolder("Clover");
                break;
            case ItemType.Card_Dia:
                folderManager.MoveHiddenFolder("Diamond");
                break;
            case ItemType.Card_Hearth:
                folderManager.MoveHiddenFolder("Hearth");
                break;
            case ItemType.Card_Spade:
                folderManager.MoveHiddenFolder("Spade");
                break;
            case ItemType.Ticket_BlackShop:
                folderManager.MoveHiddenFolder("Black");
                break;
            case ItemType.ExpansionKit_1:
            case ItemType.ExpansionKit_2:
            case ItemType.ExpansionKit_3:
                statusManager.MaxStorageUp(item.itemScore);
                break;

            case ItemType.Ticket_Down:
                folderManager.MoveToFolderByType(FolderType.Download);
                break;
            case ItemType.Ticket_Shop:
                folderManager.MoveToFolderByType(FolderType.Shop);
                break;
            case ItemType.Ticket_Special:
                folderManager.MoveToFolderByType(FolderType.RandomSpecial);
                break;

            case ItemType.Ticket_Boss:
                folderManager.MoveToSpecificFolder("Boss.1P.S.Eror404Not");
                UIManager.Instance.WindowUISetActive();
                break;

            case ItemType.ProgramRecycle:
                Debug.Log("ProgramRecycle");
                ProgramRecycleUse();
                break;

            default:
                Debug.Log("아직 정의되지 않은 아이템 사용 효과");
                break;
        }
    }

    public bool KeyUse()
    {
        // 키 아이템이 itemList에 있고, 해당 리스트에 아이템이 하나 이상 있는지 확인
        if (itemList.ContainsKey(KeyName) && itemList[KeyName].Count > 0)
        {
            Item keyItem = itemList[KeyName][0]; // 첫 번째 아이템 가져오기
            statusManager.CurrentStorage -= keyItem.ItemSize; // 사용 후 저장소 용량 줄이기
            itemList[KeyName].Remove(keyItem); // 사용한 아이템 제거

            // 해당 키가 더 이상 없으면 리스트에서 키 자체를 삭제
            if (itemList[KeyName].Count == 0)
            {
                itemList.Remove(KeyName);
            }

            ui_0_HUD.UpdateHUD(); // HUD 업데이트
            Debug.Log("잠금파일 해독 키 사용됨");
            return true;  // 키 사용 성공
        }
        else
        {
            Debug.Log("사용할 수 있는 잠금파일 해독 키가 없습니다.");
            return false;  // 키가 없으면 실패
        }
    }

    public bool ForcedDeletionUse()
    {
        return ConsumeItem(ForcedDeletionName, updateHUD: true);
    }

    public bool ProgramDeletionUse()
    {
        return ConsumeItem(ProgramDeleteItemName);
    }

    public void ProgramRecycleUse()
    {
        // Check if the current folder is a download folder
        if (folderManager.CurrentFolder.Type != FolderType.Download)
            return;

        ProgramRoom programRoom;
        programRoom = FindObjectOfType<ProgramRoom>();
        if(programRoom == null) return;
        
        programRoom.ChangeDownloadRoomProgram();
    }



    public void UseCardPack()
    {

    }

    #endregion

    #region Image Setting

    public int GetImageIndex(ItemType itemType)
    {
        if (ImageIndexMap.TryGetValue(itemType, out int index))
        {
            return index;
        }

        Debug.LogWarning($"Image index for {itemType} not found.");
        return -1; // 기본값 반환
    }

    public string GetSpriteSheetName(ItemType itemType)
    {                
        string spriteSheetName = "";
        switch (itemType)
        {
            case ItemType.Coin1:
            case ItemType.Coin5:
            case ItemType.Coin10:
            case ItemType.Coin15:
                spriteSheetName = "Item/use_Coin";
                break;
            case ItemType.Coin100:
            case ItemType.Key:
            case ItemType.CardPack:
            case ItemType.ForcedDeletion:
            case ItemType.ProgramRemove:
            case ItemType.ExpansionKit_1:
            case ItemType.ExpansionKit_2:
            case ItemType.ExpansionKit_3:
            case ItemType.ProgramRecycle:
            case ItemType.Card_Clover:
            case ItemType.Card_Spade:
            case ItemType.Card_Hearth:
            case ItemType.Card_Dia:
            case ItemType.Ticket_Random:
            case ItemType.Ticket_Down:
            case ItemType.Ticket_Shop:
            case ItemType.Ticket_Special:
            case ItemType.Ticket_BlackShop:
            case ItemType.Ticket_Boss:
                spriteSheetName = "Item/use_DropItem";
                break;
            case ItemType.Heal:
            case ItemType.TemHp:
            case ItemType.Shiled:
            case ItemType.Spark:
                spriteSheetName = "Sprites/Items/Heal";
                break;

            default:
                Debug.LogError("Unknown item type.");
                break;
        }
        return spriteSheetName;
    }

    #endregion
}
