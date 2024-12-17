using System.Collections;
using TMPro;
using UnityEngine;

public class Red_Spider : MonsterBase
{
    public float AttackCoolTime;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.Red_Spider;
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
        }
    }

    public override IEnumerator AttackPreparation()
    {
        SpriteFlipSetting();

        MAnimator.SetBool("Move", true);
        while (Vector3.Distance(transform.position, TargetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, MoveSpeed * Time.deltaTime);
            yield return null;
        }

        MAnimator.SetBool("Move", false);
        yield return new WaitForSeconds(AttackCoolTime);
    }

    public override IEnumerator RandomMoveAfterSearchFail()
    {
        TargetPosition = GetRanomPositionAround();

        SpriteFlipSetting();

        var speed = MoveSpeed * Time.deltaTime;
        float maxMoveDuration = 1.0f;  
        float elapsedTime = 0f;


        MAnimator.SetBool("Move", true);
        while (Vector3.Distance(transform.position, TargetPosition) > speed && isMoving)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= maxMoveDuration)
            {
                yield break;
            }


            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate();
        }
        MAnimator.SetBool("Move", false);
    }
}
