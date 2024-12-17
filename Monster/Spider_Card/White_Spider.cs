using System.Collections;
using UnityEngine;

public class White_Spider : MonsterBase
{
    public float AttackCoolTime; 
    public float SearchingCoolTime;
    bool First = true;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.White_Spider;
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
            yield return new WaitForSeconds(2.0f);
            if (!DetectionSuccess && DetectionPlayerPosition())
                DetectionSuccess = true;

            if (DetectionSuccess)
            {
                yield return AttackPreparation();
                DetectionSuccess = false;
            }
            else
            {
                yield return RandomMoveAfterSearchFail();
            }

            yield return new WaitForSeconds(SearchingCoolTime);
        }
    }

    public override IEnumerator AttackPreparation()
    {
        // 방향 설정
        SpriteFlipSetting();

        MAnimator.SetTrigger("Jump");
        while (Vector3.Distance(transform.position, TargetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, MoveSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(AttackCoolTime);
    }

    public override IEnumerator RandomMoveAfterSearchFail()
    {
        TargetPosition = GetRanomPositionAround();

        SpriteFlipSetting();

        var speed = MoveSpeed * Time.deltaTime;
        float maxMoveDuration = 1.0f;
        float elapsedTime = 0f;        

        while (Vector3.Distance(transform.position, TargetPosition) > speed && isMoving)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= maxMoveDuration)
            {
                yield break;
            }

            MAnimator.SetTrigger("Jump");

            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate();
        }
    }
}
