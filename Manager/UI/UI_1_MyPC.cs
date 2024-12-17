using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_1_MyPC : MonoBehaviour
{
    private static UI_1_MyPC instance = null;

    // UI Window
    public GameObject UI_W_MyPC = null;

    // Detail
    public Text AttackText;
    public Text AttackSpeedText;
    public Text BulletVelocityText;
    public Text RangeText;
    public Text MoveSpeedText;
    public Text Storage;

    // Manager
    private StatusManager statusManager = null;
    private PoolingManager poolingManager = null;

    public static UI_1_MyPC Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_1_MyPC>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UI_1_MyPC).Name);
                    instance = singletonObject.AddComponent<UI_1_MyPC>();
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
        poolingManager = PoolingManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenUI()
    {
        if(UI_W_MyPC != null)
        {
            UI_W_MyPC.SetActive(true);
            // Debug.Log("OpenUI : UI_1_MyPC");
        }
    }

    public void CloseUI()
    {
        if (UI_W_MyPC != null)
        {
            UI_W_MyPC.SetActive(false);
            // Debug.Log("CloseUI : UI_1_MyPC");
        }
    }

    public void UpdateStats()
    {
        if (statusManager != null && poolingManager != null)
        {
            // Debug.Log("UpdateStats");
            AttackText.text = statusManager.AttackPower.ToString();
            AttackSpeedText.text = statusManager.AttackSpeed.ToString();
            BulletVelocityText.text = statusManager.BulletSpeed.ToString();
            RangeText.text = statusManager.AngleRange.ToString();
            MoveSpeedText.text = statusManager.MoveSpeed.ToString();
            Storage.text = statusManager.CurrentStorage.ToString();
        }
        else
        {
            Debug.LogError("UpdateStats");
        }
    }
}
