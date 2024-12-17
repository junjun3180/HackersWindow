using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProgramManager : MonoBehaviour
{
    public static ProgramManager Instance;
    private PoolingManager PInstance;
    private StatusManager statusManager;
    private Weapon weapon;

    // ���� ������ �ִ� ���α׷� ����Ʈ
    public List<PInformation> ProgramList = new List<PInformation>();
    [Header("��������Ʈ ����")]
    [SerializeField]
    Material bulletMa;
    [SerializeField]
    Material monBulletMa;
    [SerializeField]
    float interval;
    [Header("����Ƽ ����")]
    [SerializeField]
    Weapon playerWeapon;
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
    }
    private void OnDisable()
    {
        if (bulletMa != null)
        {
            Color playerColor = bulletMa.color;
            playerColor.a = 1f; // ���İ� ����
            bulletMa.color = playerColor; // ���� ���İ� �ݿ�
        }
        if (monBulletMa != null)
        {
            Color monsterColor = monBulletMa.color;
            monsterColor.a = 1f; // ���İ� ����
            monBulletMa.color = monsterColor; // ���� ���İ� �ݿ�
        }
    }
    public void AddProgramList(PInformation NewProgram)
    {
        ProgramList.Add(NewProgram);

        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        statusManager = StatusManager.Instance;

        if (statusManager == null)
        {
            Debug.Log("statusManager is not find");
            return;
        }

        if (NewProgram.AddCoin != 0)
        {
            statusManager.CoinUp(NewProgram.AddCoin);
        }

        if (NewProgram.HPHeal != 0)
        {
            statusManager.Heal(NewProgram.HPHeal);
        }
        if (NewProgram.AttackPower != 0)
        {
            
            if (PInstance != null)
            {
                //PInstance.RefreshBulletDamage(NewProgram.AttackPower);
                statusManager.AttackPower += NewProgram.AttackPower;
            }
        }

        if (NewProgram.AttackSpeed != 0)
        {
            if (player != null)
            {
                weapon = player.GetWeapon();

                if (weapon != null)
                {
                    statusManager.AttackSpeed += NewProgram.AttackSpeed;
                    //weapon.SetAttackSpeed(NewProgram.AttackSpeed);
                }
            }
        }

        if (NewProgram.MoveSpeed != 0)
        {
            statusManager.SetSpeed(NewProgram.MoveSpeed);
        }

        if (NewProgram.BulletSpeed != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.BulletSpeed += NewProgram.BulletSpeed;
            }
        }
        //�ۼ�Ʈ ��ŭ �ø���
        if (NewProgram.AttackPerUp != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.AttackPower += statusManager.AttackPower*NewProgram.AttackPerUp;
            }
        }
        if (NewProgram.AttackSpeedPerUp != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.AttackSpeed += statusManager.AttackSpeed * NewProgram.AttackSpeedPerUp;
            }
        }
        if (NewProgram.MoveSpeedPerUp != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.MoveSpeed += statusManager.MoveSpeed * NewProgram.MoveSpeedPerUp;
            }
        }
        if (NewProgram.bulletSpeedPerUp != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.BulletSpeed += statusManager.BulletSpeed * NewProgram.bulletSpeedPerUp;
            }
        }
        //�ۼ�Ʈ��ŭ ���
        if (NewProgram.AttackPerDown != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.AttackPower *= ( 1 - NewProgram.AttackPerDown);
            }
        }
        if (NewProgram.AttackSpeedPerDown != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.AttackSpeed *= (1 - NewProgram.AttackSpeedPerDown);
            }
        }
        if (NewProgram.MoveSpeedPerDown != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.MoveSpeed *= (1 - NewProgram.MoveSpeedPerDown);
            }
        }
        if (NewProgram.bulletSpeedPerDown != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.BulletSpeed *= (1 - NewProgram.bulletSpeedPerDown);
            }
        }
        //�ۼ�Ʈ�� �����
        if (NewProgram.SetAttackPer != 0)
        {
            if (PInstance != null)
            {
                
                statusManager.AttackPower *= NewProgram.SetAttackPer;
            }
        }
        if (NewProgram.SetAttackSpeedPer != 0)
        {
            

            if (PInstance != null)
            {
                statusManager.AttackSpeed *= NewProgram.SetAttackSpeedPer;
            }
        }
        if (NewProgram.SetMoveSpeedPer != 0)
        {
            if (PInstance != null)
            {
                statusManager.MoveSpeed *= NewProgram.SetMoveSpeedPer;
            }
        }
        if (NewProgram.SetbulletSpeedPer != 0)
        {


            if (PInstance != null)
            {
                statusManager.BulletSpeed *= NewProgram.SetbulletSpeedPer;
            }
        }
        //�Ѿ�ũ�� ����
        if (NewProgram.BulletScalePerUP != 0)
        {
            switch (playerWeapon.weaponType)
            {
                case Weapon.WeaponType.BasicWeapon:
                    Bullet changeBullet = PInstance.bulletList[0].GetComponent<Bullet>();
                    // ���� ũ�⿡ BulletScalePerUP ������ŭ ũ�⸦ ����
                    Vector3 currentScale = changeBullet.transform.localScale;
                    float scaleFactor = 1 + NewProgram.BulletScalePerUP; // 1 + 0.2 => 1.2��
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

                    // ���� ũ�⿡ BulletScalePerUP ������ŭ ũ�⸦ ����
                    Vector3 currentScale = changeBullet.transform.localScale;
                    float scaleFactor = 1 - NewProgram.BulletScalePerDown; // 1 + 0.2 => 1.2��
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

                    // ���� ũ�⿡ BulletScalePerUP ������ŭ ũ�⸦ ����
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
        //���İ� �ٲ㼭 �����ϱ�
        if (NewProgram.ProgramName =="���� ����Ʈ")
        {
            if (bulletMa != null)
            {
                Color playerColor = bulletMa.color;
                playerColor.a = 0.4f; // ���İ� ����
                bulletMa.color = playerColor; // ���� ���İ� �ݿ�
            }
            if (monBulletMa != null)
            {
                Color monsterColor = monBulletMa.color;
                monsterColor.a = 0.2f; // ���İ� ����
                monBulletMa.color = monsterColor; // ���� ���İ� �ݿ�
            }
        }

        // Debug.Log("AddProgram");
    }

    public void RemoveProgram(int ProgramNumber)
    {
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        statusManager = StatusManager.Instance;

        if (statusManager == null)
        {
            Debug.Log("statusManager is not find");
            return;
        }

        if (ProgramList[ProgramNumber].AttackPower != 0)
        {
            

            if (Instance != null)
            {
                statusManager.AttackPower -= ProgramList[ProgramNumber].AttackPower;
            }
        }

        if (ProgramList[ProgramNumber].AttackSpeed != 0)
        {

            if (player != null)
            {
                weapon = player.GetWeapon();

                if (weapon != null)
                {
                    weapon.SetAttackSpeed(-ProgramList[ProgramNumber].AttackSpeed);
                }
            }
        }

        if (ProgramList[ProgramNumber].MoveSpeed != 0)
        {
            statusManager.SetSpeed(-ProgramList[ProgramNumber].MoveSpeed);
        }

        if (ProgramList[ProgramNumber].BulletSpeed != 0)
        {
            

            if (Instance != null)
            {
                statusManager.BulletSpeed -= ProgramList[ProgramNumber].BulletSpeed;
            }
        }
         //�ۼ�Ʈ �ø���ŭ ������
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
        //�ۼ�Ʈ ������ŭ �ø���
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
        //�ۼ�Ʈ �Ȱ� ������
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

        if (ProgramList[ProgramNumber].ProgramName == "���� ����Ʈ")
        {
            if (bulletMa != null)
            {
                Color playerColor = bulletMa.color;
                playerColor.a = 1f; // ���İ� ����
                bulletMa.color = playerColor; // ���� ���İ� �ݿ�
            }
            if (monBulletMa != null)
            {
                Color monsterColor = monBulletMa.color;
                monsterColor.a = 1f; // ���İ� ����
                monBulletMa.color = monsterColor; // ���� ���İ� �ݿ�
            }
        }
        ProgramList.RemoveAt(ProgramNumber);
    }
    /*private IEnumerator AttackEffect_co()
    {
        //�ڷ�ƾ���۵�
        // Bullet ������Ʈ�� ���� ���׸����� ��������

        while (true) // ���� ����
        {
            Debug.Log("�������������");
            // ���� ���� 0.5�� 1 ���̷� �����ϰ� ����
            float alpha = Random.Range(1, 10) * 0.1f;

            // ���� ���� (���� ���İ� ����)
            Color color = bulletMa.color;
            color.a = alpha;
            bulletMa.color = color;


            // ���� ������� ��� (interval��ŭ ���)
            yield return new WaitForSeconds(interval);
        }
    } */
    
}