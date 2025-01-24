using System.Collections;
using UnityEngine;

public class M_V1 : MonsterBase
{
    public float AttackCoolTime;
    public float AttackDuration;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.M_V1;
        StartCoroutine(MonsterRoutine());
    }

    public override IEnumerator MonsterRoutine()
    {
        while (true)
        {
            if (!DetectionSuccess && DetectionPlayerPosition())
                DetectionSuccess = true;

            if (DetectionSuccess)
            {
                // Debug.Log("Player 탐지 완료");
                DetectingAreaR = 15.0f;
                yield return AttackPreparation();
                yield return new WaitForSeconds(AttackCoolTime);
            }

            yield return null;
        }
    }

    public override IEnumerator AttackPreparation()
    {
        // 방향 설정
        SpriteFlipSetting();

        var speed = MoveSpeed * Time.deltaTime;
        float elapsedTime = 0f;

        // rb.bodyType = RigidbodyType2D.Dynamic;

        while (Vector3.Distance(transform.position, TargetPosition) > speed)
        {
            DetectionPlayerPosition();
            elapsedTime += Time.deltaTime;

            // Move during DurationTime
            if (elapsedTime >= AttackDuration)
            {
                MAnimator.SetBool("Move", false);
                yield break;
            }

            MAnimator.SetBool("Move", true);

            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));
            
            yield return new WaitForFixedUpdate(); 
        }
        MAnimator.SetBool("Move", false);
    }
}
