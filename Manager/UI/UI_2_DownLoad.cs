using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class UI_2_DownLoad : MonoBehaviour
{
    private static UI_2_DownLoad instance = null;

    // UI Window
    public GameObject UI_W_DownLoad = null;

    // Detail
    public GameObject Button_Program_Prefab;
    public GameObject i_Program_Detail_Image_Prefab;
    public Text t_Program_Detail_Name_Prefab;
    public Text t_Program_Detail_Explanation_Prefab;
    public Text t_Program_Detail_PowerExplanation_Prefab;

    public Transform ContentProgramGroup;
    public Button ProgramUseButton;
    public Button ProgramDeleteButton;
    private int CurrentProgram = -1;

    // Manager
    private ProgramManager programManager;

    public static UI_2_DownLoad Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_2_DownLoad>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UI_2_DownLoad).Name);
                    instance = singletonObject.AddComponent<UI_2_DownLoad>();
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
        programManager = ProgramManager.Instance;

        ProgramUseButton.onClick.AddListener(FUse_Button);
        ProgramDeleteButton.onClick.AddListener(FDelete_Button);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenUI()
    {
        if (UI_W_DownLoad != null)
        {
            UI_W_DownLoad.SetActive(true);
            GenerateProgramList();
        }
    }

    public void CloseUI()
    {
        if (UI_W_DownLoad != null)
        {
            UI_W_DownLoad.SetActive(false);
            RemoveProgramDetail();
        }
    }

    public void GenerateProgramList()
    {
        // 자손 제거
        foreach (Transform child in ContentProgramGroup)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < programManager.ProgramList.Count; i++)
        {
            GameObject newButton = Instantiate(Button_Program_Prefab, ContentProgramGroup);

            PInformation programInfo = programManager.ProgramList[i];
            Image buttonImage = newButton.GetComponent<Image>();

            if (buttonImage != null)
            {
                SetSpriteFromSheet(buttonImage, programInfo.spriteSheetName, programInfo.spriteIndex);
            }
            else
            {
                Debug.LogError("Error");
            }

            int index = i;
            newButton.GetComponent<Button>().onClick.AddListener(() => OnProgramClick(newButton));
        }

        // Delete Button Activation
        if (programManager.ProgramList.Count == 0)
            ProgramDeleteButton.gameObject.SetActive(false);
    }

    void OnProgramClick(GameObject clickedButton)
    {
        int index = clickedButton.transform.GetSiblingIndex();
        OpenProgramDetail(index);
    }

    public void SetSpriteFromSheet(Image buttonImage, string spriteSheetName, int spriteIndex)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSheetName);

        if (sprites != null && spriteIndex >= 0 && spriteIndex < sprites.Length)
        {
            buttonImage.sprite = sprites[spriteIndex];
            Debug.Log("Sprite loaded: " + sprites[spriteIndex].name);

        }
        else
        {
            Debug.LogError("Sprite not found or invalid index for spriteSheetName: " + spriteSheetName);
        }
    }

    public void OpenProgramDetail(int index)
    {
        CurrentProgram = index;
        // Detail Setting
        t_Program_Detail_Name_Prefab.text = programManager.ProgramList[index].ProgramName;
        t_Program_Detail_Explanation_Prefab.text = programManager.ProgramList[index].Explanation;
        t_Program_Detail_PowerExplanation_Prefab.text = programManager.ProgramList[index].PowerExplanation;

        // Image Setting
        Image detailImage = i_Program_Detail_Image_Prefab.GetComponent<Image>();

        if (detailImage != null)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(programManager.ProgramList[index].spriteSheetName);

            if (sprites != null && programManager.ProgramList[index].spriteIndex >= 0 && programManager.ProgramList[index].spriteIndex < sprites.Length)
            {
                detailImage.sprite = sprites[programManager.ProgramList[index].spriteIndex];
                Debug.Log("Detail Image sprite set: " + sprites[programManager.ProgramList[index].spriteIndex].name);
            }
            else
            {
                Debug.LogError("Sprite not found or invalid index for spriteSheetName: " + programManager.ProgramList[index].spriteSheetName);
            }
        }
        else
        {
            Debug.LogError("i_Program_Detail_Image_Prefab does not have an Image component.");
        }

        ProgramDeleteButton.gameObject.SetActive(true);

        Debug.Log("OpenProgramDetail");


        DeActivateButtonGroup();

        if (programManager.ProgramList[index].IsUsable)
            ProgramUseButton.gameObject.SetActive(true);

        if (programManager.ProgramList[index].IsDeletable)
            ProgramDeleteButton.gameObject.SetActive(true);
    }

    public void RemoveProgramDetail()
    {
        t_Program_Detail_Name_Prefab.text = "";
        t_Program_Detail_Explanation_Prefab.text = "";
        t_Program_Detail_PowerExplanation_Prefab.text = "";

        // Image Setting
        Image detailImage = i_Program_Detail_Image_Prefab.GetComponent<Image>();
        detailImage = null;

        DeActivateButtonGroup();
    }

    public void FDelete_Button()
    {
        Debug.Log("제거");
        RemoveProgram();
    }

    public void FUse_Button()
    {
        // 프로그램 사용 기능 구현
        Debug.Log("사용 기능 구현 해야함");

        RemoveProgram();
    }

    private void RemoveProgram()
    {
        if (CurrentProgram != -1)
        {
            programManager.RemoveProgram(CurrentProgram);
            CurrentProgram = -1;

            t_Program_Detail_Name_Prefab.text = "";
            t_Program_Detail_Explanation_Prefab.text = "";
            t_Program_Detail_PowerExplanation_Prefab.text = "";

            ProgramDeleteButton.gameObject.SetActive(false);

            GenerateProgramList();
        }

        Image detailImage = i_Program_Detail_Image_Prefab.GetComponent<Image>();

        if (detailImage != null)
        {
            detailImage.sprite = null;
        }
        else
        {
            Debug.LogError("Image component not found");
        }

        DeActivateButtonGroup();
    }

    private void DeActivateButtonGroup()
    {
        ProgramUseButton.gameObject.SetActive(false);
        ProgramDeleteButton.gameObject.SetActive(false);
    }
}
