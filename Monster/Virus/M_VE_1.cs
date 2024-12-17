using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M_VE_1 : MonsterBase
{
    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.M_VE_1;
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
                // Debug.Log("Player 탐지 완료");
                DetectingAreaR = 15.0f;
                yield return AttackPreparation();
            }

            yield return null;
        }

    }

    public override IEnumerator AttackPreparation()
    {
        DetectionPlayerPosition();
        if (!MAnimator.GetBool("Detected"))
        {
            MAnimator.SetBool("Detected", true);
            yield return new WaitForSeconds(1.40f);

            DetectingAreaR = 20;
        }

        // 방향 설정
        SpriteFlipSetting();
        rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));
        yield return null;
    }
}
