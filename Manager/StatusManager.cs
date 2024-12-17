using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public static StatusManager Instance;
    private UIManager uiManager;
    private UI_0_HUD ui_0_HUD;

    // When game is started, this base status used to Initializing Status based on this list.
    [Header("Player Base Status")]
    public float B_MaxHp;
    public float B_CurrentHp;
    public float B_TemHp;
    public float B_Shield;
    public float B_ShieldHp;
    public float B_Elect;
    private float B_HealCoolTime = 0.2f; // Playher Heal CoolTime
    private float B_HitCoolTime = 2.0f; // Player Attacked CoolTime

    // Player Attack Status
    public float B_AttackPower; // Player Attack Power
    public float B_AttackSpeed;  // Player Attack Speed
    public float B_AttackPushPower; // When Player Attacking, Push Power
    public float B_WeaponDistance; // Distance Between Player and Weapon
    public float B_AngleRange = 35f; // Player Weapon Angle Range
    public float B_BulletSpeed = 5.0f; // Bullet Speed
    public float B_BulletMaximumRange; // 총알 사거리

    // Other Status
    public float B_MoveSpeed; // 이동속도
    public int B_Coin; // 코인 개수
    public int B_CurrentStorage;
    public int B_MaxStorage;

    // When player in game, playing with this status
    // This status are no need to reset.
    [Header("Player Dynamic Status")]
    public float MaxHp; // 최대 체력
    public float CurrentHp; // 현재 체력
    public float TemHp; // 임시 체력(아이템)
    public float Shield; // 공격 막아주는 것
    public float ShieldHp;
    public float Elect;
    public MonsterBase.MonsterType DeathSign; // 사망원인
    private float HealCoolTime;
    private Coroutine healing_coroutine;
    private float HitCoolTime;
    private bool IsHit = false;

    public float AttackPower; // 공격력
    public float AttackSpeed;  // 공격속도
    public float AttackPushPower; // 어택시 밀격
    public float WeaponDistance; // 캐릭터~무기 거리
    public float AngleRange; // 캐릭터 무기 각도 범위
    public float BulletSpeed;
    public float BulletMaximumRange;

    public float MoveSpeed;
    public int Coin;
    public int CurrentStorage;
    public int MaxStorage;

    [Header("Item Drop Physical Force")]
    public float DragForce;   
    public float DropForce;
    public float AbsorptionSpeed;
    public float GetDistance;
    public float MaxDistance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        uiManager = UIManager.Instance;
        ui_0_HUD = UI_0_HUD.Instance;
        InitializeStatus();
    }

    public void InitializeStatus()
    {
        MaxHp = B_MaxHp;
        CurrentHp = B_CurrentHp;
        TemHp = B_TemHp;
        Shield = B_Shield;
        B_ShieldHp = ShieldHp;
        Elect = B_Elect;
        HealCoolTime = B_HealCoolTime;
        HitCoolTime = B_HitCoolTime;

        AttackPower = B_AttackPower;
        AttackSpeed = B_AttackSpeed;
        AttackPushPower = B_AttackPushPower;
        WeaponDistance = B_WeaponDistance;
        AngleRange = B_AngleRange;
        BulletSpeed = B_BulletSpeed;
        BulletMaximumRange = B_BulletMaximumRange;

        MoveSpeed = B_MoveSpeed;
        Coin = B_Coin;
        CurrentStorage = B_CurrentStorage;
        MaxStorage = B_MaxStorage;
    }

    // =============================== Fixed Section ===============================
    public void TakeDamage(float damage, MonsterBase.MonsterType deathSign)
    {
        if (!IsHit)
        {
            // Debug.Log("Player Take Damage");
            DeathSign = deathSign;
            StartCoroutine(Hit_Coroutine(damage));
        }
    }

    private IEnumerator Hit_Coroutine(float damage)
    {
        IsHit = true;
        if (ShieldHp > 0)
        {
            ShieldHp -= damage;
            ui_0_HUD.ShiledSet();

            if (ShieldHp <= 0)
            {
                ShieldHp = 0;
            }

            yield return new WaitForSeconds(HitCoolTime);
            IsHit = false;
            yield break;
        }

        if (Elect > 0)
        {
            Elect -= damage;
            ui_0_HUD.ElectDel();

            if (Elect <= 0)
            {
                Elect = 0;
                ElectShieldExplode();
            }

            yield return new WaitForSeconds(HitCoolTime);
            IsHit = false;
            yield break;
        }

        if (TemHp > 0)
        {
            TemHp -= damage;
            ui_0_HUD.HpSet();

            if (TemHp <= 0)
            {
                ui_0_HUD.TemHpDel();
                TemHp = 0;
            }

            yield return new WaitForSeconds(HitCoolTime);
            IsHit = false;
            yield break;
        }

        if (Shield > 0 && Shield * 3 >= CurrentHp)
        {
            Shield -= 1;
            ui_0_HUD.ShiledOff();

            if (Shield <= 0)
            {
                Shield = 0;
            }

            yield return new WaitForSeconds(HitCoolTime);
            IsHit = false;
            yield break;
        }


        if (CurrentHp > 0)
        {
            CurrentHp -= damage;
            ui_0_HUD.HpSet();

            if (CurrentHp <= 0)
            {
                Die(); // 사망 처리
            }

            yield return new WaitForSeconds(HitCoolTime);
        }

        IsHit = false;
    }

    public void SetSpeed(float newSpeed)
    {
        this.MoveSpeed += newSpeed;
    }

    public void CoinUp(int coinNum)
    {
        Coin += coinNum;
    }

    public void ElectUp(int electNum)
    {
        Elect += electNum;
    }

    public void ShieldHpUp(int shieldNum)
    {
        ShieldHp += shieldNum;
    }

    public void TemHpUp(int temHpNum)
    {
        TemHp += temHpNum;
        ui_0_HUD.TemHpSet();
    }

    public void Heal(int healNum)
    {
        if (healing_coroutine != null)
        {
            StopCoroutine(HealingCoroutine(healNum));
        }
        healing_coroutine = StartCoroutine(HealingCoroutine(healNum));

        Debug.Log("HP+");
    }

    private IEnumerator HealingCoroutine(int healNum)
    {
        for (int i = 0; i < healNum; i++)
        {
            if (CurrentHp < MaxHp)
            {
                CurrentHp += 1;
                CurrentHp = Mathf.Min(CurrentHp, MaxHp);
            }
            yield return new WaitForSeconds(HealCoolTime);
        }

        healing_coroutine = null;
    }

    private void Die()
    {
        uiManager.PlayerIsDead();
    }

    public void ElectShieldExplode()
    {
        Debug.Log("ElectShieldExplode Is Not Defined");
    }

    public void MaxStorageUp(int Value)
    {
        MaxStorage += Value;
    }
}
