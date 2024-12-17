using System.Collections;
using UnityEngine;

public class Boss_Mouse : MonsterBase
{
    // 패턴 추가 시 Heal 앞에 넣을 것.
    public enum Pattern { Idle, Fist, Click, Gaurd, Heal };
    int pattern = 0; // 시작은 Idle패턴
    private int NextPattern;
    public float PatternCoolTime;

    // Idel Pattern
    public float IdelDurationTime;

    // Fist Pattern
    public float DashSpeed = 20f;
    public float DashDuration = 0.5f;
    private bool isDashing = false;

    // Click Pattern
    public float ClickAttackDuration;
    public GameObject BulletPrefab;
    public Transform FirePoint;
    public float BulletSpeed = 10f;

    // Gaurd Pattern
    public float GaurdMoveSpeed;

    // Heal Pattern
    public bool HealPatternReady = true;
    private float rotationAngle = 180f;
    private float duration = 1.0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        BaseHP = HP;
        monsterType = MonsterType.Boss_Mouse;
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
            if (!DetectionPlayerPosition())
                Debug.Log("플레이어 없음");

            switch (pattern)
            {
                case (int)Pattern.Idle:
                    yield return Idle();
                    break;
                case (int)Pattern.Fist:
                    yield return Fist();
                    break;
                case (int)Pattern.Click:
                    yield return Click();
                    break;
                case (int)Pattern.Gaurd:
                    yield return Gaurd();
                    break;
                case (int)Pattern.Heal:
                    yield return Heal();
                    break;
            }

            yield return new WaitForSeconds(PatternCoolTime);

            // 마지막은 Heal패턴으로 사용X
            do
            {
                NextPattern = Random.Range(0, System.Enum.GetValues(typeof(Pattern)).Length - 1);
            } while (NextPattern == pattern);
            pattern = NextPattern;

            if (HealPatternReady && HP < BaseHP * 0.3f)
            {
                HealPatternReady = false;
                HP += BaseHP * 0.3f;
                pattern = (int)Pattern.Heal;
            }

            yield return null;
        }
    }

    // <idle> - 평소 모습 (기본 마우스), 플레이어 방향으로 느리게 움직임.
    public IEnumerator Idle()
    {
        var speed = MoveSpeed * Time.deltaTime;
        float elapsedTime = 0f;
        float preparationTime = 1.5f;

        while (elapsedTime < preparationTime)
        {
            DetectionPlayerPosition();

            Vector3 direction = TargetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90 - 22);

            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));

            elapsedTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
        rb.velocity = Vector3.zero;
    }

    // <Fist> - [공격 패턴1] (주먹) 플레이어 방향을 보고(0.5초 텀) 빠른 속도로 돌진. 벽 or 플레이어에게 맞으면 멈춤.
    public IEnumerator Fist()
    {
        rb.mass = 100.0f;
        float elapsedTime = 0f;
        float dashDuration = 0.5f;
        isDashing = true;
        MAnimator.SetBool("Fist", true);
        DetectionPlayerPosition();

        Vector3 direction = (TargetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        while (elapsedTime < dashDuration)
        {
            rb.MovePosition(rb.position + (Vector2)(direction * DashSpeed * Time.fixedDeltaTime));
            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector3.zero;
        MAnimator.SetBool("Fist", false);
        isDashing = false;
        Debug.Log("Fist 패턴 시간 종료 후 이동 종료");
        yield return new WaitForSeconds(1.0f);
        rb.mass = 1.0f;
        // 플레이어를 다시 바라보도록 설정
        DetectionPlayerPosition();
        direction = (TargetPosition - transform.position).normalized;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90 - 22);
    }

    // <Click> - [공격 패턴2] 플레이어 방향을 바라보고 손가락 끝에서. 빠른속도로 탄환 발사. 여러개. 
    public IEnumerator Click()
    {
        MAnimator.SetBool("Click", true);
        float ElapsedTime = 0f;
        while (ElapsedTime < ClickAttackDuration)
        {
            DetectionPlayerPosition();

            Vector3 direction = TargetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90 - 22);

            Fire();

            ElapsedTime += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        MAnimator.SetBool("Click", false);
        Debug.Log("Click 패턴 시간 종료 후 이동 종료");
    }

    // <Gaurd> - [방어 패턴] , 플레이어 방향을 보고 느리게 움직임. 데미지 1/4로 받기 지속시간 10초
    public IEnumerator Gaurd()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        MAnimator.SetBool("Gaurd", true);
        DefenseRate = 1.0f / 4.0f;
        float elapsedTime = 0f;
        float dashDuration = 10.0f;

        while (elapsedTime < dashDuration)
        {
            DetectionPlayerPosition();

            Vector3 direction = (TargetPosition - transform.position).normalized;
            rb.MovePosition(rb.position + (Vector2)(direction * GaurdMoveSpeed * Time.fixedDeltaTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        MAnimator.SetBool("Gaurd", false);
        DefenseRate = 1.0f;
    }

    // <Heal> - 제자리 고정, 체력이 30% 밑으로 떨어졌을때 10초간 체력을 30%로 회복함. <이때 보스가 공격을 받으면 회복력이 조금 떨어짐> (모래시계)
    public IEnumerator Heal()
    {
        Debug.Log("Heal");
        // 회전 초기화
        transform.rotation = Quaternion.Euler(0, 0, 0);

        MAnimator.SetTrigger("Heal"); // 애니메이션 트리거 활성화

        // 애니메이션이 2초 재생, 1초간 회전, 2초간 뒤집어진 걸로 다시 재생해야 함
        for (int i = 0; i < 2; i++)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            yield return new WaitForSeconds(1.9f);
            
            float ElapsedTime = 0f;
            float initialAngle = transform.eulerAngles.z;
            float targetAngle = initialAngle + rotationAngle;

            while (ElapsedTime < duration)
            {
                float currentAngle = Mathf.Lerp(initialAngle, targetAngle, ElapsedTime / duration);
                transform.rotation = Quaternion.Euler(0, 0, currentAngle);
                ElapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        Vector2 fireDirection = (TargetPosition - FirePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = fireDirection * BulletSpeed;
    }

    protected override void Die()
    {
        base.Die();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall"))
            {
                Debug.Log("충돌 멈춤");
                isDashing = false;
                rb.velocity = Vector2.zero;
            }
        }
    }
}
