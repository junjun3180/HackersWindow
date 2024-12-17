using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PInformation : MonoBehaviour
{
    private bool isProgramBeingAdded = false;
    public string ProgramName;
    public string Explanation;
    public string PowerExplanation;
    public int AddCoin = 0;
    public int HPHeal = 0;
    public int AttackPower = 0;
    public float AttackSpeed = 0;
    public float MoveSpeed = 0;
    public float BulletSpeed = 0;
    [Header("�ۼ�Ʈ ����")]
    public float AttackPerUp = 0;
    public float AttackSpeedPerUp = 0;
    public float MoveSpeedPerUp = 0;
    public float bulletSpeedPerUp = 0;
    [Header("�ۼ�Ʈ ��ŭ����")]
    public float AttackPerDown = 0;
    public float AttackSpeedPerDown = 0;
    public float MoveSpeedPerDown = 0;
    public float bulletSpeedPerDown = 0;
    [Header("�ۼ�Ʈ�� ����")]
    public float SetAttackPer = 0;
    public float SetAttackSpeedPer = 0;
    public float SetMoveSpeedPer = 0;
    public float SetbulletSpeedPer = 0;
    [Header("�Ѿ�ũ�⺯��")]
    public float BulletScalePerUP = 0;
    public float BulletScalePerDown = 0;
    public float SetBulletScalePer = 0;

    public bool IsUsable = true;
    public bool IsDeletable = true;



    // Image Setting
    public SpriteRenderer spriteRenderer;
    public string spriteSheetName;  // �̹��� �̸�
    public int spriteIndex; // �̹��� �� ��ȣ

    // Start is called before the first frame update
    void Start()
    {
        SetSprite(spriteSheetName, spriteIndex);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �浹�� ������Ʈ�� Player �� ��쿡�� �̺�Ʈ ó��
        if (!isProgramBeingAdded)
        {
            if (other.CompareTag("Player"))
            {
                isProgramBeingAdded = true;
                if (ProgramManager.Instance != null)
                {
                    ProgramManager.Instance.AddProgramList(this);
                }
                else
                {
                    Debug.LogError("ProgramManager instance not found.");
                }

                Destroy(gameObject);
            }
        }
    }

    public void SetSprite(string spriteSheetName, int spriteIndex)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSheetName);

        if (sprites != null && spriteIndex >= 0 && spriteIndex < sprites.Length)
        {
           spriteRenderer.sprite = sprites[spriteIndex];
        }
        else
        {
            Debug.LogError("Sprite not found or invalid index.");
        }
    }

}