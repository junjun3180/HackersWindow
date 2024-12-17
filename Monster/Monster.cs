using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    //public enum MonsterType
    //{
    //    M_V1,
    //    M_V2,
    //    M_V3,
    //    M_CardPack,
    //    M_VE_1,
    //    M_VE_2,
    //    M_SpiderCardPack,
    //    Red_Spider,
    //    White_Spider
    //}
    //// 몬스터 기본정보
    //public MonsterType monsterType; // 몬스터 타입
    //public float MoveSpeed;
    //public float AttackPower;
    //public float HP;
    //public float WaittingTime; // 탐색 쿨타임
    //public float AttackDelayTime; // 공격 준비 애니메이션 시간
    //public float AttackCoolTime; // 공격 쿨타임
    //public float DetectingAreaR; // 탐색 범위 반지름
    //private bool isMoving = true; // 이동 여부를 제어

    //private Transform player; // 플레이어의 위치
    //private Vector3 targetPosition; // 탐지된 플레이어의 위치 저장
    //private bool DetectionSuccess = false; // 탐지 성공 여부

    //// 공격 관련 변수들
    //public GameObject bulletPrefab;  // 발사할 총알 프리팹
    //public Transform firePoint;      // 총알 발사 위치
    //public float bulletSpeed = 10f;  // 총알 속도

    //public GameObject CardPrefab1;  // 발사할 총알 프리팹
    //public GameObject CardPrefab2;  // 발사할 총알 프리팹
    //public GameObject CardPrefab3;  // 발사할 총알 프리팹
    //public GameObject CardPrefab4;  // 발사할 총알 프리팹

    //public GameObject SpiderPrefab1;
    //public GameObject SpiderPrefab2;
    //public GameObject SpiderPrefab3;
    //public GameObject SpiderPrefab4;

    //// 기타 변수
    //Animator animator; //애니메이터
    //private GameManager gameManager;
    //private SpriteRenderer spriteRenderer;
    //private StatusManager statusManager;

    //void Start()
    //{
    //    gameManager = GameManager.Instance;
    //    statusManager = StatusManager.Instance;
    //    spriteRenderer = GetComponent<SpriteRenderer>();
    //    if (monsterType != MonsterType.M_SpiderCardPack)
    //        animator = GetComponent<Animator>();

    //    if (monsterType == MonsterType.M_SpiderCardPack)
    //    {
    //        animator = GetComponentInChildren<Animator>();
    //    }
    //    // Debug.Log("몬스터 확인, 루틴 시작");
    //    StartCoroutine(MonsterRoutine());
    //}

    //void Update()
    //{

    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        if (statusManager != null)
    //        {
    //            Debug.Log("Monster OnTriggerEnter2D : Take Damage");
    //            // statusManager.TakeDamage(AttackPower, monsterType);
    //        }
    //        else
    //        {
    //            Debug.Log("Monster OnTriggerEnter2D : statusManager Not Found");
    //        }
    //    }
    //}

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Wall"))
    //    {
    //        Debug.Log("There Is Wall");
    //        isMoving = false;  // 충돌 시 이동 멈춤
    //        animator.SetBool("Move", false);
    //    }
    //}

    //private IEnumerator MonsterRoutine()
    //{
    //    while (true)
    //    {
    //        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(transform.position, DetectingAreaR);

    //        foreach (Collider2D obj in detectedObjects)
    //        {
    //            if (obj.CompareTag("Player"))
    //            {
    //                player = obj.transform;
    //                // Debug.Log("플레이어 탐색 성공");

    //                targetPosition = player.position;
    //                DetectionSuccess = true;
    //                break;
    //            }
    //        }

    //        if (DetectionSuccess)
    //        {
    //            yield return AttackPreparation();
    //            DetectionSuccess = false;
    //        }
    //        else
    //        {
    //            isMoving = true;
    //            // Debug.Log("플레이어 탐색실패");

    //            switch (monsterType)
    //            {
    //                case MonsterType.M_V1:
    //                    yield return new WaitForSeconds(WaittingTime);
    //                    yield return RandomMoveAfterSearchFail();
    //                    animator.SetBool("Move", false);
    //                    break;
    //                case MonsterType.M_V2:
    //                case MonsterType.M_V3:
    //                case MonsterType.White_Spider:
    //                case MonsterType.Red_Spider:
    //                    yield return new WaitForSeconds(WaittingTime);
    //                    yield return RandomMoveAfterSearchFail();
    //                    break;
    //                case MonsterType.M_CardPack:
    //                case MonsterType.M_VE_1:
    //                case MonsterType.M_VE_2:
    //                    yield return new WaitForSeconds(WaittingTime);
    //                    break;
    //                case MonsterType.M_SpiderCardPack:
    //                    yield return AttackPreparation(); // 무조건 미니 거미 소환
    //                    break;
    //            }

    //            if (monsterType == MonsterType.M_V1)
    //            {
    //            }
    //            else if (monsterType == MonsterType.M_V2)
    //            {
    //            }
    //            else if (monsterType == MonsterType.M_V3)
    //            {
    //            }
    //            else if (monsterType == MonsterType.M_CardPack)
    //            {
    //            }
    //            else if (monsterType == MonsterType.M_VE_1)
    //            {
    //            }
    //            else if (monsterType == MonsterType.M_VE_2)
    //            {
    //            }
    //            else if (monsterType == MonsterType.M_SpiderCardPack)
    //            {
    //            }
    //        }
    //    }
    //}

    //// 공격 준비 애니메이션 처리
    //private IEnumerator AttackPreparation()
    //{
    //    // 방향 설정
    //    if (targetPosition.x > transform.position.x)
    //    {
    //        spriteRenderer.flipX = true;   // 왼쪽을 바라봄 (Flip X 활성화)
    //    }
    //    else if (targetPosition.x < transform.position.x)
    //    {
    //        spriteRenderer.flipX = false;  // 오른쪽을 바라봄 (Flip X 비활성화)
    //    }

    //    // Debug.Log("공격 준비중");

    //    if (monsterType == MonsterType.M_V1)
    //    {
    //        yield return new WaitForSeconds(AttackDelayTime);

    //        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
    //        {
    //            animator.SetBool("Move", true);
    //            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
    //            yield return null;
    //        }
    //        animator.SetBool("Move", false);
    //    }
    //    else if (monsterType == MonsterType.M_V2)
    //    {
    //        animator.SetTrigger("Shot");
    //        yield return new WaitForSeconds(AttackDelayTime);

    //        Fire();
    //    }
    //    else if (monsterType == MonsterType.M_V3)
    //    {
    //        // 직선이동
    //        animator.SetTrigger("Jump");
    //        yield return new WaitForSeconds(1.7f);
    //        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
    //        {
    //            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
    //            yield return null;
    //        }
    //    }
    //    else if (monsterType == MonsterType.M_CardPack)
    //    {
    //        // 플레이어 바라보기
    //        Vector3 direction = targetPosition - transform.position;
    //        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

    //        // 공격 처리
    //        yield return CardFire();
    //    }
    //    else if (monsterType == MonsterType.M_VE_1)
    //    {
    //        if (!animator.GetBool("Detected"))
    //        {
    //            animator.SetBool("Detected", true);
    //            yield return new WaitForSeconds(1.30f);

    //            DetectingAreaR = 30;

    //            WaittingTime = 0;
    //            AttackDelayTime = 0;
    //            AttackCoolTime = 0;
    //        }

    //        // 실시간 추적
    //        transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
    //        yield return null;
    //    }
    //    else if (monsterType == MonsterType.M_VE_2)
    //    {
    //        if (!animator.GetBool("Detected"))
    //        {
    //            animator.SetBool("Detected", true);
    //            yield return new WaitForSeconds(1.30f);

    //            WaittingTime = 1;
    //            AttackDelayTime = 1;
    //            AttackCoolTime = 1;
    //        }

    //        animator.SetTrigger("Shot");
    //        yield return new WaitForSeconds(1.05f);
    //        Fire();
    //    }
    //    else if (monsterType == MonsterType.M_SpiderCardPack)
    //    {
    //        int SpiderColor = Random.Range(0, 4); // 0, 1 흰색. 2, 3 빨강색
    //        switch (SpiderColor)
    //        {
    //            case 0:
    //            case 1:
    //                animator.SetTrigger("White_Spawn");
    //                Debug.Log("흰색 거미 소환!!");
    //                break;
    //            case 2:
    //            case 3:
    //                animator.SetTrigger("Red_Spawn");
    //                Debug.Log("빨간색 거미 소환!!");
    //                break;
    //        }
    //        // Animation Lodding
    //        yield return new WaitForSeconds(10.6f);

    //        switch (SpiderColor)
    //        {
    //            case 0:
    //                Instantiate(SpiderPrefab1, firePoint.position, firePoint.rotation);
    //                break;
    //            case 1:
    //                Instantiate(SpiderPrefab2, firePoint.position, firePoint.rotation);
    //                break;
    //            case 2:
    //                Instantiate(SpiderPrefab3, firePoint.position, firePoint.rotation);
    //                break;
    //            case 3:
    //                Instantiate(SpiderPrefab4, firePoint.position, firePoint.rotation);
    //                break;
    //        }
    //    }
    //    else if (monsterType == MonsterType.Red_Spider)
    //    {
    //        animator.SetTrigger("Move");
    //        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
    //        {
    //            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
    //            yield return null;
    //        }
    //    }
    //    else if (monsterType == MonsterType.White_Spider)
    //    {
    //        animator.SetTrigger("Jump");
    //        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
    //        {
    //            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
    //            yield return null;
    //        }
    //    }

    //    // Debug.Log("Attack End, CoolTime Start");
    //    yield return new WaitForSeconds(AttackCoolTime);
    //}

    //IEnumerator RandomMoveAfterSearchFail()
    //{
    //    targetPosition = GetRanomPositionAround();

    //    // 방향 설정
    //    if (targetPosition.x > transform.position.x)
    //    {
    //        spriteRenderer.flipX = true;   // 왼쪽을 바라봄 (Flip X 활성화)
    //    }
    //    else if (targetPosition.x < transform.position.x)
    //    {
    //        spriteRenderer.flipX = false;  // 오른쪽을 바라봄 (Flip X 비활성화)
    //    }

    //    // 이동 시작
    //    if (monsterType == MonsterType.M_V1)
    //    {
    //        while (Vector3.Distance(transform.position, targetPosition) > 0.1f && isMoving)
    //        {
    //            animator.SetBool("Move", true);
    //            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
    //            yield return null;  // 한 프레임 대기
    //        }
    //    }
    //    else if (monsterType == MonsterType.M_V2)
    //    {
    //        while (Vector3.Distance(transform.position, targetPosition) > 0.1f && isMoving)
    //        {
    //            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
    //            yield return null;
    //        }
    //    }
    //    else if (monsterType == MonsterType.M_V3)
    //    {
    //        animator.SetTrigger("Jump");
    //        yield return new WaitForSeconds(1.7f);
    //        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
    //        {
    //            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
    //            yield return null;
    //        }
    //    }
    //    else if (monsterType == MonsterType.M_CardPack)
    //    {
    //        while (Vector3.Distance(transform.position, targetPosition) > 0.1f && isMoving)
    //        {
    //            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
    //            yield return null;
    //        }
    //    }
    //    else if (monsterType == MonsterType.Red_Spider || monsterType == MonsterType.White_Spider)
    //    {
    //        if (monsterType == MonsterType.Red_Spider)
    //            animator.SetTrigger("Move");
    //        else
    //            animator.SetTrigger("Jump");

    //        while (Vector3.Distance(transform.position, targetPosition) > 0.1f && isMoving)
    //        {
    //            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
    //            yield return null;
    //        }
    //    }
    //    // Debug.Log("랜덤 위치로 이동 완료");
    //}

    //Vector3 GetRanomPositionAround()
    //{
    //    float randomAngle = Random.Range(0f, 360f);
    //    float randomDistance = Random.Range(0f, DetectingAreaR);

    //    float x = transform.position.x + randomDistance * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
    //    float y = transform.position.y + randomDistance * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

    //    return new Vector3(x, y, 0);
    //}


    //// 공격 로직
    //void Fire()
    //{
    //    Debug.Log("총알 발사~ ");
    //    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    //    Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

    //    Vector2 fireDirection = (targetPosition - firePoint.position).normalized;

    //    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
    //    rb.velocity = fireDirection * bulletSpeed;
    //}

    //IEnumerator CardFire()
    //{
    //    int CardNum = Random.Range(0, 4);

    //    /*
    //     * 2초동안 장착 애니메이션 나옴. -> 발사 -> 바로 대기 애니.
    //        4개중에 하나 클로버 하트 스페이드 다이아
    //        스페이드 다이아는 빠르게 발사 한발.
    //     */

    //    Vector3 direction = targetPosition - firePoint.position;
    //    float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    //    Debug.Log("CardNum Animation");
    //    switch (CardNum)
    //    {
    //        case 0:
    //            animator.SetTrigger("Hearth");
    //            break;
    //        case 1:
    //            animator.SetTrigger("Clover");
    //            break;
    //        case 2:
    //            animator.SetTrigger("Spade");
    //            break;
    //        case 3:
    //            animator.SetTrigger("Dia");
    //            break;
    //        default:
    //            Debug.Log("CardNum Error");
    //            break;

    //    }


    //    yield return new WaitForSeconds(1.5f);
    //    if (CardNum == 0 || CardNum == 1)
    //    {
    //        float[] angles = { baseAngle - 30, baseAngle, baseAngle + 30 };

    //        foreach (float angle in angles)
    //        {

    //            GameObject bullet = Instantiate((CardNum == 0 ? CardPrefab1 : CardPrefab2), firePoint.position, Quaternion.Euler(0, 0, angle));
    //            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

    //            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
    //            bullet.transform.Rotate(0, 0, 90);
    //            if (rb != null)
    //            {
    //                Vector2 fireDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    //                rb.velocity = fireDirection * bulletSpeed;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        GameObject bullet = Instantiate((CardNum == 2 ? CardPrefab3 : CardPrefab4), firePoint.position, Quaternion.Euler(0, 0, baseAngle));
    //        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

    //        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
    //        bullet.transform.Rotate(0, 0, 90);
    //        if (rb != null)
    //        {
    //            Vector2 fireDirection = (targetPosition - firePoint.position).normalized;
    //            rb.velocity = fireDirection * bulletSpeed;
    //        }
    //    }
    //}


    //// ========== 탐색범위 표시용(삭제예정) ==========
    //// 탐지 범위를 시각적으로 표시 (에디터 전용)
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, DetectingAreaR);
    //}

}
