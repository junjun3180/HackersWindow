using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_VE_2 : MonsterBase
{
    public float AttackCoolTime;

    public GameObject BulletPrefab;  // ¹ß»çÇÒ ÃÑ¾Ë ÇÁ¸®ÆÕ
    public Transform FirePoint;      // ÃÑ¾Ë ¹ß»ç À§Ä¡
    public float BulletSpeed = 10f;  // ÃÑ¾Ë ¼Óµµ

    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.M_VE_2;
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
                // Debug.Log("Player Å½Áö ¿Ï·á");
                DetectingAreaR = 15.0f;
                yield return AttackPreparation();
            }

            yield return null;
        }
    }

    public override IEnumerator AttackPreparation()
    {
        if (!MAnimator.GetBool("Detected"))
        {
            MAnimator.SetBool("Detected", true);
            yield return new WaitForSeconds(1.30f);

            DetectingAreaR = 15;
            AttackCoolTime = 1;
        }

        MAnimator.SetTrigger("Shot");
        yield return new WaitForSeconds(1.05f); 

        DetectionPlayerPosition();
        Fire();

        yield return new WaitForSeconds(AttackCoolTime);
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        Vector2 fireDirection = (TargetPosition - FirePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = fireDirection * BulletSpeed;
    }
}
