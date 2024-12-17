using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;

    // [SerializeField]
    // MapGenerator mapGenerator;

    // Basic UI
    public GameObject FirstStartUI;
    public Button StartButton;
    public GameObject Start_Back;
    public GameObject Start_Line;

    public GameObject DeathUI;
    public Button ReStartButton;
    public Button GoToDesktop;
    public Text PlayTimeText;
    public Text DeathSign;

    // Window UI
    public GameObject WindowUI;

    // Left_Button 
    public Button MyPC_Button;
    public Button DownLoad_Button;
    public Button My_Documents_Button;
    public Button LocalDisk_Button;
    public Button ControlOptions_Buttonton;
    public Button Help_Button;
    public Button Desktop_Button;

    // Top_Button 
    public Button UnderBar_Button;
    public Button X_Button;

    // Manger
    private StatusManager statusManager;


    // Test - 기능모듈화
    public enum UI
    {
        UI_MyPC, UI_DownLoad, UI_MyDocument, UI_LocalDisk, UI_NetWork, UI_Control, UI_Help
    }

    private UI LastOpenUINum = UI.UI_MyPC;
    private UI_0_HUD ui_0_HUD = null;
    private UI_1_MyPC ui_1_MyPC = null;
    private UI_2_DownLoad ui_2_DownLoad = null;
    private UI_3_MyDocument ui_3_MyDocument = null;
    private UI_4_LocalDisk ui_4_LocalDisk = null;
    private UI_5_NetWork ui_5_NetWork = null;
    private UI_6_Control ui_6_Control = null;
    private UI_7_Help ui_7_Help = null;
    private UI_8_ProgramInstall ui_8_ProgramInstall = null;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static UIManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    void Start()
    {
        statusManager = StatusManager.Instance;


        // UI Panel 비활성화 시작
        WindowUI.SetActive(false);
        CloseWindowUI();

        // Left Button Setting
        MyPC_Button.onClick.AddListener(FMyPC_Button);
        DownLoad_Button.onClick.AddListener(FDownLoad_Button);
        My_Documents_Button.onClick.AddListener(FMy_Documents_Button);
        LocalDisk_Button.onClick.AddListener(FLocalDisk_Button);
        ControlOptions_Buttonton.onClick.AddListener(FControlOptions_Button);
        Help_Button.onClick.AddListener(FHelp_Button);
        Desktop_Button.onClick.AddListener(FDesktop_Button);

        // Top Button Setting
        UnderBar_Button.onClick.AddListener(CloseWindowUI);
        X_Button.onClick.AddListener(CloseWindowUI);

        // Basic UI Setting
        ReStartButton.onClick.AddListener(FReStartButton);
        GoToDesktop.onClick.AddListener(FDesktop_Button);

        ui_0_HUD = UI_0_HUD.Instance;
        ui_1_MyPC = UI_1_MyPC.Instance;
        ui_2_DownLoad = UI_2_DownLoad.Instance;
        ui_3_MyDocument = UI_3_MyDocument.Instance;
        ui_4_LocalDisk = UI_4_LocalDisk.Instance;
        ui_5_NetWork = UI_5_NetWork.Instance;
        ui_6_Control = UI_6_Control.Instance;
        ui_7_Help = UI_7_Help.Instance;
        ui_8_ProgramInstall = UI_8_ProgramInstall.Instance;
    }

    void Update()
    {
        if (!ui_8_ProgramInstall.isESCDisabled && Input.GetKeyDown(KeyCode.Escape))
        {
            // Debug.Log("ui_8_ProgramInstall.isESCDisabled" + ui_8_ProgramInstall.isESCDisabled);
            // if (mapGenerator.currentMapClear)
            {
                WindowUISetActive();
            }
        }
    }

    // ================ UI 모듈화 ================
    private void CloseAllUI()
    {
        ui_1_MyPC.CloseUI();
        ui_2_DownLoad.CloseUI();
        ui_3_MyDocument.CloseUI();
        ui_4_LocalDisk.CloseUI();
        ui_5_NetWork.CloseUI();
        ui_6_Control.CloseUI();
        ui_7_Help.CloseUI();
    }

    private void CloseWindowUI()
    {
        if (WindowUI != null)
        {
            WindowUI.SetActive(false);
        }
    }

    private void OpenWindowUI()
    {
        if (WindowUI == null)
            return;

        WindowUI.SetActive(true);

        switch (LastOpenUINum)
        {
            case UI.UI_MyPC:
                ui_1_MyPC.OpenUI();
                break;
            case UI.UI_DownLoad:
                ui_2_DownLoad.OpenUI();
                break;
            case UI.UI_MyDocument:
                ui_3_MyDocument.OpenUI();
                break;
            case UI.UI_LocalDisk:
                ui_4_LocalDisk.OpenUI();
                break;
            case UI.UI_NetWork:
                ui_5_NetWork.OpenUI();
                break;
            case UI.UI_Control:
                ui_6_Control.OpenUI();
                break;
            case UI.UI_Help:
                ui_7_Help.OpenUI();
                break;
        }
    }

    private void WindowUISetActive()
    {
        if (WindowUI != null)
        {
            bool isActive = WindowUI.activeSelf;
            if (isActive)
            {
                CloseWindowUI();
                ui_0_HUD.OpenUI();
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;

                ui_0_HUD.CloseUI();
                ui_1_MyPC.UpdateStats();
                ui_2_DownLoad.GenerateProgramList();
                ui_3_MyDocument.GenerateItemList();
                ui_3_MyDocument.UpdateStorage();

                OpenWindowUI();
            }
        }
    }

    // ================ UI 모듈화 ================

    // ================ Start, End, GameOver Section ================
    public void PlayerIsDead()
    {
        Time.timeScale = 0;
        StartCoroutine(DelayUIAndGameOver(2.0f));
    }

    private IEnumerator DelayUIAndGameOver(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        float playTime = Time.time - GameManager.Instance.StartTime;
        int hours = Mathf.FloorToInt(playTime / 3600);
        int minutes = Mathf.FloorToInt((playTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(playTime % 60);
        PlayTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

        MonsterBase.MonsterType monsterType = statusManager.DeathSign;
        if (MonsterBase.MonsterNameDict.TryGetValue(monsterType, out string monsterName))
        {
            DeathSign.text = monsterName + "한테 죽음.";
        }
        else
        {
            DeathSign.text = "알 수 없는 몬스터한테 죽음.";
        }


        DeathUI.SetActive(true);
    }

    public void FReStartButton()
    {
        DeathUI.SetActive(false);
        Time.timeScale = 1;
        statusManager.InitializeStatus();

        ui_0_HUD.HpBarSet();
        // GameManager.Instance.ReStartGame();
        GameManager.Instance.ResetPlayTime();
    }


    // Button OnClickFuction
    public void FMyPC_Button()
    {
        CloseAllUI();
        ui_1_MyPC.OpenUI();
        LastOpenUINum = UI.UI_MyPC;

        ui_4_LocalDisk.AdressReset();
        ui_4_LocalDisk.SetUIAdress(UI.UI_MyPC);
    }

    public void FDownLoad_Button()
    {
        CloseAllUI();
        ui_2_DownLoad.OpenUI();
        LastOpenUINum = UI.UI_DownLoad;

        ui_4_LocalDisk.AdressReset();
        ui_4_LocalDisk.SetUIAdress(UI.UI_DownLoad);
    }

    public void FMy_Documents_Button()
    {
        CloseAllUI();
        ui_3_MyDocument.OpenUI();
        LastOpenUINum = UI.UI_MyDocument;

        ui_4_LocalDisk.AdressReset();
        ui_4_LocalDisk.SetUIAdress(UI.UI_MyDocument);
    }

    public void FLocalDisk_Button()
    {
        CloseAllUI();
        ui_4_LocalDisk.OpenUI();
        LastOpenUINum = UI.UI_LocalDisk;

        ui_4_LocalDisk.AdressReset();
        ui_4_LocalDisk.SetUIAdress(UI.UI_LocalDisk);
    }

    public void FControlOptions_Button()
    {
        CloseAllUI();
        ui_6_Control.OpenUI();
        LastOpenUINum = UI.UI_Control;

        ui_4_LocalDisk.AdressReset();
        ui_4_LocalDisk.SetUIAdress(UI.UI_Control);
    }

    public void FHelp_Button()
    {
        CloseAllUI();
        ui_7_Help.OpenUI();
        LastOpenUINum = UI.UI_Help;

        ui_4_LocalDisk.AdressReset();
        ui_4_LocalDisk.SetUIAdress(UI.UI_Control);
    }

    public void FDesktop_Button()
    {
        // PC 종료
        Application.Quit();

        // 에디터에서 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ================ HP UI Section ================
    // Fixed

    // HP UI Activation Setting

}
