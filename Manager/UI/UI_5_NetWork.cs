using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_5_NetWork : MonoBehaviour
{
    private static UI_5_NetWork instance = null;

    // UI Window
    public GameObject UI_W_NetWork = null;

    // Detail

    // Manager

    public static UI_5_NetWork Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_5_NetWork>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UI_5_NetWork).Name);
                    instance = singletonObject.AddComponent<UI_5_NetWork>();
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

    public void OpenUI()
    {
        if (UI_W_NetWork != null)
        {
            UI_W_NetWork.SetActive(true);
            // Debug.Log("OpenUI : UI_5_NetWork");
        }
    }

    public void CloseUI()
    {
        if (UI_W_NetWork != null)
        {
            UI_W_NetWork.SetActive(false);
            // Debug.Log("CloseUI : UI_5_NetWork");
        }
    }

}
