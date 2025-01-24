using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramManager : MonoBehaviour
{
    #region Manager

    public static ProgramManager Instance;
    private PoolingManager PInstance;
    private StatusManager statusManager;
    private Weapon weapon;

    #endregion

    #region Other Variables

    // 내가 가지고 있는 프로그램 리스트
    public List<PInformation> ProgramList = new List<PInformation>();
    [Header("유니티 관련")]
    [SerializeField]
    Weapon playerWeapon;

    #endregion

    #region Material

    [Header("어택이펙트 관련")]
    [SerializeField]
    Material bulletMa;
    [SerializeField]
    Material monBulletMa;

    #endregion

    #region Default Function

    private void Awake()
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

    private void Start()
    {
        PInstance = PoolingManager.Instance;
        statusManager = new StatusManager();
    }
    private void OnDisable()
    {
        if (bulletMa != null)
        {
            Color playerColor = bulletMa.color;
            playerColor.a = 1f; // 알파값 수정
            bulletMa.color = playerColor; // 색상에 알파값 반영
        }
        if (monBulletMa != null)
        {
            Color monsterColor = monBulletMa.color;
            monsterColor.a = 1f; // 알파값 수정
            monBulletMa.color = monsterColor; // 색상에 알파값 반영
        }
    }

    #endregion

    #region Add/Remove Program

    public void AddProgramList(PInformation NewProgram)
    {
        ProgramList.Add(NewProgram);

        if (statusManager == null)
        {
            Debug.Log("statusManager is not find");
            return;
        }

        statusManager.CoinUp(NewProgram.AddCoin);
        statusManager.AttackPower += NewProgram.AttackPower;
        statusManager.AttackSpeed += NewProgram.AttackSpeed;
        statusManager.SetSpeed(NewProgram.MoveSpeed);
        statusManager.BulletSpeed += NewProgram.BulletSpeed;

        if (NewProgram.HPHeal != 0)
        {
            statusManager.Heal(NewProgram.HPHeal);
        }


        //퍼센트 만큼 올리기
        if (NewProgram.AttackPerUp != 0)
        {
            statusManager.AttackPower += statusManager.AttackPower * NewProgram.AttackPerUp;
        }
        if (NewProgram.AttackSpeedPerUp != 0)
        {
            statusManager.AttackSpeed += statusManager.AttackSpeed * NewProgram.AttackSpeedPerUp;
        }
        if (NewProgram.MoveSpeedPerUp != 0)
        {
            statusManager.MoveSpeed += statusManager.MoveSpeed * NewProgram.MoveSpeedPerUp;
        }
        if (NewProgram.bulletSpeedPerUp != 0)
        {
            statusManager.BulletSpeed += statusManager.BulletSpeed * NewProgram.bulletSpeedPerUp;
        }
        //퍼센트만큼 깎기
        if (NewProgram.AttackPerDown != 0)
        {
            statusManager.AttackPower *= (1 - NewProgram.AttackPerDown);
        }
        if (NewProgram.AttackSpeedPerDown != 0)
        {
            statusManager.AttackSpeed *= (1 - NewProgram.AttackSpeedPerDown);
        }
        if (NewProgram.MoveSpeedPerDown != 0)
        {
            statusManager.MoveSpeed *= (1 - NewProgram.MoveSpeedPerDown);
        }
        if (NewProgram.bulletSpeedPerDown != 0)
        {
            statusManager.BulletSpeed *= (1 - NewProgram.bulletSpeedPerDown);
        }
        //퍼센트로 만들기
        if (NewProgram.SetAttackPer != 0)
        {
            statusManager.AttackPower *= NewProgram.SetAttackPer;
        }
        if (NewProgram.SetAttackSpeedPer != 0)
        {
            statusManager.AttackSpeed *= NewProgram.SetAttackSpeedPer;
        }
        if (NewProgram.SetMoveSpeedPer != 0)
        {
            statusManager.MoveSpeed *= NewProgram.SetMoveSpeedPer;
        }
        if (NewProgram.SetbulletSpeedPer != 0)
        {
            statusManager.BulletSpeed *= NewProgram.SetbulletSpeedPer;
        }
        //총알크기 변경
        if (NewProgram.BulletScalePerUP != 0)
        {
            switch (playerWeapon.weaponType)
            {
                case Weapon.WeaponType.BasicWeapon:
                    Bullet changeBullet = PInstance.bulletList[0].GetComponent<Bullet>();
                    // 기존 크기에 BulletScalePerUP 비율만큼 크기를 증가
                    Vector3 currentScale = changeBullet.transform.localScale;
                    float scaleFactor = 1 + NewProgram.BulletScalePerUP; // 1 + 0.2 => 1.2배
                    changeBullet.transform.localScale = new Vector3(
                        currentScale.x * scaleFactor,
                        currentScale.y * scaleFactor,
                        currentScale.z * scaleFactor
                    );
                    PInstance.ReMakeBullet(0);
                    break;
            }

        }
        if (NewProgram.BulletScalePerDown != 0)
        {
            switch (playerWeapon.weaponType)
            {
                case Weapon.WeaponType.BasicWeapon:
                    Bullet changeBullet = PInstance.bulletList[0].GetComponent<Bullet>();

                    // 기존 크기에 BulletScalePerUP 비율만큼 크기를 증가
                    Vector3 currentScale = changeBullet.transform.localScale;
                    float scaleFactor = 1 - NewProgram.BulletScalePerDown; // 1 + 0.2 => 1.2배
                    changeBullet.transform.localScale = new Vector3(
                        currentScale.x * scaleFactor,
                        currentScale.y * scaleFactor,
                        currentScale.z * scaleFactor
                    );
                    PInstance.ReMakeBullet(0);
                    break;
            }

        }
        if (NewProgram.SetBulletScalePer != 0)
        {
            switch (playerWeapon.weaponType)
            {
                case Weapon.WeaponType.BasicWeapon:
                    Bullet changeBullet = PInstance.bulletList[0].GetComponent<Bullet>();

                    // 기존 크기에 BulletScalePerUP 비율만큼 크기를 증가
                    Vector3 currentScale = changeBullet.transform.localScale;
                    float scaleFactor = NewProgram.SetBulletScalePer;
                    changeBullet.transform.localScale = new Vector3(
                        currentScale.x * scaleFactor,
                        currentScale.y * scaleFactor,
                        currentScale.z * scaleFactor
                    );
                    PInstance.ReMakeBullet(0);
                    break;
            }

        }

        //알파값 바꿔서 변경하기
        if (NewProgram.ProgramName =="어택 이펙트")
        {
            if (bulletMa != null)
            {
                Color playerColor = bulletMa.color;
                playerColor.a = 0.4f; // 알파값 수정
                bulletMa.color = playerColor; // 색상에 알파값 반영
            }
            if (monBulletMa != null)
            {
                Color monsterColor = monBulletMa.color;
                monsterColor.a = 0.2f; // 알파값 수정
                monBulletMa.color = monsterColor; // 색상에 알파값 반영
            }
        }

        // Debug.Log("AddProgram");
    }

    public void RemoveProgram(int ProgramNumber)
    {
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();

        if (statusManager == null)
        {
            Debug.Log("statusManager is not find");
            return;
        }

        statusManager.AttackPower -= ProgramList[ProgramNumber].AttackPower;
        statusManager.AttackSpeed += -ProgramList[ProgramNumber].AttackSpeed;
        statusManager.SetSpeed(-ProgramList[ProgramNumber].MoveSpeed);
        statusManager.BulletSpeed -= ProgramList[ProgramNumber].BulletSpeed;

         //퍼센트 올린만큼 내리기
        if (ProgramList[ProgramNumber].AttackPerUp != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.AttackPower -= statusManager.AttackPower * ProgramList[ProgramNumber].AttackPerUp;
            }
        }
        if (ProgramList[ProgramNumber].AttackSpeedPerUp != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.AttackSpeed -= statusManager.AttackSpeed * ProgramList[ProgramNumber].AttackSpeedPerUp;
            }
        }
        if (ProgramList[ProgramNumber].MoveSpeedPerUp != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.MoveSpeed -= statusManager.MoveSpeed * ProgramList[ProgramNumber].MoveSpeedPerUp;
            }
        }
        if (ProgramList[ProgramNumber].bulletSpeedPerUp != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.BulletSpeed -= statusManager.BulletSpeed * ProgramList[ProgramNumber].bulletSpeedPerUp;
            }
        }
        //퍼센트 내린만큼 올리기
        if (ProgramList[ProgramNumber].AttackPerDown != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.AttackPower /= (1 - ProgramList[ProgramNumber].AttackPerDown);
            }
        }
        if (ProgramList[ProgramNumber].AttackSpeedPerDown != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.AttackSpeed /= (1 - ProgramList[ProgramNumber].AttackSpeedPerDown);
            }
        }
        if (ProgramList[ProgramNumber].MoveSpeedPerDown != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.MoveSpeed /= (1 - ProgramList[ProgramNumber].MoveSpeedPerDown);
            }
        }
        if (ProgramList[ProgramNumber].bulletSpeedPerDown != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.BulletSpeed /= (1 - ProgramList[ProgramNumber].bulletSpeedPerDown);
            }
        }
        //퍼센트 된거 돌리기
        if (ProgramList[ProgramNumber].SetAttackPer != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.AttackPower /=  ProgramList[ProgramNumber].SetAttackPer;
            }
        }
        if (ProgramList[ProgramNumber].SetAttackSpeedPer != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.AttackSpeed /= ProgramList[ProgramNumber].SetAttackSpeedPer;
            }
        }
        if (ProgramList[ProgramNumber].SetbulletSpeedPer != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.BulletSpeed /= ProgramList[ProgramNumber].SetbulletSpeedPer;
            }
        }
        if (ProgramList[ProgramNumber].SetMoveSpeedPer != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.MoveSpeed /=  ProgramList[ProgramNumber].SetMoveSpeedPer;
            }
        }

        if (ProgramList[ProgramNumber].ProgramName == "어택 이펙트")
        {
            if (bulletMa != null)
            {
                Color playerColor = bulletMa.color;
                playerColor.a = 1f; // 알파값 수정
                bulletMa.color = playerColor; // 색상에 알파값 반영
            }
            if (monBulletMa != null)
            {
                Color monsterColor = monBulletMa.color;
                monsterColor.a = 1f; // 알파값 수정
                monBulletMa.color = monsterColor; // 색상에 알파값 반영
            }
        }
        ProgramList.RemoveAt(ProgramNumber);
    }

    #endregion

}
