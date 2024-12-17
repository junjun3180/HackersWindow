using System.Collections;
using UnityEngine;

public class M_CardPack : MonsterBase
{
    public Transform FirePoint;      // 총알 발사 위치
    public float BulletSpeed = 10f;  // 총알 속도
    private int CardNum;
    public float AttackCoolTime;

    // 프리팹 리스트 
    public GameObject CardPrefab1;
    public GameObject CardPrefab2;
    public GameObject CardPrefab3;
    public GameObject CardPrefab4;

    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.M_CardPack;
        StartCoroutine(MonsterRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override IEnumerator MonsterRoutine()
    {
        while (true)
        {
            if (!DetectionSuccess && DetectionPlayerPosition())
                DetectionSuccess = true;

            if (DetectionSuccess)
            {
                DetectingAreaR = 15.0f;
                yield return AttackPreparation();
            }

            yield return null;
        }
    }

    public override IEnumerator AttackPreparation()
    {
        /*
         * 2초동안 장착 애니메이션 나옴. -> 발사 -> 바로 대기 애니.
            4개중에 하나 클로버 하트 스페이드 다이아
            스페이드 다이아는 빠르게 발사 한발.
         */
        CardNum = Random.Range(0, 4);
        switch (CardNum)
        {
            case 0:
                MAnimator.SetTrigger("Hearth");
                break;
            case 1:
                MAnimator.SetTrigger("Clover");
                break;
            case 2:
                MAnimator.SetTrigger("Spade");
                break;
            case 3:
                MAnimator.SetTrigger("Dia");
                break;
            default:
                Debug.Log("CardNum Error");
                break;
        }

        float preparationTime = 1.5f; // 애니메이션 대기 시간
        float elapsedTime = 0f;

        while (elapsedTime < preparationTime)
        {
            DetectionPlayerPosition();
            Vector3 direction = TargetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        yield return Fire();

        yield return new WaitForSeconds(AttackCoolTime);
    }

    private IEnumerator Fire()
    {
        Vector3 direction = TargetPosition - FirePoint.position;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (CardNum == 0 || CardNum == 1)
        {
            float[] angles = { baseAngle - 30, baseAngle, baseAngle + 30 };

            foreach (float angle in angles)
            {

                GameObject bullet = Instantiate((CardNum == 0 ? CardPrefab1 : CardPrefab2), FirePoint.position, Quaternion.Euler(0, 0, angle));
                Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                bullet.transform.Rotate(0, 0, 90);
                if (rb != null)
                {
                    Vector2 fireDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                    rb.velocity = fireDirection * (BulletSpeed*2/5);
                }
            }
        }
        else
        {
            GameObject bullet = Instantiate((CardNum == 2 ? CardPrefab3 : CardPrefab4), FirePoint.position, Quaternion.Euler(0, 0, baseAngle));
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            bullet.transform.Rotate(0, 0, 90);
            if (rb != null)
            {
                Vector2 fireDirection = (TargetPosition - FirePoint.position).normalized;
                rb.velocity = fireDirection * BulletSpeed;
            }
        }
        yield return null;
    }
}
