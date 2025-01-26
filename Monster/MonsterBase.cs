using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : MonoBehaviour
{
    #region Manager

    protected GameManager GameManager;
    protected StatusManager statusManager;
    protected FolderManager folderManager;

    #endregion

    #region Monster Base Info

    public MonsterType monsterType;
    public float MoveSpeed;
    public float AttackPower;
    public float HP;
    protected float BaseHP;
    public float DefenseRate; // 방어력 계수
    public float DetectingAreaR;
    protected bool isMoving = true;
    private bool isDead = false; // 사망 상태 플래그

    #endregion

    #region Target Info

    protected Transform player; // Target Player
    protected Vector3 TargetPosition; // Saved Target Position
    protected bool DetectionSuccess = false;

    #endregion

    #region Variable Element

    public static Dictionary<MonsterType, string> MonsterNameDict = new Dictionary<MonsterType, string>
    {
        { MonsterType.M_V1, "M_V1이름이름" },
        { MonsterType.M_V2, "M_V2이름이름" },
        { MonsterType.M_V3, "M_V3이름이름" },
        { MonsterType.M_CardPack, "M_CardPack이름이름" },
        { MonsterType.M_VE_1, "M_VE_1이름이름" },
        { MonsterType.M_VE_2, "M_VE_2이름이름" },
        { MonsterType.M_SpiderCardPack, "M_SpiderCardPack이름이름" },
        { MonsterType.Red_Spider, "Red_Spider이름이름" },
        { MonsterType.White_Spider, "White_Spider이름이름" },
        { MonsterType.Boss_Mouse, "Boss_Mouse는 보스" }
    };

    protected SpriteRenderer SpriteRenderer;
    protected Animator MAnimator;
    protected Rigidbody2D rb;

    #endregion

    #region Default Function

    protected virtual void Start()
    {
        GameManager = GameManager.Instance;
        statusManager = StatusManager.Instance;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        MAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        folderManager = FolderManager.Instance;
        DefenseRate = 1.0f;
    }

    #endregion

    #region Interface Function. If you inherit this script, you must override these functions.

    public virtual IEnumerator MonsterRoutine()
    {
        Debug.Log("\"MonsterRoutine \"함수를 재정의하지 않음.");
        yield return null;
    }

    public virtual IEnumerator AttackPreparation()
    {
        Debug.Log("\"AttackPreparation \"함수를 재정의하지 않음.");
        yield return null;
    }

    public virtual IEnumerator RandomMoveAfterSearchFail()
    {
        Debug.Log("\"RandomMoveAfterSearchFail \"함수를 재정의하지 않음.");
        yield return null;
    }

    // Return RandomPosition
    protected virtual Vector3 GetRanomPositionAround()
    {
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(0f, DetectingAreaR);

        float x = transform.position.x + randomDistance * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
        float y = transform.position.y + randomDistance * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

        return new Vector3(x, y, 0);
    }

    #endregion

    #region Sprite Flip Setting

    protected void SpriteFlipSetting()
    {
        // 방향 설정
        if (TargetPosition.x > transform.position.x)
        {
            SpriteRenderer.flipX = true;   // 왼쪽을 바라봄 (Flip X 활성화)
        }
        else if (TargetPosition.x < transform.position.x)
        {
            SpriteRenderer.flipX = false;  // 오른쪽을 바라봄 (Flip X 비활성화)
        }
    }

    #endregion

    #region Collision Handling Section

    // Player Collision
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (statusManager != null)
                {
                    statusManager.TakeDamage(AttackPower, monsterType);
                }
                else
                {
                    Debug.Log("Monster OnTriggerEnter2D : Player Not Found");
                }
                if (monsterType != MonsterType.M_SpiderCardPack)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = 0f;
                }
            }

            if (collision.gameObject.CompareTag("Bullet"))
            {
                // Debug.Log("Monster Take Damage");
                collision.gameObject.SetActive(false);
                // Debug.Log("Attack Damage : " + statusManager.AttackPower * DefenseRate);

                Damaged(statusManager.AttackPower * DefenseRate);

            }
        }
    }

    #endregion

    #region Damage

    public virtual void Damaged(float damage)
    {
        // Debug.Log("Damaged");
        HP -= damage;
        if (HP <= 0) Die();
    }

    protected virtual void Die()
    {
        // Do not run if you are already dead
        if (isDead) return;

        isDead = true;

        folderManager.UpdateMonsterCount(-1);
        Destroy(this.gameObject);
    }

    #endregion

    #region PlayerDetection

    public bool DetectionPlayerPosition()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(transform.position, DetectingAreaR);
        rb.bodyType = RigidbodyType2D.Dynamic;

        foreach (Collider2D obj in detectedObjects)
        {
            if (obj.CompareTag("Player"))
            {
                player = obj.transform;

                TargetPosition = player.position;
                return true;
            }
        }

        return false;
    }

    #endregion

    // ========== 탐색범위 표시용 ==========
    // 탐지 범위를 시각적으로 표시 (에디터 전용)
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectingAreaR);
    }
}
