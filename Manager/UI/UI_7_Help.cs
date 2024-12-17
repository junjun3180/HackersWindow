using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_7_Help : MonoBehaviour
{
    private static UI_7_Help instance = null;

    // UI Window
    public GameObject UI_W_Help = null;

    public static UI_7_Help Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_7_Help>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UI_7_Help).Name);
                    instance = singletonObject.AddComponent<UI_7_Help>();
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
        if (UI_W_Help != null)
        {
            UI_W_Help.SetActive(true);
            // Debug.Log("OpenUI : UI_7_Help");
        }
    }

    public void CloseUI()
    {
        if (UI_W_Help != null)
        {
            UI_W_Help.SetActive(false);
            // Debug.Log("CloseUI : UI_7_Help");
        }
    }
}
