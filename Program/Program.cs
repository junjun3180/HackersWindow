using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour
{
    public enum program { AttackPower, AttackSpeed, BulletSpeed, DeleteMonster, HPHeal, MovingSpeed };
    public static int ProgramCount = 6;
    // ProgramInterface
    public string ProgramName; // 프로그램 이름
    public string Explanation; // 설명
    public string Explanation2; // 설명
    public int SpriteNum;   // 이미지 넘버
    protected bool DeleteIsPossible = false; // 삭제 가능한 프로그램인가

    // Image Loader
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    protected void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Program Delete Function
    public void DeleteProgram()
    {
        if(DeleteIsPossible)
        {

        }
    }

    // 이미지를 설정하는 매서드
    public void SetSprite(string spriteSheetName, int spriteIndex)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSheetName);

        if (sprites != null && spriteIndex < sprites.Length)
        {
            Sprite newSprite = sprites[spriteIndex];

            spriteRenderer.sprite = newSprite;
            Debug.Log("Sprite assigned: " + newSprite.name);
        }
        else
        {
            Debug.LogWarning("Sprite not found or index out of range: " + spriteSheetName + "_" + spriteIndex);
        }
    }
}
