using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_V3 : MonsterBase
{
    public float SearchingCoolTime;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.M_V3;
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
            Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(transform.position, DetectingAreaR);
            rb.bodyType = RigidbodyType2D.Dynamic;

            foreach (Collider2D obj in detectedObjects)
            {
                if (obj.CompareTag("Player"))
                {
                    player = obj.transform;

                    TargetPosition = player.position;
                    DetectionSuccess = true;
                    break;
                }
            }

            if (DetectionSuccess)
            {
                yield return AttackPreparation();
                DetectionSuccess = false;
            }
            else
            {
                isMoving = true;

                yield return RandomMoveAfterSearchFail();
            }

            yield return new WaitForSeconds(SearchingCoolTime);
        }
    }

    public override IEnumerator AttackPreparation()
    {
        // 방향 설정
        SpriteFlipSetting();

        // Waitting during Attack Ready Animation
        MAnimator.SetTrigger("Jump");
        yield return new WaitForSeconds(1.7f);

        var speed = MoveSpeed * Time.deltaTime;
        float maxMoveDuration = 1.0f;
        float elapsedTime = 0f;

        while (Vector3.Distance(transform.position, TargetPosition) > speed)
        {
            elapsedTime += Time.deltaTime;

            // 지정된 시간을 초과하면 이동 중지
            if (elapsedTime >= maxMoveDuration)
            {
                yield break; // 코루틴 종료
            }

            // Rigidbody2D를 사용하여 이동
            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate(); // 물리 업데이트 프레임에 맞춰서 대기
        }
    }

    public override IEnumerator RandomMoveAfterSearchFail()
    {
        TargetPosition = GetRanomPositionAround();

        // 방향 설정
        SpriteFlipSetting();

        MAnimator.SetTrigger("Jump");
        yield return new WaitForSeconds(1.7f);

        var speed = MoveSpeed * Time.deltaTime;
        float maxMoveDuration = 1.0f;  // 이동을 허용할 최대 시간 (예: 3초)
        float elapsedTime = 0f;        // 경과 시간

        while (Vector3.Distance(transform.position, TargetPosition) > speed && isMoving)
        {
            elapsedTime += Time.deltaTime;

            // 지정된 시간을 초과하면 이동 중지
            if (elapsedTime >= maxMoveDuration)
            {
                yield break; // 코루틴 종료
            }

            // Rigidbody2D를 사용하여 이동
            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate(); // 물리 업데이트 프레임에 맞춰서 대기
        }
    }

    protected override Vector3 GetRanomPositionAround()
    {
        // 랜덤한 각도 선택 (0도 ~ 360도)
        float randomAngle = Random.Range(0f, 360f);

        // 거리는 항상 DetectingAreaR (반지름)으로 고정
        float x = transform.position.x + DetectingAreaR * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
        float y = transform.position.y + DetectingAreaR * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

        return new Vector3(x, y, 0);
    }
}
