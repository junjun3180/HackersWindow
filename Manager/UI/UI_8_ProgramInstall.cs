using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_8_ProgramInstall : MonoBehaviour
{
    private static UI_8_ProgramInstall instance = null;

    // UI Window
    public GameObject UI_W_ProgramInstall = null;

    // Detail
    public Animator OP_ED_animator;
    public bool isESCDisabled = false;
    private int CurrentProgram;

    public bool FinishedInstall = false;

    public Image ProgramImage0;
    public Image ProgramImage1;
    public Image ProgramImage2;
    public Image ProgramImage3;

    public GameObject DownLoadUI0;
    public Button UI_0_Next;
    public Button UI_0_Exit;
    public Button UI_0_Cancel;

    public GameObject DownLoadUI1;
    public Button UI_1_Before;
    public Button UI_1_Exit;
    public Button UI_1_Next;
    public Button UI_1_Cancel;
    public Text UI_1_Info;

    public GameObject DownLoadUI2;
    public Animator DAnimator;

    public GameObject DownLoadUI3;
    public Button UI_3_Exit;
    public Button UI_3_End;

    public int CurrentUIIndex;


    // Manager
    private UI_0_HUD ui_0_HUD = null;
    private StatusManager statusManager = null;
    private PoolingManager poolingManager = null;

    public static UI_8_ProgramInstall Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_8_ProgramInstall>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UI_8_ProgramInstall).Name);
                    instance = singletonObject.AddComponent<UI_8_ProgramInstall>();
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
        ui_0_HUD = UI_0_HUD.Instance;
        statusManager = StatusManager.Instance;
        poolingManager = PoolingManager.Instance;

        UI_0_Next.onClick.AddListener(FNextButton);
        UI_0_Cancel.onClick.AddListener(FDownLoadUIExit);
        UI_0_Exit.onClick.AddListener(FDownLoadUIExit);

        UI_1_Before.onClick.AddListener(FBeforeButton);
        UI_1_Next.onClick.AddListener(FNextButton);
        UI_1_Cancel.onClick.AddListener(FDownLoadUIExit);
        UI_1_Exit.onClick.AddListener(FDownLoadUIExit);

        UI_3_End.onClick.AddListener(FDownLoadUIExit);
        UI_3_Exit.onClick.AddListener(FDownLoadUIExit);


        DAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        OP_ED_animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenUI()
    {
        if (UI_W_ProgramInstall != null)
        {
            UI_W_ProgramInstall.SetActive(true);
            ProgramInstallUI(0);
            // Debug.Log("OpenUI : UI_1_MyPC");
        }
    }

    public void CloseUI()
    {
        if (UI_W_ProgramInstall != null)
        {
            UI_W_ProgramInstall.SetActive(false);
            // Debug.Log("CloseUI : UI_1_MyPC");
        }
    }

    private void ProgramInstallUI(int index)
    {
        isESCDisabled = true;
        CurrentUIIndex = index;
        switch (index)
        {
            case 0:
                Time.timeScale = 0.0f;
                DownLoadUI0.SetActive(true);
                ui_0_HUD.CloseUI();
                break;
            case 1:
                DownLoadUI1.SetActive(true);
                break;
            case 2:
                DownLoadUI2.SetActive(true);
                StartCoroutine(PlayInstallAnimation());
                break;
            case 3:
                FinishedInstall = true;
                DownLoadUI3.SetActive(true);
                break;
            default:
                Debug.Log("Out of Index");
                break;
        }
    }

    private IEnumerator PlayInstallAnimation()
    {
        DAnimator.speed = 0.5f;
        int animationNum = Random.Range(0, 5);

        Debug.Log("Num : " + animationNum);
        float animationDuration = 2f;

        switch (animationNum)
        {
            case 0:
                DAnimator.SetTrigger("Bar_1");
                animationDuration = 6.0f;
                break;
            case 1:
                DAnimator.SetTrigger("Bar_2");
                animationDuration = 12.0f;
                break;
            case 2:
                DAnimator.SetTrigger("Bar_3");
                animationDuration = 13.0f;
                break;
            case 3:
                DAnimator.SetTrigger("Bar_4");
                animationDuration = 12.0f;
                break;
            case 4:
                DAnimator.SetTrigger("Bar_5");
                animationDuration = 6.0f;
                break;
            default:
                Debug.Log("Out of Index");
                break;
        }

        yield return new WaitForSecondsRealtime(animationDuration);

        DownLoadUI2.SetActive(false);
        ProgramInstallUI(CurrentUIIndex + 1);
    }

    public void FNextButton()
    {
        switch (CurrentUIIndex)
        {
            case 0:
                DownLoadUI0.SetActive(false);
                ProgramInstallUI(CurrentUIIndex + 1);
                break;
            case 1:
                DownLoadUI1.SetActive(false);
                ProgramInstallUI(CurrentUIIndex + 1);
                break;
            default:
                Debug.Log("Out of Index");
                break;
        }
    }

    public void FDownLoadUIExit()
    {
        if (OP_ED_animator != null)
        {
            Time.timeScale = 1.0f;
            OP_ED_animator.SetTrigger("Ending");
            StartCoroutine(PlayCloseAnimation());
        }
    }

    private IEnumerator PlayCloseAnimation()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        switch (CurrentUIIndex)
        {
            case 0:
                DownLoadUI0.SetActive(false);
                break;
            case 1:
                DownLoadUI1.SetActive(false);
                break;
            case 3:
                DownLoadUI3.SetActive(false);
                break;
            default:
                Debug.Log("Out of Index");
                break;
        }
        ui_0_HUD.OpenUI();
        isESCDisabled = false;
        CloseUI();
    }

    public void FBeforeButton()
    {
        CurrentUIIndex = CurrentUIIndex - 1;
        DownLoadUI1.SetActive(false);
        DownLoadUI0.SetActive(true);
    }

}
