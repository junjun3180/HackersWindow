using Mono.Cecil.Cil;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_3_MyDocument : MonoBehaviour
{
    private static UI_3_MyDocument instance = null;

    // UI Window
    public GameObject UI_W_MyDocument = null;

    // Item Drag Area
    public GameObject WindowUI;

    // Detail
    public GameObject Button_Item_Prefab;
    public GameObject DragIcon_Prefab;
    public GameObject i_Item_Detail_Image_Prefab;
    public Text t_Item_Detail_Name_Prefab;
    public TextMeshProUGUI t_Item_Detail_Explanation_Prefab;
    public Text t_Item_Detail_Size_Prefab;
    public GameObject DeleteButton;
    public GameObject UseButton;
    public GameObject UseButtonBlack;


    public Image i_StorageView;
    public Text t_StorageRate;

    public Transform ContentItemGroup;

    // Current Selected Item;
    private Item currentItem = null;

    // Manager
    private ItemManager itemManager = null;
    private StatusManager statusManager = null;

    public static UI_3_MyDocument Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_3_MyDocument>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UI_3_MyDocument).Name);
                    instance = singletonObject.AddComponent<UI_3_MyDocument>();
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
        itemManager = ItemManager.Instance;
        statusManager = StatusManager.Instance;

        DeleteButton.GetComponentInChildren<Button>().onClick.AddListener(FItemDelete_Button);
        UseButton.GetComponentInChildren<Button>().onClick.AddListener(FItemUse_Button);

        RemoveItemDetail();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenUI()
    {
        if (UI_W_MyDocument != null)
        {
            GenerateItemList();
            UI_W_MyDocument.SetActive(true);
            // Debug.Log("OpenUI : UI_3_MyDocument");
        }
    }

    public void CloseUI()
    {
        if (UI_W_MyDocument != null)
        {
            UI_W_MyDocument.SetActive(false);
            RemoveItemDetail();
            // Debug.Log("CloseUI : UI_3_MyDocument");
        }
    }

    public void GenerateItemList()
    {
        foreach (Transform child in ContentItemGroup)
        {
            Destroy(child.gameObject);
        }

        foreach (var kvp in itemManager.itemList)
        {
            string itemName = kvp.Key;
            List<Item> items = kvp.Value;

            foreach (Item itemInfo in items)
            {
                GameObject newButton = Instantiate(Button_Item_Prefab, ContentItemGroup);

                ItemDragHandler dragHandler = newButton.AddComponent<ItemDragHandler>();  // ItemDragHandler 추가
                dragHandler.item = itemInfo;  // 아이템 정보 전달
                dragHandler.windowUI = WindowUI;  // WindowUI 오브젝트 참조 전달
                dragHandler.Button_Drag_Prefab = DragIcon_Prefab;

                Transform childImageTransform = newButton.transform.Find("Image");
                Transform childTextTransform = newButton.transform.Find("Text");
                if (childImageTransform != null)
                {
                    Image ButtonImage = childImageTransform.GetComponent<Image>();
                    Text ButtonText = childTextTransform.GetComponent<Text>();
                    if (ButtonImage != null)
                    {
                        SetItemImage(ButtonImage, itemInfo.itemType);
                        ButtonText.text = itemInfo.ItemName;

                        newButton.GetComponent<Button>().onClick.AddListener(() => OpenItemDetail(itemInfo));
                    }
                    else
                    {
                        Debug.LogError("Error: Button image not found.");
                    }
                }
                else
                {
                    Debug.LogError("자식 오브젝트를 찾을 수 없습니다.");
                }

            }
        }
    }

    public void SetItemImage(Image buttonImage, Item.ItemType itemType)
    {
        Sprite[] sprites;
        Sprite itemSprite = null;
        int ImageIndex = itemManager.GetImageIndex(itemType);
        string spriteSheetName = itemManager.GetSpriteSheetName(itemType);

        sprites = Resources.LoadAll<Sprite>(spriteSheetName);
        itemSprite = sprites[ImageIndex];

        if (itemSprite != null)
        {
            buttonImage.sprite = itemSprite;
            // Debug.Log($"Item Image loaded: {itemType} -> {itemSprite.name}");
        }
        else
        {
            Debug.LogError($"Sprite not found for item type: {itemType} at path: {spriteSheetName}");
        }
    }

    public void OpenItemDetail(Item CurItem)
    {
        currentItem = CurItem;
        Image DetailImage = i_Item_Detail_Image_Prefab.GetComponent<Image>();
        SetItemImage(DetailImage, CurItem.itemType);
        t_Item_Detail_Name_Prefab.text = CurItem.ItemName;
        t_Item_Detail_Explanation_Prefab.text = CurItem.ItemInfomation;
        t_Item_Detail_Size_Prefab.text = CurItem.ItemSize.ToString() + "MB";

        if (CurItem.IsDeletable) DeleteButton.SetActive(true);
        else DeleteButton.SetActive(false);

        if (CurItem.IsUsable) { UseButton.SetActive(true); UseButtonBlack.SetActive(false); }
        else { UseButton.SetActive(false); UseButtonBlack.SetActive(true); }
    }

    public void UpdateStorage()
    {
        i_StorageView.fillAmount = (float)statusManager.CurrentStorage / (statusManager.B_MaxStorage);
        if (i_StorageView.fillAmount >= 0.8f) i_StorageView.color = new Color(1f, 0.1949685f, 0.1949685f); else i_StorageView.color = new Color(0.1960784f, 1f, 0.6738115f);
        t_StorageRate.text = statusManager.MaxStorage.ToString() + "MB 중 " + (statusManager.MaxStorage - statusManager.CurrentStorage).ToString() + "MB 사용 가능";
    }

    public void RemoveItemDetail()
    {
        currentItem = null;
        Image DetailImage = i_Item_Detail_Image_Prefab.GetComponent<Image>();
        DetailImage.sprite = null;
        t_Item_Detail_Name_Prefab.text = "";
        t_Item_Detail_Explanation_Prefab.text = "";
        t_Item_Detail_Size_Prefab.text = "";

        // 버튼 비활성화
        DeleteButton.SetActive(false);
        UseButton.SetActive(false);
        UseButtonBlack.SetActive(false);
    }

    private void FItemUse_Button()
    {
        if (itemManager != null)
        {
            itemManager.RemoveItem(currentItem);
            UpdateStorage();
            Debug.Log("사용 기능 구현해야 함");
        }

        // 리스트 갱신
        GenerateItemList();
        // 디테일 창 삭제
        RemoveItemDetail();
    }

    private void FItemDelete_Button()
    {
        if (itemManager != null)
        {
            itemManager.DropDownItem(currentItem);
            UpdateStorage();
        }

        // 리스트 갱신
        GenerateItemList();
        // 디테일 창 삭제
        RemoveItemDetail();
    }
}
