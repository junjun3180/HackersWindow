using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramRoom : MonoBehaviour
{
    #region Manager

    ProgramManager programManager;
    UI_8_ProgramInstall ui_8_ProgramInstall;

    #endregion

    #region Definition

    // DownLoad Room에서 나올 수 있는 프로그램 리스트
    public List<PInformation> ProgramList = new List<PInformation>();
    public int ProgramID;
    public SpriteRenderer spriteRenderer;


    private bool Checking = false;

    #endregion

    #region Default Function

    // Start is called before the first frame update
    void Start()
    {
        programManager = ProgramManager.Instance;
        ui_8_ProgramInstall = UI_8_ProgramInstall.Instance;

        ProgramID = Random.Range(0, ProgramList.Count);
        SetProgramImage(ProgramID);
    }

    // Update is called once per frame
    void Update()
    {
        if(Checking)
        {
            if(ui_8_ProgramInstall.FinishedInstall)
            {
                AddProgram();
            }
        }
    }

    #endregion

    #region Collider

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // UI Open
            Checking = true;
            ui_8_ProgramInstall.FinishedInstall = false;
            ui_8_ProgramInstall.OpenUI();
            ui_8_ProgramInstall.UI_1_Info.text = ProgramList[ProgramID].Explanation;
            ui_8_ProgramInstall.ProgramName.text = ProgramList[ProgramID].ProgramName;
        }
    }

    #endregion

    #region Program Install UI Setting

    public void AddProgram()
    {
        // Add Program
        programManager.AddProgramList(ProgramList[ProgramID]);

        // DeleteImage
        spriteRenderer.sprite = null;

        transform.gameObject.SetActive(false);
    }

    public void SetProgramImage(int id)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(ProgramList[ProgramID].spriteSheetName);

        if (sprites != null && ProgramList[ProgramID].spriteIndex >= 0 && ProgramList[ProgramID].spriteIndex < sprites.Length)
        {
            spriteRenderer.sprite = sprites[ProgramList[ProgramID].spriteIndex];
            ui_8_ProgramInstall.ProgramImage0.sprite = sprites[ProgramList[ProgramID].spriteIndex]; 
            ui_8_ProgramInstall.ProgramImage1.sprite = sprites[ProgramList[ProgramID].spriteIndex];
            ui_8_ProgramInstall.ProgramImage2.sprite = sprites[ProgramList[ProgramID].spriteIndex];
            ui_8_ProgramInstall.ProgramImage3.sprite = sprites[ProgramList[ProgramID].spriteIndex];
        }
        else
        {
            Debug.LogError("Sprite not found or invalid index.");
        }
    }

    #endregion

    // 추후 프로그램 변경 아이템용
    public void ChangeDownloadRoomProgram()
    {
        int NewID;

        do
        {
            NewID = Random.Range(0, ProgramList.Count);

            // ProgramID와 NewID가 다르고, programManager.ProgramList의 모든 ProgramName과 다를 때까지 반복
        } while (NewID == ProgramID ||
                 programManager.ProgramList.Exists(program => program.ProgramName == ProgramList[ProgramID].ProgramName));

        ProgramID = NewID;
        SetProgramImage(ProgramID);
    }
}
