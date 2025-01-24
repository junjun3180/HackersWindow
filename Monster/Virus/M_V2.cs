using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M_V2 : MonsterBase
{
    public float AttackCoolTime;
    public GameObject BulletPrefab;  // ¹ß»çÇÒ ÃÑ¾Ë ÇÁ¸®ÆÕ
    public Transform FirePoint;      // ÃÑ¾Ë ¹ß»ç À§Ä¡
    public float BulletSpeed = 10f;  // ÃÑ¾Ë ¼Óµµ


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.M_V2;
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
                // Debug.Log("Player Å½Áö ¿Ï·á");
                DetectingAreaR = 15.0f;
                yield return AttackPreparation();
            }

            yield return null;
        }
    }

    public override IEnumerator AttackPreparation()
    {
        var speed = MoveSpeed * Time.deltaTime;
        float elapsedTime = 0f;

        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(transform.position, DetectingAreaR);
        rb.bodyType = RigidbodyType2D.Dynamic;

        while (Vector3.Distance(transform.position, TargetPosition) > speed)
        {
            SpriteFlipSetting();
            DetectionPlayerPosition();
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= AttackCoolTime)
            {
                DetectionPlayerPosition();
                MAnimator.SetTrigger("Shot");

                float animationTime = 1.4f;
                float animElapsedTime = 0f;
                while (animElapsedTime < animationTime)
                {
                    DetectionPlayerPosition();
                    rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));
                    animElapsedTime += Time.deltaTime;
                    yield return null;
                }

                Fire();

                elapsedTime = 0;
            }
            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    // °ø°Ý ·ÎÁ÷
    private void Fire()
    {
        GameObject bullet = Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        Vector2 fireDirection = (TargetPosition - FirePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = fireDirection * BulletSpeed;
    }
}
